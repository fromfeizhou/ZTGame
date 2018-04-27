using System.Reflection;
using ReflectionUnityEditr;
using UnityEngine;

namespace ReflectionUnityEditr{
	public class ZoomableArea : RefUnityEditr{
		public static ZoomableArea Create(bool minimalGUI, bool enableSliderZoom){
			object[] constructorParams = new []{
				(object)minimalGUI,
				(object)enableSliderZoom
			};
			return new ZoomableArea (constructorParams);
		}
		private ZoomableArea(object[] constructorParams):base("ZoomableArea",constructorParams){
		}

		public enum YDirection
		{
			Positive,
			Negative
		}

		public bool uniformScale{
			get{
				bool tmpValue = false;
				if (GetPropertyValue ("uniformScale", ref tmpValue)) {
					return tmpValue;
				}
				return tmpValue;
			}
			set{ 
				SetPropertyValue ("uniformScale", value);
			}
		}



		private YDirection m_UpDirection = YDirection.Positive;
		public YDirection upDirection
		{
			get
			{
				int tmpValue = 0;
				if (GetPropertyValue<int> ("upDirection", ref tmpValue, true)) {
					m_UpDirection = (YDirection)tmpValue;
				}
				return m_UpDirection;
			}
			set
			{
				SetPropertyValue ("upDirection", (int)value, true);
			}
		}
	
		public Vector2 scale{
			get{ 
				Vector2 tmpValue = default(Vector2);
				if (GetPropertyValue ("scale", ref tmpValue)) {
					return tmpValue;
				}
				return tmpValue;
			}
		}

		public Rect shownAreaInsideMargins{
			get{ 
				Rect tmpValue = default(Rect);
				if (GetPropertyValue ("shownAreaInsideMargins", ref tmpValue)) {
					return tmpValue;
				}
				return tmpValue;
			}set{ 
				SetPropertyValue ("shownAreaInsideMargins", value);
			}
		}
	
		public void Test(){
			MethodInfo methodInfo = null;
			if (GetMethod ("DrawingToViewTransformPoint",ref methodInfo,new []{typeof(Vector2)})) {
				Debug.Log (methodInfo.ReturnType);
				foreach (ParameterInfo pi in methodInfo.GetParameters()) {
					Debug.Log ("[DrawingToViewTransformPoint]" + pi.ParameterType);
				}
			}
		}

		public void Test1(){
			MethodInfo methodInfo = null;
			if (GetMethods ("IsPanEvent",ref methodInfo,BindingFlags.NonPublic)) {
				Debug.Log (methodInfo.ReturnType);
				foreach (ParameterInfo pi in methodInfo.GetParameters()) {
					Debug.Log ("[DrawingToViewTransformPoint]" + pi.ParameterType);
				}
			}
		}

		public void Test2()
		{
			BindingFlags binding = BindingFlags.Static | BindingFlags.NonPublic;

			Debug.Log (binding == BindingFlags.Static);
			Debug.Log (binding == BindingFlags.Public);
			Debug.Log (binding == BindingFlags.NonPublic);
		}

	}
}