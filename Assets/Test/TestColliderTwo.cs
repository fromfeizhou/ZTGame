using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColliderTwo : MonoBehaviour {

    TestCoiller collider1;
    TestCoiller collider2;
    void Start()
    {
        collider1 = transform.Find("1").GetComponent<TestCoiller>();
        collider2 = transform.Find("2").GetComponent<TestCoiller>();

    }
	
	// Update is called once per frame
	void Update () {
		
        if(ZTCollider.CheckCollision( collider1.collider,collider2.collider))
        {
            Debug.LogError(">>>>>>");
        }

	}
}
