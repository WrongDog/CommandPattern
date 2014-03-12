using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Package;
using System.Configuration;
using MessageQueueLibrary;

namespace SenderConsoleApplication
{
    
    class Program
    {
        static string queuePath = @".\Private$\myqueue.q";		
        static void Main(string[] args)
        {
            queuePath = ConfigurationManager.AppSettings["DataQueue"];
            Random rnd = new Random(DateTime.Now.Millisecond);
            int count = 1;
            MessageQueueBase mq = MessageQueueFactory.Create(queuePath,
              new Type[] { typeof(Package.MessagePackage) });
            while (true)
            {
                //bo.Value = rnd.Next(1000, 9999);
                //bo.Name = rnd.NextDouble().ToString();
                Parallel.For(0, count, (i) => mq.Send(MessagePackage.CreateFrom(new BOLibrary.FakeBo1() { Name = rnd.NextDouble().ToString(), Value = rnd.Next(1000, 9999), Value2 = rnd.Next(1000, 9999) })));
                
                Console.WriteLine("send {0}",count);
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 10));
            }
            
            //Console.ReadLine();
            
        }
        
    }
}
