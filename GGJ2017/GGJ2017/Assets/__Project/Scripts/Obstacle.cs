using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public int energyAbsorption = 1;

    private bool _deactivationScheduled = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (_deactivationScheduled)
        {
            OnDestroyRoutine();
            _deactivationScheduled = true;
        }
	}

    public void DeactivateMe()
    {
        _deactivationScheduled = true;
    }

    public void OnDestroyRoutine()
    {
        gameObject.SetActive(false);
    }
}
