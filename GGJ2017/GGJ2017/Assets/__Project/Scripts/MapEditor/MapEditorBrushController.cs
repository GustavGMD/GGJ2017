using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MapEditorBrushController : MonoBehaviour {
    public int brush = -1;
    public int oldBrush = -1;

    public GameObject saveSceneUi;
    public float gridSize = 1;

    MapEditorController mapEditorController;

    private GameObject brushInstance = null;
    private MapEditorTileDescriptor descriptor;
    public GameObject gridLinePrefab;
    public GameObject uiPanel;
    

    public InputField gridSizeInput;
    

    public Dropdown brushDropdown;

    private Dictionary<KeyCode, int> keyBrushes = new Dictionary<KeyCode, int>();
    private TileDescriptorList descriptorList = null;

    public void listBrushes()
    {
        brushDropdown.ClearOptions();

        List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
        int i = 1;
        foreach(MapEditorTileDescriptor descriptor in descriptorList.tileDescriptors)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.image = descriptor.icon;
            data.text = i.ToString() + " - " + descriptor.type;
            i++;

            optionList.Add(data);
        }
        brushDropdown.AddOptions(optionList);
    }

    public void gridInputChanged()
    {
        string gridSizetext = gridSizeInput.text;
        float n;
        if(!float.TryParse(gridSizetext, out n))
        {
            Debug.Log("tamanho de grid inválido");
        }
        else
        {
            gridSize = n;
        }

        updateGrid();
    }
    public void updateGrid()
    {
        gridSizeInput.text = gridSize.ToString();
        drawGrid();
    }

    void clearGrid()
    {
        Transform t = transform.FindChild("grid");
        if (t != null)
        {
            Destroy(t.gameObject);
        }
    }
    void drawGrid()
    {
        clearGrid();
        
        GameObject gridObject = new GameObject("grid");
        gridObject.transform.parent = transform;

        int lines = 100;
        int lineSize = 100;
        for(int i = -lines/2; i < lines/2; i++)
        {
            float position = i * gridSize;

            GameObject hline = Instantiate(gridLinePrefab);
            LineRenderer hlineRenderer = hline.GetComponent<LineRenderer>();
            hlineRenderer.SetPosition(0, new Vector3(-lineSize, position, 0));
            hlineRenderer.SetPosition(1, new Vector3(lineSize, position, 0));
            hline.transform.parent = gridObject.transform;

            GameObject vline = Instantiate(gridLinePrefab);
            LineRenderer vlineRenderer = vline.GetComponent<LineRenderer>();
            vlineRenderer.SetPosition(0, new Vector3(position, -lineSize, 0));
            vlineRenderer.SetPosition(1, new Vector3(position, lineSize, 0));
            vline.transform.parent = gridObject.transform;

        }
    }
    void Start()
    {
        descriptorList = GameObject.FindObjectOfType<TileDescriptorList>();
        if(descriptorList == null)
        {
            Debug.Log("Tile Descriptor List not found");
        }
        updateGrid();
        mapEditorController = FindObjectOfType<MapEditorController>();
        listBrushes();
        KeyCode[] keycodes =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };
        for(int i = 0; i < keycodes.Length; i++)
        {
            keyBrushes[keycodes[i]] = i;
        }

        brush = -1;
        saveSceneUi.SetActive(true);
    }

    void UpdateInput()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        List<KeyCode> keyCodes = new List<KeyCode>(keyBrushes.Keys);
        for (int i = 0; i < keyCodes.Count; i++)
        {
            if(Input.GetKeyDown(keyCodes[i]))
            {
                brush = keyBrushes[keyCodes[i]];
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    brush += keyCodes.Count;
                }
                Debug.Log("atualizando key: " + brush);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            brush = -1;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            brush--;
            if(brush < 0)
            {
                brush = descriptorList.tileDescriptors.Length - 1;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            brush++;
            if(brush > descriptorList.tileDescriptors.Length-1)
            {
                brush = 0;
            }
        }
    }

    public void BrushSelected()
    {
        brush = brushDropdown.value;
        brushChanged();
    }
    // Update is called once per frame
    void Update ()
    {
        UpdateInput();

        if(brush != oldBrush)
        {
            brushChanged();
        }
        if (brushInstance == null)
        {
            brush = -1;
            brushChanged();
        }

        if (brush == -1)
        {
            saveSceneUi.SetActive(true);
            return;
        }

        saveSceneUi.SetActive(false);

        Vector3 position;
        position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        position.z = -mapEditorController.currentLayer ;
        brushInstance.transform.position = position;
        lockGrid();
        if (Input.GetMouseButton(0))
        {        
            if(mapEditorController.objectsInArea(brushInstance.transform.position, gridSize).Count == 0)
            {
                paint();
            }
        }
        if (Input.GetMouseButton(1))
        {
            Debug.Log(brushInstance.transform.position);
            mapEditorController.removeInArea(brushInstance.transform.position, gridSize);
        }
    }

    
    void lockGrid()
    {
        if(gridSize == 0)
        {
            return;
        }
        float halfGridSize = gridSize / 2.0f;
        
        Vector3 pos = brushInstance.transform.position;
        pos.x = ((Mathf.Floor(pos.x / gridSize)) * gridSize) + halfGridSize;
        pos.y = ((Mathf.Floor(pos.y / gridSize)) * gridSize) + halfGridSize;
        brushInstance.transform.position = pos;
    }

    void paint()
    {

        GameObject go = mapEditorController.instantiateWithType(descriptor.type);
        go.transform.position = 
            new Vector3(brushInstance.transform.position.x, brushInstance.transform.position.y, go.transform.position.z);
        
    }

    void brushChanged()
    {

        if(brushInstance != null)
        {
            Destroy(brushInstance);
        }
        
        if (descriptorList.tileDescriptors.Length <= brush)
        {
            brush = -1;
        }

        oldBrush = brush;
        if(brush == -1)
        {
            if(oldBrush != -1)
            {
                Destroy(brushInstance);
            }
        }
        else
        {
            descriptor = descriptorList.tileDescriptors[brush];
            brushInstance = Instantiate(descriptor.prefab);
            brushDropdown.value = brush;
        }
        
    }
}
