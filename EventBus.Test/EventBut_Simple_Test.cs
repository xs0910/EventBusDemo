using Shouldly;
using System;
using Xunit;

namespace EventBus.Test
{
    public class EventBut_Simple_Test : EventBusTestBase
    {
        [Fact]
        public void Should_Call_Handler_On_Event_Has_Registered()
        {
            TestEventBus.Register<TestEventData>(typeof(TestEventHandler));

            TestEventBus.Trigger<TestEventData>(new TestEventData(1));
            TestEventHandler.TestValue.ShouldBe(1);
        }
    }
}
