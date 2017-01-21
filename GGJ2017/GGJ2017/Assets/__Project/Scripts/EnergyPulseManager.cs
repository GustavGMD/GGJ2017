using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPulseManager : MonoBehaviour {

    public GameObject pulseParticlesModel;
    public int pulseParticlesPoolSize = 40;
    public List<GameObject> pulseParticlesInactivePool;
    public List<GameObject> pulseParticlesActivePool;

    // Use this for initialization
    void Start () {
        InitializePulseParticlePool();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializePulseParticlePool()
    {
        for (int i = 0; i < pulseParticlesPoolSize; i++)
        {
            GameObject __tempObject = (GameObject)Instantiate(pulseParticlesModel, Vector3.zero, Quaternion.identity);
            __tempObject.SetActive(false);
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

    }
}
