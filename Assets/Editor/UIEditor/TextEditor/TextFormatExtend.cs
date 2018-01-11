using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

[CustomEditor(typeof(MTextFormat))]
public class TextFormatExtend : Editor
{
    private string style = "";
    private string textKey = "";
    private MTextFormat mTextFormat;
    private Outline outline;
    private Text text;

    public void OnEnable()
    {
        mTextFormat = target as MTextFormat;
        text = mTextFormat.gameObject.GetComponentInChildren<Text>();
        outline = mTextFormat.gameObject.GetComponentInChildren<Outline>();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Style");
        style = EditorGUILayout.TextField(mTextFormat.style);
        EditorGUILayout.LabelField("KeyWord");
        textKey = EditorGUILayout.TextField(mTextFormat.textKey);

        if (textKey != mTextFormat.textKey)
        {
            mTextFormat.textKey = textKey;
            text.text = LocalString.GetWord(mTextFormat.textKey);
            EditorUtility.SetDirty(mTextFormat);
            EditorUtility.SetDirty(text);
        }

        if (style != mTextFormat.style)
        {
            mTextFormat.style = style;
            EditorUtility.SetDirty(mTextFormat);
            changeTextStyle();
        }

       
        base.DrawDefaultInspector();
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
