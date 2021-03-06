using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventBus.Test
{
    public abstract class EventBusTestBase
    {
        protected IEventBus TestEventBus;

        protected EventBusTestBase()
        {
            TestEventBus = new EventBus();
            TestEventBus.RegisterAllEventHandlerFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
