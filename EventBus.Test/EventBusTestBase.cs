using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Test
{
    public abstract class EventBusTestBase
    {
        protected IEventBus TestEventBus;

        public EventBusTestBase()
        {
            TestEventBus = new EventBus();
        }

    }
}
