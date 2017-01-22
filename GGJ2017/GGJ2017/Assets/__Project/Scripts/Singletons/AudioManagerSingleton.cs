using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AudioManagerSingleton : MonoBehaviour {

    /// <summary>
    /// This enumerator should be edited by the user of this asset to suit its own needs. Each element corresponds to an AudioClip file in the Resources/Audio directory. The files in that directory must be named in a way that the order of the elements in that directory correponds to the order of the elements in this enum.
    /// </summary>
	public enum AudioClipName
    {
        MENU_SOUND,
        WAVE_1,
		DEAD_1,
		DEAD_2,
		NARRATIVE,
		TRILHA,
		BUTTON_1,
		BUTTON_2,
		BUTTON_3,
		WIN,
		BACKGROUND,
		WALL_BROKEN_1,
		WALL_BROKEN_2,
		WALL_BROKEN_3,
		WALL_BROKEN_4
    }

    /// <summary>
    /// AudioTypes may be used to group up sounds and perform actions to those groups only. For example, you may have Sound Effects(SFX) and Music groups, so the user may choose to mute/unmute one.
    /// </summary>
    public enum AudioType
    {
        SFX,
        MUSIC
    }

    /// <summary>
    /// The amount of AudioClip objects present at the same time in the scene. Change this number as you wish. This limits the number of sounds playing at the same time. If all objects are active playing a sounds, new sounds won't be played.
    /// </summary>
    public int audioSourcePoolSize = 10;
    /// <summary>
    /// A volume value that affects all sounds. Setting a 0(zero) value will mute all sounds. This is different from the MuteAll() method.
    /// </summary>
    public float masterVolume = 1;

	public float sfxVolume = 1;

	public float musicVolume = 1;

    private bool _initialized = false;
    private List<AudioSource> _audioSourceInactivePool;
    private List<AudioSource> _audioSourceActivePool;
    private List<int[]> _audioSourceActivePoolParameters; //int[5] = {soundID, AudioClipName, AudioType, Loop?, Volume multiplier}
    private AudioClip[] _audioClips;
    private int _soundIDcounter = 0;
    private int _volumeIntegerBase = 10000;

    //This Region contains the structure for this whole class to work as a Singleton
    #region Singleton
    private static bool _isShuttingDown;

    static private AudioManagerSingleton _instance;
    static public AudioManagerSingleton instance
    {
        get
        {
            if (_instance == null)
            {        
				_instance = SafeInstantiate();
				DontDestroyOnLoad (_instance);
                {
                    _instance.Initialize();
                }
            }

            return _instance;
        }
    }
    public void OnApplicationQuit()
    {
        _isShuttingDown = true;
    }

    public static AudioManagerSingleton SafeInstantiate()
    {
        if (_isShuttingDown)
            return null;
        else
            return new GameObject("AudioManagerSingleton", typeof(AudioManagerSingleton)).GetComponent<AudioManagerSingleton>();
    }
    #endregion

    /// <summary>
    /// Intializes the object's variables. Instantiates GameObjects with AudioSource components and inserts them in the pool system. Loads AudioClips from Resources/Audio directory.
    /// </summary>
    public void Initialize()
    {
        if (!_initialized)
        {
            _audioSourceInactivePool = new List<AudioSource>();
            _audioSourceActivePool = new List<AudioSource>();
            _audioSourceActivePoolParameters = new List<int[]>();

            //generate a pool of AudioSources
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                //_audioSourcePool.Add((GameObject)Instantiate(new GameObject(), gameObject.transform.position, Quaternion.identity));
                GameObject __tempAudioSource = new GameObject("AudioSource_" + i);
                __tempAudioSource.transform.parent = gameObject.transform;
                __tempAudioSource.AddComponent<AudioSource>().playOnAwake = false;
                _audioSourceInactivePool.Add(__tempAudioSource.GetComponent<AudioSource>());
            }

            //load all audio files from resources, which should be already ordered according o AudioType enum
            _audioClips = Resources.LoadAll<AudioClip>("Audio");

            _initialized = true;
        }
    }

    /// <summary>
    /// Plays an instance of the specified AudioClip. 
    /// </summary>
    /// <param name="p_audioClipName"> The name of the AudioClip as defined in the enum structure of this class. </param>
    /// <param name="p_audioType"> The type of the sound. This is used for controlling a group of sounds. </param>
    /// <param name="p_loop"> Sets the AudioClip to Loop. By doing so you must stop it manually. </param>
    /// <param name="p_volume"> Sets the volume for this AudioClip. From 0(inclusive) to 1(inclusive). </param>
    /// <returns> Returns an ID for the AudioClip being played, which may be used to stop the sound. </returns>
    public int PlaySound(AudioClipName p_audioClipName, AudioType p_audioType, bool p_loop = false, float p_volume = 1f)
    {
        CheckForFinishedAudioClips();
        if(_audioSourceInactivePool.Count > 0)
        {
            int[] __parameters = new int[5];
            int __ID = _soundIDcounter;

            _soundIDcounter++;
            __parameters[0] = __ID;
            __parameters[1] = (int)p_audioClipName;
            __parameters[2] = (int)p_audioType;
            __parameters[3] = p_loop ? 1 : 0;
            __parameters[4] = (int)(p_volume * _volumeIntegerBase);

            _audioSourceInactivePool[0].clip = _audioClips[(int)p_audioClipName];
            _audioSourceInactivePool[0].loop = p_loop;
			float soundType;
			if (p_audioType == AudioType.MUSIC) {
				soundType = musicVolume;
			} else {
				soundType = sfxVolume;
			}
			_audioSourceInactivePool[0].volume = ((float)__parameters[4] / _volumeIntegerBase) * masterVolume * soundType;
            _audioSourceInactivePool[0].Play();

            AddToActiveList(__parameters);
            return __ID;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Stops an instance of the specified AudioClip. This method uses the ID generated and returned by the PlaySound() method.
    /// </summary>
    /// <param name="p_soundID"> The ID of the sound being stopped.  </param>
    public void StopSound(int p_soundID)
    {
        for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
        {
            if(_audioSourceActivePoolParameters[i][0] == p_soundID)
            {
                _audioSourceActivePool[i].Stop();
                RemoveFromActiveList(i);
            }
        }
    }

	//Stop Sound Group
	public void StopSound(AudioType type)
	{
		for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
		{
			if ((AudioType)_audioSourceActivePoolParameters[i][2] == type)
			{
				_audioSourceActivePool[i].Stop();
				RemoveFromActiveList(i);
			}
		}
	}



    /// <summary>
    /// This method runs through the Active Pool searching for finished AudioClips, removing them from that pool and making them available again at the Inactive Pool.
    /// </summary>
    public void CheckForFinishedAudioClips()
    {        
        if (_audioSourceActivePool.Count > 0)
        {
            bool __done = false;
            while (!__done)
            {
                for (int i = 0; i < _audioSourceActivePool.Count; i++)
                {
                    if (!_audioSourceActivePool[i].isPlaying)
                    {
                        RemoveFromActiveList(i);
                        break;
                    }
                    
                    if (i == _audioSourceActivePool.Count - 1)
                    {
                        __done = true;
                    }
                }
                if (_audioSourceActivePool.Count == 0)
                {
                    __done = true;
                }
            }
        }
    }

    /// <summary>
    /// This method performs a common routine for this class, by taking an element from the Inactive Pool and adding it to the Active Pool with the given parameters.
    /// </summary>
    /// <param name="p_parameters"> The parameters defined for this specific element. The parameters are the following structure: int[5] = {soundID, AudioClipName, AudioType, Loop?, Volume multiplier} </param>
    private void AddToActiveList(int[] p_parameters)
    {
        _audioSourceActivePool.Add(_audioSourceInactivePool[0]);
        _audioSourceActivePoolParameters.Add(p_parameters);
        _audioSourceInactivePool.RemoveAt(0);
    }

    /// <summary>
    /// This method performs another common routine for this class. It takes an element from the Active Pool and puts it back at the Inactive Pool.
    /// </summary>
    /// <param name="p_index"> The index of the element in the Active Pool to be removed. </param>
    private void RemoveFromActiveList(int p_index)
    {
        _audioSourceInactivePool.Add(_audioSourceActivePool[p_index]);
        _audioSourceActivePool.RemoveAt(p_index);
        _audioSourceActivePoolParameters.RemoveAt(p_index);
    }

    #region Mute/Unmute Methods
    /// <summary>
    /// Mutes all ACTIVE sounds. Note that this does not pause the sounds. This doesn't prevent new sounds from playing. New sounds won't be muted.
    /// </summary>
    public void MuteAll()
    {
        for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
        {
            MuteSound(_audioSourceActivePoolParameters[i][0]);
        }
    }

    /// <summary>
    /// Mutes all sounds belonging to the given type. Note that this does not pause the sounds.
    /// </summary>
    /// <param name="p_type"> The type of the sounds being muted. </param>
    public void MuteSound(AudioType p_type)
    {
        for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
        {
            if ((AudioType)_audioSourceActivePoolParameters[i][2] == p_type)
            {
                MuteSound(_audioSourceActivePoolParameters[i][0]);
            }
        }
    }

    /// <summary>
    /// Mutes the given sound according to its ID. Note that this does not pause the sound.
    /// </summary>
    /// <param name="p_soundID"> The ID for the sound to be muted. You may get the ID for a given sound from the PlaySound() method. </param>
    public void MuteSound(int p_soundID)
    {
        for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
        {
            if(_audioSourceActivePoolParameters[i][0] == p_soundID)
            {
                _audioSourceActivePool[i].volume = 0;
            }
        }
    }
    
    /// <summary>
    /// Unmutes all sounds.
    /// </summary>
    public void UnmuteAll()
    {
        for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
        {
            UnmuteSound(_audioSourceActivePoolParameters[i][0]);
        }
    }

    /// <summary>
    /// Unmutes all sounds of a given type.
    /// </summary>
    /// <param name="p_type"> The type of the sounds being unmuted</param>
    public void UnmuteSound(AudioType p_type)
    {
        for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
        {
            if ((AudioType)_audioSourceActivePoolParameters[i][2] == p_type)
            {
                UnmuteSound(_audioSourceActivePoolParameters[i][0]);
            }
        }
    }

    /// <summary>
    /// Unmutes a given sound according to its ID.
    /// </summary>
    /// <param name="p_soundID"> The ID for the sound to be unmuted. You may get the ID for a given sound from the PlaySound() method. </param>
    public void UnmuteSound(int p_soundID)
    {
        for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
        {
            if (_audioSourceActivePoolParameters[i][0] == p_soundID)
            {
                _audioSourceActivePool[i].volume = ((float)_audioSourceActivePoolParameters[i][4] / _volumeIntegerBase) * masterVolume;
            }
        }
    }
    #endregion

    /// <summary>
    /// Changes an active sound's volume.
    /// </summary>
    /// <param name="p_soundID"> The ID for the sound to be unmuted. You may get the ID for a given sound from the PlaySound() method. </param>
    /// <param name="p_volume"> The volume value to be changed to in this sound.From 0(inclusive) to 1(inclusive).</param>
    public void ChangeVolume(int p_soundID, float p_volume)
    {
        for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
        {
            if (_audioSourceActivePoolParameters[i][0] == p_soundID)
            {
                _audioSourceActivePoolParameters[i][4] = (int)(p_volume * _volumeIntegerBase);
                _audioSourceActivePool[i].volume = ((float)_audioSourceActivePoolParameters[i][4] / _volumeIntegerBase) * masterVolume;
            }
        }
    }

	public void ChangeVolume(AudioType type, float p_volume)
	{
		for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
		{
			if ( (AudioType)_audioSourceActivePoolParameters[i][2] == type)
			{
//				_audioSourceActivePoolParameters[i][4] = (int)(p_volume * _volumeIntegerBase);
				_audioSourceActivePool[i].volume = (((float)_audioSourceActivePoolParameters[i][4] / _volumeIntegerBase) * masterVolume) * p_volume;
			}
		}
	}

	public void ChangeVolume()
	{
		for (int i = 0; i < _audioSourceActivePoolParameters.Count; i++)
		{
//			_audioSourceActivePoolParameters[i][4] = (int)(masterVolume * _volumeIntegerBase);
			if ((AudioType)_audioSourceActivePoolParameters [i] [2] == AudioManagerSingleton.AudioType.MUSIC) {
				_audioSourceActivePool [i].volume = (((float)_audioSourceActivePoolParameters [i] [4] / _volumeIntegerBase) * masterVolume )* musicVolume;
			} else {
				_audioSourceActivePool [i].volume = (((float)_audioSourceActivePoolParameters [i] [4] / _volumeIntegerBase) * masterVolume) * sfxVolume;
			}
		}
	}
}
