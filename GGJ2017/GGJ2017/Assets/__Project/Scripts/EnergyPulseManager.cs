using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPulseManager : MonoBehaviour {

    public GameObject pulseParticlesModel;
    public int pulseParticlesPoolSize = 40;
    public List<GameObject> pulseParticlesInactivePool;
    public List<GameObject> pulseParticlesActivePool;

    public int particlesPerPulse = 15;
    public float pulseForce = 10;

    // Use this for initialization
    void Start () {
        InitializePulseParticlePool();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EmitParticles();
        }
        if (Input.GetMouseButtonDown(0))
        {
            EmitParticles();
        }
    }

    public void InitializePulseParticlePool()
    {
        for (int i = 0; i < pulseParticlesPoolSize; i++)
        {
            GameObject __tempObject = (GameObject)Instantiate(pulseParticlesModel, Vector3.zero, Quaternion.identity);
            __tempObject.SetActive(false);
            __tempObject.GetComponent<EnergyPulseParticle>().pulseForce = pulseForce;
            __tempObject.GetComponent<EnergyPulseParticle>().onDestroy += delegate(GameObject p_Object)
            {
                //remove from active pool and add to inactive pool
                pulseParticlesInactivePool.Add(p_Object);
                p_Object.SetActive(false);                
                pulseParticlesActivePool.Remove(p_Object);
            };
            pulseParticlesInactivePool.Add(__tempObject);
        }
    }

    public void EmitParticles()
    {
        GameObject __firstObject = null;
        //transform.position = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30));
        if (pulseParticlesInactivePool.Count >= particlesPerPulse)
        {
            for (int i = 0; i < particlesPerPulse; i++)
            {
                if (i == 0) __firstObject = pulseParticlesInactivePool[0];
                float __angle = 2 * Mathf.PI * ((float)i / particlesPerPulse);
                pulseParticlesInactivePool[0].SetActive(true);
                pulseParticlesInactivePool[0].GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(__angle), Mathf.Sin(__angle)) * pulseForce;
                GameObject[] lineVertices;
                if (i > 0)
                {
                    lineVertices = new GameObject[] { pulseParticlesActivePool[pulseParticlesActivePool.Count - 1] };
                }
                else
                {
                    lineVertices = new GameObject[0];
                }
                pulseParticlesInactivePool[0].GetComponent<EnergyPulseParticle>().EmissionStarted(lineVertices);
                pulseParticlesInactivePool[0].transform.position = (Vector2)transform.position;

                pulseParticlesActivePool.Add(pulseParticlesInactivePool[0]);
                pulseParticlesInactivePool.RemoveAt(0);
            }
            __firstObject.GetComponent<EnergyPulseParticle>().EmissionStarted(new GameObject[] { pulseParticlesActivePool[pulseParticlesActivePool.Count - 1] });
        }
        else
        {
            Debug.Log("Not enough objects in the pool to do this action");
        }
    }
}
