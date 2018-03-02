using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginPanel_ServerItem : MonoBehaviour{
	public class sItemData
	{
		public sItemData(string str)
		{
			string[] param = str.Trim().Split('=');
			if(param.Length == 2){
				Name = param[0];
				string[] addParam = param[1].Split(':');
				if(addParam.Length == 2)
				{
					Ip = addParam[0];
					Port = int.Parse(addParam[1]);
				}
			}
		}
		public string Name;
		public string Ip;
		public int Port;

		public override string ToString ()
		{
			return string.Format ("{0}={1}:{2}",Name,Ip,Port);
		}
	}

	private sItemData _itemData;
	public sItemData ItemData{
		get{ 
			return _itemData;
		}
	}
	private Text _txtServerName;
	public void Init(sItemData itemData, System.Action<sItemData> onClick)
	{
		_itemData = itemData;
		_txtServerName = transform.Find ("Txt_ServerName").GetComponent<Text> ();
		transform.Find ("Btn_Confirm").GetComponent<Button> ().onClick.AddListener (()=>
			{
				if(onClick != null)
				{
					onClick(itemData);
				}
			});
		Update_ServerName ();
	}

	public void Update_ServerName(bool isCurSelect = false)
	{
		_txtServerName.text = _itemData.Name;
		if (isCurSelect) {
			_txtServerName.text += "(当前选择)";
		}
	}

}
