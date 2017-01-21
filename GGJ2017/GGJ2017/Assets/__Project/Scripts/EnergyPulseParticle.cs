using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPulseParticle : MonoBehaviour {

    public event Action<GameObject> onDestroy;
    
    public float timeLimit = 2;
    public float count = 0;

    public LineRenderer lineRenderer;
    public List<GameObject> lineVertices;

	public void EmissionStarted(GameObject[] p_vertices)
    {
        lineVertices.Clear();
        for (int i = 0; i < p_vertices.Length; i++)
        {
            lineVertices.Add(p_vertices[i]);
        }
        UpdateLine();
        count = 0;
    }

    public void FixedUpdate()
    {
        if(count < timeLimit)
        {
            count += Time.fixedDeltaTime;
        }
        else
        {
            onDestroy(gameObject);
        }
        UpdateLine();
    }

    public void UpdateLine()
    {
        lineRenderer.SetPositions(GenerateLineVertices());
        lineRenderer.material.color = new Color(1, 1, 1, 1);
        //lineRenderer.material.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1 - (count / timeLimit));
    }

    public Vector3[] GenerateLineVertices()
    {
        if (lineVertices.Count > 0)
        {
            if (lineVertices.Count > 1)
            {
                return new Vector3[] { lineVertices[0].transform.position, transform.position, lineVertices[1].transform.position };
            }
            else
            {
                return new Vector3[] { lineVertices[0].transform.position, transform.position };
            }
        }
        else
        {
            return new Vector3[] { transform.position };
        }
    }
}
