using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MessageQueueLibrary
{
    public class MessageQueueFactory
    {
        public static MessageQueueBase Create(string url,Type[] types)
        {
            string messageQueueType = ConfigurationManager.AppSettings["MessageQueueType"];
            return (MessageQueueBase)Activator.CreateInstance(Type.GetType(messageQueueType), url, types);
            //return new MSMessageQueue(url, types);
        }
    }
}
