using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;

namespace nUpdate.Administration.Core
{
    public class CodeDomHelper
    {
        private readonly CodeDomProvider _cSharpCodeDomProvider = new CSharpCodeProvider();
        private readonly CompilerParameters _compileParameters = new CompilerParameters();

        public IEnumerable<CompilerError> CompileCode(string sourceCode)
        {
            var referencedAssemblies = sourceCode.Split('\r', '\n').Where(item => item.StartsWith("using"));
            foreach (var assembly in referencedAssemblies)
            {
                try
                {
                    _compileParameters.ReferencedAssemblies.Add(String.Format("{0}.dll", assembly.Split(' ')[1].Replace(";", String.Empty)));
                }
                catch (IndexOutOfRangeException)
                {
                    // Ignored, will be caught as syntax error
                }
            }

            _compileParameters.GenerateInMemory = true;
            _compileParameters.GenerateExecutable = false;
            CompilerResults compilerResults = _cSharpCodeDomProvider.CompileAssemblyFromSource(_compileParameters, sourceCode);
            return compilerResults.Errors.Cast<CompilerError>().Where(ce => !ce.IsWarning);
        }
    }
}
