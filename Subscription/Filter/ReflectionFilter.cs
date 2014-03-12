using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;

namespace Subscription.Filter
{
    public class ReflectionFilter:IFilter
    {
        private dynamic filterObject;
        //private string boType;
        private string filterstring;
        public ReflectionFilter(string filter)//string boType,
        {
            //this.boType = boType;
            filterstring = filter;
            BuildMethod(filter);
        }
        private void BuildMethod(string filter)
        {
            string source = "using System;" +
                            "using System.Collections.Generic;" +
                            "using System.Linq;" +
                            "using System.Text;" +
                            "    public class Filter" +
                            "    {" +
                            "        public bool IsMatch(dynamic obj)" +
                            "        {" +
                            "            return " + filter + ";" +
                            "        }" +
                            "    }";
                            
 
            Assembly assembly = Compiler.Compile(source);
            if (assembly != null)
            {
                filterObject = Activator.CreateInstance(assembly.GetType("Filter"));
            }
            else
            {
                throw new Exception("");
            }
           
        }
        public bool IsMatch(object obj)
        {
            //if (obj.GetType().Name != this.boType) return false;
            return (bool)filterObject.IsMatch(obj);
        }

        public string FilterString
        {
            get
            {
                return filterstring;
            }
        }
    }
    public class DisposableReflectionFilter : IFilter, IDisposable
    {
        private AppDomain appdomain = null;
        private CompilerWrapper compiler = null;
        private string filterstring;
        public DisposableReflectionFilter(string filter)
        {
            filterstring = filter;
            appdomain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
            BuildMethod(filter);
        }
        private void BuildMethod(string filter)
        {

            string source = "using System;" +
                            "using System.Collections.Generic;" +
                            "using System.Linq;" +
                            "using System.Text;" +
                            "    public class Filter" +
                            "    {" +
                            "        public bool IsMatch(dynamic obj)" +
                            "        {" +
                            "            return " + filter + ";" +
                            "        }" +
                            "    }";




            compiler = (CompilerWrapper)appdomain.CreateInstanceFromAndUnwrap(Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), typeof(CompilerWrapper).FullName);
            compiler.Compile(source);

        }
        public void Dispose()
        {
            if (appdomain != null)
            {
                compiler = null;
                AppDomain.Unload(appdomain);
                appdomain = null;
            }
        }

        public bool IsMatch(object obj)
        {
            return compiler.IsMatch(obj);
        }

        public string FilterString
        {
            get { throw new NotImplementedException(); }
        }
    }
    public class CompilerWrapper : MarshalByRefObject
    {
        private Assembly assembly = null;
        private dynamic filterObject;
        public CompilerWrapper()
        {

        }
        public bool Compile(string source)
        {
            assembly = Compiler.Compile(source);
            if (assembly != null)
            {
                filterObject = Activator.CreateInstance(assembly.GetType("Filter"));
                if (filterObject != null) return true;
            }
            return false;
        }
        public bool IsMatch(object obj)
        {
            return (bool)filterObject.IsMatch(obj);
        }
    }
    public sealed class Isolated<T> : IDisposable where T : MarshalByRefObject
    {
        private AppDomain domain;
        private T instance;

        public Isolated()
        {
            domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(),
               null, AppDomain.CurrentDomain.SetupInformation);

            Type type = typeof(T);

            instance = (T)domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
        }

        public T Instance
        {
            get
            {
                return instance;
            }
        }

        public void Dispose()
        {
            if (domain != null)
            {
                AppDomain.Unload(domain);

                domain = null;
            }
        }
    }
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

            return CompileWithReferences(sourcecode, language, referencedassemblies.ToArray());
        }

        private static Assembly CompileWithReferences(string sourcecode, CompileLanguage language, string[] references)
        {
            CodeDomProvider comp = null;
            switch (language)
            {
                case CompileLanguage.VisualBasic:
                    comp = new Microsoft.VisualBasic.VBCodeProvider();
                    break;
                case CompileLanguage.CSharp:
                default:
                    comp = new Microsoft.CSharp.CSharpCodeProvider();
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
