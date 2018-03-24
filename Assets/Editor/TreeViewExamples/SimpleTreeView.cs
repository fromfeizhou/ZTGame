using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;


namespace UnityEditor.TreeViewExamples
{
	class SimpleTreeView : TreeView
	{
		public System.Action<TreeViewItem,TreeViewItem> OnChangeItem;
		private int _index = -1;
		public List<TreeViewItem> allItems = new List<TreeViewItem>();
		private List<cModule> _moduleList;
		public SimpleTreeView(TreeViewState treeViewState): base(treeViewState)
		{
			Reload();
			Load ();
		}

		private void Load()
		{
			
		}

		protected override TreeViewItem BuildRoot ()
		{
			// BuildRoot is called every time Reload is called to ensure that TreeViewItems 
			// are created from data. Here we just create a fixed set of items, in a real world example
			// a data model should be passed into the TreeView and the items created from the model.

			// This section illustrates that IDs should be unique and that the root item is required to 
			// have a depth of -1 and the rest of the items increment from that.
			var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
			_moduleList = ParseProto.ParseProtoToModuleList();
			int itemId = 1;
			for (int i = 0; i < _moduleList.Count; i++) {
				cModule model = _moduleList [i];

				TreeViewItem item = new TreeViewItem () {
					id = itemId++,
					depth = 0,
					displayName = "Model:[" + model.moduleId + "]:" + model.moduleEn,
				};
				allItems.Add (item);

				for(int j = 0;j<model.commandList.Count;j++)
				{
					cCommand command = model.commandList [j];
					TreeViewItem commandItem = new TreeViewItem () {
						id = itemId++,
						depth = 1,
						displayName = "Command:[" + model.moduleId + "][" +  command.commandId + "]:" + command.commandEn,
					};
					allItems.Add (commandItem);
				}

			}
			/*
			allItems = new List<TreeViewItem> 
			{
				new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
				new TreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
				new TreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
				new TreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
				new TreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
				new TreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
				new TreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
				new TreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
				new TreeViewItem {id = 9, depth = 2, displayName = "Lizard"},
			};
			*/
			SetupParentsAndChildrenFromDepths (root, allItems);
			
			return root;
		}

		public override void OnGUI (UnityEngine.Rect rect)
		{
			base.OnGUI (rect);
			DoCheckSelect ();
		}
		private void DoCheckSelect(){
			if (HasSelection ()) {
				IList<int> selects = GetSelection ();
				for (int i = 0; i < selects.Count; i++) {
					if (_index != selects [i]) {
						if (_index <= 0)
							_index = selects [i];
						if (OnChangeItem != null) {

							TreeViewItem pre = allItems [_index-1];
							TreeViewItem cur = allItems [selects [i]-1];
							OnChangeItem (pre,cur);
						}
						_index = selects [i];
					}
				}
			}
		}
	
		public cModule GetModelData(TreeViewItem item)
		{
			return _moduleList.Find (a => a.moduleEn.Equals (item.displayName.Split(':')[2]));
		}
	}
}

