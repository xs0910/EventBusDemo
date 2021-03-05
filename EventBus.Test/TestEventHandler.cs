using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Test
{
    public class TestEventHandler : IEventHandler<TestEventData>
    {
        public static int TestValue { get; set; }
        public void HandleEvent(TestEventData eventData)
        {
            TestValue = eventData.TestValue;
        }
    }
}
