using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

/*
 * How To Use? Tutorial
 * When creating a new event, please create an event based on AnEvent class.
 * Then when adding Listener add according to example in OnEnable()->  EventManager.INSTANCE.AddListener<ADeEvent>(this.yourmethodname));
 * Remember to add RemoveListener in OnDisable by replacing AddListener to RemoveListener
 * After that, When you trying to call event, please use AddEvent([YourEventClass]). Example -> EventManager.INSTANCE.AddEvent(new BDeEvent("If you have parameter"));
 * That's all!
 * 
 * Refer to AnEvent to create an event
 * 
 */

namespace KC_Custom
{
    public enum EEVENTRESULT
    {
        EVENT_NOTIFY_SUCCESS = 0,
        EVENT_NOTIFY_FAILED___ON_SCENE_CHANGE,
        EVENT_NOTIFY_FAILED___NO_LISTENER,
        EVENT_NOTIFY_FAILED___ON_APPLICATION_QUIT
    }

    public class EventManager : MonobehaviourSingleton<EventManager>
    {
        private Dictionary<System.Type, EventAction> m_listenerDict = new Dictionary<System.Type, EventAction>();
        private Dictionary<Delegate, EventAction> m_lookupDict = new Dictionary<Delegate, EventAction>();

        private static bool _On_PAUSED = false;
        private static bool _ON_APPLICATION_QUIT = false;

        //==================================================

        #region OVERRIDE

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            this.Clear();
            base.OnDisable();
        }

        protected override void OnApplicationPause(bool pause)
        {
            base.OnApplicationPause(pause);
            _On_PAUSED = pause;
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            if (!_ON_APPLICATION_QUIT)
            {
                _ON_APPLICATION_QUIT = true;
            }
        }

        #endregion OVERRIDE

        //==================================================

        #region FUNCTIONS

        //Public Function Below

        public static EEVENTRESULT AddEvent(AnEvent evt)
        {
            if (_ON_APPLICATION_QUIT) { return EEVENTRESULT.EVENT_NOTIFY_FAILED___ON_APPLICATION_QUIT; }


            return GetInstance().AddEvent_Private(evt);
        }

        public static void AddListener<T>(GenericEventAction<T> listener) where T : AnEvent
        {
            if (_ON_APPLICATION_QUIT) { return; }
            GetInstance().AddListener_Private<T>(listener);
        }

        public static void RemoveListener<T>(GenericEventAction<T> listener) where T : AnEvent
        {
            //if ( _ON_SCENE_CHANGE || _ON_APPLICATION_QUIT ) { return; }
            GetInstance().RemoveListener_Private<T>(listener);
        }

        //Private Function Below

        private EEVENTRESULT AddEvent_Private(AnEvent evt)
        {
            EventAction listener;
            if (!this.m_listenerDict.TryGetValue(evt.GetType(), out listener))
            {
                Debug.Log("[EventManager WARNING] Add Event Failed: " + evt.GetType());
                return EEVENTRESULT.EVENT_NOTIFY_FAILED___NO_LISTENER;
            }

            listener.Invoke(evt);
            return EEVENTRESULT.EVENT_NOTIFY_SUCCESS;
        }

        private void AddListener_Private<T>(GenericEventAction<T> listener) where T : AnEvent
        {
            // To avoid multiple registration of same listener
            if (this.m_lookupDict.ContainsKey(listener))
            { return; }

            EventAction actualListener = (evt) => listener(evt as T);
            this.m_lookupDict[listener] = actualListener;

            EventAction tempListener;
            if (this.m_listenerDict.TryGetValue(typeof(T), out tempListener))
            {
                this.m_listenerDict[typeof(T)] = tempListener += actualListener;
            }
            else
            {
                this.m_listenerDict[typeof(T)] = actualListener;
            }
        }

        private void RemoveListener_Private<T>(GenericEventAction<T> listener) where T : AnEvent
        {
            EventAction actualListener;
            if (this.m_lookupDict.TryGetValue(listener, out actualListener))
            {
                EventAction tempListener;
                if (this.m_listenerDict.TryGetValue(typeof(T), out tempListener))
                {
                    tempListener -= actualListener;
                    if (tempListener == null)
                    {
                        this.m_listenerDict.Remove(typeof(T));
                    }
                    else
                    {
                        this.m_listenerDict[typeof(T)] = tempListener;
                    }
                }
                this.m_lookupDict.Remove(listener);
            }
        }

        private void Clear()
        {
            this.m_lookupDict.Clear();
            this.m_listenerDict.Clear();
        }

        #endregion FUNCTIONS

        //==================================================
    }

}