using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPulseParticle : MonoBehaviour {

    public event Action<GameObject> onDestroy;
    public event Action onCollisionWithPlayer;
    public event Action onReadyToCollide;
    
    public float timeLimit = 2;
    public float count = 0;
    public float energyLevel = 2;
    public float maximumEnergyLevel = 5;
    public float pulseForce = 0;

    public LineRenderer lineRenderer;
    public List<GameObject> lineVertices;

    private bool _velocityUpdateScheduled = false;
    private bool _destructionScheduled = false;

    private float timeSpawned = 0;
    private float timeToKillPlayer = 0.5f;

	public void EmissionStarted(GameObject[] p_vertices)
    {
        lineVertices.Clear();
        for (int i = 0; i < p_vertices.Length; i++)
        {
            p_vertices[i].GetComponent<EnergyPulseParticle>().onDestroy += RemoveVerticeOnDestroy;
            lineVertices.Add(p_vertices[i]);
        }
        //UpdateLine();
        UpdateVelocity();
        count = 0;
        timeSpawned = 0;
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
        if (_destructionScheduled)
        {
            OnDestroyRoutine();
           _destructionScheduled = false;
        }

        timeSpawned += Time.fixedDeltaTime;
        if(timeSpawned > timeToKillPlayer)
        {
            onReadyToCollide();
        }
    }

    public void UpdateLine()
    {
        lineRenderer.SetPositions(GenerateLineVertices());
        //lineRenderer.material.color = new Color(1, 1, 1, 1);
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
        if (collision.gameObject.tag == "obstacle")
        {
            energyLevel -= collision.gameObject.GetComponent<Obstacle>().energyAbsorption;
            if (energyLevel <= 0)
            {
                OnDestroyRoutine();
            }
            //_velocityUpdateScheduled = true;
            UpdateVelocity();
        }
        else if (collision.gameObject.tag == "destructable")
        {
            energyLevel -= collision.gameObject.GetComponent<Obstacle>().energyAbsorption;
            if (energyLevel <= 0)
            {
                _destructionScheduled = true;
            }
            collision.gameObject.GetComponent<Obstacle>().DeactivateMe();
        }
        else if (collision.gameObject.tag == "player")
        {            
            onCollisionWithPlayer();
        }
    }

    public void UpdateVelocity()
    {
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * (energyLevel / 2) * pulseForce;
        //GetComponent<TrailRenderer>().material.color = new Color((((float)energyLevel / maximumEnergyLevel)/2) + 0.5f, 0, (float)energyLevel / maximumEnergyLevel, (((float)energyLevel / maximumEnergyLevel) / 4) + 0.75f);
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
