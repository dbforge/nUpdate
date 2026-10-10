using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace ApiDocsCheck;

/// <summary>
///     The public API of an assembly as documentation comment IDs: the public types, and their public members and the
///     protected ones of types that can be derived from.
/// </summary>
/// <remarks>
///     Left out, because nobody calls them by name or a tool writes them: types the build tools generate, property
///     and event accessors, compiler-generated members (those of records), overrides of
///     <see cref="object.Equals(object)" /> and <see cref="object.GetHashCode" />, the members of delegates, and a
///     parameterless constructor that is a type's only one.
/// </remarks>
public static class ApiSurface
{
    public static IEnumerable<string> Read(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var pe = new PEReader(stream);
        var reader = pe.GetMetadataReader();
        var ids = new List<string>();
        foreach (var handle in reader.TypeDefinitions)
        {
            var type = reader.GetTypeDefinition(handle);
            if (!IsVisible(reader, type) || IsGenerated(reader, type))
                continue;
            ids.Add("T:" + TypeName(reader, handle));
            if (!IsDelegate(reader, type))
                ids.AddRange(Members(reader, handle, type));
        }

        return ids;
    }

    private static IEnumerable<string> Members(MetadataReader reader, TypeDefinitionHandle handle,
        TypeDefinition type)
    {
        var owner = TypeName(reader, handle);
        var inheritable = (type.Attributes & TypeAttributes.Sealed) == 0;
        var provider = new DocIdTypeProvider();
        bool Visible(MethodAttributes access) =>
            access is MethodAttributes.Public or MethodAttributes.FamORAssem ||
            (access == MethodAttributes.Family && inheritable);

        var methods = type.GetMethods().Select(reader.GetMethodDefinition)
            .Where(m => Visible(m.Attributes & MethodAttributes.MemberAccessMask) &&
                        !IsCompilerGenerated(reader, m.GetCustomAttributes()))
            .ToList();
        var constructors = methods.Where(m => reader.GetString(m.Name) == ".ctor").ToList();
        var onlyDefaultConstructor = constructors.Count == 1 &&
                                     constructors[0].DecodeSignature(provider, null).ParameterTypes.Length == 0;
        foreach (var method in methods)
        {
            var name = reader.GetString(method.Name);
            var special = (method.Attributes & MethodAttributes.SpecialName) != 0;
            if (special && name != ".ctor" && !name.StartsWith("op_", StringComparison.Ordinal))
                continue;
            if (name == ".ctor" && onlyDefaultConstructor)
                continue;
            var signature = method.DecodeSignature(provider, null);
            if (IsObjectOverride(method, name, signature))
                continue;
            var arity = signature.GenericParameterCount > 0 ? "``" + signature.GenericParameterCount : "";
            var id = $"M:{owner}.{(name == ".ctor" ? "#ctor" : name)}{arity}{Parameters(signature.ParameterTypes)}";
            yield return name == ".ctor" || signature.ReturnType == "System.Void" ? id : id + "~" + signature.ReturnType;
        }

        foreach (var property in type.GetProperties().Select(reader.GetPropertyDefinition))
        {
            var accessors = property.GetAccessors();
            var visible = new[] { accessors.Getter, accessors.Setter }.Where(a => !a.IsNil)
                .Select(reader.GetMethodDefinition)
                .Any(a => Visible(a.Attributes & MethodAttributes.MemberAccessMask));
            if (!visible || IsCompilerGenerated(reader, property.GetCustomAttributes()))
                continue;
            var signature = property.DecodeSignature(provider, null);
            yield return $"P:{owner}.{reader.GetString(property.Name)}{Parameters(signature.ParameterTypes)}~{signature.ReturnType}";
        }

        foreach (var @event in type.GetEvents().Select(reader.GetEventDefinition))
        {
            var adder = reader.GetMethodDefinition(@event.GetAccessors().Adder);
            if (!Visible(adder.Attributes & MethodAttributes.MemberAccessMask))
                continue;
            yield return $"E:{owner}.{reader.GetString(@event.Name)}~{DecodeType(reader, @event.Type, provider)}";
        }

        var isEnum = BaseTypeName(reader, type) == "System.Enum";
        foreach (var field in type.GetFields().Select(reader.GetFieldDefinition))
        {
            var access = (MethodAttributes)(int)(field.Attributes & FieldAttributes.FieldAccessMask);
            if (!Visible(access) || (field.Attributes & FieldAttributes.RTSpecialName) != 0 ||
                IsCompilerGenerated(reader, field.GetCustomAttributes()))
                continue;
            var id = $"F:{owner}.{reader.GetString(field.Name)}";
            yield return isEnum ? id : id + "~" + field.DecodeSignature(provider, null);
        }
    }

    private static string Parameters(ImmutableArray<string> types) =>
        types.Length == 0 ? "" : "(" + string.Join(",", types) + ")";

    private static bool IsObjectOverride(MethodDefinition method, string name, MethodSignature<string> signature)
    {
        var overrides = (method.Attributes & (MethodAttributes.Virtual | MethodAttributes.NewSlot)) ==
                        MethodAttributes.Virtual;
        return overrides && ((name == "Equals" && signature.ParameterTypes is ["System.Object"]) ||
                             (name == "GetHashCode" && signature.ParameterTypes.Length == 0));
    }

    private static bool IsVisible(MetadataReader reader, TypeDefinition type)
    {
        switch (type.Attributes & TypeAttributes.VisibilityMask)
        {
            case TypeAttributes.Public:
                return true;
            case TypeAttributes.NestedPublic:
                return IsVisible(reader, reader.GetTypeDefinition(type.GetDeclaringType()));
            case TypeAttributes.NestedFamily or TypeAttributes.NestedFamORAssem:
                var declaring = reader.GetTypeDefinition(type.GetDeclaringType());
                return (declaring.Attributes & TypeAttributes.Sealed) == 0 && IsVisible(reader, declaring);
            default:
                return false;
        }
    }

    /// <summary>
    ///     A type the build tools write, such as the XAML loaders of Avalonia (whose names are no C# identifiers) or the
    ///     <c>GeneratedInternalTypeHelper</c> of WPF (marked <c>[GeneratedCode]</c>).
    /// </summary>
    private static bool IsGenerated(MetadataReader reader, TypeDefinition type) =>
        !reader.GetString(type.Name).All(c => char.IsLetterOrDigit(c) || c is '_' or '`') ||
        type.GetCustomAttributes().Select(reader.GetCustomAttribute).Any(a =>
            AttributeTypeName(reader, a) == "System.CodeDom.Compiler.GeneratedCodeAttribute");

    private static bool IsDelegate(MetadataReader reader, TypeDefinition type) =>
        BaseTypeName(reader, type) == "System.MulticastDelegate";

    private static string? BaseTypeName(MetadataReader reader, TypeDefinition type) =>
        type.BaseType.IsNil ? null : DecodeType(reader, type.BaseType, new DocIdTypeProvider());

    private static bool IsCompilerGenerated(MetadataReader reader, CustomAttributeHandleCollection attributes) =>
        attributes.Select(reader.GetCustomAttribute).Any(a =>
            AttributeTypeName(reader, a) == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");

    private static string? AttributeTypeName(MetadataReader reader, CustomAttribute attribute) =>
        attribute.Constructor.Kind switch
        {
            HandleKind.MemberReference => DecodeType(reader,
                reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor).Parent,
                new DocIdTypeProvider()),
            HandleKind.MethodDefinition => TypeName(reader,
                reader.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor).GetDeclaringType()),
            _ => null
        };

    private static string DecodeType(MetadataReader reader, EntityHandle handle, DocIdTypeProvider provider) =>
        handle.Kind switch
        {
            HandleKind.TypeDefinition => TypeName(reader, (TypeDefinitionHandle)handle),
            HandleKind.TypeReference => DocIdTypeProvider.TypeName(reader, (TypeReferenceHandle)handle),
            HandleKind.TypeSpecification => reader.GetTypeSpecification((TypeSpecificationHandle)handle)
                .DecodeSignature(provider, null),
            _ => "?"
        };

    internal static string TypeName(MetadataReader reader, TypeDefinitionHandle handle)
    {
        var type = reader.GetTypeDefinition(handle);
        var name = reader.GetString(type.Name);
        var declaring = type.GetDeclaringType();
        if (!declaring.IsNil)
            return TypeName(reader, declaring) + "." + name;
        var ns = reader.GetString(type.Namespace);
        return ns.Length == 0 ? name : ns + "." + name;
    }
}

/// <summary>Writes types the way documentation comment IDs do: <c>System.Collections.Generic.List{System.String}</c>, <c>`0</c>, <c>``0</c>, <c>[]</c>, <c>@</c>.</summary>
internal sealed class DocIdTypeProvider : ISignatureTypeProvider<string, object?>
{
    public string GetPrimitiveType(PrimitiveTypeCode typeCode) => "System." + typeCode switch
    {
        PrimitiveTypeCode.IntPtr => "IntPtr",
        PrimitiveTypeCode.UIntPtr => "UIntPtr",
        PrimitiveTypeCode.TypedReference => "TypedReference",
        _ => typeCode.ToString()
    };

    public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) =>
        ApiSurface.TypeName(reader, handle);

    public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) =>
        TypeName(reader, handle);

    public string GetTypeFromSpecification(MetadataReader reader, object? genericContext,
        TypeSpecificationHandle handle, byte rawTypeKind) =>
        reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);

    public string GetSZArrayType(string elementType) => elementType + "[]";

    public string GetArrayType(string elementType, ArrayShape shape) =>
        elementType + "[" + string.Join(",", Enumerable.Repeat("0:", shape.Rank)) + "]";

    public string GetByReferenceType(string elementType) => elementType + "@";

    public string GetPointerType(string elementType) => elementType + "*";

    public string GetPinnedType(string elementType) => elementType;

    public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;

    public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) =>
        System.Text.RegularExpressions.Regex.Replace(genericType, "`[0-9]+", "") + "{" +
        string.Join(",", typeArguments) + "}";

    public string GetGenericTypeParameter(object? genericContext, int index) => "`" + index;

    public string GetGenericMethodParameter(object? genericContext, int index) => "``" + index;

    public string GetFunctionPointerType(MethodSignature<string> signature) => "function pointer";

    internal static string TypeName(MetadataReader reader, TypeReferenceHandle handle)
    {
        var type = reader.GetTypeReference(handle);
        var name = reader.GetString(type.Name);
        if (type.ResolutionScope.Kind == HandleKind.TypeReference)
            return TypeName(reader, (TypeReferenceHandle)type.ResolutionScope) + "." + name;
        var ns = reader.GetString(type.Namespace);
        return ns.Length == 0 ? name : ns + "." + name;
    }
}
