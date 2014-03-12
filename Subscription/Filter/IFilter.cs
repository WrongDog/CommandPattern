using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscription.Filter
{
    
    public interface IFilter
    {
        string FilterString { get; }
        bool IsMatch(object obj);
    }
}
