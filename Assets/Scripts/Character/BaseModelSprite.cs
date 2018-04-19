using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


public enum EquipType
{
    Node=0,
    Main = 1,
    Equip
}


public class EquipData
{
    public EquipType type;
    public string path;

}


public enum ZTAnimationType
{
    Animator = 1,
    Animation = 2
}

public class AnimationBase
{
    public virtual void Play(string name) { }

    public virtual void Play(string name, int layout, int index)
    {
        Debug.LogError("非Animator不可使用该接口");
    }

    public static AnimationBase GetAnimationBase(int type, GameObject go)
    {
        ZTAnimationType animationType = ZTAnimationType.Animator;
        AnimationBase baseAction = null;
        if (System.Enum.IsDefined(typeof(ZTAnimationType), type))
            animationType = (ZTAnimationType)type;

        switch (animationType)
        {
            case ZTAnimationType.Animation:
                baseAction = new AnimationAction(go);
                break;
            case ZTAnimationType.Animator:
                baseAction = new AnimatorAction(go);
                break;
        }

        return baseAction;

    }
}

public class AnimatorAction : AnimationBase
{
    private Animator animator;
    public AnimatorAction(GameObject go)
    {
        if (go != null)
            animator = go.GetComponent<Animator>();
    }

    public override void Play(string name)
    {
        if (animator != null)
            animator.Play(name);
    }

    public override void Play(string name, int layout, int index)
    {
        if (animator != null)
            animator.Play(name,layout,index);
    }
}

public class AnimationAction : AnimationBase
{
    private Animation animation;
    public AnimationAction(GameObject go)
    {
        if (go != null)
            animation = go.GetComponent<Animation>();
    }

    public override void Play(string name)
    {
        if (animation != null)
            animation.Play(name);
    }
}


public class BaseModelSprite
{
    protected int animationType = 1;
    protected Transform modelParent;
    protected GameObject model;
    protected AnimationBase animator;
    protected int transLv = -1;

    //创建
    public virtual void CreateModel(string path, Transform parent, int type) { }

    //
    public virtual void CreateModel(Transform parent, int type, Dictionary<EquipType, string> PathDic) { }

    //销毁
    public virtual void RemoveAnimatorView() { }
    //播放
    public virtual void Play(string animationName) { }
    //透明度
    public virtual void ChangeTranslucence(int level) { }
    //设置装备
    public virtual void SetEquipDatas(List<int> equips) { }
}

public class RoleModelSprite : BaseModelSprite
{

    private Dictionary<EquipType, GameObject> modelObjDic = new Dictionary<EquipType, GameObject>();
    private Dictionary<EquipType, string> modelPartPath = new Dictionary<EquipType, string>();
    private List<string> assetPaths = new List<string>();
    private List<UnityAction<Object, string>> assetCallbackList = new List<UnityAction<Object, string>>();

    private int loadIndex = 0;

    public override void CreateModel(string path, Transform parent, int type)
    {
        if (null != animator)
        {
            model.SetActive(true);
            return;
        }

        AssetManager.LoadAsset(path, (Object target, string comePath) =>
        {
            GameObject prefab = target as GameObject;
            if (null != prefab)
            {
                GameObject go = GameObject.Instantiate(prefab, parent);
                model = go;
                animator = AnimationBase.GetAnimationBase(type, go);
                ChangeTranslucence(0);
            }
        });
    }

    public override void CreateModel(Transform parent, int type, Dictionary<EquipType, string> PathDic)
    {
        modelParent = parent;
        animationType = type;
        assetPaths.Clear();
        assetPaths.Add(GetPathByType(PathDic, EquipType.Main));
        assetPaths.Add(GetPathByType(PathDic, EquipType.Equip));

        assetCallbackList.Clear();
        assetCallbackList.Add(OnLoadObjCallback);
        assetCallbackList.Add(OnLoadObjCallback);
        assetCallbackList.Add((x, y) => OnCreateModelFinish());
        loadIndex = 0;
        LoadNextAsset();
    }

    private string GetPathByType(Dictionary<EquipType, string> PathDic, EquipType type)
    {
        string path = "";
        PathDic.TryGetValue(type, out path);
        return path;
    }

    private bool GetCurTypeByLoadingIndex(out EquipType type)
    {
        type = EquipType.Node;
        int index = loadIndex + 1;
        if (!System.Enum.IsDefined(typeof(ZTAnimationType), index))
            return false;
        else
        {
            type = (EquipType) index;
            return true;
        }
    }

    private void LoadNextAsset()
    {
        if (loadIndex >= assetPaths.Count)
        {
            if (loadIndex < assetCallbackList.Count)
                assetCallbackList[loadIndex](null, null);
            return;
        }
        EquipType type;
        if (!GetCurTypeByLoadingIndex(out type))
            return;
        if (string.IsNullOrEmpty(assetPaths[loadIndex]) || assetPaths[loadIndex].Equals(GetPathByType(modelPartPath, type)))
        {
            if (string.IsNullOrEmpty(assetPaths[loadIndex]))
                DestroyOldPart(type);
            loadIndex++;
            LoadNextAsset();
        }
        else
        {
            modelPartPath[type] = assetPaths[loadIndex];
            AssetManager.LoadAsset(assetPaths[loadIndex], OnLoadFinish);

        }
    }
    //部件加载完成
    private void OnLoadFinish(Object target, string path)
    {
        assetCallbackList[loadIndex](target, path);
        loadIndex++;
        LoadNextAsset();
    }

    //private bool test = true;
    //模型组装完毕
    private void OnCreateModelFinish()
    {
        ChangeTranslucence(0);
    }

    public void OnLoadObjCallback(Object target, string path)
    {
        EquipType type;
        if (!GetCurTypeByLoadingIndex(out type))
            return;

        if (!modelPartPath[type].Equals(path)) return;
        GameObject prefab = target as GameObject;
        if (null == prefab) return;
        GameObject go = GameObject.Instantiate(prefab, modelParent);
        if (type == EquipType.Main)
        {
            model = go;
            animator = AnimationBase.GetAnimationBase(animationType, go);
            OnModelChange(go);
        }
        else
        {
            Transform tempParent = model.transform.Find(CharaDefine.CharaPartParent[type]);
            if (tempParent != null)
            {
                go.transform.SetParent(tempParent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = Vector3.one;
            }
        }
        modelObjDic[type] = go;
    }
    //改变衣服时候，挂件要挂到新衣服上面
    private void OnModelChange(GameObject go)
    {
        foreach (var item in modelObjDic )
        {
            if (item.Value == null||item.Key== EquipType.Main) return;
            Transform temp = go.transform.Find(CharaDefine.CharaPartParent[item.Key]);
            item.Value.transform.SetParent(temp);
        }
       
    }
    //删除旧部件
    private void DestroyOldPart(EquipType type)
    {
        if (modelObjDic.ContainsKey(type))
        {
            if (modelObjDic[type])
            {
                GameObject.Destroy(modelObjDic[type]);
                modelObjDic.Remove(type);
            }
        }
    }

    public override void RemoveAnimatorView()
    {
        if (null != animator)
        {
            model.SetActive(false);
            GameObject.Destroy(model);
            model = null;
            animator = null;
            //暂时销毁处理
            // this.StartCoroutine(timeOut());
        }
    }

    IEnumerator timeOut()
    {
        yield return new WaitForSeconds(60);
        //非活动状态 清理东西对象
        if (!model.activeSelf)
        {
            if (null != model)
            {
                GameObject.Destroy(model);
            }
            model = null;
            animator = null;
        }
    }

    public override void Play(string animationName)
    {
        if (null == animator) return;
        animator.Play(animationName,0,0);
    }

    public override void ChangeTranslucence(int level)
    {
        if (this.transLv != level)
        {
            this.transLv = level;
            UpdateTranslucence();
        }
    }

    //0 常规 1隐藏 2半透明
    private void UpdateTranslucence()
    {
        if (null == animator) return;
        if (transLv == 1)
        {
            if (model.activeSelf)
                model.SetActive(false);
        }
        else
        {
            if (!model.activeSelf)
                model.SetActive(true);
            Renderer[] renders = model.transform.GetComponentsInChildren<Renderer>();// model.transform.Find("equitPos").GetComponent<SkinnedMeshRenderer>();
            for (int index = 0; index < renders.Length; index++)
            {
                Renderer render = renders[index];
                if (transLv == 0)
                {
                    render.material.shader = Shader.Find("Custom/PengLuOccTransVF");
                }
                else
                {
					render.material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");//")//");Unlit/ZTAlphaBlend
                    Color color = render.material.color;
                    render.material.color = new Color(color.r, color.g, color.b, 0.5f);
                }
            }

            
        }
    }

    public override void SetEquipDatas(List<int> equips)
    {

    }
}

