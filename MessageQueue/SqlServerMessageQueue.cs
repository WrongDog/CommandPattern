using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageQueueLibrary
{
    public class SqlServerMessageQueue:MessageQueueBase
    {
        public override event MessageQueueBase.MessageHandler OnMessageRecieved;
        public SqlServerMessageQueue(string connection, Type[] paramTypes)
            : base(connection)
        {
        }
        public override bool Send(object obj)
        {
            throw new NotImplementedException();
        }

        public override bool Listen(int numberOfSimultaneousRequests)
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

       
    }
}
