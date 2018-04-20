using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum testEnum{

	One,
	two
}

public class testenum : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //foreach (int  myCode in Enum.GetValues(typeof(testEnum))) {
        //    string strName = Enum.GetName(typeof(testEnum), myCode);//获取名称
        //    Debug.LogError(strName);
        //}
        //Debug.LogError((testEnum.two).ToString());

        foreach (testEnum myCode in Enum.GetValues(typeof(testEnum)))
        {
            string strName = myCode.ToString();//获取名称
            Debug.LogError(strName);
            Debug.LogError(myCode);
        }



	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
