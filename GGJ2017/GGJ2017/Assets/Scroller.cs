using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scroller : MonoBehaviour {


	public float movementspeed = 0.0f;

	// Use this for initialization
	void Start () {
		AudioManagerSingleton.instance.PlaySound (
			AudioManagerSingleton.AudioClipName.TRILHA, AudioManagerSingleton.AudioType.MUSIC, false, 0.3f);
		StartCoroutine (Example());
	}

	IEnumerator Example() {
		yield return new WaitForSeconds(7);
		AudioManagerSingleton.instance.PlaySound (
			AudioManagerSingleton.AudioClipName.NARRATIVE, AudioManagerSingleton.AudioType.MUSIC, false, 1);
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Space)) {
			transform.Translate (Vector3.up * 350 * Time.deltaTime);
		} else {
			transform.Translate (Vector3.up * movementspeed * Time.deltaTime);
		}

		if( transform.position.y >= 1400){
			SceneManager.LoadScene ("Ingame");
		}
	}
}
