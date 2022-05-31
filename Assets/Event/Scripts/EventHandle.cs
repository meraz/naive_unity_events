
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Meraz.Event
{
    public class EventHandle
    {
        public EventHandle(EventType eventType, Action<Event> action)
        {
            this.eventType = eventType;
            this.action = action;
            id = ++lastId;
        }

        static int lastId = 0;
        private readonly int id; // TODO meraz perhaps this is not needed
        public readonly EventType eventType;
        public readonly Action<Event> action;
    }
}