using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Xml;
using UnityEditor.Animations;

class CreatePrefabs
{

    public static string CharacterRoot = "Assets/Models/CharacterRoot/";
    private const string defaultAnimationName = "@stand.FBX";
    public static string CharacterMaterialRoot = "Assets/Models/CharacterRoot/{0}/material/";
    public static string CharacterModelRoot = "Assets/Models/CharacterRoot/{0}/model/";
    public static string CharacterAnimationRoot = "Assets/Models/CharacterRoot/{0}/animation/";
    public static string CharacterPrefabRoot = "Assets/Models/CharacterRoot/{0}/prefab/";
    public static string CharacterTextureRoot = "Assets/Models/CharacterRoot/{0}/texture/";

    public static string CharacterAnimatorPath = "Assets/Models/Animator/";
    public static string ModelTexturePath = "Assets/Models/CharacterRoot/{0}/{1}T";
    public static string ModelMaterialPath = "Assets/Models/CharacterRoot/{0}/{1}M.mat";
    public static string ModelFBXPath = "Assets/Models/CharacterRoot/{0}/{1}.FBX";
    public static string ModelPrefabPath = "Assets/Models/CharacterRoot/{0}/{1}F.prefab";

    public static string ModelAnimationAssetPath = "Assets/Models/Animator/{0}/model/";

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
        string prefabPath = PathManager.CombinePath(foldPath, "model.prefab");
        PrefabUtility.CreatePrefab(prefabPath, go);
        // GameObject.DestroyImmediate(go);
    }
   
    //public  static string CharacterAssetRootPath="Asset/Models/CharacterRoot/"

    [MenuItem("ZTTool/Character/CreateAllPrefabs")]
    private static void CreateAllCharacterPrefab()
    {
        string[] dirs = Directory.GetDirectories(CharacterRoot, "*", SearchOption.TopDirectoryOnly);

        foreach (string dir in dirs)
        {

            int lastLineIndex = dir.LastIndexOf("/");
            if (lastLineIndex == -1)
                continue;
            string foldName = dir.Substring(lastLineIndex + 1);

            CreateRolePrefab_1(foldName);
            Debug.LogError("foldName: Create Finish+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    [MenuItem("ZTTool/Character/CreateSelectPrefabs")]
    private static void CreateSelectCharacterPrefab()
    {
        Object[] file = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel);
        string path = AssetDatabase.GetAssetPath(file[0]);
        if (File.GetAttributes(path).CompareTo(FileAttributes.Directory) == 0)//操作文件夹
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Parent.Name.Equals("CharacterRoot"))//检测
            {
                //CreateRolePrefab(file[0].name);
                CreateRolePrefab_1(file[0].name);
            }
        }
    }

    //換裝邏輯：
    //分两个部分，武器跟衣服两部分，每个部分整个换，模型资源输出为整体
    private static void CreateRolePrefab(string roleName)
    {
        AnimatorController ac = CreateControl(roleName);

        string path = string.Format(CharacterModelRoot, roleName);
        string[] models = Directory.GetFiles(path, "*.FBX", SearchOption.AllDirectories);
        string materialRoot = string.Format(CharacterMaterialRoot, roleName);
        string prefabRoot = string.Format(CharacterPrefabRoot, roleName);

        for (int index = 0; index < models.Length; index++)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath(models[index], typeof(GameObject)) as GameObject;
            if (go.name.Contains("Equip")) //武器
            {
                SetMaterial(go.transform, materialRoot);
                string prefabPath = prefabRoot + go.name + ".prefab";
                PrefabUtility.CreatePrefab(prefabPath, go);
            }
            else
            {
                //绑定材质
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform tf = go.transform.GetChild(i);
                    string partName = tf.name;
                    if (!partName.Contains("Bip"))
                        SetMaterial(tf, materialRoot);
                }
                //带@识别为动画
                if (ac != null && !models[index].Contains("@"))
                {
                    if (!Directory.Exists(prefabRoot))
                    {
                        Directory.CreateDirectory(prefabRoot);
                    }
                    string prefabPath = prefabRoot + go.name + ".prefab";
                    go.GetComponent<Animator>().runtimeAnimatorController = ac;
                    PrefabUtility.CreatePrefab(prefabPath, go);
                }
                else
                {
                    //动画文件设置loop  模型资源有问题 想注释
                    // SetClipLoop(models[index]);
                }
            }
        }
    }

    private static void SetMaterial(Transform go, string path)
    {
        path += go.name + ".mat";
        SkinnedMeshRenderer sr = go.GetComponent<SkinnedMeshRenderer>();
        if (sr != null)
            sr.material =
                AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Material)) as
                    UnityEngine.Material;
    }

    #region Animatro 动作相关
    //创建控制器
    private static AnimatorController CreateControl(string roleName)
    {
        string pathRoot = string.Format(CharacterAnimationRoot, roleName);
        if (!Directory.Exists(pathRoot))
            Directory.CreateDirectory(pathRoot);

        string ControllerPath = pathRoot + roleName + ".Controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine machine = layer.stateMachine;
        string defaultFbxPath = string.Format(CharacterModelRoot, roleName) + roleName + defaultAnimationName;
        AnimatorState defaultState = null;
        AnimatorState tempState = null;
        if (File.Exists(defaultFbxPath))//规定stand动画为默认
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath(defaultFbxPath, typeof(AnimationClip)) as AnimationClip;
            if (clip != null)
            {
                defaultState = machine.AddState(clip.name, new Vector3(300f, -300f, 0));
                defaultState.motion = clip;
                machine.defaultState = defaultState;
            }
        }
        if (defaultState == null)
        {
            Debug.LogError("模型资源没有待机动画 不规范 ！！！");
            return controller;
        }
        int tempIndex = 0;
        string fbxRoot = string.Format(CharacterModelRoot, roleName);
        string[] models = Directory.GetFiles(fbxRoot, "*.FBX", SearchOption.AllDirectories);
        for (int index = 0; index < models.Length; index++)
        {
            if (models[index].Contains(defaultAnimationName)) continue;
            if (models[index].Contains("@")) //动作文件
            {

                Object[] objects = AssetDatabase.LoadAllAssetsAtPath(models[index]);
                for (int m = 0; m < objects.Length; m++)
                {

                    if (objects[m] is AnimationClip)
                    {
                        AnimationClip clip = (AnimationClip)objects[m];
                        if (clip.name.StartsWith("__")) continue;//资源不规范会带出这类前缀的无效clip，过滤
                        tempState = machine.AddState(clip.name, new Vector3(300f + 30 * tempIndex, -250f + tempIndex * 50f, 0));
                        tempState.motion = clip;
                        var temp = defaultState.AddTransition(tempState);
                        temp.hasExitTime = false;
                        tempIndex++;
                    }
                }

            }
        }

        return controller;
    }

    #endregion


    #region 新版创建逻辑

    private static void CreateRolePrefab_1(string roleName)
    {


        AnimatorController ac = CreateModelController(roleName);
        string prefabPath = string.Format(ModelFBXPath, roleName, roleName);
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        foreach (Renderer smr in go.GetComponentsInChildren<Renderer>())
        {
            SetModelMaterial(smr.transform, go.name);
        }
        if (ac != null)
        {
            go.GetComponent<Animator>().runtimeAnimatorController = ac;
        }
        PrefabUtility.CreatePrefab(string.Format(ModelPrefabPath, roleName, roleName), go);
    }

    private static string SetModelTexturePath(string name)
    {
        string tempPath = name + ".png";
        if (File.Exists(tempPath))
            return tempPath;
        tempPath = name + ".tga";
        if (File.Exists(tempPath))
            return tempPath;
        tempPath = name + "jpg";
        if (File.Exists(tempPath))
            return tempPath;
        return "";
    }

    private static void SetModelMaterial(Transform go, string modelName)
    {
        string materialPath = string.Format(ModelMaterialPath, modelName, go.name);
        Renderer sr = go.GetComponent<Renderer>();

        string textruePath = SetModelTexturePath(string.Format(ModelTexturePath, modelName, go.name));
        Texture modelTexture = AssetDatabase.LoadAssetAtPath(textruePath, typeof(Texture)) as Texture;

        if (sr != null)
        {
            Material tempMaterial = AssetDatabase.LoadAssetAtPath(materialPath, typeof(UnityEngine.Material)) as
                UnityEngine.Material;
            if (tempMaterial != null)
            {
                tempMaterial.mainTexture = modelTexture;
                EditorUtility.SetDirty(tempMaterial);
                AssetDatabase.SaveAssets();
                sr.material = tempMaterial;
            }
        }


    }

    //10100
    private static AnimatorController CreateModelController(string roleName)
    {
        string animationFBXRoot = string.Format(ModelAnimationAssetPath, roleName);
        if (Directory.Exists(animationFBXRoot))
        {
            string[] animationFBXs = Directory.GetFiles(animationFBXRoot, "*.FBX", SearchOption.AllDirectories);
            for (int index = 0; index < animationFBXs.Length; index++)
            {
                SetClipLoop(animationFBXs[index]);
            }
        }
        string controllerDirName = GetModelControllerDirName(roleName);
        if (string.IsNullOrEmpty(controllerDirName)) return null;
        string animatorRootPath = CharacterAnimatorPath + controllerDirName;
        string ControllerPath = animatorRootPath + "/" + controllerDirName + ".Controller";

        bool animatorIsExists = File.Exists(ControllerPath);
        if (roleName == controllerDirName || animatorIsExists) //创建
        {

            string animationAssetRootPath = string.Format(ModelAnimationAssetPath, controllerDirName);

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            AnimatorControllerLayer layer = controller.layers[0];
            AnimatorStateMachine machine = layer.stateMachine;
            string defaultFbxPath = animationAssetRootPath + controllerDirName + defaultAnimationName;
            AnimatorState defaultState = null;
            AnimatorState tempState = null;
            if (File.Exists(defaultFbxPath)) //规定stand动画为默认
            {
                AnimationClip clip =
                    AssetDatabase.LoadAssetAtPath(defaultFbxPath, typeof(AnimationClip)) as AnimationClip;
                if (clip != null)
                {
                    defaultState = machine.AddState(clip.name, new Vector3(300f, -300f, 0));
                    defaultState.motion = clip;
                    machine.defaultState = defaultState;
                }
            }
            if (defaultState == null)
            {
                Debug.LogError("模型资源没有待机动画 不规范 ！！！");
                return controller;
            }
            int tempIndex = 0;
            string fbxRoot = string.Format(ModelAnimationAssetPath, controllerDirName);
            string[] models = Directory.GetFiles(fbxRoot, "*.FBX", SearchOption.AllDirectories);
            for (int index = 0; index < models.Length; index++)
            {
                if (models[index].Contains(defaultAnimationName)) continue;
                if (models[index].Contains("@")) //动作文件
                {
                    Object[] objects = AssetDatabase.LoadAllAssetsAtPath(models[index]);
                    for (int m = 0; m < objects.Length; m++)
                    {

                        if (objects[m] is AnimationClip)
                        {
                            AnimationClip clip = (AnimationClip)objects[m];
                            if (clip.name.StartsWith("__")) continue; //资源不规范会带出这类前缀的无效clip，过滤
                            tempState = machine.AddState(clip.name,
                                new Vector3(300f + 30 * tempIndex, -250f + tempIndex * 50f, 0));
                            tempState.motion = clip;
                            var temp = defaultState.AddTransition(tempState);
                            temp.hasExitTime = false;
                            tempIndex++;
                        }
                    }

                }
            }
            return controller;
        }
        else
        {
            return AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        }
    }

    public const int ModelInterval = 10000;//模型类型id区间
    public const int PartInterval = 100;//部件区间
    public static string GetModelControllerDirName(string name)
    {
        int id = -1;
        if (!int.TryParse(name, out id))
        {
            Debug.LogError("ModelName is Error");
            return "";
        }

        int modelType = id / ModelInterval * ModelInterval;
        int partType = id % ModelInterval / PartInterval * PartInterval;

        if (Directory.Exists(CharacterAnimatorPath + (modelType + partType)))
            return (modelType + partType + "");
        return "";
    }

    #endregion



    #region Clip设置

    public static readonly List<string> LoopClipName = new List<string>() { "stand", "move", "stealth", "crawl" };
    private static void SetClipLoop(string fxbPath)
    {
        ModelImporter modelImporter = AssetImporter.GetAtPath(fxbPath) as ModelImporter; //as 类型转换
        if (modelImporter == null)
            return;

        List<ModelImporterClipAnimation> actions = new List<ModelImporterClipAnimation>();
        foreach (ModelImporterClipAnimation a in modelImporter.defaultClipAnimations)//
        {
            if (LoopClipName.Contains(a.name))//|| a.name.Contains("stand") || a.name.Contains("move"))
            {
                a.loopTime = true;
            }
            actions.Add(a);
        }

        modelImporter.clipAnimations = actions.ToArray();
        modelImporter.SaveAndReimport();
    }
    #endregion







    [MenuItem("ZTTool/Character/Test")]
    public static void TestClip()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.LogError(path + ">>>>>>>>");
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);

        for (int m = 0; m < objects.Length; m++)
        {

            if (objects[m] is AnimationClip)
            {
                Debug.LogError(objects[m].name + "_________________");
            }
        }
    }
}