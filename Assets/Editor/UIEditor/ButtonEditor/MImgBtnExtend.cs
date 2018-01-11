using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;


[CustomEditor(typeof(MImgBtnFormat))]
public class MImageBtnExtend : MBaseBtnExtend
{
    private string imgName = "";
    private MImgBtnFormat mImgButtonFormat;
    private RawImage imgLab;

    public override void OnEnable()
    {
        base.OnEnable();

        mImgButtonFormat = target as MImgBtnFormat;
        imgLab = mImgButtonFormat.gameObject.GetComponentInChildren<RawImage>();
    }
    public override void OnInspectorGUI()
    {

        EditorGUILayout.LabelField("ImgLabName");
        imgName = EditorGUILayout.TextField(mImgButtonFormat.imgLabName);

        if (imgName != mImgButtonFormat.imgLabName)
        {
            mImgButtonFormat.imgLabName = imgName;
            changeBtnLabel();
            EditorUtility.SetDirty(mImgButtonFormat);
        }

        base.OnInspectorGUI();
    }

    private void changeBtnLabel()
    {
        imgLab.texture = AssetDatabase.LoadAssetAtPath(mImgButtonFormat.GetBtnLabResPath(), typeof(Texture)) as Texture;
        imgLab.SetNativeSize();
        EditorUtility.SetDirty(imgLab);

    }
}

