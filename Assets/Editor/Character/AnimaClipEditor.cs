using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Xml;
using UnityEditor.Animations;

class AnimaClipEditor
{
    //创建Prefab  
    [MenuItem("MyTool/Character/AnimaClipEditor")]

    static void CreatePrefabSelect()
    {
        TextAsset target = Selection.activeObject as TextAsset;
        if (null == target) return;
        Debug.Log(target.name);
       
        //整体路径
        string filePathWithName = AssetDatabase.GetAssetPath(target);
        //带后缀的文件名
        string fileNameWithExtension = Path.GetFileName(filePathWithName);
        //不带后缀的文件名
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
        //不带文件名的路径
        string filePath = filePathWithName.Replace(fileNameWithExtension, "");

        string foldPath = filePath + fileNameWithoutExtension + "_Action/";
        Debug.Log(foldPath);
        if (!Directory.Exists(foldPath))
        {
            Directory.CreateDirectory(foldPath);
        }
        string[] lines = target.text.Split("\n"[0]);
        for (int i = 0; i < lines.Length; i++)
        {
            string strLine = lines[i];
            if (strLine != "" && strLine.Contains("import"))
            {
                string[] keyValue = strLine.Replace("_import", "").Split(' ');
                if (keyValue.Length >= 3)
                {
                    Animation anim = new Animation();
                    anim.name = keyValue[0];
                    //anim.firstFrame = int.Parse(keyValue[1]);
                    //anim.lastFrame = int.Parse(keyValue[2]);
                    //anim.loop = true;
                    anim.wrapMode = WrapMode.Loop;
                    string assetPath = PathManager.CombinePath(foldPath, anim.name + ".anim");
                    Debug.Log(assetPath);
                    AssetDatabase.CreateAsset(anim, assetPath);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreatePrefab(string filePath)
    {
        string path = PathManager.CombinePath(filePath, CreatePrefabs.m_ModeName + ".fbx");
        GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

        string prefabPath = PathManager.CombinePath(filePath, "model.prefab");
        PrefabUtility.CreatePrefab(prefabPath, go);
        // GameObject.DestroyImmediate(go);
    }



    //public class AnimModelSet : AssetPostprocessor
    //{
    //    void OnPreprocessModel()
    //    {

    //        if (assetPath.Contains("FirstPlayers"))
    //        {
    //            ModelImporter textureImporter = assetImporter as ModelImporter;
    //            editorImporterUtil.clipArrayListCreater creater = new editorImporterUtil.clipArrayListCreater();
    //            creater.addClip("idle", 0, 50, true, WrapMode.Loop);
    //            textureImporter.clipAnimations = creater.getArray();
    //        }
    //    }
    //}

    //namespace editorImporterUtil
    //{
    //    public class clipArrayListCreater
    //    {
    //        private List<ModelImporterClipAnimation> clipList = new List<ModelImporterClipAnimation>();
    //        public void addClip(string name, int firstFrame, int lastFrame, bool loop, WrapMode wrapMode)
    //        {
    //            ModelImporterClipAnimation tempClip = new ModelImporterClipAnimation();
    //            tempClip.name = name;
    //            tempClip.firstFrame = firstFrame;
    //            tempClip.lastFrame = lastFrame;
    //            tempClip.loop = loop;
    //            tempClip.wrapMode = wrapMode;
    //            clipList.Add(tempClip);
    //        }

    //        public ModelImporterClipAnimation[] getArray()
    //        {
    //            return clipList.ToArray();
    //        }
    //    }

    //}
}