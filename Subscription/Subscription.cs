using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Subscription.Filter;
using Subscription.Adapter;

namespace Subscription
{
    public class Subscription
    {

        public readonly IAdapter postbackAdapter;
        public readonly IFilter filter;
        public Subscription(IFilter filter,IAdapter postbackAdapter)
        {
          
            this.filter = filter;
            this.postbackAdapter = postbackAdapter;
        }
        public void Process(dynamic bo)
        {
            try
            {
                if (filter.IsMatch(bo))
                {
                    postbackAdapter.SendMessage(bo);
                }
            }
            catch (Exception)
            {// handle exception
                
            }
        }
    }
}
