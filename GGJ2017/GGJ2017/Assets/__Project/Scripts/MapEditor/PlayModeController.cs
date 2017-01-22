using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeController : MonoBehaviour {
    public GameObject playerPrefab;
    MapEditorController mapEditController;
    MapEditorBrushController brushController;
    GameObject playerInstance = null;
    bool playMode = false;
	// Use this for initialization
	void Start () {
        mapEditController = GetComponent<MapEditorController>();
        brushController = GetComponent<MapEditorBrushController>();
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P))
        {
            if(playMode)
            {
                exitPlayMode();
            }
            else
            {
                enterPlayMode();
            }
            playMode = !playMode;
        }

        if(playMode && Input.GetKeyDown(KeyCode.Space))
        {
            playerInstance.transform.position = Vector3.up;
        }
	}

    void enterPlayMode()
    {
        playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        mapEditController.enabled = false;
        brushController.enabled = false;
    }

    void exitPlayMode()
    {
        Destroy(playerInstance);
        mapEditController.enabled = true;
        brushController.enabled = true;
    }
}
