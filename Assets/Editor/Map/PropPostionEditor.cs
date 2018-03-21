using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(PropPostionCreate))]
public class PropPostionEditor : Editor
{
    private PropPostionCreate propPostionEditor;
    // Use this for initialization
    void Start()
    {

    }

    void OnEnable()
    {
        //获取当前编辑自定义Inspector的对象
        propPostionEditor = (PropPostionCreate)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("");
        EditorGUILayout.Space();
        if (GUILayout.Button("添加道具点"))
        {
            int index = propPostionEditor.transform.childCount;
            GameObject tempObj = new GameObject(index + "");
            tempObj.transform.SetParent(propPostionEditor.transform);
            tempObj.AddComponent<MapPropPostionType>();
            GameoObjectSelectIcon.SetIcon(tempObj, GameoObjectSelectIcon.LabelIcon.Blue);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("生成Text"))
        {
            
            StringBuilder tempStr = new StringBuilder();
            tempStr.AppendLine("[");
            Transform temp = propPostionEditor.transform;
            for (int index = 0; index < temp.childCount; index++)
            {
                Transform trans = temp.GetChild(index);
                MapPropPostionType tempType = trans.GetComponent<MapPropPostionType>();
                tempStr.Append("{");
                string data = trans.position.ToString();
                tempStr.Append(trans.position.x + ",");
                tempStr.Append(trans.position.y + ",");
                tempStr.Append(trans.position.z + ",");
                if (tempType.typeId.Count == 0)
                    tempStr.Append("[]},\n");
                else
                    tempStr.Append("[");

                for (int i = 0; i < tempType.typeId.Count; i++)
                {
                    if (i == tempType.typeId.Count - 1)
                        tempStr.Append(tempType.typeId[i] + "]},\n");
                    else
                        tempStr.Append(tempType.typeId[i] + ",");
                }
            }
            tempStr.AppendLine("]");

            if (File.Exists(MapDefine.MapPropPostionDataSavePath))
                File.Delete(MapDefine.MapPropPostionDataSavePath);
            File.WriteAllText(MapDefine.MapPropPostionDataSavePath, tempStr.ToString());

            AssetDatabase.Refresh();
        }
        
    }
}


