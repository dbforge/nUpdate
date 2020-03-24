// CodeDomHelper.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.CSharp;

namespace nUpdate
{
    public class CodeDomHelper
    {
        private readonly CompilerParameters _compileParameters = new CompilerParameters();
        private readonly CodeDomProvider _cSharpCodeDomProvider = new CSharpCodeProvider();

        public void ExecuteScript(string sourceCode, object[] args)
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
                throw new Exception(
                    $"({compilerError.Line},{compilerError.Column}: Error {compilerError.ErrorNumber}): {compilerError.ErrorText}");

            var entryPoint = compilerResults.CompiledAssembly.EntryPoint;
            var entryPointInstance = compilerResults.CompiledAssembly.CreateInstance(entryPoint.Name);
            var parameters = args;
            entryPoint.Invoke(entryPointInstance, parameters);
        }
    }
}