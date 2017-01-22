using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileDescriptorList))]
public class TileDescriptorListEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileDescriptorList myScript = (TileDescriptorList)target;
        if (GUILayout.Button("Add Selected Prefab"))
        {
            GameObject currentObject = Selection.activeGameObject;
            if(currentObject == null)
            {
                Debug.Log("nenhum objeto selecionado");
                return;
            }

            if(PrefabUtility.GetPrefabObject(currentObject) == null)
            {
                Debug.Log("Selecione um prefab");
                return;
            }
            MapEditorTileDescriptor[] newTileList = new MapEditorTileDescriptor[myScript.tileDescriptors.Length + 1];
            for (int i  = 0; i < myScript.tileDescriptors.Length; i++)
            {
                newTileList[i] = myScript.tileDescriptors[i];
            }

            MapEditorTileDescriptor tileDescriptor = new MapEditorTileDescriptor();
            tileDescriptor.prefab = currentObject;
            tileDescriptor.type = currentObject.name;

            newTileList[myScript.tileDescriptors.Length] = tileDescriptor;

            myScript.tileDescriptors = newTileList;
            Debug.Log(  "Adicionado: " + tileDescriptor.type );
           // myScript.BuildObject();
        }
    }
}
