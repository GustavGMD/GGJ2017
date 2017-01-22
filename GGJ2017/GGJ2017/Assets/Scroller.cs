using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scroller : MonoBehaviour {

	public string[] intro;
	public float off;
	public float speed = 100;

	public int movementspeed=100;

	// Use this for initialization
	void Start () {
		
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
