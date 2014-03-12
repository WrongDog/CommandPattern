using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOLibrary
{
    [Serializable]
    public class FakeBo1
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public int Value2 { get; set; }
        public override string ToString()
        {
            return string.Format("Name:{0},Value:{1},Value2:{2}", Name, Value, Value2);
        }
    }
}
