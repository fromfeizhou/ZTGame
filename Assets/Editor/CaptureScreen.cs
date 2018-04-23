using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CaptureScreenHelper{
	[MenuItem ("ZTTool/CaptureScreen")]
	public static void CaptureScreen(){
		GameObject cameraGo = GameObject.FindGameObjectWithTag("CaptureScreen");
		if (cameraGo != null && cameraGo.tag.Equals("CaptureScreen")) {
			Camera camera = cameraGo.GetComponent<Camera> ();
			if (camera != null) {
				Texture2D tex2D = GameTool.CaptureScreen (camera);
				string path = Application.dataPath + "/ResourcesLib/Images/PanelPng/Map/map.png";
				File.WriteAllBytes (path, tex2D.EncodeToPNG ());
				AssetDatabase.Refresh ();
			}
		}
	}
}
   