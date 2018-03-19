using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

/************************************************************************/
/*               fbx自动导入的时候，设置    importMaterials = false                                                    */
/************************************************************************/
class DisableMaterialImport : AssetPostprocessor
{


    void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;

        //if (assetPath.Contains(CreatePrefabs.CharacterRoot))
        //{
        //    if (assetPath.Contains("@"))//动作文件不导入材质
        //        modelImporter.importMaterials = false;
        //    else
        //        modelImporter.importMaterials = true;

        //}
        //else
      //  modelImporter.importMaterials = false;
    }

    void OnPostprocessModel(GameObject go)
    {
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