using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;


namespace UnityEditor.ProtoTreeView
{
	class ProtoTreeViewWindow : EditorWindow
	{
		// We are using SerializeField here to make sure view state is written to the window 
		// layout file. This means that the state survives restarting Unity as long as the window
		// is not closed. If omitting the attribute then the state just survives assembly reloading 
		// (i.e. it still gets serialized/deserialized)
		[SerializeField] TreeViewState m_TreeViewState;

		// The TreeView is not serializable it should be reconstructed from the tree data.
		ProtoTreeView m_TreeView;
		SearchField m_SearchField;

		void OnEnable ()
		{
			// Check if we already had a serialized view state (state 
			// that survived assembly reloading)
			if (m_TreeViewState == null)
				m_TreeViewState = new TreeViewState ();

			m_TreeView = new ProtoTreeView(m_TreeViewState);
			m_TreeView.OnChangeItem = OnItemChange;
			m_SearchField = new SearchField ();
			m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
		}

		void OnGUI ()
		{
			DoToolbar ();
			DoTreeView ();
			GUILayout.Label (tmpOutput);

		}
		string tmpOutput = "";
		void OnItemChange(ProtoTreeViewItem selectItem)
		{
			Debug.Log (selectItem.displayName);
		}

		void DoToolbar()
		{
			GUILayout.BeginHorizontal (EditorStyles.toolbar);
			GUILayout.Space (100);
			GUILayout.FlexibleSpace();
			m_TreeView.searchString = m_SearchField.OnToolbarGUI (m_TreeView.searchString);
			GUILayout.EndHorizontal();
		}

		void DoTreeView()
		{
			Rect rect = GUILayoutUtility.GetRect(0, 512, 0, 512);
			m_TreeView.OnGUI(rect);
		}
	

		// Add menu named "My Window" to the Window menu
		[MenuItem ("TreeView Examples/Simple Tree Window")]
		static void ShowWindow ()
		{
			
			ProtoTreeViewWindow window = EditorWindow.GetWindowWithRect<ProtoTreeViewWindow> (new Rect(0,0,512,512));
			window.titleContent = new GUIContent ("ProtoBrowser");
			window.Show ();
		}

	}    
}
