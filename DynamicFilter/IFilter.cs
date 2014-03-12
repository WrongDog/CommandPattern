using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicFilter
{
    
    public interface IFilter
    {
        bool IsMatch(dynamic obj);
    }
    //public class Filter:IFilter
    //{
    //    public bool IsMatch(dynamic obj)
    //    {
    //        return obj.number > 10;
    //    }
    //}
}
