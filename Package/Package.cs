using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Package
{
    
    [Serializable]
    public abstract class Package
    {
        public string ID { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string Creator { get; set; }
        public abstract string PackageHandler { get; }
        public string PackageType { get; set; }
        public string PackageData { get; set; }
        
    }

    //[Serializable]
    //public class Header
    //{
    //    public string ID { get; set; }
    //    public DateTime CreateDateTime { get; set; }
    //    public string Creator { get; set; }
    //    public string PackageHandler { get; set; }
    //}
    //[Serializable]
    //public class Body
    //{
    //    public string PackageType { get; set; }
    //    public string PackageData { get; set; }
    //}
    //[Serializable]
    //public abstract class Package
    //{
    //    public Header Header { get; set; }
    //    public Body Body { get; set; }

    //}
}
