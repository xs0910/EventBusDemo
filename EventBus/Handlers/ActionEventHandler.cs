using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    internal class ActionEventHandler<TEventData> : IEventHandler<TEventData> where TEventData : IEventData
    {
        public Action<TEventData> Action { get; private set; }

        public ActionEventHandler(Action<TEventData> handler)
        {
            Action = handler;
        }

        public void HandleEvent(TEventData eventData)
        {
            Action(eventData);
        }
    }
}
