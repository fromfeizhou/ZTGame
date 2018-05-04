using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.Serialization;
using System;
using XLua;

[LuaCallCSharp]
public class ZTCircle : MaskableGraphic,ISerializationCallbackReceiver, ILayoutElement {
   

	[FormerlySerializedAs("m_Frame")]
	[SerializeField]
	private Sprite m_Sprite;
	public Sprite sprite 
	{ 
		get 
		{ 
			return m_Sprite; 
		} 
		set
		{
			if (!m_Sprite.Equals (value)) {
				m_Sprite = value;
				SetAllDirty();
			}
		}
	}

	[NonSerialized]
	private Sprite m_OverrideSprite;
	public Sprite overrideSprite 
	{
		get 
		{ 
			return m_OverrideSprite == null ? sprite : m_OverrideSprite; 
		} 
		set
		{ 
			if (!m_OverrideSprite.Equals (value)) {
				m_OverrideSprite = value;
				SetAllDirty();
			}
		} 
	}


	public override Texture mainTexture
	{
		get
		{
			return overrideSprite == null ? s_WhiteTexture : overrideSprite.texture;
		}
	}

	public float pixelsPerUnit
	{
		get
		{
			float spritePixelsPerUnit = 100;
			if (sprite)
				spritePixelsPerUnit = sprite.pixelsPerUnit;

			float referencePixelsPerUnit = 100;
			if (canvas)
				referencePixelsPerUnit = canvas.referencePixelsPerUnit;

			return spritePixelsPerUnit / referencePixelsPerUnit;
		}
	}

	public void OnAfterDeserialize()
	{
	}

	public void OnBeforeSerialize()
	{
	}

	public virtual void CalculateLayoutInputHorizontal() { }
	public virtual void CalculateLayoutInputVertical() { }

	public virtual float minWidth { get { return 0; } }

	public virtual float preferredWidth
	{
		get
		{
			if (overrideSprite == null)
				return 0;
			return overrideSprite.rect.size.x / pixelsPerUnit;
		}
	}

	public virtual float flexibleWidth { get { return -1; } }

	public virtual float minHeight { get { return 0; } }

	public virtual float preferredHeight
	{
		get
		{
			if (overrideSprite == null)
				return 0;
			return overrideSprite.rect.size.y / pixelsPerUnit;
		}
	}

	public virtual float flexibleHeight { get { return -1; } }

	public virtual int layoutPriority { get { return 0; } }

    public float thickness = 5;
	[Range(1,20)]
	public int QuarterSegNum = 1;
	private float _radius;
	public float Radius{
		get{ 
			return _radius;
		}
		set{ 
			if (_radius != value) {
				_radius = value;
				rectTransform.sizeDelta = Vector2.one * _radius;
			}
		}
	}
	private int segement{
		get{ 
			return QuarterSegNum * 4;
		}
	}
	private UIVertex[] _verArray;
	private UIVertex[] verArray{
		get{
			if(_verArray == null || segement * 2 != _verArray.Length)
				_verArray = new UIVertex[segement * 2];
			return _verArray;
		}
	}

	float degreeDelta{
		get{ 
			return (float)(2 * Mathf.PI / segement);
		}
	}

	public Vector2 GetPos(){
		return rectTransform.anchoredPosition;
	}

	public void SetPos(Vector2 pos){
		rectTransform.anchoredPosition = pos;
	}

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

		float outerRadius = _radius + thickness * 0.5f;
		float innerRadius = _radius - thickness * 0.5f;

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

        float uvCenterX = (uv.x + uv.z) * 0.5f;
        float uvCenterY = (uv.y + uv.w) * 0.5f;

        float curDegree = 0;
		for (int i = 0; i < QuarterSegNum; i++)
		{
			float cosA = Mathf.Cos(curDegree);
			float sinA = Mathf.Sin(curDegree);

			float inPosX = cosA * innerRadius;
			float inPosY = sinA * innerRadius;
			float outPosX = cosA * outerRadius;
			float outPosY = sinA * outerRadius;

			int inIdx  = i * 2;
			int outIdx = inIdx + 1;

			verArray[inIdx ].position.Set (inPosX,  inPosY,  0);
			verArray[outIdx].position.Set (outPosX, outPosY, 0);

			verArray[inIdx  + QuarterSegNum * 2 ].position.Set (-inPosY,  inPosX,  0);
			verArray[outIdx + QuarterSegNum * 2 ].position.Set (-outPosY, outPosX, 0);

			verArray[inIdx  + QuarterSegNum * 4 ].position.Set (-inPosX,  -inPosY,  0);
			verArray[outIdx + QuarterSegNum * 4 ].position.Set (-outPosX, -outPosY, 0);

			verArray[inIdx  + QuarterSegNum * 6 ].position.Set (inPosY,  -inPosX,  0);
			verArray[outIdx + QuarterSegNum * 6 ].position.Set (outPosY, -outPosX, 0);

			curDegree += degreeDelta;

		}

		for (int i = 0; i < verArray.Length; i++) {
			verArray [i].color = color;
			verArray [i].uv2.Set (verArray [i].position.x + uvCenterX, verArray [i].position.y + uvCenterY);
			vh.AddVert (verArray [i]);
		}

		int triangleCount = segement*3*2 - 6;
		for (int i = 0, vIdx = 0; i < triangleCount; i += 6, vIdx += 2)
		{
			vh.AddTriangle(vIdx+1, vIdx, vIdx+3);
			vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
		}
    	
		//首尾顶点相连
		vh.AddTriangle(segement*2 - 1, segement*2 - 2, 1);
		vh.AddTriangle(segement*2 - 2, 0, 1);
	}
}
