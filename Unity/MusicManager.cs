using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<AudioEvent> musicEvents = new();
    private Dictionary<string, AudioEvent> musicEventsDict = new();
    private AudioEvent currentMusic;

    private void Start()
    {
        foreach (AudioEvent audioEvent in musicEvents)
        {
            
        }
    }

    public void PlayMusic(AudioEvent newMusic)
    {
        currentMusic = newMusic;
        currentMusic.Play();
    }

    public void StopCurrentMusic()
    {
        if (currentMusic != null)
        {
            currentMusic.Stop();
        }
    }

    public void FadeoutCurrentMusic()
    {
        if (currentMusic != null)
        {
            currentMusic.Fadeout();
        }
    }

    public void SwitchMusic(AudioEvent newMusic)
    {
        StopCurrentMusic();

        currentMusic = newMusic;
        currentMusic.Play();
    }

    public void CrossfadeMusic(AudioEvent newMusic)
    {
        FadeoutCurrentMusic();

        currentMusic = newMusic;
        currentMusic.Play();
    }

    public void SwitchAfterFadeout(AudioEvent newMusic)
    {
        if (currentMusic != null)
        {
            currentMusic.OnComplete(newMusic.Play);
            currentMusic.Fadeout();
        }
    }
}
