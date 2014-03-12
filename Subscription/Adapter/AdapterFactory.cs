using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscription.Adapter
{
    public abstract class AdapterFactory
    {
        public static IAdapter Get(string setting)
        {
            int seperate = setting.IndexOf(":");
            string adaptertype =setting.Substring(0, seperate);
            string adaptersetting = setting.Substring(seperate+1,setting.Length-seperate-1);
            return (IAdapter)Activator.CreateInstance(Type.GetType(adaptertype), adaptersetting);
        }
    }
}
