using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscription
{
    public class SubscriptionList
    {
        
        protected Dictionary<string, List<Subscription>> subscriptions = new Dictionary<string,List<Subscription>>();
        protected Filter.FilterPool filterPool = new Filter.FilterPool();
        public SubscriptionList()
        {
           
        }
        public void HandleControlMessage(SubscriptionControl.SubscriptionControl control)
        {
            if (control.AddSubscription)
            {
                AddSubscription(control.Type, control.Filter, control.PostBack);
            }
            else
            {
                RemoveSubscription(control.Type, control.Filter, control.PostBack);
            }
        }
        public void AddSubscription(string type, string filter,string postback)
        {
            lock (((IDictionary)subscriptions).SyncRoot)
            {
                List<Subscription> typedsubscriptions;
                if (subscriptions.ContainsKey(type))
                {
                    subscriptions.TryGetValue(type, out typedsubscriptions);
                }
                else
                {
                    typedsubscriptions = new List<Subscription>();
                    subscriptions.Add(type, typedsubscriptions);
                }
                typedsubscriptions.Add(new Subscription( filterPool.GetFilter(filter), Adapter.AdapterFactory.Get(postback)));
            }
        }
        public void RemoveSubscription(string type, string filter, string postback)
        {
            lock (((IDictionary)subscriptions).SyncRoot)
            {
                List<Subscription> typedsubscriptions;
                if (subscriptions.ContainsKey(type))
                {
                    subscriptions.TryGetValue(type, out typedsubscriptions);
                    typedsubscriptions.RemoveAll((s) => s.filter.FilterString == filter && s.postbackAdapter.AdapterString == postback);
             
                }
            }
        }
        public void RemoveSubscription(string postback)
        {
            lock (((IDictionary)subscriptions).SyncRoot)
            {
                foreach (List<Subscription> typedsubscriptions in subscriptions.Values)
                {
                    typedsubscriptions.RemoveAll((s) => s.postbackAdapter.AdapterString == postback);
                }
            }
        }
        public void Process(object bo)
        {           
            string boType = bo.GetType().Name;
            lock (((IDictionary)subscriptions).SyncRoot)
            {
                if (subscriptions.ContainsKey(boType))
                {
                    List<Subscription> matchedSubscriptions;
                    subscriptions.TryGetValue(boType, out matchedSubscriptions);

                    Parallel.ForEach<Subscription>(matchedSubscriptions, subscription => subscription.Process(bo));
                    //foreach (Subscription subscription in matchedSubscriptions) Task.Factory.StartNew(() => subscription.Process(bo));

                    //foreach (Subscription subscription in matchedSubscriptions) subscription.Process(bo);
                }
            }
        }
        public IEnumerable<SubscriptionControl.SubscriptionControl> GetAllSubscription()
        {
            lock (((IDictionary)subscriptions).SyncRoot)
            {
                foreach (KeyValuePair<string,List<Subscription>> kvpair in subscriptions)
                {
                    string type = kvpair.Key;
                    List<Subscription> subs= kvpair.Value;
                    foreach (Subscription sub in subs)
                    {
                        yield return new SubscriptionControl.SubscriptionControl(){Type = type,Filter= sub.filter.FilterString,PostBack= sub.postbackAdapter.AdapterString,AddSubscription = true};
                    }
                }
            }
        }
    }
}
