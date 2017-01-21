using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float movementspeed;

	// Use this for initialization
	void Start () {
		movementspeed = 5.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		if( Input.GetKey( KeyCode.A) || Input.GetKey( KeyCode.LeftArrow) ){
			transform.Translate (Vector3.left * movementspeed * Time.deltaTime); 
		}

		if( Input.GetKey( KeyCode.W) || Input.GetKey( KeyCode.UpArrow) ){
			transform.Translate (Vector3.up * movementspeed * Time.deltaTime); 
		}

		if( Input.GetKey( KeyCode.S) || Input.GetKey( KeyCode.DownArrow) ){
			transform.Translate (Vector3.down * movementspeed * Time.deltaTime); 
		}

		if( Input.GetKey( KeyCode.D) || Input.GetKey( KeyCode.RightArrow) ){
			transform.Translate (Vector3.right * movementspeed * Time.deltaTime); 
		}

	}
}
