//#define FB_EVENTROUTER_THROWEXCEPTIONS 
#if FB_EVENTROUTER_THROWEXCEPTIONS
//#define EVENTROUTER_REQUIRELISTENER // Uncomment this if you want listeners to be required for sending events.
#endif

using System;
using UnityEngine;
using System.Collections.Generic;

namespace FancyUnity
{
	/// <summary>
	/// MMGameEvents are used throughout the game for general game events (game started, game ended, life lost, etc.)
	/// </summary>
	public struct GameEvent
	{
		public string EventName;
		public GameEvent(string newName)
		{
			EventName = newName;
		}
		static GameEvent e;
		public static void Trigger(string newName)
		{
			e.EventName = newName;
			FBEventSystem.TriggerEvent(e);
		}
	}

	/// <summary>
	/// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
	/// Events are structs, you can define any kind of events you want. This manager comes with MMGameEvents, which are 
	/// basically just made of a string, but you can work with more complex ones if you want.
	/// 
	/// To trigger a new event, from anywhere, do YOUR_EVENT.Trigger(YOUR_PARAMETERS)
	/// So MMGameEvent.Trigger("Save"); for example will trigger a Save MMGameEvent
	/// 
	/// you can also call MMEventManager.TriggerEvent(YOUR_EVENT);
	/// For example : MMEventManager.TriggerEvent(new MMGameEvent("GameStart")); will broadcast an MMGameEvent named GameStart to all listeners.
	///
	/// To start listening to an event from any class, there are 3 things you must do : 
	///
	/// 1 - tell that your class implements the MMEventListener interface for that kind of event.
	/// For example: public class GUIManager : Singleton<GUIManager>, MMEventListener<MMGameEvent>
	/// You can have more than one of these (one per event type).
	///
	/// 2 - On Enable and Disable, respectively start and stop listening to the event :
	/// void OnEnable()
	/// {
	/// 	this.MMEventStartListening<MMGameEvent>();
	/// }
	/// void OnDisable()
	/// {
	/// 	this.MMEventStopListening<MMGameEvent>();
	/// }
	/// 
	/// 3 - Implement the MMEventListener interface for that event. For example :
	/// public void OnMMEvent(MMGameEvent gameEvent)
	/// {
	/// 	if (gameEvent.EventName == "GameOver")
	///		{
	///			// DO SOMETHING
	///		}
	/// } 
	/// will catch all events of type MMGameEvent emitted from anywhere in the game, and do something if it's named GameOver
	/// </summary>
	[ExecuteAlways]
	public static class FBEventSystem
	{
		private static Dictionary<Type, List<EventListenerBase>> _subscribersList;

		static FBEventSystem()
		{
			_subscribersList = new Dictionary<Type, List<EventListenerBase>>();
		}

		/// <summary>
		/// Adds a new subscriber to a certain event.
		/// </summary>
		/// <param name="listener">listener.</param>
		/// <typeparam name="Event">The event type.</typeparam>
		public static void AddListener<Event>(IEventListener<Event> listener) where Event : struct
		{
			Type eventType = typeof(Event);
			
			if (!_subscribersList.ContainsKey(eventType))
				_subscribersList[eventType] = new List<EventListenerBase>();

			if (!SubscriptionExists(eventType, listener))
				_subscribersList[eventType].Add(listener);
		}

		public static void AddListener<Event>(Action callback) where Event : struct
		{
			AddListener(new SimpleEventListener<Event>(callback));
		}


		public static void AddListener<Event,TData>(Action<TData> callback,TData data) where Event : struct
		{
			AddListener(new EventListenerWithData<Event,TData>(callback,data));
		}

		/// <summary>
		/// Removes a subscriber from a certain event.
		/// </summary>
		/// <param name="listener">listener.</param>
		/// <typeparam name="Event">The event type.</typeparam>
		public static void RemoveListener<Event>(IEventListener<Event> listener) where Event : struct
		{
			Type eventType = typeof(Event);

			if (!_subscribersList.ContainsKey(eventType))
			{
#if FB_EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
				return;
#endif
			}

			List<EventListenerBase> subscriberList = _subscribersList[eventType];

#if FB_EVENTROUTER_THROWEXCEPTIONS
	            bool listenerFound = false;
#endif

			for (int i = 0; i < subscriberList.Count; i++)
			{
				if (subscriberList[i] == listener)
				{
					subscriberList.Remove(subscriberList[i]);
#if FB_EVENTROUTER_THROWEXCEPTIONS
					    listenerFound = true;
#endif

					if (subscriberList.Count == 0)
					{
						_subscribersList.Remove(eventType);
					}

					return;
				}
			}

#if FB_EVENTROUTER_THROWEXCEPTIONS
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		        }
#endif
		}

		/// <summary>
		/// Triggers an event. All instances that are subscribed to it will receive it (and will potentially act on it).
		/// </summary>
		/// <param name="newEvent">The event to trigger.</param>
		/// <typeparam name="Event">The 1st type parameter.</typeparam>
		public static void TriggerEvent<Event>(Event newEvent) where Event : struct
		{
			List<EventListenerBase> list;
			if (!_subscribersList.TryGetValue(typeof(Event), out list))
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
				return;
#endif

			for (int i = 0; i < list.Count; i++)
			{
				(list[i] as IEventListener<Event>).OnEvent(newEvent);
			}
		}

		/// <summary>
		/// Checks if there are subscribers for a certain type of events
		/// </summary>
		/// <returns><c>true</c>, if exists was subscriptioned, <c>false</c> otherwise.</returns>
		/// <param name="type">Type.</param>
		/// <param name="receiver">Receiver.</param>
		private static bool SubscriptionExists(Type type, EventListenerBase receiver)
		{
			List<EventListenerBase> receivers;

			if (!_subscribersList.TryGetValue(type, out receivers)) return false;

			bool exists = false;

			for (int i = 0; i < receivers.Count; i++)
			{
				if (receivers[i] == receiver)
				{
					exists = true;
					break;
				}
			}

			return exists;
		}
	}

	/// <summary>
	/// Static class that allows any class to start or stop listening to events
	/// </summary>
	public static class EventListenerExt
	{
		public delegate void Delegate<T>(T eventType);

		public static void EnableEvents<EventType>(this IEventListener<EventType> caller) where EventType : struct
		{
			FBEventSystem.AddListener(caller);
		}

		public static void DisableEvents<EventType>(this IEventListener<EventType> caller) where EventType : struct
		{
			FBEventSystem.RemoveListener(caller);
		}
	}

	/// <summary>
	/// Event listener basic interface
	/// </summary>
	public interface EventListenerBase { };

	/// <summary>
	/// A public interface you'll need to implement for each type of event you want to listen to.
	/// </summary>
	public interface IEventListener<T> : EventListenerBase
	{
		void OnEvent(T evt);
	}

	public class SimpleEventListener<Event> : IEventListener<Event>
	{
		protected Action _callback;

		public SimpleEventListener(Action callback)
		{
			_callback = callback;
		}

		public virtual void OnEvent(Event eventType)
        {
			_callback?.Invoke();
        }
    }

	public class EventListenerWithData<Event,TData> : IEventListener<Event>
	{
		protected Action<TData> _callback;
		protected TData _data;
		public EventListenerWithData(Action<TData> callback, TData data)
		{
			_callback = callback;
			_data = data;
		}
		public void OnEvent(Event eventType)
		{
			_callback?.Invoke(_data);
		}
    }
}