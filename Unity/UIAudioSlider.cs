using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAudioSlider : MonoBehaviour
{
    [Tooltip("\"bus:/[vcaName]\"")]
    public string busPath;
    private FMOD.Studio.Bus bus;

    [SerializeField] private string displayNameOverride = "";
    [SerializeField] private GameObject nameObj;

    [SerializeField] private GameObject valueObj;

    public void Start()
    {
        bus = FMODUnity.RuntimeManager.GetBus(busPath);
        nameObj.GetComponent<TMP_Text>().text = ParseDisplayName();
        InitVolume();
    }

    // Save settings to PlayerPrefs on destroy.
    private void OnDestroy()
    {
        bus.getVolume(out float volume);
        PlayerPrefs.SetFloat(busPath, volume);
    }

    // Display name will be the same as the FMOD Bus, unless displayNameOverride exists.
    private string ParseDisplayName()
    {
        if (displayNameOverride != "")
        {
            return displayNameOverride;
        }

        string displayName = "";

        string[] parts = busPath.Split('/');
        if (parts.Length == 2 && parts[1] == "") displayName = "Master"; // "bus:/" = Master bus
        else if (parts.Length >= 2) displayName = parts[parts.Length - 1];

        return displayName;
    }

    // Load saved volume, or default to bus volume if none is saved.
    private void InitVolume()
    {
        float volume;

        if (PlayerPrefs.HasKey(busPath))
        {
            volume = PlayerPrefs.GetFloat(busPath);
            bus.setVolume(volume);
        }
        else
        {
            bus.getVolume(out volume);
        }
        
        GetComponent<Slider>().value = volume;
    }

    // Sliders should be 0% - 200% volume (default 100%)
    public void UpdateVolume(System.Single volume)
    {
        bus.setVolume(volume);
    }

    public void UpdateText(System.Single value)
    {
        // Convert from range 0 - 2 to range 0 - 200
        valueObj.GetComponent<TMP_Text>().text = (value * 100).ToString("f0") + "%";
    }
}
