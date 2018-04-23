using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;

[LuaCallCSharp]
public class GameTool
{
	public static string ColorHex = string.Empty;
	   
	private static string _Parse(params object[] args)
	{
		string str = "";

		for (int i = 0; i < args.Length; i++)
		{
			if (i == 0)
			{
				if (null != args[i])
					str = args[i].ToString();
				else
					str = "null";
			}
			else
			{
				if (null != args[i])
					str = str + "  " + args[i].ToString();
				else
					str = str + "  " + "null";
			}
		}
		return str;
	}
	public static void LogC(params object[] args)
	{
		string str = string.Format("<color={0}>{1}</Color>",ColorHex,_Parse (args));
		Debug.Log(str);
	}

	public static void Log(params object[] args)
	{
		Debug.Log(_Parse (args));
	}

	public static void LogWarning(params object[] args)
	{
		Debug.LogWarning(_Parse(args));
	}

    public static void LogError(params object[] args)
    {
		Debug.LogError(_Parse(args));
    }

    //是否点击在ui上
    public static bool IsPointerOverUIObject(Vector2 screenPosition)
    {
        //判断是否点击的是UI，有效应对安卓没有反应的情况，true为UI  
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //是否已经释放了
    public static bool IsDestroyed(GameObject gameObject)
    {
        return gameObject == null && !ReferenceEquals(gameObject, null);
    }
    
    //获取两点夹角
    public static float SignedAngleBetween(Vector3 a, Vector3 b)
    {
        Vector3 n = Vector3.down;
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));
        float signed_angle = angle * sign;
        return (signed_angle < 0) ? 360 + signed_angle : signed_angle;
    }

	public static Vector2 GetWorldToScreenPoint(Vector3 scenePos,Canvas canvas)
	{
		RectTransform rectCanvas = canvas.transform as RectTransform;
		Vector3 ScreenPos = Camera.main.WorldToScreenPoint (scenePos);
		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (rectCanvas, ScreenPos, canvas.worldCamera, out pos);
		return pos;
	}
	   
	public static Vector2 GetScreenPosToParentPos(RectTransform parentRect, Vector2 screenPos,Canvas canvas)
	{
		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRect, screenPos, canvas.worldCamera, out pos);
		return pos;
	}

	public static void SetGameObjectLayer(GameObject gameObject, string layerName){
		Debug.Log (gameObject);
		Debug.Log (layerName);
		int layerNum = LayerMask.NameToLayer (layerName);
		Transform[] transGo = gameObject.GetComponentsInChildren<Transform> ();
		for (int i = 0; i < transGo.Length; i++)
			transGo [i].gameObject.layer = layerNum;
	}

	public static Texture2D CaptureScreen(Camera camera)
	{
		
		Rect rect = new Rect (0,0,2048,2048);
		// 创建一个RenderTexture对象  
	    RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);  
	    // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
	    camera.targetTexture = rt;  
	    camera.Render();  
	        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
	        //ps: camera2.targetTexture = rt;  
	        //ps: camera2.Render();  
	        //ps: -------------------------------------------------------------------  
	  
	    // 激活这个rt, 并从中中读取像素。  
	    RenderTexture.active = rt;  
	    Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24,false);  
	    screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
	    screenShot.Apply();  
	  
	    // 重置相关参数，以使用camera继续在屏幕上显示  
	    camera.targetTexture = null;  
	        //ps: camera2.targetTexture = null;  
	    RenderTexture.active = null; // JC: added to avoid errors  
		GameObject.DestroyImmediate(rt);  
	    // 最后将这些纹理数据，成一个png图片文件  
	    byte[] bytes = screenShot.EncodeToPNG();  
	    string filename = Application.dataPath + "/Screenshot.png";  
	    System.IO.File.WriteAllBytes(filename, bytes);  
	    Debug.Log(string.Format("截屏了一张照片: {0}", filename));  

		return screenShot;
	}
}
