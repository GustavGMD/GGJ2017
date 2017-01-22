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
    public Collider2D playercollider;
    public bool dead;
    public Rigidbody2D rb;

	public static int backgroundId = -1;

    // Use this for initialization
    void Awake() {
        playercollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        playercollider.enabled = true;
        dead = false;
        GameObject startPoint = GameObject.Find("StartPoint");
        if (startPoint != null) {
            transform.position = startPoint.transform.position;
            startPoint.SetActive(false);
        }
        anim.ResetTrigger("died");
		playTheme ();
    }
	
	// Update is called once per frame
	void Update () {
        if (!dead) {
            horizontalSpeed = Input.GetAxis("Horizontal");
            verticalSpeed = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(horizontalSpeed, verticalSpeed, 0) * Time.deltaTime * movementspeed);

            if (horizontalSpeed > 0)
            {
                transform.localScale = new Vector3((Mathf.Abs(transform.localScale.x) * -1), transform.localScale.y, transform.localScale.z);
            }
            else if (horizontalSpeed < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            anim.SetFloat("horizontalspeed", Mathf.Abs(horizontalSpeed));
            anim.SetFloat("verticalspeed", Mathf.Abs(verticalSpeed));
        }
    }

	public bool menu = false;
	private int musica;
	void playTheme(){
		if (!menu) {
			if (backgroundId != -1) {
				AudioManagerSingleton.instance.ChangeVolume ( backgroundId,1);
			} else {
				backgroundId = AudioManagerSingleton.instance.PlaySound (
					AudioManagerSingleton.AudioClipName.BACKGROUND, AudioManagerSingleton.AudioType.MUSIC, true, 1);
			}
		} 

	}
		
        //Time.timeScale = 0;
    public void DefeatRoutine() {
        anim.SetTrigger("died");
        playercollider.enabled = false;
        rb.Sleep();
        dead = true;
        StartCoroutine(WaitSeconds(1.7f));
		AudioManagerSingleton.instance.ChangeVolume ( backgroundId, 0.3f);
		AudioManagerSingleton.instance.PlaySound (
			AudioManagerSingleton.AudioClipName.DEAD_1, AudioManagerSingleton.AudioType.MUSIC, false, 1.2f);
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
			AudioManagerSingleton.instance.ChangeVolume ( backgroundId,0.3f);
            victoryPanel.ShowGameOverPanel();
			AudioManagerSingleton.instance.PlaySound (
				AudioManagerSingleton.AudioClipName.WIN, AudioManagerSingleton.AudioType.SFX, false, 1);
        }
    }
}
