// CodeDomHelper.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;

namespace nUpdate.Administration
{
    public class CodeDomHelper
    {
        private readonly CompilerParameters _compileParameters = new CompilerParameters();
        private readonly CodeDomProvider _cSharpCodeDomProvider = new CSharpCodeProvider();

        public IEnumerable<CompilerError> CompileCode(string sourceCode)
        {
            var referencedAssemblies = sourceCode.Split('\r', '\n').Where(item => item.StartsWith("using"));
            foreach (var assembly in referencedAssemblies)
                try
                {
                    _compileParameters.ReferencedAssemblies.Add(
                        $"{assembly.Split(' ')[1].Replace(";", string.Empty)}.dll");
                }
                catch (IndexOutOfRangeException)
                {
                    // Ignored, will be caught as syntax error
                }

            _compileParameters.GenerateInMemory = true;
            _compileParameters.GenerateExecutable = false;
            CompilerResults compilerResults = _cSharpCodeDomProvider.CompileAssemblyFromSource(_compileParameters,
                sourceCode);
            return compilerResults.Errors.Cast<CompilerError>().Where(ce => !ce.IsWarning);
        }
    }
}