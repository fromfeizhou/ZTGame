using System.Collections.Generic;  
using System.IO;  
using UnityEditor;  
using UnityEngine;  
using System.Xml;
using UnityEditor.Animations;

class CreatePrefabs
{
    public static string m_ModeName = "Male";
    public static Dictionary<string, string> m_materialList;
    //创建Prefab  
    [MenuItem("MyTool/Character/Create Prefabs")]

    static void CreatePrefabSelect()
    {
        if (null == m_materialList)
        {
            m_materialList = new Dictionary<string, string>();
            m_materialList.Add("eyes", "male_eyes_blue.mat");
            m_materialList.Add("face-1", "male_face-1.mat");
            m_materialList.Add("face-2", "male_face-2.mat");
            m_materialList.Add("hair-1", "male_hair-1_blond.mat");
            m_materialList.Add("hair-2", "male_hair-2_blond.mat");
            m_materialList.Add("pants-1", "male_pants-1_blue.mat");
            m_materialList.Add("pants-2", "male_pants-2_blue.mat");
            m_materialList.Add("shoes-1", "male_shoes-1_black.mat");
            m_materialList.Add("shoes-2", "male_shoes-2_brown.mat");
            m_materialList.Add("top-1", "male_top-1_blue.mat");
            m_materialList.Add("top-2", "male_top-2_gray.mat");
        }
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            if (!(o is GameObject)) continue;
            if (o.name.Contains("@")) continue;
            if (!AssetDatabase.GetAssetPath(o).Contains("/Characters/")) continue;

            //整体路径
            string filePathWithName = AssetDatabase.GetAssetPath(o);
            //带后缀的文件名
            string fileNameWithExtension = Path.GetFileName(filePathWithName);
            //不带文件名的路径
            string filePath = filePathWithName.Replace(fileNameWithExtension, "");

            CreatePrefab(filePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreatePrefab(string filePath)
    {
        string path = PathManager.CombinePath(filePath, CreatePrefabs.m_ModeName + ".fbx");
        GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        //关联材质
        for (int i = 0; i < go.transform.childCount; i++)
        {
            Transform tf = go.transform.GetChild(i);
            SkinnedMeshRenderer sr = tf.GetComponent<SkinnedMeshRenderer>();
            if (null != sr)
            {
                string partName = tf.name;
                string matPath = PathManager.CombinePath(filePath + "Materials/", m_materialList[partName]);
                sr.material = AssetDatabase.LoadAssetAtPath(matPath, typeof(UnityEngine.Material)) as UnityEngine.Material;
            }
        }
        //创建状态机
        AnimatorController ac = AutoCreateControl.CreateControl(filePath);
        go.GetComponent<Animator>().runtimeAnimatorController = ac;
        //GlobalDefine.SetLayer(go, GlobalDefine.Layer_character);

        string foldPath = filePath + "Prefabs/";
        if (!Directory.Exists(foldPath))
        {
            Directory.CreateDirectory(foldPath);
        }
        string prefabPath = PathManager.CombinePath(foldPath ,"model.prefab");
        PrefabUtility.CreatePrefab(prefabPath, go);
        // GameObject.DestroyImmediate(go);
    }
}