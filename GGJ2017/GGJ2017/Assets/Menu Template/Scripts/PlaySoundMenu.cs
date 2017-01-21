using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioManagerSingleton.instance.PlaySound (
			AudioManagerSingleton.AudioClipName.MENU_SOUND, AudioManagerSingleton.AudioType.MUSIC, true, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
