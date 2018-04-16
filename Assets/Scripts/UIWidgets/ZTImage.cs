using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*ImageType
 * Simple,
 * Sliced,
 * Tiled,
 * Filled
*/

/*ImageFillMethod
{
	Horizontal,
	Vertical,
	Radial90,
	Radial180,
	Radial360
}
*/ 

/*ImageOrigin
[Horizontal] 	Left:0 			Right:1
[Vertical] 		Bottom:0 		Top:1
[Radial 90] 	BottomLeft:0, 	TopLeft:1, 	TopRight:2, BottomRight:3
[Radial 180] 	Bottom:0, 		Left:1, 	Top:2, 		Right:3
[Radial 360] 	Bottom:0, 		Right:1, 	Top:2, 		Left:3
*/

/*DefaultAmount*/
/*Colckwish*/

public class ZTImageBase : Image , IWidget, IPointerClickHandler
{
	#if UNITY_EDITOR 
	public void InitEditor (string paramStr)
	{
		string[] param = paramStr.Trim().Split ('_');
		string assetName = param [param.Length - 1];
		string assetPath = "Assets/ResourcesLib/Images/PanelPng/" + assetName + ".png";
		sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite> (assetPath);
		base.SetNativeSize ();
	}
	#endif

	private WidgetAnimation _widgetAnim;
	public WidgetAnimation WidgetAnim {
		get {
			return _widgetAnim;
		}
	}

	public void Init (string paramStr)
	{
		
	}

	public UnityEngine.Events.UnityAction<PointerEventData> onClickImg;
	public virtual void OnPointerClick (PointerEventData eventData){
		if (onClickImg != null)
			onClickImg (eventData);
	}
}

public class ZTImage : ZTImageBase
{
	
}
