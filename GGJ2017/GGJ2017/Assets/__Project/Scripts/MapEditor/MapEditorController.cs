using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MapEditorController : MonoBehaviour {
    
    public InputField fileNameInputField;
    public Dropdown fileNameLoadDropdown;
    public List<GameObject>[] layers = new List<GameObject>[5];
    public Toggle[] layerToggles;
    public Dropdown layerDropdown;
    public int currentLayer = 0;

    private TileDescriptorList descriptorList;
    private string directory = "maps";

    void Awake()
    {
        if (Application.loadedLevelName != "MapEditor")
        {
            Debug.LogError("Esse componnente não deveria estar nessa cena! Remova-o e adicione o prefab TileDescriptorList");
            GetComponent<MapEditorBrushController>().enabled = false;
        }
    }
	// Use this for initialization
	void Start ()
    {

        descriptorList = FindObjectOfType<TileDescriptorList>();
        if(descriptorList == null)
        {
            Debug.Log("tile descriptor list not found");
        }
        for(int i = 0; i < layers.Length; i++)
        {
            layers[i] = new List<GameObject>();
        }

        loadMapList();
        LayerMarked();
        LayerSelected();
    }

    void loadMapList()
    {
        fileNameLoadDropdown.ClearOptions();
        List<string> options = new List<string>();
        options.Add("limpar");

        DirectoryInfo dir = new DirectoryInfo(directory);
        if(!dir.Exists)
        {
            Directory.CreateDirectory(directory);

        }
        else
        {
            FileInfo[] files = dir.GetFiles("*.map", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                options.Add(file.Name);
            }

        }

        fileNameLoadDropdown.AddOptions(options);
        if (options.Count > 1)
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

    public void setLayer(int layer)
    {
        currentLayer = layer;
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
            if(o == null)
            {
                continue;
            }
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
        setLayer(0);
    }
    
    public void SaveWithName(string filename)
    {
        StreamWriter fileWriter = new StreamWriter(directory + "/" + filename + ".map");
        for (int i = 0; i < layers.Length; i++)
        {
            fileWriter.WriteLine("layer " + i.ToString());
            foreach(GameObject o in layers[i])
            {
                if (o == null)
                {
                    continue;
                }
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
        foreach (MapEditorTileDescriptor descriptor in descriptorList.tileDescriptors)
        {
            if(descriptor.type == typeString)
            {
                GameObject go = Instantiate(descriptor.prefab);
                MapEditorTile tile = go.AddComponent<MapEditorTile>();
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -currentLayer);
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
        if(go == null)
        {
            return null;
        }

        go.transform.position = new Vector3(x, y, go.transform.position.z);
        
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
                o.transform.position.y > position.y - halfBrush &&
                o.transform.position.y < position.y + halfBrush)
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
