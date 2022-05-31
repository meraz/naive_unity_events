using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meraz.Event
{
    public class EventManager : MonoBehaviour
    {
        private Dictionary<EventType, List<Event>> queuedEvents = new Dictionary<EventType, List<Event>>();
        private Dictionary<EventType, List<EventHandle>> listeners = new Dictionary<EventType, List<EventHandle>>();

        public bool QueueEvent(Event eventToQueue)
        {
            if (eventToQueue.eventType != EventType.Unset)
            {
                List<Event> events;
                if (queuedEvents.TryGetValue(eventToQueue.eventType, out events))
                {
                    events.Add(eventToQueue);
                }
                else
                {
                    events = new List<Event>
                {
                    eventToQueue
                };
                    queuedEvents.Add(eventToQueue.eventType, events);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public EventHandle RegisterListener(EventType eventType, Action<Event> callback)
        {
            EventHandle handle = new EventHandle(eventType, callback);

            List<EventHandle> handles;
            if (listeners.TryGetValue(eventType, out handles))
            {
                handles.Add(handle);
            }
            else
            {
                handles = new List<EventHandle>
            {
                handle
            };
                listeners.Add(eventType, handles);
            }
            return handle;
        }

        public void Deregister(EventHandle handle)
        {
            if (listeners.ContainsKey(handle.eventType))
            {
                listeners[handle.eventType].Remove(handle);
            }
        }

        void LateUpdate()
        {
            var processingEvents = new Dictionary<EventType, List<Event>>();

            foreach(EventType key in queuedEvents.Keys)
            {
                processingEvents.Add(key, queuedEvents[key]);
            }
            queuedEvents.Clear();

            foreach (KeyValuePair<EventType, List<Event>> eventList in processingEvents)
            {
                foreach (Event e in eventList.Value)
                {
                    foreach (EventHandle handle in listeners[eventList.Key])
                    {
                        try
                        {
                            handle.action(e);
                        }
                        catch (System.Exception)
                        {
                            // TODO meraz remove action in question
                        }
                    }
                }
            }
        }
    }

}