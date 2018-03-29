using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;


namespace UnityEditor.ProtoTreeView
{
	public class ProtoTreeViewItem : TreeViewItem{
		public enum eType
		{
			model,command,message
		}

		public eType Type{ get; private set;}
		public sModel _model;
		public sCommand _command;
		public sMessage _message;
	
		public ProtoTreeViewItem(sModel model)
		{
			Type = eType.model;
			this._model = model;
			this.id = int.Parse(model.id);
			this.depth = 0;
			this.displayName =  "[" + this.id + "]" + model.nameEn;
		}

		public ProtoTreeViewItem(sModel model, sCommand command)
		{
			Type = eType.command;
			this._model = model;
			this._command = command;
			this.id = int.Parse(command.modelId) * 100 + int.Parse(command.id);
			this.depth = 1;
			this.displayName = "[" + this.id + "]" + command.nameEn;
		}

		public ProtoTreeViewItem(sModel model, sCommand command, sMessage message)
		{
			Type = eType.message;
			this._model = model;
			this._command = command;
			this._message = message;
			this.id = int.Parse(message.modelId) * 10000 + int.Parse(message.commandId) * 100 + int.Parse(message.id);
			this.depth = 2;
			this.displayName = message.nameEn;
		}

		public override string ToString ()
		{
			string output = string.Empty;
			switch (Type) {
				case eType.model:
				{
					output = _model.ToString ();
					break;
				}
				case eType.command:
				{
					output += _model.ToString ();
					output += _command.ToString ();
					break;
				}
				case eType.message:
				{
					output += _model.ToString ();
					output += _command.ToString ();
					output += _message.ToString ();
					break;
				}
			}
			return output;
		}
	}
}

