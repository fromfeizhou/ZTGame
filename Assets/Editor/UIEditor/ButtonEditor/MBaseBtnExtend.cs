using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;


[CustomEditor(typeof(MBaseBtnFormat))]
public class MBaseBtnExtend : Editor
{
    protected string btnStyle = "";
    protected string btnSize = "";

    protected MBaseBtnFormat btnFormat;
    protected Image ImgBtn;
    protected GameObject gameObject;

    public virtual void OnEnable()
    {
        btnFormat = target as MBaseBtnFormat;
        ImgBtn = btnFormat.gameObject.GetComponent<Image>();
        gameObject = btnFormat.gameObject;
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("BtnStyle");
        btnStyle = EditorGUILayout.TextField(btnFormat.btnType);
        
        EditorGUILayout.LabelField("BtnSize");
        btnSize = EditorGUILayout.TextField(btnFormat.mBtnSize);

        if (btnStyle != btnFormat.btnType)
        {
            btnFormat.btnType = btnStyle;
            changeBtnStyle();
            EditorUtility.SetDirty(btnFormat);
        }

        if (btnSize != btnFormat.mBtnSize)
        {
            btnFormat.mBtnSize = btnSize;
            changeBtnSize();
            EditorUtility.SetDirty(btnFormat);
        }

        base.DrawDefaultInspector();
    }

    private void changeBtnStyle()
    {
        ImgBtn.sprite = AssetDatabase.LoadAssetAtPath(btnFormat.GetBtnComResPath(), typeof(Sprite)) as Sprite;
        EditorUtility.SetDirty(ImgBtn);
    }

    private void changeBtnSize()
    {
        MButtonData data = ButtonFormatDefine.GetFormat(btnSize);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(data.width, data.height);
        EditorUtility.SetDirty(gameObject);
    }
    
}

