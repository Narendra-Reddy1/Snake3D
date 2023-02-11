// This is a C# Event Handler (notification center) for Unity. It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other.

using System;
using System.Collections.Generic;

namespace SnakeGame
{
    // These are callbacks (delegates) that can be used by the messengers defined in EventHandler class below
    public delegate void Callback(Object arg);
    public delegate object CallbackWithReturnType(Object args);

    /*** A handler for events that have one parameter of type T. ***/
    public static class GlobalEventHandler
    {

        private static Dictionary<EventID, Delegate> eventTable = new Dictionary<EventID, Delegate>();
        public static void AddListener(EventID eventType, Callback handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Create an entry for this event type if it doesn't already exist.
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, null);
                }
                // Add the handler to the event.
                eventTable[eventType] = (Callback)eventTable[eventType] + handler;
            }
        }
        public static void RemoveListener(EventID eventType, Callback handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Only take action if this event type exists.
                if (eventTable.ContainsKey(eventType))
                {
                    // Remove the event handler from this event.
                    eventTable[eventType] = (Callback)eventTable[eventType] - handler;

                    // If there's nothing left then remove the event type from the event table.
                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }
        public static void AddListener(EventID eventType, CallbackWithReturnType handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Create an entry for this event type if it doesn't already exist.
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, null);
                }
                // Add the handler to the event.
                //If there are multiple methods bind to the same event then the last binded method's returned value will be sent.
                eventTable[eventType] = (CallbackWithReturnType)eventTable[eventType] + handler;
            }
        }

        public static void RemoveListener(EventID eventType, CallbackWithReturnType handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Only take action if this event type exists.
                if (eventTable.ContainsKey(eventType))
                {
                    // Remove the event handler from this event.
                    eventTable[eventType] = (CallbackWithReturnType)eventTable[eventType] - handler;

                    // If there's nothing left then remove the event type from the event table.
                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }
        public static void TriggerEvent(EventID eventType, System.Object arg = null)
        {
            Delegate d;
            // Invoke the delegate only if the event type is in the dictionary.
            if (eventTable.TryGetValue(eventType, out d))
            {
                // Take a local copy to prevent a race condition if another thread
                // were to unsubscribe from this event.
                Callback callback = (Callback)d;

                // Invoke the delegate if it's not null.
                if (callback != null)
                {
                    callback(arg);
                }
            }
        }
        public static object TriggerEventForReturnType(EventID eventType, object arg = null)
        {
            Delegate d;
            object returnValue = null;
            // Invoke the delegate only if the event type is in the dictionary.
            if (eventTable.TryGetValue(eventType, out d))
            {
                // Take a local copy to prevent a race condition if another thread
                // were to unsubscribe from this event.
                CallbackWithReturnType callback = (CallbackWithReturnType)d;

                // Invoke the delegate if it's not null.
                if (callback != null)
                {
                    returnValue = callback(arg);
                }
            }
            //If there are multiple methods bind to the same event then the last binded method's returned value will be sent.
            return returnValue;
        }

        public static void CleanUpTable()
        {
            eventTable.Clear();
        }
    }
    public enum EventID
    {
        EVENT_ON_SWIPE_DETECTED,
        EVENT_FOOD_COLLECTED,
        EVENT_COLLIDED_TO_OBSTACLE,
        REQUEST_PHOTON_TO_CONNECT_MASTER_SERVER,
        EVENT_PHOTON_CONNECTED_TO_MASTER_SERVER,
        REQUEST_PHOTON_TO_CREATE_ROOM,
        REQUEST_PHOTON_TO_JOIN_ROOM,
        EVENT_ON_ROOM_CREATED,
        EVENT_ON_ROOM_CREATION_FAILED,
        EVENT_ON_PLAYER_JOINED_ROOM,
        EVENT_PLAYER_LEFT_ROOM,
        EVENT_ON_ROOM_JOIN_FAILED,
    }
}