using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Meraz.Event;
using System;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class EventManagerTest
{
    public class EventManagerTestInternal : EventManager, IMonoBehaviourTest
    {
        public bool IsTestFinished
        {
            get { return false; }
        }
    }

    [UnityTest]
    public IEnumerator QueueAndProcessEvent()
    {
        var internalEventManager = new MonoBehaviourTest<EventManagerTestInternal>();
        var eventManager = internalEventManager.component;

        bool succeed = false;
        Action<Meraz.Event.Event> action = (e) => { succeed = true; };
        eventManager.RegisterListener(Meraz.Event.EventType.AudioEvent, action);

        Meraz.Event.Event evenToQueue = new Meraz.Event.Event
        {
            eventType = Meraz.Event.EventType.AudioEvent
        };
        eventManager.QueueEvent(evenToQueue);
        
        Assert.IsFalse(succeed);

        yield return null; // Wait a frame

        Assert.IsTrue(succeed);
    }

    [UnityTest]
    public IEnumerator DeregisterBeforeCall()
    {
        var internalEventManager = new MonoBehaviourTest<EventManagerTestInternal>();
        var eventManager = internalEventManager.component;

        bool succeed = false;
        Action<Meraz.Event.Event> action = (e) => { succeed = true; };
        EventHandle handle = eventManager.RegisterListener(Meraz.Event.EventType.AudioEvent, action);

        Meraz.Event.Event evenToQueue = new Meraz.Event.Event
        {
            eventType = Meraz.Event.EventType.AudioEvent
        };
        eventManager.QueueEvent(evenToQueue);

        eventManager.Deregister(handle);

        Assert.IsFalse(succeed);

        yield return null; // Wait a frame

        Assert.IsFalse(succeed);
    }

    [UnityTest]
    public IEnumerator TwoEvents_OneListener()
    {
        var internalEventManager = new MonoBehaviourTest<EventManagerTestInternal>();
        var eventManager = internalEventManager.component;

        int called = 0;
        Action<Meraz.Event.Event> action = (e) => { called++; };
        eventManager.RegisterListener(Meraz.Event.EventType.AudioEvent, action);

        Meraz.Event.Event evenToQueue1 = new Meraz.Event.Event
        {
            eventType = Meraz.Event.EventType.AudioEvent
        };
        eventManager.QueueEvent(evenToQueue1);

        Meraz.Event.Event evenToQueue2 = new Meraz.Event.Event
        {
            eventType = Meraz.Event.EventType.AudioEvent
        };
        eventManager.QueueEvent(evenToQueue2);

        Assert.AreEqual(0, called);

        yield return null; // Wait a frame

        Assert.AreEqual(2, called);
    }

    [UnityTest]
    public IEnumerator OneEvent_TwoListener()
    {
        var internalEventManager = new MonoBehaviourTest<EventManagerTestInternal>();
        var eventManager = internalEventManager.component;

        bool call1 = false;
        Action<Meraz.Event.Event> action1 = (e) => { call1 = true; };
        eventManager.RegisterListener(Meraz.Event.EventType.AudioEvent, action1);

        bool call2 = false;
        Action<Meraz.Event.Event> action2 = (e) => { call2 = true; };
        eventManager.RegisterListener(Meraz.Event.EventType.AudioEvent, action2);

        Meraz.Event.Event evenToQueue = new Meraz.Event.Event
        {
            eventType = Meraz.Event.EventType.AudioEvent
        };
        eventManager.QueueEvent(evenToQueue);

        Assert.IsFalse(call1);
        Assert.IsFalse(call2);

        yield return null; // Wait a frame

        Assert.IsTrue(call1);
        Assert.IsTrue(call2);
    }

}
