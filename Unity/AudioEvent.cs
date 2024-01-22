using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[Serializable]
public class AudioEvent
{
    [SerializeField] private EventReference _event;
    private EventInstance _eventInstance;

    private EVENT_CALLBACK eventCallback;
    private Dictionary<EVENT_CALLBACK_TYPE, List<Action>> callbackHandler;

    public AudioEvent()
    {
        InitCallbackHandler();
    }

    private void InitCallbackHandler()
    {
        callbackHandler = new Dictionary<EVENT_CALLBACK_TYPE, List<Action>>();
        eventCallback = new EVENT_CALLBACK(Callback);
    }

    void OnDestroy()
    {
        _eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _eventInstance.release();
    }

    public void Play()
    {
        if (_event.IsNull)
            return;

        _eventInstance = RuntimeManager.CreateInstance(_event);
        _eventInstance.setCallback(eventCallback);
        _eventInstance.start();
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

    public void Stop()
    {
        if (!_eventInstance.isValid())
            return;

        _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public bool IsPlaying()
    {
        if (!_eventInstance.isValid())
            return false;

        _eventInstance.getPlaybackState(out PLAYBACK_STATE state);
        if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            return true;

        return false;
    }

    #region Callbacks

    #region Abstracted Callback Functions
    public void OnStart(Action callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.STARTED, callback);
    }

    public void OnRestart(Action callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.RESTARTED, callback);
    }

    public void OnComplete(Action callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.STOPPED, callback);
    }

    public void OnBeat(Action callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.TIMELINE_MARKER, callback);
    }

    public void OnMarker(Action callback)
    {
        SetCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT, callback);
    }
    #endregion

    public void SetCallback(EVENT_CALLBACK_TYPE type, Action callback)
    {
        callbackHandler.TryGetValue(type, out List<Action> value);
        if (value == null)
        {
            callbackHandler.Add(type, new List<Action>());
        }

        callbackHandler[type].Add(callback);
    }

    private FMOD.RESULT Callback(EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters)
    {
        callbackHandler.TryGetValue(type, out List<Action> value);
        if (value == null)
        {
            return FMOD.RESULT.OK;
        }

        foreach (Action action in callbackHandler[type])
        {
            action.Invoke();
        }
        return FMOD.RESULT.OK;
    }
    #endregion
}
