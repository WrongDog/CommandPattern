using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.Reflection;

namespace DynamicFilter
{
    public class ReflectionFilter:IFilter
    {
        private dynamic filterObject;
        public ReflectionFilter(string filter)
        {
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
           
        }
        public bool IsMatch(dynamic obj)
        {
            return (bool)filterObject.IsMatch(obj);
        }
    }
}
