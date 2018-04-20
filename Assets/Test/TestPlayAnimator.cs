using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayAnimator : MonoBehaviour
{
    private Animator temp = null;
	// Use this for initialization
	void Start ()
	{
	    temp = transform.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyUp(KeyCode.A))
	    {
	        temp.Play("attack2");

        }
	    if (Input.GetKeyUp(KeyCode.S))
	    {
	        temp.Play("attack4");
        }

    }
}
