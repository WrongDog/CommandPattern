using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Package;


namespace MSMQPerformanceTest
{
    class Program
    {
        static string QueuePath = @".\Private$\testqueue.q";
        static long sendcount = 0;
        static long recievecount = 0;
        static int delay = 5;
        static Action<object> Out = new Action<object>((s) => { System.Diagnostics.Trace.WriteLine(s); Console.WriteLine(s); });
        static Action<string, object, object> Out2 = new Action<string, object, object>((s,o1,o2) => { Out( String.Format(s,o1,o2)); });
        static void Main(string[] args)
        {
            
            // open the queue
            if (!MessageQueue.Exists(QueuePath)) MessageQueue.Create(QueuePath);
            MessageQueue mqReceive = new MessageQueue(QueuePath);
            MessageQueue.EnableConnectionCache = false;
            // Control Queue permissions here:
            mqReceive.SetPermissions("Everyone",
                MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
            Console.WriteLine(mqReceive.MulticastAddress);
            Type[] paramTypes = new Type[1];
            paramTypes[0] = typeof(Package.MessagePackage);
            mqReceive.Formatter = new XmlMessageFormatter(paramTypes);
            //mqReceive.Formatter = new BinaryMessageFormatter();
			int numberOfSimultaneousRequests=10;	 
            try
            {
                for (int i = 0; i < numberOfSimultaneousRequests; i++)mqReceive.BeginReceive(TimeSpan.FromSeconds(5),
                    mqReceive, new AsyncCallback(OnMessageArrival));

            }
            catch (Exception e)
            {
                Out(e);
            }

            sendcount = 1000;
            Out2(" Send {0} message to MQ and recieve with {1} ", sendcount, numberOfSimultaneousRequests);
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Parallel.For(0, sendcount, (i) => QueueJob(QueuePath, Package.MessagePackage.CreateFrom(new BOLibrary.FakeBo1())));   
            watch.Stop();         
            Out2("Parallel send,               recieved {0} in {1} ms", Interlocked.Read(ref recievecount), watch.ElapsedMilliseconds);

            //Interlocked.Exchange(ref recievecount, 0);
            //watch.Start();
            //ThreadPool.QueueUserWorkItem(
            //    o=>QueueJob(QueuePath, Package.MessagePackage.CreateFrom(new BOLibrary.FakeBo1()))                 
            //    );
            //watch.Stop();
            //Out("ThreadPool, recieve{0} in {1} ms", Interlocked.Read(ref recievecount), watch.ElapsedMilliseconds);
           

            Interlocked.Exchange(ref recievecount, 0);
            watch.Start();
            for (int idx = 0; idx < sendcount; idx++)
            {
                QueueJob(mqReceive, Package.MessagePackage.CreateFrom(new BOLibrary.FakeBo1()));
            }
            watch.Stop();
            Out2("Serial send not creating mq, recieved {0} in {1} ms", Interlocked.Read(ref recievecount), watch.ElapsedMilliseconds);

            Interlocked.Exchange(ref recievecount, 0);
            watch.Start();
            for (int idx = 0; idx < sendcount; idx++)
            {
                QueueJob(QueuePath, Package.MessagePackage.CreateFrom(new BOLibrary.FakeBo1()));
            }
            watch.Stop();
            Out2("Serial send,                 recieved {0} in {1} ms", Interlocked.Read(ref recievecount), watch.ElapsedMilliseconds);
            Console.ReadLine();
            mqReceive.Close();
        }
        /// <summary>
        /// not thread safe
        /// </summary>
        /// <param name="mq"></param>
        /// <param name="message"></param>
        private static void QueueJob(MessageQueue mq, MessagePackage message)
        {
            try
            {
            
                // send the job object
                mq.Send(message, "Message Test");
                //System.Threading.Interlocked.Increment(ref sendcount);
               
            }
            catch (Exception e)
            {
                Out(e.ToString());

            }

        }
        private static void QueueJob(string destinationQueue ,MessagePackage message)
        {
            try
            {
                // open the queue
                MessageQueue mq = new MessageQueue(destinationQueue);
                // set the message to durable.
                mq.DefaultPropertiesToSend.Recoverable = true;
                // set the formatter to Binary, default is XML
                //mq.Formatter = new BinaryMessageFormatter();
                // send the job object
                mq.Send(message, "Message Test");
                mq.Close();
            }
            catch (Exception e)
            {
                Out(e.ToString());

            }

        }
        private static void OnMessageArrival(IAsyncResult ar)
        {
            MessageQueue mq = (MessageQueue)ar.AsyncState;
            Package.Package message;
            try
            {
              

                Message msg = mq.EndReceive(ar);
                message = (Package.Package)msg.Body;
                if (message != null)
                {
                    System.Threading.Interlocked.Increment(ref recievecount);
                    Thread.Sleep(delay);//simulate processing delay
      
                }
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
                Out(e.Message);
            }
            finally
            {
                mq.BeginReceive(TimeSpan.FromSeconds(5), mq, new AsyncCallback(OnMessageArrival));
            }
        }
    }
}
