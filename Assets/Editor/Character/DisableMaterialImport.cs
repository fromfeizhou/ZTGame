using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

/************************************************************************/
/*               fbx自动导入的时候，设置    importMaterials = false    
 *               此功能由负责导入角色的美术使用 
/************************************************************************/
class DisableMaterialImport : AssetPostprocessor
{
    private static bool IsEnable = false;

   void OnPreprocessModel()
    {

        ModelImporter modelImporter = assetImporter as ModelImporter;

        if (modelImporter != null)
            modelImporter.importMaterials = false;
    }

    void OnPostprocessModel(GameObject go)
    {
        if (!IsEnable) return;
        //此处资源未序列化，不可对资源进行移动，在此创建所需文件夹
        if (assetPath.Contains("/CharacterRoot/") && assetPath.Contains(".FBX"))
        {
            bool isAnimation = false;
            string name = go.name;
            if (go.name.Contains("@")) //动作
            {
                isAnimation = true;
                name = go.name.Split('@')[0];

                foreach (SkinnedMeshRenderer smr in go.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    Object.DestroyImmediate(smr.sharedMesh, true);
                    Object.DestroyImmediate(smr.gameObject);
                }

                foreach (Transform o in go.transform)
                {
                    if (!o.name.Contains("Bip"))
                        Object.DestroyImmediate(o.gameObject);

                }
            }
            int id = -1;
            int.TryParse(name, out id);
            if (id == -1)
            {
                Debug.LogError("模型资源命名有误：" + go.name);
                return;
            }
            if (isAnimation)
            {
                string animatorDir = CreatePrefabs.CharacterAnimatorPath +
                                     (id / 10000 * 10000 + (id % 10000) / 100 * 100);
                if (!Directory.Exists(animatorDir))
                    Directory.CreateDirectory(animatorDir);
                string subDir = animatorDir + "/model/";
                if (!Directory.Exists(subDir))
                    Directory.CreateDirectory(subDir);
                string newPath = subDir + go.name + ".FBX";
                if (File.Exists(newPath))
                    AssetDatabase.DeleteAsset(newPath);
            }
            else
            {
                string path = CreatePrefabs.CharacterRoot + go.name;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string filePath = path + "/" + go.name + ".FBX";
                if (File.Exists(filePath))
                    AssetDatabase.DeleteAsset(filePath);

                foreach (Renderer smr in go.GetComponentsInChildren<Renderer>())
                {
                    Material mater = new Material(Shader.Find("Mobile/Diffuse"));//Legacy Shaders/Transparent/Diffuse"));
                    AssetDatabase.CreateAsset(mater, string.Format(CreatePrefabs.ModelMaterialPath, go.name, smr.name));

                    //AssetDatabase.CreateAsset();
                }

            }
            AssetDatabase.Refresh();
            return;
            if (!assetPath.Contains(CreatePrefabs.CharacterRoot)) return;

            if (assetPath.Contains("@"))
            {
                // For animation FBX's all unnecessary Objects are removed.
                // This is not required but improves clarity when browsing assets.

                // Remove SkinnedMeshRenderers and their meshes.
                foreach (SkinnedMeshRenderer smr in go.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    Object.DestroyImmediate(smr.sharedMesh, true);
                    Object.DestroyImmediate(smr.gameObject);
                }

                foreach (Transform o in go.transform)
                {
                    if (!o.name.Contains("Bip"))
                        Object.DestroyImmediate(o.gameObject);

                }
            }
            else
            {
                FileInfo info = new FileInfo(assetPath);
                if (info.Directory.Name == "CharacterRoot")
                {
                    string fileName = info.Name.Substring(0, info.Name.IndexOf("."));
                    string filePath = string.Format(CreatePrefabs.CharacterPrefabRoot, fileName);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    filePath = string.Format(CreatePrefabs.CharacterAnimationRoot, fileName);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    filePath = string.Format(CreatePrefabs.CharacterModelRoot, fileName);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    filePath = string.Format(CreatePrefabs.CharacterMaterialRoot, fileName);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    filePath = string.Format(CreatePrefabs.CharacterTextureRoot, fileName);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                }
            }
            AssetDatabase.Refresh();

        }
    }

    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if(!IsEnable) return;
        foreach (string str in importedAsset)
        {
            if (str.Contains("/CharacterRoot/") && str.Contains(".FBX"))
            {
                bool isAnimation = false;
                GameObject tempGo = AssetDatabase.LoadAssetAtPath<GameObject>(str);
                string name = tempGo.name;
                if (tempGo.name.Contains("@"))//动作
                {
                    isAnimation = true;
                    name = tempGo.name.Split('@')[0];
                }
                int id = -1;
                int.TryParse(name, out id);
                if (id == -1)
                {
                    Debug.LogError("模型资源命名有误：" + tempGo.name);
                    continue;
                }
                string newPath = "";
                if (isAnimation)
                {
                    string animatorDir = CreatePrefabs.CharacterAnimatorPath +
                                         (id / 10000 * 10000 + (id % 10000) / 100 * 100);
                    string subDir = animatorDir + "/model/";
                    newPath = subDir + tempGo.name + ".FBX";
                }
                else
                    newPath = CreatePrefabs.CharacterRoot + tempGo.name + "/" + tempGo.name + ".FBX";
                string moveData = AssetDatabase.MoveAsset(str, newPath);
                Debug.Log(moveData);
            }
        }
        AssetDatabase.Refresh();
    }

    //public void OnPreprocessModel()
    //{
    //    ModelImporter modelImporter = assetImporter as ModelImporter;
    //    modelImporter.animationType = ModelImporterAnimationType.Legacy;
    //    try
    //    {
    //        string fileAnim;
    //        if (DragAndDrop.paths.Length <= 0)
    //        {
    //            return;
    //        }
    //        fileAnim = DragAndDrop.paths[0];
    //        string ClipText = Path.ChangeExtension(fileAnim, ".txt");
    //        StreamReader file = new StreamReader(ClipText);
    //        string sAnimList = file.ReadToEnd();
    //        file.Close();
    //        //  
    //        if (EditorUtility.DisplayDialog("FBX Animation Import from file",
    //            fileAnim, "Import", "Cancel"))
    //        {
    //            System.Collections.ArrayList List = new ArrayList();
    //            ParseAnimFile(sAnimList, ref List);

    //            //modelImporter.clipAnimations. = true;  
    //            modelImporter.clipAnimations = (ModelImporterClipAnimation[])
    //                List.ToArray(typeof(ModelImporterClipAnimation));

    //            EditorUtility.DisplayDialog("Imported animations",
    //                "Number of imported clips: "
    //                + modelImporter.clipAnimations.GetLength(0).ToString(), "OK");
    //        }
    //    }
    //    catch { }
    //    // (Exception e) { EditorUtility.DisplayDialog("Imported animations", e.Message, "OK"); }  
    //}
    //void ParseAnimFile(string sAnimList, ref System.Collections.ArrayList List)
    //{
    //    string[] lines = sAnimList.Split("\n"[0]);
    //    for (int i = 0; i < lines.Length; i++)
    //    {
    //        string strLine = lines[i];
    //        if (strLine != "" && strLine.Contains("import"))
    //        {
    //            string[] keyValue = strLine.Replace("_import", "").Split(' ');
    //            if (keyValue.Length >= 3)
    //            {
    //                ModelImporterClipAnimation clip = new ModelImporterClipAnimation();
    //                clip.name = keyValue[0];
    //                clip.firstFrame = int.Parse(keyValue[1]);
    //                clip.lastFrame = int.Parse(keyValue[2]);
    //                clip.loop = true;
    //                if (keyValue[0].Contains("idle") || keyValue[0].Contains("run") || keyValue[0].Contains("win"))
    //                {
    //                    clip.wrapMode = WrapMode.Loop;
    //                }
    //                else
    //                {
    //                    clip.wrapMode = WrapMode.Once;
    //                }
    //                List.Add(clip);
    //            }
    //        }
    //    }
    //}

}