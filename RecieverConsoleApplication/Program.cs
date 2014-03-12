using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using Package;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using Subscription;
using Subscription.Filter;
using MessageQueueLibrary;
using System.Configuration;

namespace RecieverConsoleApplication
{
    /// <summary>
    /// it may be better to add a feature using performancecounter to get private working set against filters count to trigger a "restart"
    /// which means start a new process and destroy current one
    /// </summary>
    class Program
    {
        static string queuePath = @".\Private$\myqueue.q";
        static string subscriptionControlQueuePath = @".\Private$\mycontrolqueue.q";
        static Assembly boassembly;
        static SubscriptionList subscriptionList = new SubscriptionList();
        static void Main(string[] args)
        {
            string codebase = Assembly.GetExecutingAssembly().CodeBase;
            codebase = codebase.Substring(8, codebase.Length - 8);
            boassembly = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(codebase), "BOLibrary.dll"));

            subscriptionControlQueuePath = ConfigurationManager.AppSettings["ControlQueue"];
            queuePath = ConfigurationManager.AppSettings["DataQueue"];
            Console.WriteLine("Controlqueue:{0}", subscriptionControlQueuePath);
           
            MessageQueueBase mq = MessageQueueFactory.Create(queuePath,
                new Type[] { typeof(Package.MessagePackage) });
            mq.OnMessageRecieved += new MessageQueueBase.MessageHandler(mq_OnMessageRecieved);
            mq.Listen(10);

            MessageQueueBase controlq = MessageQueueFactory.Create(subscriptionControlQueuePath,
              new Type[] { typeof(SubscriptionControl.SubscriptionControl) });
            controlq.OnMessageRecieved += new MessageQueueBase.MessageHandler(controlq_OnMessageRecieved);
            controlq.Listen(1);


            Console.ReadLine();
        }

        static void controlq_OnMessageRecieved(object messageContent)
        {
            SubscriptionControl.SubscriptionControl message = (SubscriptionControl.SubscriptionControl)messageContent;
            if (message != null)
            {
                Console.WriteLine("SubscriptionControl message recieved Type:{0},Filter:{1},PostBack:{2},Add new ? {3}", message.Type, message.Filter, message.PostBack, message.AddSubscription);
                subscriptionList.HandleControlMessage(message);
               
            }
        }

        static void mq_OnMessageRecieved(object messageContent)
        {
            Package.Package message = (Package.Package)messageContent;
            if (message != null)
            {
                Console.WriteLine("{0},{1}", message.CreateDateTime, message.Creator);
                Type type = GetType(message.PackageType);
                XmlSerializer xmlser = new XmlSerializer(type);
                dynamic bo = xmlser.Deserialize(new MemoryStream(Convert.FromBase64String(message.PackageData)));
                Console.WriteLine("Name{0},Value{1},Value2{2}",bo.Name, bo.Value,bo.Value2);
                subscriptionList.Process(bo);
            }
        }
        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            type = boassembly.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
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
                    Console.WriteLine("{0},{1}", message.CreateDateTime, message.Creator);
                    Type type = GetType(message.PackageType);
                    XmlSerializer xmlser = new XmlSerializer(type);
                    dynamic bo = xmlser.Deserialize(new MemoryStream(Convert.FromBase64String(message.PackageData)));
                    Console.WriteLine(bo.Value);
                    subscriptionList.Process(bo);
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
                Console.WriteLine(e.Message);
            }
            finally
            {
                mq.BeginReceive(TimeSpan.FromSeconds(5), mq, new AsyncCallback(OnMessageArrival));
            }
        }
    }
}
