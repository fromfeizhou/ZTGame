using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLineee : MonoBehaviour {

	// Use this for initialization
	void Start () {

	    Ray ray = new Ray(transform.position, -Vector3.up * transform.position.y * 2 + transform.position);
	    RaycastHit[] hit;
	    hit = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Roof"));

    }
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 targetPos = transform.position + 100 * Vector3.up;
	   // targetPos = transform.position - targetPos;

        Debug.DrawLine(transform.position, targetPos, Color.red);
		
	}
}
