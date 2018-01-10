using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SkillAssetCreate
{
     [MenuItem("Assets/Create/Skill/SkillAsset", false, 80)]
    public static void main()
    {
        string path = GetSelectedPathOrFallback();
        SkillAsset skillAsset = AssetDatabase.LoadAssetAtPath(path + "/skillTemp.asset", typeof(SkillAsset)) as SkillAsset;
        bool isNewAsset = skillAsset == null ? true : false;
        if (isNewAsset)
        {
            skillAsset = ScriptableObject.CreateInstance<SkillAsset>();
            skillAsset.ListSkillGroup = new List<SkillAssetInforGroup>();
            AssetDatabase.CreateAsset(skillAsset,path + "/skillTemp.asset");
        }
    }


    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

}
