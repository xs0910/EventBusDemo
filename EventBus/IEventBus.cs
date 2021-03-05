using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public interface IEventBus
    {
        void Register<TEventData>(Type eventHandler);
        void UnRegister<TEventData>(Type eventHandler);
        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;
    }
}
