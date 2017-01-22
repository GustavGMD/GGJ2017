using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour {

	public Button myButton;

	// Use this for initialization
	void Start () {
		myButton.GetComponent<Button>().onClick.AddListener(() => { playEffect(); });
	}
	
	void playEffect(){
		print ("PLay");
		AudioManagerSingleton.instance.PlaySound (
			AudioManagerSingleton.AudioClipName.BUTTON_1, AudioManagerSingleton.AudioType.SFX, false, 1);
	}
}
