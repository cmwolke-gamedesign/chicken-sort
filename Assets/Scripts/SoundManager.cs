using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioClip[] clips;
    private Dictionary<string, AudioSource> audioSources;
    private Dictionary<string, float> mutedMusicValues;
    private Dictionary<string, float> mutedSoundValues;
    // Use this for initialization
    void Start () {
        this.audioSources = new Dictionary<string, AudioSource>();
        this.mutedMusicValues = new Dictionary<string, float>();
        this.mutedSoundValues = new Dictionary<string, float>();
        foreach (AudioClip clip in clips)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.playOnAwake = false;
            this.audioSources.Add(clip.name, source); 
        }
        SendMessage("SoundsReady");
	}

    public void PlayAudio(string name, bool loop = false, float volume = 1f, float pitch = 1f)
    {
        /*EXISTS?*/
        if (!this.audioSources.ContainsKey(name))
        {
            print("sound '" + name + "' doesn't exist");
            return;
        }
        /* CHECK WETHER SOUNDS/MUSIC ARE MUTED AND ADD TO RESPECTIVE STORES */
        if (name.Contains("music") && !Settings.Music)
        {
            this.mutedMusicValues.Add(name, volume);
            volume = 0; 
        }
        if (!name.Contains("music") && !Settings.SoundEffects)
        {
            this.mutedSoundValues.Add(name, volume);
            volume = 0;
        }

        /* SET VALUES AND PLAY */
        AudioSource sourceToPlay = this.audioSources[name];
        sourceToPlay.loop = loop;
        sourceToPlay.volume = volume;
        sourceToPlay.pitch = pitch;
        sourceToPlay.Play();
    }

    public bool IsAudioPlaying(string name)
    {
        if (!this.audioSources.ContainsKey(name))
        {
            print("sound '" + name + "' doesn't exist");
            return false;
        }
        return this.audioSources[name].isPlaying;
    }
	
    public void ToggleMusic(bool set)
    {
        if (set == false)
        {
            foreach (AudioSource source in this.audioSources.Values) // just in case multiple music clips are playing
            {
                if (source.clip.name.Contains("music")) 
                {
                    this.mutedMusicValues.Add(source.clip.name, source.volume);
                    source.volume = 0f;
                }
            }
        } else
        {
            foreach(string key in this.mutedMusicValues.Keys)
            {
                this.audioSources[key].volume = this.mutedMusicValues[key];
            }
            this.mutedMusicValues = new Dictionary<string, float>();
        }
    }

    public void ToggleSounds(bool set)
    {
        print("Setting sounds to " + set);
        if (set == false)
        {
            foreach (AudioSource source in this.audioSources.Values) // all sounds.
            {
                if (!source.clip.name.Contains("music"))
                {
                    print("Muting " + source.clip.name);
                    this.mutedSoundValues.Add(source.clip.name, source.volume);
                    source.volume = 0f;
                }
            }
            print("Muted " + this.mutedSoundValues.Count + "Sources");
        }
        else
        {
            foreach (string key in this.mutedSoundValues.Keys)
            {
                print("Unmuting " + key);
                this.audioSources[key].volume = this.mutedSoundValues[key];
            }
            this.mutedSoundValues = new Dictionary<string, float>();
        }
    }
}

