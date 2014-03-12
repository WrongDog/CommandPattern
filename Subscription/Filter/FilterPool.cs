using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscription.Filter
{
    public class FilterPool
    {
        Dictionary<string, IFilter> filters = new Dictionary<string, IFilter>();
        public IFilter GetFilter(string filter)
        {
            IFilter storedFilter;
            if (filters.ContainsKey(filter))
            {
                filters.TryGetValue(filter, out storedFilter);        
            }
            else
            {
                storedFilter = new DynamicExpressionFilter(filter);
                filters.Add(filter, storedFilter);
            }
            return storedFilter;
        }
    }
}
