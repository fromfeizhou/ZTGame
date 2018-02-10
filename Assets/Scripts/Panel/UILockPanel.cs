using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILockPanel : MonoBehaviour {

	public float delTime;
	private float curTime;
	private int index;
	private string[] points = new string[]{".","..","..."};
	private string _notice;

	public Text TxtNotice;

	public void Show(string notice)
	{
		curTime = delTime;
		_notice = notice;
		UpdateNotice ();
		gameObject.SetActive (true);
	}

	public void Hide()
	{
		gameObject.SetActive (false);
	}

	void Update()
	{
		curTime += Time.deltaTime;
		if (curTime >= delTime) {
			if (++index >= points.Length) {
				index = 0;
			}
			curTime = 0;
			UpdateNotice ();
		}
	}

	private void UpdateNotice()
	{
		TxtNotice.text = _notice + points [index];
	}
}
