using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;



public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;
    
    Resolution [] resolutions;

    void Start()
    {
       resolutions = Screen.resolutions;
       
       resolutionDropdown.ClearOptions();

       List<string> options = new List<string>();
       int currentResolutionIndex = 0;
       for (int i = 0; i < resolutions.Length; i++)
       {
           string option = resolutions[i].width + " x " + resolutions[i].height;
           options.Add(option);

           if (resolutions[i].width == Screen.currentResolution.width &&
               resolutions[i].height == Screen.currentResolution.height)
           {
               currentResolutionIndex = i;
           }
       }

       resolutionDropdown.AddOptions(options);
       resolutionDropdown.value = currentResolutionIndex;
       resolutionDropdown.RefreshShownValue();
       
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volump",volume);
    }

    public void SetQuality(int qualin)
    {
        QualitySettings.SetQualityLevel(qualin);
    }

    public void Setfullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    public void Setresolution(int resolindex)
    {
        Resolution resolution = resolutions[resolindex];
        
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
