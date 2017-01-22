using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scroller : MonoBehaviour {


	public float movementspeed = 0.0f;

	public string nextLevel = "MenuScene" ;

	private int backgroundMusic;

	// Use this for initialization
	void Start () {
		backgroundMusic = AudioManagerSingleton.instance.PlaySound (
			AudioManagerSingleton.AudioClipName.TRILHA, AudioManagerSingleton.AudioType.MUSIC, false, 0.5f);
		StartCoroutine (Example());
		StartCoroutine (NextScene ());
	}

	IEnumerator Example() {
		yield return new WaitForSeconds(7);
		AudioManagerSingleton.instance.PlaySound (
			AudioManagerSingleton.AudioClipName.NARRATIVE, AudioManagerSingleton.AudioType.MUSIC, false, 1);
		AudioManagerSingleton.instance.ChangeVolume ( backgroundMusic, 0.3f);
	}

	IEnumerator NextScene() {
		yield return new WaitForSeconds(73);
		SceneManager.LoadScene (nextLevel);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Space)) {
			SceneManager.LoadScene (nextLevel);
		} else {
			transform.Translate (Vector3.up * movementspeed * Time.deltaTime);
		}

//		if( transform.position.y >= 1400){
//			SceneManager.LoadScene (nextLevel);
//		}
	}

	void OnDestroy() {
		StopCoroutine ("NextScene");
	}
}
