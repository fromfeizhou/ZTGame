using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleBuilder : MonoBehaviour
{
    //资源存放路径
    static string assetsDir = Application.dataPath + "/ResourcesLib";
    static string modelsDir = Application.dataPath + "/Models";
    static string prefabsDir = Application.dataPath + "/Prefabs";
    static string mapDir = Application.dataPath + "/Map";
    static string luaScript = Application.dataPath + "/LuaScript";
    static string particlesDir = Application.dataPath + "/Particles";
    //打包后存放路径
    const string assetBundlesPath = "../../";

    static BuildTarget target = BuildTarget.Android;
    [MenuItem("CYH_Tools/AB_Packager/ClearAbName")]
    public static void ClearAssetBundleName()
    {
        //清除所有的AssetBundleName
        ClearAllAssetBundleName();
        Debug.Log("BuildAssetBundleName Finish");
    }
    [MenuItem("CYH_Tools/AB_Packager/BuildAbName")]
    public static void BuildAssetBundleName()
    {
        //清除所有的AssetBundleName
        ClearAllAssetBundleName();
        //设置指定路径下所有需要打包的assetbundlename
        SetAssetBundlesName(assetsDir);
        SetAssetBundlesName(modelsDir);
        SetAssetBundlesName(prefabsDir);
        SetAssetBundlesName(mapDir);
        SetAssetBundlesName(particlesDir);
        SetAssetBundlesName(luaScript,"luaScript");
        EditorUtility.ClearProgressBar();

        Debug.Log("BuildAssetBundleName Finish");
    }

    [MenuItem("CYH_Tools/AB_Packager/Build_2_IPhone")]
    public static void BuildiPhoneResource()
    {
        target = BuildTarget.iOS;
        if (EditorUserBuildSettings.activeBuildTarget != target)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, target);
        }
        BuildAssetResource(assetBundlesPath + "IPhone/AssetBundle");
    }

    [MenuItem("CYH_Tools/AB_Packager/Build_2_Android")]
    public static void BuildAndroidResource()
    {
        target = BuildTarget.Android;
        if (EditorUserBuildSettings.activeBuildTarget != target)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, target);
        }
        BuildAssetResource(assetBundlesPath + "Android/AssetBundle");
    }

    [MenuItem("CYH_Tools/AB_Packager/Build_2_Windows")]
    public static void BuildWindowsResource()
    {
        target = BuildTarget.StandaloneWindows;
        if (EditorUserBuildSettings.activeBuildTarget != target)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, target);
        }
        BuildAssetResource(assetBundlesPath + "Wins/AssetBundle");
    }

    [MenuItem("CYH_Tools/AB_Packager/Build_ALL_Android")]
    public static void BuildAllAndroidResource()
    {
        CSObjectWrapEditor.Generator.ClearAll();
        CSObjectWrapEditor.Generator.GenAll();
        target = BuildTarget.Android;
        BuildAssetBundleName();
        string path = Application.dataPath + "/StreamingAssets/AssetBundle";
        if (Directory.Exists(path))
        {
            File.Delete(path);
        }
        BuildAssetResource(path);

        string pathout = "E:/ZTGameAndroid.apk";
        if (Directory.Exists(pathout))
        {
            File.Delete(pathout);
        }
        BuildPipeline.BuildPlayer(GetBuildScenes(), pathout, BuildTarget.Android, BuildOptions.None);
    }

    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }


    static void BuildAssetResource(string assetPath)
    {
        //文件不存在就创建  
        if (!Directory.Exists(assetPath))
        {
            Directory.CreateDirectory(assetPath);
        }

        BuildPipeline.BuildAssetBundles(assetPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log(assetPath + "  BuildAssetResource Finish!");
    }



    /// <summary>
    /// 清除所有的AssetBundleName，由于打包方法会将所有设置过AssetBundleName的资源打包，所以自动打包前需要清理
    /// </summary>
    static void ClearAllAssetBundleName()
    {
        //获取所有的AssetBundle名称
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();

        //强制删除所有AssetBundle名称
        for (int i = 0; i < abNames.Length; i++)
        {
            EditorUtility.DisplayProgressBar("清除ab名", abNames[i], i*1.0f/abNames.Length);
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }
        EditorUtility.ClearProgressBar();
    }

    /** 需要过滤的文件 */
    private static List<string> _filteredAssets = new List<string> { ".meta", ".xlsx", ".DS_Store" };

    /// <summary>
    /// 设置所有在指定路径下的AssetBundleName
    /// </summary>
    static void SetAssetBundlesName(string _assetsPath,string inAbName = "")
    {
        //先获取指定路径下的所有Asset，包括子文件夹下的资源
        DirectoryInfo dir = new DirectoryInfo(_assetsPath);
        FileSystemInfo[] files = dir.GetFileSystemInfos(); //GetFileSystemInfos方法可以获取到指定目录下的所有文件以及子文件夹

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] is DirectoryInfo)  //如果是文件夹则递归处理(过滤png)
            {
                if (!files[i].Name.Contains("SpritePng"))
                {
                    SetAssetBundlesName(files[i].FullName, inAbName);
                }
            }
            else if (!_filteredAssets.Exists(a => files[i].Name.EndsWith(a))) //如果是文件的话，则设置AssetBundleName，并排除掉.meta文件
            {
                string abName = inAbName;
                if (abName == "")
                {
                    abName = dir.FullName.Remove(0, dir.FullName.IndexOf("Assets")).Replace('\\', '_').Replace('/', '_');
                }
                EditorUtility.DisplayProgressBar("设置名字", abName,i*1.0f/ files.Length);
                SetABName(files[i].FullName, abName);     //逐个设置AssetBundleName
            }
        }
    }

    /// <summary>
    /// 设置单个AssetBundle的Name
    /// </summary>
    ///<param name="filePath">
    static void SetABName(string assetPath, string abName)
    {
        string importerPath = "Assets" + assetPath.Substring(Application.dataPath.Length);  //这个路径必须是以Assets开始的路径
        AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);  //得到Asset
        if (assetImporter == null)
        {
            Debug.LogErrorFormat("[{0}]found out Asset. assetPath:{1}, abName:{2}", "AssetBundleBuilder", assetPath, abName);
            return;
        }
        assetImporter.assetBundleName = abName;    //最终设置assetBundleName
    }
}