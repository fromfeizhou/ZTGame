using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

using UnityEditor;


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


	public string GetDefine(){
		string formatStr = "\n--@brife {0}\nPB_{1} = {{\n{2}}}\n";
		string formatCommand = "\t{0} = {1}, \t\t--{2}\n";

		string CommandStr = string.Format (formatCommand,"MODEL",moduleId,"模块ID");
		for (int i = 0; i < commandList.Count; i++) {
			cCommand cmd = commandList [i];
			string commandName = cmd.commandEn.Substring (moduleEn.Length + 1);
			CommandStr += string.Format (formatCommand,commandName.ToUpper(),cmd.commandId,cmd.commandCn.ToUpper());
		}

		return string.Format (formatStr,moduleCn,moduleEn.ToUpper(),CommandStr);
	}

	public string GetDataTable(){
		string formatTable = "\n\t[{0}] = {{\n{1}\t}},\n";
		string formatProtocal = "\t\t[{0}] = \"{1}\",\n";
		string ProtocalStr = string.Empty;
		for (int i = 0; i < commandList.Count; i++) {
			cCommand cmd = commandList [i];
			ProtocalStr += string.Format (formatProtocal, cmd.commandId, cmd.commandEn);
		}
		return string.Format (formatTable,moduleId,ProtocalStr);
	}
}

public class ParseProto{

	private const string protoPath = "BuildProto/protobuf.proto";
	private const string ErrorCodePath = "ecode_zh.hrl";
	private const string ProtocalDefine_SavePath = "Assets/LuaScript/ProtoBuff/ProtocolDefine.txt";
	private const string NetProtocal_SavePath = "Assets/LuaScript/ProtoBuff/NetProtocol.txt";


	private static void SaveCSharpNetFacadeFiles(List<cModule> moduleList)
	{
		if (File.Exists (NetProtocal_SavePath))
			File.Delete (NetProtocal_SavePath);

		string scriptStr = string.Empty;
		for (int i = 0; i < moduleList.Count; i++)
		{
			scriptStr += moduleList [i].GetDefine ();
		}
		File.WriteAllText (ProtocalDefine_SavePath,scriptStr.Trim());

		string formatModule = "MODEL_PB = {{{0}}}";
		string ModuleStr = string.Empty;
		for (int i = 0; i < moduleList.Count; i++)
		{
			ModuleStr += moduleList [i].GetDataTable ();
		}
		File.WriteAllText (NetProtocal_SavePath,string.Format (formatModule,ModuleStr));

	}

	private static List<cModule> ParseProtoToModuleList(){
		List<cModule> moduleList = new List<cModule> ();
		List<string> allLines = new List<string> (File.ReadAllLines (protoPath));
		List<string> message = allLines.FindAll (a => a.Contains ("_s2c"));
		for (int i = 0; i < message.Count; i++) {
			message [i] = message [i].Replace ("message", string.Empty).Replace ("{", string.Empty).Replace ("}", string.Empty).Trim ();
		}

		List<string> contentsModel = allLines.FindAll (a => (a.Contains ("#model_begin#") && a.Contains ("#model_end#")));
		for (int i = 0; i < contentsModel.Count; i++) {
			moduleList.Add (cModule.Create (contentsModel[i]));
		}

		List<string> contentsCommand = allLines.FindAll (a => (a.Contains ("#begin#") && a.Contains ("#end#")));
		for (int i = 0; i < contentsCommand.Count; i++) {
			cCommand command = cCommand.Create (contentsCommand[i]);

			int idx = moduleList.FindIndex(a=>a.moduleId == command.moduleId && a.moduleEn == command.moduleEn);
			if (idx >= 0)
				moduleList [idx].commandList.Add (command);
			else
				UnityEngine.Debug.LogError ("Found Module. moduleId:" + command.moduleId +  "moduleEn:" + command.moduleEn);
		}
		moduleList.Sort ((left,right)=>left.moduleId.CompareTo(right.moduleId));
		return moduleList;
	}

	[MenuItem("Tools/ParseProtoFile")]
	private static void ParseProtoFile()
	{
		/** 编译 *.proto */
		string batPath = Application.dataPath + @"\..\BuildProto\";
		batPath = batPath.Replace("/","\\");
		ProcessCommand ("build.bat",batPath);  
		AssetDatabase.Refresh ();

		/** 解析 *.proto */
		List<cModule> moduleList = ParseProtoToModuleList();
		SaveCSharpNetFacadeFiles (moduleList);
		AssetDatabase.Refresh ();
	}

	//[MenuItem("Tools/ParseErrorCode")]
	private static void ParseErrorCode()
	{
		
	}


	/** 执行 bat 批处理 */
	private static void ProcessCommand(string command, string workPath){
		System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo (command);
		info.CreateNoWindow = false;
		info.ErrorDialog = true;
		info.UseShellExecute = true;
		info.WorkingDirectory = workPath;
		if (info.UseShellExecute) {
			info.RedirectStandardOutput = false;
			info.RedirectStandardError = false;
			info.RedirectStandardInput = false;
		} else {
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.RedirectStandardInput = true;

			info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
			info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
		}

		System.Diagnostics.Process process = System.Diagnostics.Process.Start (info);

		if (!info.UseShellExecute) {
			Debug.Log (process.StandardOutput);
			Debug.Log (process.StandardError);
		}

		process.WaitForExit ();
		process.Close ();

	}

}
