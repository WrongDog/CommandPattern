using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Subscription.Adapter
{
    public class FileAdapter:IAdapter
    {
        protected string filename;
        public FileAdapter(string filename)
        {
            this.filename = filename;
        }
        public string AdapterString
        {
            get { return filename; }
        }

        public bool SendMessage(object bo)
        {
            try
            {
                string text = string.Format("{0}", bo);
                using (StreamWriter sout = new StreamWriter(filename))
                {
                    sout.WriteLine(text);
                    sout.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
