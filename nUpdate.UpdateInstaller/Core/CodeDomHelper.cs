// Copyright © Dominic Beger 2018

using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using nUpdate.UpdateInstaller.Exceptions;

namespace nUpdate.UpdateInstaller.Core
{
    public class CodeDomHelper
    {
        private readonly CompilerParameters _compileParameters = new CompilerParameters();
        private readonly CodeDomProvider _cSharpCodeDomProvider = new CSharpCodeProvider();

        public void ExecuteScript(string sourceCode)
        {
            var referencedAssemblies = sourceCode.Split('\r', '\n').Where(item => item.StartsWith("using"));
            foreach (var assembly in referencedAssemblies)
                _compileParameters.ReferencedAssemblies.Add($"{assembly.Split(' ')[1].Replace(";", string.Empty)}.dll");

            _compileParameters.GenerateInMemory = false;
            _compileParameters.GenerateExecutable = true;
            CompilerResults compilerResults = _cSharpCodeDomProvider.CompileAssemblyFromSource(_compileParameters,
                sourceCode);
            foreach (
                CompilerError compilerError in compilerResults.Errors.Cast<CompilerError>().Where(ce => !ce.IsWarning))
                throw new CompileException(
                    $"({compilerError.Line},{compilerError.Column}: Error {compilerError.ErrorNumber}): {compilerError.ErrorText}");

            MethodInfo entryPoint = compilerResults.CompiledAssembly.EntryPoint;
            object entryPointInstance = compilerResults.CompiledAssembly.CreateInstance(entryPoint.Name);
            object[] parameters = {new[] {Program.AimFolder}};
            entryPoint.Invoke(entryPointInstance, parameters);
        }
    }
}