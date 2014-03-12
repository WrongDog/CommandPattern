using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageQueueLibrary;
using System.Configuration;

namespace SubscriptionControlBridgeConsoleApplication
{
    class Program
    {
        static string defaultreciever = @".\Private$\mycontrolqueue.q";
        static string subscriptionControlQueuePath = @".\Private$\controlbridgequeue.q";
        //pass subscription to other server and pass new changes
        static Dictionary<string, MessageQueueBase> recievers = new Dictionary<string, MessageQueueBase>();
        //static SubscriptionList subscriptionList = new SubscriptionList();//too heavy here
        static Dictionary<string, SubscriptionControl.SubscriptionControl> subscriptionList = new Dictionary<string, SubscriptionControl.SubscriptionControl>();
        static void Main(string[] args)
        {
            defaultreciever = ConfigurationManager.AppSettings["DefaultReciever"];
            subscriptionControlQueuePath = ConfigurationManager.AppSettings["SubscriptionControlQueuePath"];
            LoadSubscriptionList(subscriptionList);
            recievers.Add(defaultreciever, MessageQueueFactory.Create(defaultreciever, new Type[] { typeof(SubscriptionControl.SubscriptionControl) }));
            MessageQueueBase mq = MessageQueueFactory.Create(subscriptionControlQueuePath, new Type[] { typeof(SubscriptionControl.SubscriptionControl) });
            mq.OnMessageRecieved += new MessageQueueBase.MessageHandler(mq_OnMessageRecieved);
            mq.Listen(1);
            while (true)
            {
                string newreciever = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(newreciever)) return;
                if (!recievers.Keys.Contains(newreciever))
                {
                    MessageQueueBase newmq = MessageQueueFactory.Create(newreciever, new Type[] { typeof(SubscriptionControl.SubscriptionControl) });
                    foreach (SubscriptionControl.SubscriptionControl sub in subscriptionList.Values)
                    {                   
                        newmq.Send(sub);
                    }
                    recievers.Add(newreciever, newmq);
                    Console.WriteLine(newreciever + "Added");
                }
                else
                {
                    recievers.Remove(newreciever);
                    Console.WriteLine(newreciever + "Removed");
                }
            }
        }

        static void mq_OnMessageRecieved(object messageContent)
        {
            SubscriptionControl.SubscriptionControl controlmsg =(SubscriptionControl.SubscriptionControl)messageContent;
            string key = controlmsg.ToString();
            if (!controlmsg.AddSubscription)
            {
                subscriptionList.Remove(key);//also should have record removed from DB
            }
            else
            {
                if (!subscriptionList.ContainsKey(key)) subscriptionList.Add(key, controlmsg);//add record to db
            }
            Parallel.ForEach<MessageQueueBase>(recievers.Values, (mq) => mq.Send(messageContent));
        }
        static void LoadSubscriptionList(Dictionary<string, SubscriptionControl.SubscriptionControl> subscriptionList)
        {
            //load from DB
        }
    }
}
