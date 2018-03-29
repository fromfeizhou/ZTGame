using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;


namespace UnityEditor.ProtoTreeView
{
	public class sVariable{
		private const string patterns = @"[^ ^\[][0-9a-z_]+|[0-9]|[a-z]|//.*";//解析message

		public string flag = string.Empty;
		public string type = string.Empty;
		public string nameEn = string.Empty;
		public string index = string.Empty;
		public string nameCn = string.Empty; 
		public string defValue = string.Empty;

		public static sVariable ValueOf(string str){
			sVariable tmpVar = new sVariable ();
			MatchCollection matchCol = Regex.Matches (str, patterns, RegexOptions.Multiline);

			List<string> paramList = new List<string> ();
			for (int i = 0; i < matchCol.Count; i++) {
				if (matchCol.Count > 5 && matchCol [i].Value.Equals ("default")) {
					tmpVar.defValue = matchCol [i+++1].Value;
				} else {
					paramList.Add (matchCol [i].Value);
				}
			}
			tmpVar.flag = paramList [0];
			tmpVar.type = paramList [1];
			tmpVar.nameEn = paramList [2];
			tmpVar.index = paramList [3];
			if(paramList.Count > 4)
				tmpVar.nameCn = paramList [4];
			return tmpVar;
		}

		public override string ToString ()
		{
			string output = string.Empty;
			output += flag + ", ";
			output += type + ", ";
			output += nameEn + ", ";
			output += index + ", ";
			output += nameCn;
			if (!string.IsNullOrEmpty (defValue))
				output += ", " + defValue;
			return output;
		}

	}
	public class sMessage{
		public string modelId = string.Empty;
		public string commandId = string.Empty;
		public string id = string.Empty;
		public string nameEn = string.Empty;
		public string nameCn = string.Empty; 
		private List<sVariable> varList = new List<sVariable>();
		public static sMessage ValueOf(string msgStr)
		{
			sMessage msg = new sMessage ();
			List<string> contents = new List<string> (msgStr.Split ('\n'));
			if (contents [0].StartsWith ("//")) {
				msg.nameCn = contents [0].Replace ("//", string.Empty).Trim();
				contents.RemoveAt (0);
			}
			if (contents [0].StartsWith ("message")) {
				msg.nameEn = Regex.Matches (contents [0], "[^ ].\\w+[^ ]", RegexOptions.Multiline) [1].Value;
				contents.RemoveAt (0);
			}
			for (int i = 0; i < contents.Count; i++) {
				if (contents [i].Trim().Length == 1 || contents [i].Trim().StartsWith("//"))
					continue;
				msg.varList.Add (sVariable.ValueOf(contents[i]));
			}
			return msg;
		}

		public override string ToString ()
		{
			string title = nameCn + "\n" + nameEn;
			string output = string.Empty;
			for (int i = 0; i < varList.Count; i++) {
				output += "        " + varList [i].ToString () + "\n";
			}

			string Split = "----------------------------------------------------------------\n";
			return Split + string.Format ("{0}\n{{\n{1}}}",title,output);
		}


		public List<string> GetTypeDependent()
		{
			List<string> typeList = new List<string> ();
			for (int i = 0; i < varList.Count; i++) {
				if (typeList.Exists (a => a.Equals (varList [i].type)))
					continue;
				typeList.Add (varList [i].type);
			}
			return typeList;
		}
	}
	public class sCommand{
		public string id;
		public string modelId;
		public string nameEn;
		public string nameCn;
		public sMessage msg_c2s;
		public sMessage msg_s2c;
		public static sCommand ValueOf(string content, string pattern){
			sCommand command = new sCommand ();
			MatchCollection matchCol = Regex.Matches (content, pattern, RegexOptions.Multiline);
			command.nameEn = matchCol [2].Value;
			command.modelId = matchCol [3].Value;
			command.id = matchCol [4].Value;
			command.nameCn = matchCol [5].Value;
			return command;
		}

		public override string ToString ()
		{
			string output = string.Empty;
			output += "----------------------------------------------------------------\n";
			output += "指令信息:\n";
			output += "    CommandId:" + id + "\n";
			output += "    NameEn:" + nameEn + "\n";
			output += "    NameCn:" + nameCn + "\n";
			return output;
		}
	}
	public class sModel{
		public string id;
		public string nameEn;
		public string nameCn;
		public List<sCommand> cmdList = new List<sCommand> ();
		public static sModel ValueOf(string content, string pattern)
		{
			sModel model = new sModel ();
			MatchCollection matchCol = Regex.Matches (content, pattern, RegexOptions.Multiline);
			model.nameEn = matchCol [1].Value;
			model.id = matchCol [2].Value;
			model.nameCn = matchCol [3].Value;
			return model;
		}
		public void AddCommand(sCommand command)
		{
			cmdList.Add (command);
		}

		public int CompareTo(sModel model){
			int destModel = int.Parse (model.id);
			int scrModel = int.Parse (this.id);
			return scrModel.CompareTo (destModel);
		}

		public override string ToString ()
		{
			string output = string.Empty;
			output += "----------------------------------------------------------------\n";
			output += "模块信息:\n";
			output += "    ModelId:" + id + "\n";
			output += "    NameEn:" + nameEn + "\n";
			output += "    NameCn:" + nameCn + "\n";
			return output;
		}
	}
	public class sCommon{
		public int id = 0;
		public string nameEn;
		public string nameCn; 
		public List<sMessage> msgList = new List<sMessage>();
		private List<string> buffStr = new List<string>();
		public void AddStr(string str)
		{
			buffStr.Add (str);
		}
	}
	public class ProtoTreeView : TreeView
	{
		public System.Action<ProtoTreeViewItem> OnChangeItem;
		private SearchField m_SearchField;

		private const string protoPath = "BuildProto/protobuf.proto";
		private Dictionary<int,ProtoTreeViewItem> itemDic = new Dictionary<int, ProtoTreeViewItem>();
		List<sMessage> commonMsgList = new List<sMessage>();
		List<sModel> modelList = new List<sModel> ();
		private int selected;
		public ProtoTreeView(TreeViewState treeViewState): base(treeViewState)
		{
			Reload();
			m_SearchField = new SearchField ();
			m_SearchField.downOrUpArrowKeyPressed += base.SetFocusAndEnsureSelectedItem;
		}


		protected override TreeViewItem BuildRoot ()
		{
			selected = -1;
			Clear ();
			LoadProto ();
			TreeViewItem root = new TreeViewItem (){depth = -1};
			SetupDepthsFromParentsAndChildren (root);

			for (int i = 0; i < modelList.Count; i++) {
				sModel model = modelList [i];
				ProtoTreeViewItem modelItem = new ProtoTreeViewItem (model);
				for(int j = 0;j<model.cmdList.Count;j++)
				{
					sCommand command = model.cmdList [j];

					ProtoTreeViewItem commonItem = new ProtoTreeViewItem (model,command);
					if (command.msg_c2s != null) {
						ProtoTreeViewItem c2sItem = new ProtoTreeViewItem (model,command,command.msg_c2s);
						commonItem.AddChild (c2sItem);
						itemDic [c2sItem.id] = c2sItem;
					}
					if (command.msg_s2c != null) {
						ProtoTreeViewItem s2cItem = new ProtoTreeViewItem (model,command,command.msg_s2c);
						commonItem.AddChild (s2cItem);
						itemDic [s2cItem.id] = s2cItem;
					}
					modelItem.AddChild (commonItem);
					itemDic [commonItem.id] = commonItem;

				}
				root.AddChild (modelItem);
				itemDic [modelItem.id] = modelItem;
			}
			SetupDepthsFromParentsAndChildren (root);
			return root;
   		}
		public override void OnGUI (UnityEngine.Rect rect)
		{
			base.searchString = m_SearchField.OnToolbarGUI (new Rect(rect.x,rect.y,rect.width,17),base.searchString);
			base.OnGUI (new Rect(rect.x,rect.y+17,rect.width,rect.height-17));
			DoCheckSelect ();
		}
		private void DoCheckSelect(){
			if (HasSelection ()) {
				IList<int> selects = GetSelection ();
				if (selects != null && selects.Count > 0) {
					if (selected != selects [0] && OnChangeItem != null) {
						selected = selects [0];
						OnChangeItem (itemDic [selected]);
					}
				}
			}
		}

		public void CollectMsgDependent(sMessage scrMsg, List<sMessage> destMsgList){
			List<string> msgDependentList = scrMsg.GetTypeDependent ();
			for (int i = 0; i < msgDependentList.Count; i++) {
				sMessage msg = commonMsgList.Find (a => a.nameEn.Equals (msgDependentList[i]));
				if (msg != null) {
					CollectMsgDependent (msg,destMsgList);
					if(!destMsgList.Exists(a=>a.nameEn.Equals(msg.nameEn))){
						destMsgList.Add (msg);
					}
				}
			}
		}

		public void Clear(){
			if (commonMsgList != null) {
				commonMsgList.Clear ();
			}
			if (modelList != null) {
				modelList.Clear ();
			}

			if (itemDic != null) {
				itemDic.Clear ();
			}
		}
		public void LoadProto(){
			string allText = System.IO.File.ReadAllText (protoPath).Trim();
			string patternMsgCommon = @"(//.\w+\n|)message p.\w+ \{[\s\S]*?\}";//匹配出“p_”开头的message，包含注释
			string patternMsgModel = @"(//.\w+\n|)message [^p].\w+ \{[\s\S]*?\}";//匹配出非“p_”开头的message，包含注释
			string patternModel = @"##model_begin.*##model_end##";//收集模块
			string patternCommond = @"##begin.*##end##";//收集协议号
			string patternParam = @"[^#]+";//解析指令

			List<sMessage> modelMsgList = new List<sMessage> ();

			foreach (Match match in Regex.Matches(allText,patternMsgCommon,RegexOptions.Multiline)) {
				commonMsgList.Add(sMessage.ValueOf (match.Value));
			}
			foreach (Match match in Regex.Matches(allText,patternMsgModel,RegexOptions.Multiline)) {
				modelMsgList.Add(sMessage.ValueOf (match.Value));
			}

			foreach (Match match in Regex.Matches(allText,patternModel,RegexOptions.Multiline)) {
				modelList.Add(sModel.ValueOf (match.Value,patternParam));
			}

			modelList.Sort((left,right)=>{return left.CompareTo(right);});
			foreach (Match match in Regex.Matches(allText,patternCommond,RegexOptions.Multiline)) {
				sCommand command = sCommand.ValueOf (match.Value,patternParam);

				command.msg_c2s = modelMsgList.Find (a=>a.nameEn.Equals(command.nameEn + "_c2s"));
				if (command.msg_c2s != null) {
					command.msg_c2s.id = "1";
					command.msg_c2s.nameCn = command.nameCn;
					command.msg_c2s.modelId = command.modelId;
					command.msg_c2s.commandId = command.id;
				}
				
				command.msg_s2c = modelMsgList.Find (a=>a.nameEn.Equals(command.nameEn + "_s2c"));
				if (command.msg_s2c != null) {
					command.msg_s2c.id = "2";
					command.msg_s2c.nameCn = command.nameCn;
					command.msg_s2c.modelId = command.modelId;
					command.msg_s2c.commandId = command.id;
				}

				sModel model = modelList.Find (a => a.id == command.modelId);
				if (model != null)
					model.AddCommand (command);
				else
					Debug.LogError ("found modelId:" + command.modelId);
			}
		}

		private const string ProtocalDefine_SavePath = "Assets/LuaScript/ProtoBuff/ProtocolDefineNew.txt";
		private const string NetProtocal_SavePath = "Assets/LuaScript/ProtoBuff/NetProtocolNew.txt";

		public void Export()
		{
			Export_ProtocolDefine (ProtocalDefine_SavePath);
			Export_NetProtocol(NetProtocal_SavePath);
		}

		private void Export_ProtocolDefine(string path)
		{
			string formatStr = "PROTOCOL = {{\n{0}}}\n";
			string cmdStr = string.Empty;
			for (int i = 0; i < modelList.Count; i++) {
				cmdStr += "--@brife " + modelList [i].nameCn + "\n";
				for (int j = 0; j < modelList [i].cmdList.Count; j++) {
					int modelId = int.Parse( modelList [i].cmdList [j].modelId);
					int commandId = int.Parse( modelList [i].cmdList [j].id);
					cmdStr += "        " + modelList [i].cmdList [j].nameEn.ToUpper() + " = " + modelId * 1000 + commandId + "        --" + modelList [i].cmdList [j].nameCn + "\n";
				}
				cmdStr += "\n";
			}

			if (System.IO.File.Exists (path)) {
				System.IO.File.Delete (path);
			}
			System.IO.File.WriteAllText (path,string.Format(formatStr,cmdStr).Trim());
		}

		private void Export_NetProtocol(string path)
		{
			string formatStr = "MODEL_PB = {{\n{0}}}\n";
			string cmdStr = string.Empty;
			for (int i = 0; i < modelList.Count; i++) {
				for (int j = 0; j < modelList [i].cmdList.Count; j++) {
					int modelId = int.Parse( modelList [i].cmdList [j].modelId);
					int commandId = int.Parse( modelList [i].cmdList [j].id);
					cmdStr += "        [" + modelId * 1000 + commandId + "] = \"" + modelList [i].cmdList[j].nameEn + "\",\n";
				}
			}

			if (System.IO.File.Exists (path)) {
				System.IO.File.Delete (path);
			}
			System.IO.File.WriteAllText (path,string.Format(formatStr,cmdStr).Trim());
		}

	}
}

