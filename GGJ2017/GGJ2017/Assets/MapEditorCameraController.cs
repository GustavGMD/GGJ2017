using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorCameraController : MonoBehaviour {

    float speed = 10;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += Input.GetAxis("Horizontal") * Vector3.right * Time.deltaTime * speed;
        transform.position += Input.GetAxis("Vertical") * Vector3.forward * Time.deltaTime * speed;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = new Vector3(0, 20, 0);
        }
    }

    


}
