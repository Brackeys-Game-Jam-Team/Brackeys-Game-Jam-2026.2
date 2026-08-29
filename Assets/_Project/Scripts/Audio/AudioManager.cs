using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundClip
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop = false;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private List<SoundClip> musicClips = new();
    [SerializeField] private List<SoundClip> sfxClips = new();
    [SerializeField] private List<SoundClip> voiceClips = new();

    private Dictionary<string, SoundClip> musicLookup;
    private Dictionary<string, SoundClip> sfxLookup;
    private Dictionary<string, SoundClip> voiceLookup;

    private AudioClip currentMusic;

    private void Awake()
    {
        musicLookup = BuildLookup(musicClips, "Music");
        sfxLookup = BuildLookup(sfxClips, "SFX");
        voiceLookup = BuildLookup(voiceClips, "Voice");
    }

    public void PlayMusic(string name)
    {
        if (!TryGetClip(musicLookup, name, "Music", out var sound))
            return;

        if (sound.clip == currentMusic)
            return;

        currentMusic = sound.clip;
        musicSource.clip = sound.clip;
        musicSource.pitch = sound.pitch;
        musicSource.loop = true;
        //musicSource.volume = masterVolume * musicVolume * sound.volume;
        musicSource.Play();
    }

    public void StopMusic(float fadeOut = 0f)
    {
        if (fadeOut <= 0f)
        {
            musicSource.Stop();
            return;
        }
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void PlaySFX(string name)
    {
        if (!TryGetClip(sfxLookup, name, "SFX", out var sound))
            return;

        sfxSource.pitch = sound.pitch;
        sfxSource.PlayOneShot(sound.clip, sound.volume);
    }

    public void PlayVoice(string name)
    {
        if (!TryGetClip(voiceLookup, name, "Voice", out var sound))
            return;

        voiceSource.pitch = sound.pitch;
        voiceSource.PlayOneShot(sound.clip, sound.volume);
    }


    private Dictionary<string, SoundClip> BuildLookup(List<SoundClip> clips, string category)
    {
        var dict = new Dictionary<string, SoundClip>(StringComparer.OrdinalIgnoreCase);

        foreach (var s in clips)
        {
            if (s.clip == null || string.IsNullOrEmpty(s.name))
                continue;

            if (!dict.TryAdd(s.name, s))
                Debug.LogWarning($"[AudioManager] Duplicate {category} clip name: \"{s.name}\"");
        }

        return dict;
    }

    private bool TryGetClip(Dictionary<string, SoundClip> lookup, string name, string category, out SoundClip clip)
    {
        if (lookup.TryGetValue(name, out clip))
            return true;

        Debug.LogWarning($"[AudioManager] {category} clip \"{name}\" not found.");
        clip = null;
        return false;
    }
}