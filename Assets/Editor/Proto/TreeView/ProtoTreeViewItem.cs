using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;


namespace UnityEditor.ProtoTreeView
{
	public class ProtoTreeViewItem : TreeViewItem{
		private sModel _model;
		private sCommand _command;
		private sMessage _message;
	
		public ProtoTreeViewItem(sModel model)
		{
			this._model = model;
			this.id = int.Parse(model.id);
			this.depth = 0;
			this.displayName = "Model:[" + this.id + "]:" + model.nameEn;
		}

		public ProtoTreeViewItem(sCommand command)
		{
			this._command = command;
			this.id = int.Parse(command.modelId) * 100 + int.Parse(command.id);
			this.depth = 1;
			this.displayName = "Command:[" + this.id + "]:" + command.nameEn;
		}

		public ProtoTreeViewItem(sMessage message)
		{
			this._message = message;
			this.id = int.Parse(message.modelId) * 10000 + int.Parse(message.commandId) * 100 + int.Parse(message.id);
			this.depth = 2;
			this.displayName = "message:[" + this.id + "]:" + message.nameEn;
		}

		public override string ToString ()
		{
			return string.Empty;
		}
	}
}

