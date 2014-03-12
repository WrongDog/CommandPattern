using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageQueueLibrary;
using System.Configuration;
namespace SubscriptionControlConsole
{
    class Program
    {
        static string subscriptionControlQueuePath = @".\Private$\controlbridgequeue.q";
        static void Main(string[] args)
        {
            subscriptionControlQueuePath =  ConfigurationManager.AppSettings["SubscriptionControlQueuePath"];
            MessageQueueBase mq = MessageQueueFactory.Create(subscriptionControlQueuePath, new Type[] { typeof(SubscriptionControl.SubscriptionControl)});
            SubscriptionControl.SubscriptionControl control = new SubscriptionControl.SubscriptionControl();
            control.Type = "FakeBo1";
            control.PostBack = "Subscription.Adapter.ConsoleAdapter:Process" + System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
            while (true)
            {
                Console.WriteLine("Quit?");
                if (Console.ReadLine().Trim().ToLower() != "y")
                {
                    Console.WriteLine("Filter?");
                    control.Filter = Console.ReadLine();
                    Console.WriteLine("Add new?");
                    control.AddSubscription = Convert.ToBoolean(Console.ReadLine());
                    Console.WriteLine("Post back:{0}", control.PostBack);
                    string tmppostback = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(tmppostback)) control.PostBack = tmppostback;
                    mq.Send(control);
                }
            }
        }
    }
}
