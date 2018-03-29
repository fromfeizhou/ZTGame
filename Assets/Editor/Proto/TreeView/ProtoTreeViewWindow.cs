using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;


namespace UnityEditor.ProtoTreeView
{
	class ProtoTreeViewWindow : EditorWindow
	{
		
		[SerializeField] TreeViewState m_TreeViewState;

		ProtoTreeView m_TreeView;

		public int toolbarInt = 0;
		void OnEnable ()
		{
			if (m_TreeViewState == null)
				m_TreeViewState = new TreeViewState ();

			m_TreeView = new ProtoTreeView(m_TreeViewState);
			m_TreeView.OnChangeItem = OnItemChange;
		}
		string tmpOutput = "";
		Vector2 scrolViewPos;
		void OnGUI ()
		{
			GUILayout.BeginArea (new Rect(0,0,treeWidth,base.position.height));
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("刷新")) {
				m_TreeView.Reload ();
			}

			if (GUILayout.Button ("解析")) {
				m_TreeView.Export ();
			}
			GUILayout.EndHorizontal ();
			Rect rect = GUILayoutUtility.GetRect (treeWidth,base.position.height);
			m_TreeView.OnGUI(rect);
			GUILayout.EndArea ();

			GUILayout.BeginArea (new Rect(lineWidth + treeWidth,0,base.position.width-treeWidth,base.position.height));
			scrolViewPos = GUILayout.BeginScrollView (scrolViewPos);
			GUILayout.Label (tmpOutput);
			GUILayout.EndScrollView ();
			GUILayout.EndArea ();

			EditorGUI.DrawRect (new Rect(treeWidth,0,lineWidth,base.position.height),Color.gray);
		}



		float lineWidth = 3;
		void OnItemChange(ProtoTreeViewItem selectItem)
		{
			tmpOutput = selectItem.ToString () + "\n";
			if (selectItem.Type == ProtoTreeViewItem.eType.message) {
				
				List<sMessage> commonMsgList = new List<sMessage> ();
				m_TreeView.CollectMsgDependent (selectItem._message,commonMsgList);
				if (commonMsgList.Count > 0) {
					tmpOutput += "====================================================\n";
					tmpOutput += "相关依赖Msg\n";
					for (int i = 0; i < commonMsgList.Count; i++) {
						tmpOutput += commonMsgList[i].ToString() + "\n\n";
					}
				}
			}
		}

		private const float winHegith = 800;
		private const float winWidth = 800;
		private const float treeWidth = 260;
		private const float treeHeight = winHegith;

		private Vector2 infoScrollViewPos;
		private string infoStr;
	
		[MenuItem ("Tools/ProtoBrowser")]
		static void ShowWindow ()
		{
			ProtoTreeViewWindow window = EditorWindow.GetWindow<ProtoTreeViewWindow> (true,"ProtoBrowser",true);
			window.Show ();
		}

	}    
}
