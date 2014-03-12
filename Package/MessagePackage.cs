using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Package
{
    [Serializable]
    public class MessagePackage:Package
    {
        private MessagePackage()
        {
        }
        private MessagePackage(string creator,string packageType,string packageData)
        {


            this.ID = Guid.NewGuid().ToString();
            this.Creator = creator;
            this.CreateDateTime = DateTime.UtcNow;
            this.PackageType = packageType;
            this.PackageData = packageData;

           

        }
    



        public static MessagePackage CreateFrom(object bo)
        {

            return new MessagePackage(
            creator: System.Diagnostics.Process.GetCurrentProcess().ProcessName,
            packageType: bo.GetType().FullName,
            packageData: Serialize(bo)
            );
          
            
        }
        private static string Serialize(object bo)
        {
            string value;
            XmlSerializer xmlser = new XmlSerializer(bo.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                xmlser.Serialize(ms, bo);
                value = Convert.ToBase64String(ms.GetBuffer());
            }
            return value;
        }
        public override string PackageHandler
        {
            get { return "MessageHandler"; }
        }
    }
}
