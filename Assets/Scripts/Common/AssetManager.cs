using System.IO;
using UnityEngine;
using UnityEngine.Events;
using XLua;

[LuaCallCSharp]
public class AssetManager
{
    /* 
     * @brief 加载资源
     * @param path 资源路径
     * @param callback 回调函数
     */
    public static void LoadAsset(string path, UnityAction<Object, string> callback = null, System.Type type = null)
    {
        // Windows 平台分隔符为 '/', OS 平台 路径分隔符为 '\'， 此处是一个大坑
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = path.Replace('\\', '/');
        }

#if UNITY_EDITOR
		ZTSceneManager.GetInstance().StartCoroutine(AnsyLoadAsset(path,callback,type));
#else
        string fileName = System.IO.Path.GetFileName(path);
        string fileNameEx = System.IO.Path.GetFileNameWithoutExtension(path);
        string abName = path.Replace(fileName, "").Replace('/', '_');
        abName = abName.Substring(0, abName.Length - 1).ToLower();
        //AssetBundle bundle = AssetBundleManager.GetInstance().LoadAssetBundleAndDependencies(abName);
        ////加载assetBundleManifest文件    
        //if (null != bundle)
        //{
        //    Object obj2 = bundle.LoadAsset(fileNameEx);
        //    callback(obj2, path);
        //    return;
        //}
        //callback(null, path);

        ZTAssetBundleManager.GetInstance().LoadSyncAssetBundleAndDependencies(abName, fileNameEx, (Object gameObject) =>
        {
            //加载assetBundleManifest文件    
            if (null != gameObject)
            {
                callback(gameObject, path);
                return;
            }
            callback(null, path);
        },type);
#endif
    }

    private static System.Collections.IEnumerator AnsyLoadAsset(string path, UnityAction<Object, string> callback = null, System.Type type = null)
	{
		yield return null;
        //编辑器模式下 资源获取
#if UNITY_EDITOR
        Object obj = null;
		if (null != type)
		{
			obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);
		}
		else
		{
			obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);
		}
		if (null != callback) {
			callback (obj, path);
		} else
			callback (null, path);
#endif
    }

    /* 
     * @brief 加载资源
     * @param path 资源路径
     * @param callback 回调函数
     */
    public static byte[] LoadLuaAsset(string path)
    {
        // Windows 平台分隔符为 '/', OS 平台 路径分隔符为 '\'， 此处是一个大坑
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = path.Replace('\\', '/');
        }

#if UNITY_EDITOR
        Object obj = null;
        //lua ab包地址
        path = PathManager.LuaPath + "/" + path;
        //编辑器模式下 资源获取
        obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);
        TextAsset text = (TextAsset)obj;
        if (null != text)
            return text.bytes;
        return null;

#else
        string filename = DownLoadCommon.GetLuaHotFullName(path);
        if (File.Exists(filename))
        {
            try
            {
                string res = File.ReadAllText(filename);
                return System.Text.Encoding.UTF8.GetBytes(res);
            }catch(System.Exception e)
            {
                Debug.Log("LoadLuaAsset=====3>>" + e.ToString());
            }
        }

        Object obj2 = null;
        string fileNameEx = System.IO.Path.GetFileNameWithoutExtension(path);
        AssetBundle bundle = ZTAssetBundleManager.GetInstance().LoadAssetBundleAndDependencies("luascript");
        //加载assetBundleManifest文件    
        if (null != bundle)
        {   
            obj2 = bundle.LoadAsset(fileNameEx);
        }
        TextAsset text2 = (TextAsset)obj2;
        if (null != text2)
            return text2.bytes;

        return null;
#endif
    }

    public static byte[] LoadPbAsset(string path)
	{
		return LoadLuaAsset(path);
	}

    //加载所有资源
    public static void LoadAllAsset(string path, UnityAction<Object[], string> callback = null)
    {
#if UNITY_EDITOR
        //编辑器模式下 资源获取
        Object[] objs = null;
        objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
        if (null != callback)
        {
            callback(objs, path);
        }
        return;
#endif
    }

    public static void Destroy()
    {
        Debug.Log("AssetManager Destroy");
    }

}
