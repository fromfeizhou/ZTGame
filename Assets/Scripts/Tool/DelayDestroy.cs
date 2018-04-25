using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour {

	public float DelayDestroyTime = 1.0f;
	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds (DelayDestroyTime);
		Destroy(gameObject);
	}
}
