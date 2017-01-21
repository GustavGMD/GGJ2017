using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class MapEditorTile : MonoBehaviour {
    public string type;

    public string toSaveString()
    {
        string str;
        //type, x, z
        str = type + ", " + transform.position.x + ", " + transform.position.z;
        
        return str;
    }
}
