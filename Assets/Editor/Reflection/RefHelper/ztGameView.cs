using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using ReflectionUnityEditr;

public class ztGameView : EditorWindow{
	[MenuItem("Window/General/ztGame %2", false, 1)]
	static void ShowGameView()
	{
		ztGameView windown = EditorWindow.GetWindow<ztGameView>(true);
		windown.titleContent = new GUIContent ("ztGame编辑工具");
	}
	ZoomableArea m_ZoomArea;
	public ztGameView(){
		InitializeZoomArea();
	}

	void InitializeZoomArea()
	{
		if (m_ZoomArea != null)
			m_ZoomArea = null;
		
		m_ZoomArea = ZoomableArea.Create (true, false);

		Debug.Log ("m_ZoomArea.uniformScale:" + m_ZoomArea.uniformScale);
		m_ZoomArea.uniformScale = true;
		Debug.Log (m_ZoomArea.uniformScale);
		m_ZoomArea.uniformScale = false;
		Debug.Log (m_ZoomArea.uniformScale);

		Debug.Log ("m_ZoomArea.upDirection:" + m_ZoomArea.upDirection);
		Debug.Log (m_ZoomArea.upDirection);
		m_ZoomArea.upDirection = ZoomableArea.YDirection.Negative;
		Debug.Log (m_ZoomArea.upDirection);

		Debug.Log ("m_ZoomArea.scale:" + m_ZoomArea.scale);
		Debug.Log (m_ZoomArea.scale);

		m_ZoomArea.Test ();
		Debug.Log ("====================");
		m_ZoomArea.Test1 ();
		Debug.Log ("====================");
		m_ZoomArea.Test2 ();

	}


	private int Out(int aa){
		return 0;
	}

	private int Out(int aa, int bb){
		return 1;
	}
}
   