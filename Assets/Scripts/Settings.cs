using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings {


    public static bool SoundEffects = true;
    public static bool Music = true;

    public static string GameMode;

    public static void ToggleMusic()
    {
        Music = !Music;
        foreach (SoundManager sm in GameObject.FindObjectsOfType<SoundManager>())
        {
            sm.ToggleMusic(Music);
        }
    }

    public static void ToggleSounds()
    {
        SoundEffects = !SoundEffects;
        MonoBehaviour.print("Settings.ToggleSounds(" + SoundEffects + ")");
        foreach (SoundManager sm in GameObject.FindObjectsOfType<SoundManager>())
        {
            sm.ToggleSounds(SoundEffects);
        }
    }
    
}
