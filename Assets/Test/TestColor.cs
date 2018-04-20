using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Renderer temp= transform.GetComponent<Renderer>();
        if (temp != null)
        {
            Color obj_color = temp.material.color;

            obj_color.a = 0.5f;
           temp.material.SetColor("_Color", obj_color);
        }
        
        

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
