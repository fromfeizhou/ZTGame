using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectObjRightKeyTool : MonoBehaviour
{

    //获取资源路径（Hierarchy中）
    [MenuItem("GameObject/GetSelectObjPath", false, 0)]
    public static void GetSelectObjPath()
    {
        GetSelectObjPathToCopy();
    }
    //获取资源路径（Project中）
    [MenuItem("Assets/GetSelectObjPath", false, 0)]
    public static void GetdSelectObjPath()
    {
        GetSelectObjPathToCopy();
    }

    private static void GetSelectObjPathToCopy()
    {
        string path = "";
        UnityEngine.Object obj = Selection.activeObject;
        if (obj == null)
        {
            Debug.LogError("You must select Obj first!");
            return;
        }
        path = AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(path))
        {
            Transform selectChild = Selection.activeTransform;
            if (selectChild != null)
            {
                path = selectChild.name;
                while (selectChild.parent != null)
                {
                    selectChild = selectChild.parent;
                    path = string.Format("{0}/{1}", selectChild.name, path);
                }
            }
        }
        TextEditor textEditor = new TextEditor();
        textEditor.text = path;
        textEditor.OnFocus();
        textEditor.Copy();
        Debug.LogError(path + ">>>>>>>>>>>>>>>>>>>>");
    }






}
