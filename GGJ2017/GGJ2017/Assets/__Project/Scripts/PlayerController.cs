using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float movementspeed;
    public ShowPanels defeatPanel;
    public ShowPanels victoryPanel;
    public Animator anim;
    public float verticalSpeed = 0;
    public float horizontalSpeed = 0;

    // Use this for initialization
    void Awake() {
        GameObject startPoint = GameObject.Find("StartPoint");
        if (startPoint != null) {
            transform.position = startPoint.transform.position;
            startPoint.SetActive(false);
        }
        anim.ResetTrigger("died");
    }
	
	// Update is called once per frame
	void Update () {
		
        horizontalSpeed = Input.GetAxis("Horizontal");
        verticalSpeed = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontalSpeed, verticalSpeed, 0) * Time.deltaTime * movementspeed);

        if(horizontalSpeed > 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if(horizontalSpeed < 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        anim.SetFloat("horizontalspeed", Mathf.Abs(horizontalSpeed));
        anim.SetFloat("verticalspeed", Mathf.Abs(verticalSpeed));
    }

    public void DefeatRoutine() {
        anim.SetTrigger("died");
        StartCoroutine(WaitSeconds(1.7f));
    }

    public IEnumerator WaitSeconds(float secs) {
        yield return new WaitForSeconds(secs);
        ContinueDeath();
    }

    public void ContinueDeath() {
        defeatPanel.ShowGameOverPanel();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "levelGoal")
        {
            victoryPanel.ShowGameOverPanel();
        }
    }
}
