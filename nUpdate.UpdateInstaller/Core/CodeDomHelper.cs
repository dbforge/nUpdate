using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using nUpdate.UpdateInstaller.Exceptions;

namespace nUpdate.UpdateInstaller.Core
{
    public class CodeDomHelper
    {
        private readonly CodeDomProvider _cSharpCodeDomProvider = new CSharpCodeProvider();
        private readonly CompilerParameters _compileParameters = new CompilerParameters();

        public void ExecuteScript(string sourceCode)
        {
            var referencedAssemblies = sourceCode.Split('\r', '\n').Where(item => item.StartsWith("using"));
            foreach (var assembly in referencedAssemblies)
            {
                _compileParameters.ReferencedAssemblies.Add(String.Format("{0}.dll", assembly.Split(' ')[1].Replace(";", String.Empty)));
            }

            _compileParameters.GenerateInMemory = false;
            _compileParameters.GenerateExecutable = true;
            CompilerResults compilerResults = _cSharpCodeDomProvider.CompileAssemblyFromSource(_compileParameters, sourceCode);
            foreach (CompilerError compilerError in compilerResults.Errors.Cast<CompilerError>().Where(ce => !ce.IsWarning))
            {
                throw new CompileException(String.Format("({0},{1}: Error {2}): {3}", compilerError.Line, compilerError.Column, compilerError.ErrorNumber, compilerError.ErrorText));
            }

            MethodInfo entryPoint = compilerResults.CompiledAssembly.EntryPoint;
            object entryPointInstance = compilerResults.CompiledAssembly.CreateInstance(entryPoint.Name);
            object[] parameters =  { new[] { Program.AimFolder }};
            entryPoint.Invoke(entryPointInstance, parameters);
        }
    }
}