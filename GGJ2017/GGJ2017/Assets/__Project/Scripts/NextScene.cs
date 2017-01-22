using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour {

    public string scene;

    void CallNextScene()
    {
        SceneManager.LoadScene(scene);
    }

    void Update()
    {
        if (Time.time >= 8.1f)
        {
            CallNextScene();
        }
    }
}
