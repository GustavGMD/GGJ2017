using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetAudioLevels : MonoBehaviour {

	public AudioMixer mainMixer;					//Used to hold a reference to the AudioMixer mainMixer

	public AudioManagerSingleton mixer;

	void Start () {
		mixer = AudioManagerSingleton.instance;
	}

	//Call this function and pass in the float parameter musicLvl to set the volume of the AudioMixerGroup Music in mainMixer
	public void SetMusicLevel(float musicLvl)
	{
		if (AudioManagerSingleton.instance !=null && AudioManagerSingleton.instance.musicVolume != null) {
			AudioManagerSingleton.instance.musicVolume = musicLvl;
			AudioManagerSingleton.instance.ChangeVolume (AudioManagerSingleton.AudioType.MUSIC, musicLvl);
		}
		mainMixer.SetFloat("musicVol", musicLvl);
	}

	//Call this function and pass in the float parameter sfxLevel to set the volume of the AudioMixerGroup SoundFx in mainMixer
	public void SetSfxLevel(float sfxLevel)
	{
		if (AudioManagerSingleton.instance !=null && AudioManagerSingleton.instance.sfxVolume != null) {
			AudioManagerSingleton.instance.sfxVolume = sfxLevel;
			AudioManagerSingleton.instance.ChangeVolume (AudioManagerSingleton.AudioType.SFX, sfxLevel);
		}
		mainMixer.SetFloat("sfxVol", sfxLevel);
	}

	public void SetMasterVolume(float masterLvl){
		if (AudioManagerSingleton.instance !=null && AudioManagerSingleton.instance.masterVolume != null) {
			AudioManagerSingleton.instance.masterVolume = masterLvl;
			AudioManagerSingleton.instance.ChangeVolume();
		}
	}
}
