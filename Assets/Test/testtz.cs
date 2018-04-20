using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testtz : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    string param = 0.4892 + "";
        Debug.LogError("param:>>>>>" + param);
	    int paramValue = string.IsNullOrEmpty(param) ? 0 : (int)(float.Parse(param) * 100);

	    Debug.LogError(">>>>>> " + paramValue);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
