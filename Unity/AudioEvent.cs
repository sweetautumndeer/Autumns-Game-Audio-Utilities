using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioEvent
{
    private EventReference _audioEvent;
    private EventInstance _eventInstance;

    EVENT_CALLBACK eventCallback;
    private Dictionary<EVENT_CALLBACK_TYPE, List<Action>> callbackHandler = new();

    AudioEvent(EventReference _audioEvent)
    {
        this._audioEvent = _audioEvent;
        InitCallbackHandler();
    }

    private void InitCallbackHandler()
    {
        eventCallback = new EVENT_CALLBACK(Callback);
    }

    void OnDestroy()
    {
        _eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _eventInstance.release();
    }

    public void Play()
    {
        if (_audioEvent.IsNull)
            return;

        _eventInstance = RuntimeManager.CreateInstance(_audioEvent);
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
        callbackHandler[EVENT_CALLBACK_TYPE.STARTED].Add(callback);
    }

    public void OnRestart(Action callback)
    {
        callbackHandler[EVENT_CALLBACK_TYPE.RESTARTED].Add(callback);
    }

    public void OnComplete(Action callback)
    {
        callbackHandler[EVENT_CALLBACK_TYPE.STOPPED].Add(callback);
    }

    public void OnBeat(Action callback)
    {
        callbackHandler[EVENT_CALLBACK_TYPE.TIMELINE_MARKER].Add(callback);
    }

    public void OnMarker(Action callback)
    {
        callbackHandler[EVENT_CALLBACK_TYPE.TIMELINE_BEAT].Add(callback);
    }
    #endregion

    public void SetCallback(EVENT_CALLBACK_TYPE type, Action callback)
    {
        callbackHandler[type].Add(callback);
    }

    private FMOD.RESULT Callback(EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters)
    {
        foreach (Action action in callbackHandler[type])
        {
            action.Invoke();
        }
        return FMOD.RESULT.OK;
    }
    #endregion
}
