using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float movementspeed;
    public ShowPanels panel;
    public Animator anim;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetAxis("Horizontal") < 0){
			transform.Translate (Vector3.left * movementspeed * Time.deltaTime);
            anim.SetFloat("horizontalspeed", GetComponent<Rigidbody2D>().velocity.x);
		}

		if(Input.GetAxis("Vertical") > 0)
        {
			transform.Translate (Vector3.up * movementspeed * Time.deltaTime);
            anim.SetFloat("verticalspeed", GetComponent<Rigidbody2D>().velocity.y);
        }

		if(Input.GetAxis("Vertical") < 0)
        {
			transform.Translate (Vector3.down * movementspeed * Time.deltaTime);
            anim.SetFloat("verticalspeed", GetComponent<Rigidbody2D>().velocity.y);
        }

		if(Input.GetAxis("Horizontal") > 0){
			transform.Translate (Vector3.right * movementspeed * Time.deltaTime);
            anim.SetFloat("horizontalspeed", GetComponent<Rigidbody2D>().velocity.x);
        }

        anim.SetFloat("movementspeed", Mathf.Abs(GetComponent<Rigidbody2D>().velocity.magnitude));
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
