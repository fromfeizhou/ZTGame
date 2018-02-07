using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.game.client.network;
using System.IO;

[System.Serializable]
public class cCommand
{
	public int moduleId;
	public string moduleEn = string.Empty;

	public string commandEn = string.Empty;
	public string commandCn = string.Empty;

	public int commandId;

	public static cCommand Create(string str){
		cCommand command = new cCommand ();
		string tmpStr = str;
		tmpStr = tmpStr.Substring (tmpStr.IndexOf ("#begin#") - 1);
		tmpStr = tmpStr.Replace ("#begin#",string.Empty);
		tmpStr = tmpStr.Replace ("#end#",string.Empty);
		List<string> param = new List<string> (tmpStr.Trim ().Split ('#'));
		param.RemoveAll(a=>string.IsNullOrEmpty(a));
		command.moduleEn = param [0];
		command.commandEn = param [1];
		command.moduleId = int.Parse(param [2]);
		command.commandId = int.Parse(param [3]);
		command.commandCn = param [4];
		return command;
	}
}

[System.Serializable]
public class cModule
{
	public string moduleEn = string.Empty;
	public string moduleCn = string.Empty;
	public int moduleId;
	public List<cCommand> commandList = new List<cCommand>();
	public static cModule Create(string str){
		cModule module = new cModule ();
		string tmpStr = str;
		tmpStr = tmpStr.Substring (tmpStr.IndexOf ("#model_begin#") - 1);
		tmpStr = tmpStr.Replace ("#model_begin#",string.Empty);
		tmpStr = tmpStr.Replace ("#model_end#",string.Empty);
		List<string> param = new List<string> (tmpStr.Trim ().Split ('#'));
		param.RemoveAll(a=>string.IsNullOrEmpty(a));
		module.moduleEn = param [0];
		module.moduleId = int.Parse(param [1]);
		module.moduleCn = param [2];
		return module;
	}
}

public class NetWorkManagerTester : MonoSingleton<NetWorkManagerTester> {
	
	public List<cModule> moduleList = new List<cModule> ();

	public override void Init ()
	{
		NetWorkManager.Instace.Init (null);
		LoadModuleInfo ();
	}
	private void LoadModuleInfo()
	{
		string[] files = Directory.GetFiles ("Assets/Resources/TestProtocolInfo","*.txt");
		for (int i = 0; i < files.Length; i++) {
			cModule module = JsonUtility.FromJson<cModule> (File.ReadAllText (files [i]));
			for (int j = 0; j < module.commandList.Count; j++) {
				byte curModuleId = (byte)module.commandList [j].moduleId;
				if (NetWorkManager.Instace.ModuleNetFacadeDic.ContainsKey (curModuleId)) {
					if (NetWorkManager.Instace.ModuleNetFacadeDic [(byte)module.commandList [j].moduleId].methodInfoDic.ContainsKey ((byte)module.commandList [j].commandId)) {
						moduleList.Add (module);
					}
				}
			}
		}
	}

}
