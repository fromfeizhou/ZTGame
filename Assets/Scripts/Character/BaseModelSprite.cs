using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EquipType
{
    Cloth,
    Equip
}


public class EquipData
{
    public EquipType type;
    public string path;

}


public class BaseModelSprite
{
    protected GameObject model;
    protected Animation animator;
    protected bool isTrans;

    //创建
    public virtual void CreateModel(string path,Transform parent) { }
    //销毁
    public  virtual  void RemoveAnimatorView() { }
    //播放
    public virtual void Play(string animationName) { }
    //透明度
    public  virtual  void ChangeTranslucence(bool isTrans) { }
    //设置装备
    public  virtual  void SetEquipDatas(List<int> equips) { }
}

public class RoleModelSprite : BaseModelSprite
{
    public override void CreateModel(string path,Transform parent)
    {
        if (null != animator)
        {
            model.SetActive(true);
            return;
        }

        AssetManager.LoadAsset(path, (Object target, string comePath) => {
            GameObject prefab = target as GameObject;
            if (null != prefab)
            {
                GameObject go = GameObject.Instantiate(prefab, parent);
                model = go;
                animator = go.GetComponent<Animation>();
            }
        });
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
        animator.Play(animationName);
    }

    public override void ChangeTranslucence(bool isTrans)
    {
        if(this.isTrans != isTrans)
        {
            this.isTrans = isTrans;
            UpdateTranslucence();
        }
    }

    private void UpdateTranslucence()
    {
        if (null == animator) return;
        if (isTrans)
        {
            animator.gameObject.SetActive(true);
            SkinnedMeshRenderer render = animator.gameObject.transform.Find("equitPos").GetComponent<SkinnedMeshRenderer>();

            if (!isTrans)
            {
                render.material.shader = Shader.Find("Custom/PengLuOccTransVF");
            }
            else
            {
                render.material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
                Color color = render.material.color;
                render.material.color = new Color(color.r, color.g, color.b, 125);
            }

        }
        else
        {
            animator.gameObject.SetActive(false);
        }
    }

    public override void SetEquipDatas(List<int> equips)
    {

    }
}

