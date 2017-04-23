using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour 
{
    public enum Music
    {
        Menu,
        Game,

        Count
    }

    public enum SoundEffect
    {
        ButtonClick,
        Death,
        YellowBallCollected,
        RoundStart,

        Count
    }


	public static AudioManager              instance;


    Dictionary<Music, AudioClip>            _music;
    Dictionary<SoundEffect, AudioClip>      _soundEffects;


    //Use two audio sources, one for sound effects, one for music
    AudioSource[]                           _soundEffectSources;
    AudioSource                             _loopingSoundEffectSource;
    AudioSource                             _musicSource;

    bool                                    _muteMusic;
    bool                                    _muteSoundEffects;

    const int                               AUDIO_SOURCES = 12;


    /// <summary>
    /// Initialize on creation.
    /// </summary>
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        _muteMusic = false;
        _muteSoundEffects = false;

        _music = new Dictionary<Music, AudioClip>((int)Music.Count);
        _soundEffects = new Dictionary<SoundEffect, AudioClip>((int)SoundEffect.Count);

        LoadAudioSources();
        LoadContent();

        SceneLoader.targetScene = SceneLoader.Scene.Menu;
        SceneLoader.LoadTargetScene();
    }

    /// <summary>
    /// Create the neccessary audio sources.
    /// </summary>
    void LoadAudioSources()
    {
        _soundEffectSources = new AudioSource[AUDIO_SOURCES-2];

        for (int i = 0; i < AUDIO_SOURCES; i++)
        {//adds audio source componenets to the object
            gameObject.AddComponent<AudioSource>();
        }

        AudioSource[] sources = GetComponents<AudioSource>();

        int size = _soundEffectSources.Length;
        int index = 0;

        for (; index < size; index++)
        {//sets sound effect sources
            _soundEffectSources[index] = sources[index];
            _soundEffectSources[index].playOnAwake = false;
            _soundEffectSources[index].loop = false;
        }

        //sets a music source
        _musicSource = sources[index++];
        _musicSource.playOnAwake = true;
        _musicSource.loop = true;

        //sets a looping sound effect source
        _loopingSoundEffectSource = sources[index++];
        _loopingSoundEffectSource.playOnAwake = false;
        _loopingSoundEffectSource.loop = false;
    }

    /// <summary>
    /// Load all of the audio content
    /// </summary>
    void LoadContent()
    {
        _music.Add(Music.Menu, Resources.Load<AudioClip>("Audio/Soundtracks/MenuMusic"));
        _music.Add(Music.Game, Resources.Load<AudioClip>("Audio/Soundtracks/GameMusic"));

        _soundEffects.Add(SoundEffect.ButtonClick, Resources.Load<AudioClip>("Audio/SoundEffects/ButtonSound"));
        _soundEffects.Add(SoundEffect.Death, Resources.Load<AudioClip>("Audio/SoundEffects/ObstacleHit"));
        _soundEffects.Add(SoundEffect.YellowBallCollected, Resources.Load<AudioClip>("Audio/SoundEffects/YellowBallCollected"));
        _soundEffects.Add(SoundEffect.RoundStart, Resources.Load<AudioClip>("Audio/SoundEffects/RoundStart"));
    }

    /// <summary>
    /// Player specific music (song)
    /// </summary>
    /// <param name="music"></param>
    public void PlayMusic(Music music)
    {
        if (!_muteMusic)
        {//if music isn't muted
            if (_musicSource.clip != _music[music])
            {//if the music clip isn't already playing
                _musicSource.Stop();
                _musicSource.clip = _music[music];
                _musicSource.Play();
            }
        }
    }

    /// <summary>
    /// Play a specific sound effect.
    /// </summary>
    /// <param name="soundEffect"></param>
    public void PlaySoundEffect(SoundEffect soundEffect)
    {
        if (!_muteSoundEffects)
        {// if not muted
            for (int i = 0; i < _soundEffectSources.Length; i++)
            {//look for an avaialable channel
                if (!_soundEffectSources[i].isPlaying)
                {//if its free play the sound effect
                    _soundEffectSources[i].PlayOneShot(_soundEffects[soundEffect]);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// mutes all of the auio
    /// </summary>
    /// <param name="mute"></param>
    public void Mute(bool mute)
    {
        _muteMusic = mute;
        _muteSoundEffects = mute;
        AudioListener.pause = mute;
    }
}
