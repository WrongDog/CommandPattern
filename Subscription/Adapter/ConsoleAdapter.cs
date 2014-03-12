using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscription.Adapter
{
    public class ConsoleAdapter:IAdapter
    {
        string setting;
        public ConsoleAdapter(string setting)
        {
            this.setting = setting;
        }
        public bool SendMessage(object bo)
        {
            //System.Threading.Thread.Sleep(1000);
            Console.WriteLine("{0},{1}",setting,bo);
            return true;
        }

        public string AdapterString
        {
            get { return setting; }
        }
    }
}
