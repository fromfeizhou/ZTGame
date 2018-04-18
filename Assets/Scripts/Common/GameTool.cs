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


	public static void SetGameObjectLayer(GameObject gameObject, string layerName){
		Debug.Log (gameObject);
		Debug.Log (layerName);
		int layerNum = LayerMask.NameToLayer (layerName);
		Transform[] transGo = gameObject.GetComponentsInChildren<Transform> ();
		for (int i = 0; i < transGo.Length; i++)
			transGo [i].gameObject.layer = layerNum;
	}
}
