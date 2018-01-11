using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;


[CustomEditor(typeof(MTextBtnFormat))]
public class MTextBtnExtend : MBaseBtnExtend
{
    private string textKey = "";
    private string style = "";
    private MTextBtnFormat mTextButtonFormat;
    private Outline outline;
    private Text text;

    public override void OnEnable()
    {
        base.OnEnable();

        mTextButtonFormat = target as MTextBtnFormat;
        text = mTextButtonFormat.gameObject.GetComponentInChildren<Text>();
        outline = mTextButtonFormat.gameObject.GetComponentInChildren<Outline>();
    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.LabelField("Style");
        style = EditorGUILayout.TextField(mTextButtonFormat.style);
        EditorGUILayout.LabelField("KeyWord");
        textKey = EditorGUILayout.TextField(mTextButtonFormat.textKey);

        if (style != mTextButtonFormat.style)
        {
            mTextButtonFormat.style = style;
            changeTextStyle();
            EditorUtility.SetDirty(mTextButtonFormat);
        }
       
        if (textKey != mTextButtonFormat.textKey)
        {
            mTextButtonFormat.textKey = textKey;
            text.text = LocalString.GetWord(mTextButtonFormat.textKey);
            EditorUtility.SetDirty(text);
            EditorUtility.SetDirty(mTextButtonFormat);
        }


        base.OnInspectorGUI();

    }
   
    //修改文本
    private void changeTextStyle()
    {
        MTextFormatData data = TextFormatDefine.GetFormat(style);
        text.color = data.color;
        text.fontSize = data.fontSize;
        outline.enabled = data.isOutline;
        outline.effectColor = data.effectColor;
        EditorUtility.SetDirty(outline);
        EditorUtility.SetDirty(text);

    }
    
}

