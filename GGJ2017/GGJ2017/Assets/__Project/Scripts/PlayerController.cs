﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float movementspeed;
    public ShowPanels panel;
    public Animator anim;
    public float verticalSpeed = 0;
    public float horizontalSpeed = 0;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
        horizontalSpeed = Input.GetAxis("Horizontal");
        verticalSpeed = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontalSpeed, verticalSpeed, 0) * Time.deltaTime * movementspeed);

        anim.SetFloat("horizontalspeed", Mathf.Abs(horizontalSpeed));
        anim.SetFloat("verticalspeed", Mathf.Abs(verticalSpeed));
    }

    public void DefeatRoutine()
    {
        //Application.LoadLevel(Application.loadedLevel);
        panel.ShowGameOverPanel();
        //Time.timeScale = 0;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "pulseElement")
        {
            //kill the player? Maybe this is better done at player's script...
            DefeatRoutine();
        }
    }
}
