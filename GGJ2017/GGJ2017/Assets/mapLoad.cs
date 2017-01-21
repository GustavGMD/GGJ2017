using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapLoad : MonoBehaviour {
    public string mapName;
	// Use this for initialization
	void Start () {
        GetComponent<MapEditorController>().LoadWithName(mapName);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
