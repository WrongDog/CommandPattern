using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SubscriptionControl
{
    [Serializable]
    public class SubscriptionControl 
    {
        public string Type { get; set; }
        public string Filter { get; set; }
        public string PostBack { get; set; }
        public bool AddSubscription { get; set; }
        public override string ToString()
        {
            return String.Format("Type:{0},Filter:{1},PostBack:{2}", this.Type, Filter, PostBack);
        }
        
    }
}
