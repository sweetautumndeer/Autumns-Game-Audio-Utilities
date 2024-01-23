/////////////////////////////////
// Autumn Moulios
// Last Updated 22 January 2024
//
//
/////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

[Serializable]
public class AudioEvent
{
    [SerializeField] private EventReference _event;
    private EventInstance _eventInstance;
    private Transform attenuationPoint;

    private EVENT_CALLBACK _eventCallback;
    private Dictionary<EVENT_CALLBACK_TYPE, UnityEvent> _callbackHandler;

    // Serialized callback references to appear in inspector, added to _callbackHandler on init.
    [Serializable]
    public struct AudioEventCallbacks
    {
        public UnityEvent onStart;
        public UnityEvent onRestart;
        public UnityEvent onCompleted;
        public UnityEvent onBeat;
        public UnityEvent onMarker;
    }
    [SerializeField] private AudioEventCallbacks _callbacks;

    public AudioEvent()
    {
        InitCallbackHandler();
    }

    public AudioEvent(Transform attenuationPoint)
    {
        this.attenuationPoint = attenuationPoint;
        InitCallbackHandler();
    }

    public string GetName()
    {
        string[] path = _event.Path.Split("/");
        return path[path.Length - 1];
    }

    private void InitCallbackHandler()
    {
        _callbackHandler = new Dictionary<EVENT_CALLBACK_TYPE, UnityEvent>();
        _eventCallback = new EVENT_CALLBACK(Callback);

        // Initialize UnityEvents and add them to the callback dictionary.
        _callbacks.onStart = new();
        _callbackHandler[EVENT_CALLBACK_TYPE.STARTED] = _callbacks.onStart;
        _callbacks.onRestart = new();
        _callbackHandler[EVENT_CALLBACK_TYPE.RESTARTED] = _callbacks.onRestart;
        _callbacks.onCompleted = new();
        _callbackHandler[EVENT_CALLBACK_TYPE.STOPPED] = _callbacks.onCompleted;
        _callbacks.onBeat = new();
        _callbackHandler[EVENT_CALLBACK_TYPE.TIMELINE_BEAT] = _callbacks.onBeat;
        _callbacks.onMarker = new();
        _callbackHandler[EVENT_CALLBACK_TYPE.TIMELINE_MARKER] = _callbacks.onMarker;
    }


    void OnDestroy()
    {
        _eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _eventInstance.release();
    }

    // Summary:
    //      Play
    public void Play()
    {
        if (_event.IsNull)
            return;

        _eventInstance = RuntimeManager.CreateInstance(_event);
        if (attenuationPoint != null)
        {
            _eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(attenuationPoint));
        }
        _eventInstance.setCallback(_eventCallback);
        _eventInstance.start();
    }

    // Summary:
    //      aaaaaa
    public void Play(Transform attenuationPoint)
    {
        if (_event.IsNull)
            return;

        _eventInstance = RuntimeManager.CreateInstance(_event);
        _eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(attenuationPoint));
        _eventInstance.setCallback(_eventCallback);
        _eventInstance.start();
    }

    public void PlayOneShot()
    {
        if (_event.IsNull)
            return;

        _eventInstance = RuntimeManager.CreateInstance(_event);
        if (attenuationPoint != null)
        {
            _eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(attenuationPoint));
        }
        _eventInstance.setCallback(_eventCallback);
        _eventInstance.start();
        _eventInstance.release();
    }

    public void PlayOneShot(Transform attenuationPoint)
    {
        if (_event.IsNull)
            return;

        _eventInstance = RuntimeManager.CreateInstance(_event);
        _eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(attenuationPoint));
        _eventInstance.setCallback(_eventCallback);
        _eventInstance.start();
        _eventInstance.release();
    }

    public void Pause()
    {
        if (!_eventInstance.isValid())
            return;

        _eventInstance.setPaused(true);
    }

    public void Resume()
    {
        if (!_eventInstance.isValid())
            return;

        _eventInstance.setPaused(false);
    }

    public void Fadeout()
    {
        if (!_eventInstance.isValid())
            return;

        _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void Stop()
    {
        if (!_eventInstance.isValid())
            return;

        _eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public bool Playing
    {
        get
        {
            if (!_eventInstance.isValid())
                return false;

            _eventInstance.getPlaybackState(out PLAYBACK_STATE state);
            return (state != FMOD.Studio.PLAYBACK_STATE.STOPPED);
        }
    }

    #region Callbacks

    #region Abstracted Callback Functions
    public void OnStart(UnityAction callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.STARTED, callback);
    }

    public void OnRestart(UnityAction callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.RESTARTED, callback);
    }

    public void OnComplete(UnityAction callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.STOPPED, callback);
    }

    public void OnBeat(UnityAction callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.TIMELINE_MARKER, callback);
    }

    public void OnMarker(UnityAction callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT, callback);
    }
    #endregion

    public void SetCallback(EVENT_CALLBACK_TYPE type, UnityAction callback)
    {
        _callbackHandler.TryGetValue(type, out UnityEvent value);
        if (value == null)
        {
            _callbackHandler.Add(type, new UnityEvent());
        }

        _callbackHandler[type].AddListener(callback);
    }

    private FMOD.RESULT Callback(EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters)
    {
        _callbackHandler.TryGetValue(type, out UnityEvent value);
        if (value == null)
        {
            return FMOD.RESULT.OK;
        }

        _callbackHandler[type].Invoke();
        return FMOD.RESULT.OK;
    }
    #endregion
}
