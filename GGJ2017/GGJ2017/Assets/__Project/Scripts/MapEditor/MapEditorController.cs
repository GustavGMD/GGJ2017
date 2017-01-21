﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class MapEditorController : MonoBehaviour {
    public string directory;
    public string loadMapName;
    public bool ui = false;
    public InputField fileNameInputField;
    public Dropdown fileNameLoadDropdown;
    public MapEditorTileDescriptor[] tileDescriptors = new MapEditorTileDescriptor[0];
    private List<GameObject> objects = new List<GameObject>();
    private List<GameObject>[] layers = new List<GameObject>[5];
   
    public Toggle[] layerToggles;
    public Dropdown layerDropdown;
    public int currentLayer = 0;

    
	// Use this for initialization
	void Start ()
    {
        for(int i = 0; i < layers.Length; i++)
        {
            layers[i] = new List<GameObject>();
        }
        if(Application.loadedLevelName == "MapEditor")
        {
            loadMapList();
            LayerMarked();
            LayerSelected();

        }
        else
        {
            LoadWithName(loadMapName);
        }
        
    }

    void loadMapList()
    {
        DirectoryInfo dir = new DirectoryInfo(directory);
        FileInfo[] files = dir.GetFiles("*.map", SearchOption.AllDirectories);
        fileNameLoadDropdown.ClearOptions();
        List<string> options = new List<string>();
        options.Add("limpar");
        foreach (FileInfo file in files)
        {
            options.Add(file.Name);
        }

        fileNameLoadDropdown.AddOptions(options);
        if(options.Count > 1)
        {
            Load();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Save()
    {
        string filename = fileNameInputField.text;
        fileNameInputField.text = "";
        SaveWithName(filename);
        loadMapList();
    }

    public void LayerSelected()
    {
        int.TryParse(layerDropdown.options[layerDropdown.value].text, out currentLayer);
    }

    public void LayerMarked()
    {
        for(int i = 0; i < layerToggles.Length; i++)
        {
            setLayerVisible(i, layerToggles[i].isOn);
        }
    }

    public void setLayerVisible(int layer, bool visible)
    {
        foreach(GameObject o in layers[layer])
        {
            o.SetActive(visible);
        }
    }

    public void Load()
    {
        string filename = fileNameLoadDropdown.options[fileNameLoadDropdown.value].text;
        if(filename == "limpar")
        {
            Limpar();
            return;
        }
        LoadWithName(filename);
    }
    
    public void SaveWithName(string filename)
    {
        StreamWriter fileWriter = new StreamWriter(directory + "/" + filename + ".map");
        for (int i = 0; i < layers.Length; i++)
        {
            fileWriter.WriteLine("layer " + i.ToString());
            foreach(GameObject o in layers[i])
            {
                MapEditorTile tile = o.GetComponent<MapEditorTile>();
                fileWriter.WriteLine(tile.toSaveString());
            }
        }

        fileWriter.Close();
    }

    public void Limpar()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            foreach (GameObject o in layers[i])
            {
                Destroy(o);
            }
            layers[i].Clear();
        }
        
    }

    public GameObject instantiateWithType(string typeString)
    {
        foreach (MapEditorTileDescriptor descriptor in tileDescriptors)
        {
            if(descriptor.type == typeString)
            {
                GameObject go = Instantiate(descriptor.prefab);
                MapEditorTile tile = go.AddComponent<MapEditorTile>();
                go.transform.position = new Vector3(go.transform.position.x, currentLayer, go.transform.position.z);
                tile.type = typeString;
                layers[currentLayer].Add(go);
                return go;
            }
        }

        return null;
    }
    public GameObject createFromLoadString(string loadString)
    {
        string[] values = loadString.Split(',');
        if(values.Length < 3)
        {
            Debug.Log("Invalid load line: " + loadString);
        }

        string typeString = values[0].Trim();
        string xString = values[1].Trim();
        string zString = values[2].Trim();
        
        float x, z;
        if (!float.TryParse(xString, out x))
        {
            Debug.Log("Invalid x value in line: " + loadString);
            return null;
        }
        if (!float.TryParse(zString, out z))
        {
            Debug.Log("Invalid z value in line: " + loadString);
            return null;
        }

        GameObject go = instantiateWithType(typeString);
        if(go == null)
        {
            return null;
        }

        go.transform.position = new Vector3(x, go.transform.position.y, z);
        
        return go;
    }

    public void LoadWithName(string filename)
    {
        Limpar();
        Debug.Log("carregando");
        string[] lines = File.ReadAllLines(directory + "/" + filename);
        foreach(string line in lines)
        {
            if(line.StartsWith("layer"))
            {
                string layerIdStr = line.Replace("layer", "").Trim();
                if(!int.TryParse(layerIdStr, out currentLayer))
                {
                    Debug.Log("valor de layer inválido na linha: " + line);
                }
                continue;
            }
            GameObject go = createFromLoadString(line);
            if(go == null)
            {
                continue;
            }
        }
    }

    public List<GameObject> objectsInArea(Vector3 position, float brushSize)
    {
        List<GameObject> objects = new List<GameObject>();
        float halfBrush = brushSize / 2;

        foreach (GameObject o in layers[currentLayer])
        {
            if (o == null)
            {
                continue;
            }
            if (o.transform.position.x > position.x - halfBrush &&
                o.transform.position.x < position.x + halfBrush &&
                o.transform.position.z > position.z - halfBrush &&
                o.transform.position.z < position.z + halfBrush)
            {
                objects.Add(o);
            }
        }
        return objects;
    }

    public void removeInArea(Vector3 position, float brushSize)
    {

        List<GameObject> objects = objectsInArea(position, brushSize);
        foreach (GameObject o in objects)
        {
            if(o == null)
            {
                continue;
            }
            Destroy(o);
        }
    }
}