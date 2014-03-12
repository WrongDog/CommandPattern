using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
namespace DynamicFilter
{
    public enum CompileLanguage
    {
        CSharp,
        VisualBasic
    }
    public class Compiler
    {

       

        public static Assembly Compile(string sourcecode, CompileLanguage language = CompileLanguage.CSharp)
        {
            List<string> referencedassemblies = new List<string>();
            referencedassemblies.Add("system.dll");
            referencedassemblies.Add("system.core.dll");
            referencedassemblies.Add("Microsoft.CSharp.dll");
            referencedassemblies.Add("system.xml.dll");
            referencedassemblies.Add("system.xml.linq.dll");
            referencedassemblies.Add("system.data.dll");
            referencedassemblies.Add("System.Data.DataSetExtensions.dll");

            return CompileWithReferences(sourcecode, language,referencedassemblies.ToArray());
        }

        private static Assembly CompileWithReferences(string sourcecode,CompileLanguage language, string[] references)
        {
            CodeDomProvider comp =null;
            switch (language ){
                case CompileLanguage.VisualBasic:
                    comp = new Microsoft.VisualBasic.VBCodeProvider();
                    break;
                case CompileLanguage.CSharp:
                default:
                    comp = new CSharpCodeProvider();
                    break;
            }
            CompilerParameters cp = new CompilerParameters();
            foreach (string reference in references)
            {
                cp.ReferencedAssemblies.Add(reference);
            }
            cp.GenerateInMemory = true;



            CompilerResults cr = comp.CompileAssemblyFromSource(cp, sourcecode);
            if (cr.Errors.HasErrors)
            {
                string error = string.Empty;
                foreach (CompilerError err in cr.Errors)
                {
                    error += err.ErrorText + System.Environment.NewLine;
                }
                System.Diagnostics.Trace.WriteLine(error);
                return null;
            }

            return cr.CompiledAssembly;
        }

    }
}
