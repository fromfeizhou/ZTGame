using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.game.client.network;

public class NetWorkManagerTesterPanel : MonoBehaviour {

	private NetWorkManagerTester tester{
		get{ 
			return NetWorkManagerTester.GetInstance ();
		}
	}
	public Dropdown _dropdownModule;
	public Dropdown _dropdownCommand;

	void Start()
	{
		tester.Init ();
		SetDropDownList ();
	}

	public void SetDropDownList()
	{
		_dropdownModule.ClearOptions ();
		List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData> ();
		for (int i = 0; i < tester.moduleList.Count; i++) {
			if(!optionData.Exists(a=>a.text.Equals(tester.moduleList [i].moduleCn)))
			{
				Dropdown.OptionData data = new Dropdown.OptionData ();
				data.text = tester.moduleList [i].moduleCn;
				optionData.Add (data);
			}
		}
		_dropdownModule.AddOptions (optionData);
		UpdateCommandList ();
	}


	private string GetCurModule{
		get{ 
			return _dropdownModule.options [_dropdownModule.value].text;
		}
	}

	public void OnChangeModule(int index)
	{
		UpdateCommandList ();
	}

	public void UpdateCommandList(){
		Debug.Log ("Module：" + GetCurModule);
		List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData> ();
		cModule curModule = tester.moduleList.Find (a => a.moduleCn.Equals (GetCurModule));
		if (curModule != null) {
			for (int i = 0; i < curModule.commandList.Count; i++) {
				//if(curModule.commandList.Exists(a=>a.commandCn == curModule.commandList[i].commandCn))
					optionData.Add (new Dropdown.OptionData (){ text = curModule.commandList[i].commandCn});
			}
		}
		_dropdownCommand.ClearOptions ();
		_dropdownCommand.AddOptions (optionData);

	}

	public void OnBtnClick_Connect()
	{
		NetWorkManager.Instace.Connect ();
	}
}
