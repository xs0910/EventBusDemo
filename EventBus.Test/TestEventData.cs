using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Test
{
    public class TestEventData : EventData
    {
        public int TestValue { get; set; }

        public TestEventData(int data)
        {
            TestValue = data;
        }
    }
}
