using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.game.client.network;

public class UISystemNoticePanel : MonoBehaviour {

	public Text _txtPing;

	void Update()
	{
		_txtPing.text = NetWorkManager.Instace.Ping.ToString();
	}
}
