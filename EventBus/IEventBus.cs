using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public interface IEventBus
    {
        void Register<TEventData>(IEventHandler eventHandler);
        void Register<TEventData>(Action<TEventData> action) where TEventData : IEventData;
        void UnRegister<TEventData>(IEventHandler eventHandler);
        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;
    }
}
