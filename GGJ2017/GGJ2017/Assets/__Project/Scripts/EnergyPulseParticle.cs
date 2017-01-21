using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPulseParticle : MonoBehaviour {

    public event Action<GameObject> onDestroy;
    
    public float timeLimit = 2;
    public float count = 0;
    public float energyLevel = 2;
    public float pulseForce = 0;

    public LineRenderer lineRenderer;
    public List<GameObject> lineVertices;

	public void EmissionStarted(GameObject[] p_vertices)
    {
        lineVertices.Clear();
        for (int i = 0; i < p_vertices.Length; i++)
        {
            p_vertices[i].GetComponent<EnergyPulseParticle>().onDestroy += RemoveVerticeOnDestroy;
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
            OnDestroyRoutine();
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
                lineRenderer.numPositions = 3;
                return new Vector3[] { lineVertices[0].transform.position, transform.position, lineVertices[1].transform.position };
            }
            else
            {
                lineRenderer.numPositions = 2;
                return new Vector3[] { lineVertices[0].transform.position, transform.position };                
            }
        }
        else
        {
            lineRenderer.numPositions = 1;
            return new Vector3[] { transform.position };
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "obstacle")
        {
            energyLevel -= collision.gameObject.GetComponent<Obstacle>().energyAbsorption;
            UpdateVelocity();
            if(energyLevel <= 0)
            {
                OnDestroyRoutine();
            }
        }
        else if(collision.gameObject.tag == "player")
        {
            //kill the player? Maybe this is better done at player's script...
        }
    }

    public void UpdateVelocity()
    {
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * (energyLevel / 2) * pulseForce;
    }

    public void OnDestroyRoutine()
    {
        onDestroy(gameObject);
    }

    public void RemoveVerticeOnDestroy(GameObject p_object)
    {
        p_object.GetComponent<EnergyPulseParticle>().onDestroy -= RemoveVerticeOnDestroy;
        lineVertices.Remove(p_object);
    }
}
