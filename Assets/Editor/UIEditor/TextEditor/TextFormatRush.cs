using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class TextFormatRush
{
    [MenuItem("MyTool/Rush/TextFormatRush")]
    private static void RecordPointAddFlame()
    {
        string srcPath = "Assets/UILib/Prefabs";
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { srcPath });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            GameObject tempPrefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            //GameObject tempPrefabScene = PrefabUtility.InstantiatePrefab(tempPrefab) as GameObject;
            //检查当前预设
            checkItem(tempPrefab.transform);
            //检查子预设
            foreach (Transform item in tempPrefab.transform)
            {
               checkItem(item);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("TextFormatRush Done");
    }

    //检查相关元素 是否包含要刷新内容
    private static void checkItem(Transform item)
    {
        MTextFormat[] textFormats = item.GetComponentsInChildren<MTextFormat>();
        if (textFormats.Length > 0)
        {
            foreach (MTextFormat format in textFormats)
            {
                rushText(format.gameObject.transform, format.style, format.textKey);
            }
        }
        MTextBtnFormat[] btnFormats = item.GetComponentsInChildren<MTextBtnFormat>();
        if (btnFormats.Length > 0)
        {
            foreach (MTextBtnFormat format in btnFormats)
            {
                rushText(format.gameObject.transform, format.style, format.textKey);
            }
        }
    }

    //刷新文本显示 并保存
    private static void rushText(Transform item, string style, string textKey)
    {
        UnityEngine.UI.Text txt = item.GetComponentInChildren<UnityEngine.UI.Text>();
        Outline outline = item.GetComponentInChildren<Outline>();
        txt.text = LocalString.GetWord(textKey);
        MTextFormatData data = TextFormatDefine.GetFormat(style);
        txt.color = data.color;
        txt.fontSize = data.fontSize;
        outline.enabled = data.isOutline;
        outline.effectColor = data.effectColor;
        EditorUtility.SetDirty(outline);
        EditorUtility.SetDirty(txt);
    }

}
