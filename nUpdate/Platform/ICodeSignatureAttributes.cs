namespace nUpdate.Platform;

/// <summary>
///     The extended attributes macOS keeps code signatures in (<c>com.apple.cs.*</c>). <c>codesign</c> stores the
///     signature of a file in an app bundle's <c>Contents/MacOS</c> that is not Mach-O code, such as a .NET assembly,
///     in them, and a zip does not carry extended attributes: nUpdate Administration writes them into the package
///     manifest and the installer sets them again, so the replaced bundle still verifies.
/// </summary>
internal interface ICodeSignatureAttributes
{
    /// <summary>The code signature attributes of the file by name; empty off macOS and for a file without any.</summary>
    IReadOnlyDictionary<string, byte[]> Read(string path);

    /// <summary>Sets a code signature attribute. Does nothing off macOS.</summary>
    /// <exception cref="ArgumentException">The name is not a code signature attribute (<c>com.apple.cs.*</c>).</exception>
    void Write(string path, string name, byte[] value);
}
