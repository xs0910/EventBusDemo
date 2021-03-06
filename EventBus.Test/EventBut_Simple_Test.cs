using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace EventBus.Test
{
    public class EventBut_Simple_Test : EventBusTestBase
    {
        [Fact]
        public void Should_Call_Handler_On_Event_Has_Registered()
        {
            TestEventBus.Register<TestEventData>(new TestEventHandler());

            TestEventBus.Trigger<TestEventData>(new TestEventData(1));

            TestEventHandler.TestValue.ShouldBe(1);
        }

        [Fact]
        public void Should_Auto_Call_Without_Registered()
        {
            // Assert.Throws<KeyNotFoundException>(() => { TestEventBus.Trigger<TestEventData>(new TestEventData(1)); });
            TestEventBus.Trigger<TestEventData>(new TestEventData(1));
            TestEventHandler.TestValue.ShouldBe(1);
        }

        [Fact]
        public void Should_Not_Trigger_After_UnRegistered()
        {
            TestEventBus.UnRegister<TestEventData>(typeof(TestEventHandler));

            TestEventBus.Trigger<TestEventData>(new TestEventData(2));

            TestEventHandler.TestValue.ShouldBe(0);
        }
    }
}
