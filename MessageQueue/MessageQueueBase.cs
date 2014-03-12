using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageQueueLibrary
{
    
    public abstract class MessageQueueBase
    {
        protected string url;
        public delegate void MessageHandler(object messageContent);
        public abstract event MessageHandler OnMessageRecieved;
        public MessageQueueBase(string url)
        {
            this.url = url;
        }
        public abstract bool Send(object obj);
        public abstract bool Listen(int numberOfSimultaneousRequests);
        public abstract void Stop();
    }
}
