using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

namespace MessageQueueLibrary
{
    public class MSMessageQueue:MessageQueueBase
    {
        protected Type[] paramTypes;
        protected bool running = false;
        public override event MessageQueueBase.MessageHandler OnMessageRecieved;
        public MSMessageQueue(string url, Type[] paramTypes)
            : base(url)
        {
            this.paramTypes = paramTypes;
            if (!MessageQueue.Exists(url)) MessageQueue.Create(url);
        }
        public override bool Send(object obj)
        {
            try
            {
                // open the queue
                MessageQueue mq = new MessageQueue(url);
                // set the message to durable.
                mq.DefaultPropertiesToSend.Recoverable = true;
                // set the formatter to Binary if needed, default is XML
                //mq.Formatter = new BinaryMessageFormatter();
                // send the job object
                mq.Send(obj, "Message Test");
                mq.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public override bool Listen(int numberOfSimultaneousRequests=1)
        {
            MessageQueue mqReceive = new MessageQueue(url);
            MessageQueue.EnableConnectionCache = false;
            // Control Queue permissions here:
            mqReceive.SetPermissions("Everyone",
                MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
            Console.WriteLine(mqReceive.MulticastAddress);
            mqReceive.Formatter = new XmlMessageFormatter(paramTypes);
            //mqReceive.Formatter = new BinaryMessageFormatter();
            //int numberOfSimultaneousRequests = 1;
            try
            {
                for (int i = 0; i < numberOfSimultaneousRequests; i++) mqReceive.BeginReceive(TimeSpan.FromSeconds(5),
                     mqReceive, new AsyncCallback(OnMessageArrival));
                running = true;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private void OnMessageArrival(IAsyncResult ar)
        {
            MessageQueue mq = (MessageQueue)ar.AsyncState;
            try
            {

                
                Message msg = mq.EndReceive(ar);
                OnMessageRecieved(msg.Body);
                
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    //Log Error

                }
            }
            catch (Exception e)
            {
               
            }
            finally
            {
                if(running)mq.BeginReceive(TimeSpan.FromSeconds(5), mq, new AsyncCallback(OnMessageArrival));
            }
        }



        public override void Stop()
        {
            running = false;
        }

       
    }
}
