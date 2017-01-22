using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLevelChange : MonoBehaviour {

	public Button myButton;

	// Use this for initialization
	void Start () {
		myButton.GetComponent<Button>().onClick.AddListener(() => { playEffect(); });
	}

	void playEffect(){
		print ("PLay");
		AudioManagerSingleton.instance.StopSound (AudioManagerSingleton.AudioType.MUSIC);
		PlayerController.backgroundId = -1;
	}
}
