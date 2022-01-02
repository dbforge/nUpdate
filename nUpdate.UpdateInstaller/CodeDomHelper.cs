// CodeDomHelper.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.CSharp;
using nUpdate.UpdateInstaller.Exceptions;

namespace nUpdate.UpdateInstaller
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
            var compilerResults = _cSharpCodeDomProvider.CompileAssemblyFromSource(_compileParameters,
                sourceCode);
            foreach (
                var compilerError in compilerResults.Errors.Cast<CompilerError>().Where(ce => !ce.IsWarning))
                throw new CompileException(
                    $"({compilerError.Line},{compilerError.Column}: Error {compilerError.ErrorNumber}): {compilerError.ErrorText}");

            var entryPoint = compilerResults.CompiledAssembly.EntryPoint;
            var entryPointInstance = compilerResults.CompiledAssembly.CreateInstance(entryPoint.Name);
            object[] parameters = {new[] {Program.AimFolder}};
            entryPoint.Invoke(entryPointInstance, parameters);
        }
    }
}