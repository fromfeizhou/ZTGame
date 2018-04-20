using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void testFun()
    {


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;   //从transform到target绘制一条蓝色的线

        for (float i = 0; i < 100; i+= 0.2f)
        {
            for (float j = 0; j < 100; j+=0.2f)
            {
                Gizmos.DrawLine(new Vector3(i, 1, 0), new Vector3(i, 1, MapDefine.MAPITEMTOTALSIZE));
                Gizmos.DrawLine(new Vector3(0, 1, j), new Vector3(MapDefine.MAPITEMTOTALSIZE, 1, j));

            }
        }
  
    }
}
