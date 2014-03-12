using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscription.Adapter
{
    public interface IAdapter
    {
        string AdapterString { get; }
        bool SendMessage(object bo);
    }
}
