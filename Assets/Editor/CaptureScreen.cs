using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CaptureScreenHelper{
	[MenuItem ("Tools/CaptureScreen")]
	public static void CaptureScreen(){
		if (Selection.objects [0].GetType () == typeof(GameObject)) {
			GameObject cameraGo = Selection.objects [0] as GameObject;
			if (cameraGo != null && cameraGo.tag.Equals("CaptureScreen")) {
				Camera camera = cameraGo.GetComponent<Camera> ();
				if (camera != null) {
					Texture2D tex2D = GameTool.CaptureScreen (camera);
					File.WriteAllBytes ("CaptureScreen.png", tex2D.EncodeToPNG ());
				}
			}
		}
	}
}
   