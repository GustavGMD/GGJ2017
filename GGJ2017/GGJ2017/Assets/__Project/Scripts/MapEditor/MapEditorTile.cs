using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class MapEditorTile : MonoBehaviour {
    public string type;

    public string toSaveString()
    {
        string str;
        //type, x, y
        str = type + ", " + transform.position.x + ", " + transform.position.y;
        
        return str;
    }
}
