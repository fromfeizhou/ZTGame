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
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("根据文件生成物资点"))
        {
            CreatePropPostionByText();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("生成Text并标记"))
        {
            //时间关系先用两个表处理
            CreateNetData();
            CreateClienData();

            AssetDatabase.Refresh();
        }


    }

    private void CreatePropPostionByText()
    {
        if (File.Exists(MapDefine.ClienMapPropPostionDataSavePath))
        {
            string[] datas = File.ReadAllLines(MapDefine.ClienMapPropPostionDataSavePath);

            if (datas.Length > 0)
            {
                int count = propPostionEditor.transform.childCount;
                for (int index = count-1; index >= 0; index--)
                {
                    GameObject.DestroyImmediate(propPostionEditor.transform.GetChild(index).gameObject);
                }
            }

            for (int index = 0; index < datas.Length; index++)
            {
                string[] temp = datas[index].Split('#');
                if (temp.Length == 2)
                {
                    string[] posData = temp[0].Split(',');
                    bool isNew = string.IsNullOrEmpty(temp[1]);
                    string name = isNew ? "New" : temp[1];

                    Vector3 pos = new Vector3(float.Parse(posData[0])*0.01f, float.Parse(posData[1]) * 0.01f,
                        float.Parse(posData[2]) * 0.01f);
                    GameObject tempObj = new GameObject(name);
                    tempObj.transform.SetParent(propPostionEditor.transform);
                    tempObj.transform.position = pos;
                    MapPropPostionType mapPropPostionType = tempObj.AddComponent<MapPropPostionType>();
                    if (!isNew)
                    {
                        string[] ids = temp[1].Split('_');
                        for (int i = 0; i < ids.Length; i++)
                        {
                            mapPropPostionType.typeId.Add(int.Parse(ids[i]));
                        }
                        GameoObjectSelectIcon.SetIcon(tempObj, GameoObjectSelectIcon.LabelIcon.Blue);
                    }
                    else
                    {
                    GameoObjectSelectIcon.SetIcon(tempObj, GameoObjectSelectIcon.LabelIcon.Red);

                    }
                } 
            }
        }
    }

    private void CreateClienData()
    {
        StringBuilder tempStr = new StringBuilder();
        Transform temp = propPostionEditor.transform;
        for (int index = 0; index < temp.childCount; index++)
        {
            StringBuilder tempName = new StringBuilder();
            Transform trans = temp.GetChild(index);
            MapPropPostionType tempType = trans.GetComponent<MapPropPostionType>();
            string data = trans.position.ToString();
            tempStr.Append(Mathf.FloorToInt(trans.position.x * 100f) + ",");
            tempStr.Append(Mathf.FloorToInt(trans.position.y * 100f) + ",");
            tempStr.Append(Mathf.FloorToInt(trans.position.z * 100f) + ",");
            tempStr.Append("#");
            if (tempType.typeId.Count <= 0)
                tempStr.Append("\n");
            for (int i = 0; i < tempType.typeId.Count; i++)
            {
                if (i == tempType.typeId.Count - 1)
                    tempStr.Append(tempType.typeId[i]+ "\n");
                else
                    tempStr.Append(tempType.typeId[i] + "_");
                tempName.Append(tempType.typeId[i] + "_");
            }
            if (tempType.typeId.Count > 0)
            {
                trans.name = tempName.ToString();
                GameoObjectSelectIcon.SetIcon(trans.gameObject, GameoObjectSelectIcon.LabelIcon.Blue);
            }
            else
                GameoObjectSelectIcon.SetIcon(trans.gameObject, GameoObjectSelectIcon.LabelIcon.Red);
        }

        if (File.Exists(MapDefine.ClienMapPropPostionDataSavePath))
            File.Delete(MapDefine.ClienMapPropPostionDataSavePath);
        File.WriteAllText(MapDefine.ClienMapPropPostionDataSavePath, tempStr.ToString());
    }

    //后端解析
    private void CreateNetData()
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
            tempStr.Append(Mathf.FloorToInt(trans.position.x*100f) + ",");
            tempStr.Append(Mathf.FloorToInt(trans.position.y * 100f) + ",");
            tempStr.Append(Mathf.FloorToInt(trans.position.z * 100f) + ",");
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
    }

}


