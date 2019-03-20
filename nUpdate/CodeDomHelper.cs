using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace nUpdate
{
    public class CodeDomHelper
    {
        private readonly CodeDomProvider _cSharpCodeDomProvider = new CSharpCodeProvider();
        private readonly CompilerParameters _compileParameters = new CompilerParameters();

        public void ExecuteScript(string sourceCode, object[] args)
        {
            var referencedAssemblies = sourceCode.Split('\r', '\n').Where(item => item.StartsWith("using"));
            foreach (var assembly in referencedAssemblies)
            {
                _compileParameters.ReferencedAssemblies.Add($"{assembly.Split(' ')[1].Replace(";", string.Empty)}.dll");
            }

            _compileParameters.GenerateInMemory = false;
            _compileParameters.GenerateExecutable = true;
            CompilerResults compilerResults = _cSharpCodeDomProvider.CompileAssemblyFromSource(_compileParameters,
                sourceCode);
            foreach (
                CompilerError compilerError in compilerResults.Errors.Cast<CompilerError>().Where(ce => !ce.IsWarning))
            {
                throw new Exception(
                    $"({compilerError.Line},{compilerError.Column}: Error {compilerError.ErrorNumber}): {compilerError.ErrorText}");
            }

            MethodInfo entryPoint = compilerResults.CompiledAssembly.EntryPoint;
            object entryPointInstance = compilerResults.CompiledAssembly.CreateInstance(entryPoint.Name);
            object[] parameters = args;
            entryPoint.Invoke(entryPointInstance, parameters);
        }
    }
}