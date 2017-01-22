using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;


public class MapLoader {
    static string directory = "maps";

    static TileDescriptorList descriptorList = null;
    static int currentLayer = 0;
    static List<GameObject>[] layers = new List<GameObject>[5];
    static GameObject mapParent = null;

    [MenuItem("Level/level1")]
    private static void NewMenuOption()
    {
        loadLevel("level1.map");
    }
    [MenuItem("Level/level2")]
    private static void NewMenuOption2()
    {
        loadLevel("level2.map");
    }
    [MenuItem("Level/level3")]
    private static void NewMenuOption3()
    {
        loadLevel("level3.map");
    }
    [MenuItem("Level/level4")]
    private static void NewMenuOptio4n()
    {
        loadLevel("level4.map");
    }
    [MenuItem("Level/level5")]
    private static void NewMenuOption5()
    {
        loadLevel("level5.map");
    }
    [MenuItem("Level/level6")]
    private static void NewMenuOption6()
    {
        loadLevel("level6.map");
    }
    [MenuItem("Level/level7")]
    private static void NewMenuOption7()
    {
        loadLevel("level7.map");
    }
    [MenuItem("Level/level8")]
    private static void NewMenuOption8()
    {
        loadLevel("level8.map");
    }

    [MenuItem("Level/level9")]
    private static void NewMenuOption9()
    {
        loadLevel("level9.map");
    }

    public static List<GameObject>[] loadLevel(string filename)
    {

        mapParent = GameObject.Find("__map_parent");
        if(mapParent != null)
        {
            GameObject.DestroyImmediate(mapParent);
        }
        mapParent = new GameObject("__map_parent");

        for(int i = 0; i < layers.Length; i++)
        {
            layers[i] = new List<GameObject>();
        }

        descriptorList = GameObject.FindObjectOfType<TileDescriptorList>();

        if (descriptorList == null)
        {
            Debug.Log("TileDescriptorList não encontrado");
            return layers;
        }

        string[] lines = File.ReadAllLines(directory + "/" + filename);
        foreach (string line in lines)
        {
            if (line.StartsWith("layer"))
            {
                continue;
            }
            GameObject go = createFromLoadString(line);
            if (go == null)
            {
                continue;
            }
        }

        return layers;
    }


    public static GameObject createFromLoadString(string loadString)
    {
        string[] values = loadString.Split(',');
        if (values.Length < 3)
        {
            Debug.Log("Invalid load line: " + loadString);
        }

        string typeString = values[0].Trim();
        string xString = values[1].Trim();
        string zString = values[2].Trim();

        float x, y;
        if (!float.TryParse(xString, out x))
        {
            Debug.Log("Invalid x value in line: " + loadString);
            return null;
        }
        if (!float.TryParse(zString, out y))
        {
            Debug.Log("Invalid z value in line: " + loadString);
            return null;
        }

        GameObject go = instantiateWithType(typeString);
        if (go == null)
        {
            return null;
        }
        
        go.transform.position = new Vector3(x, y, go.transform.position.z);

        return go;
    }

    public static GameObject instantiateWithType(string typeString)
    {
        foreach (MapEditorTileDescriptor descriptor in descriptorList.tileDescriptors)
        {
            if (descriptor.type == typeString)
            {
                GameObject go = GameObject.Instantiate(descriptor.prefab);
                MapEditorTile tile = go.AddComponent<MapEditorTile>();
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -currentLayer);
                tile.type = typeString;
                go.name = descriptor.prefab.name;
                layers[currentLayer].Add(go);
                go.transform.parent = mapParent.transform;
                return go;
            }
        }

        return null;
    }
}
