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
    protected Animator animator;
    protected bool isTrans;

    //创建
    public virtual void CreateModel(string path) { }
    //销毁
    public  virtual  void RemoveAnimatorView() { }
    //播放
    public virtual void Play(string animationName) { }
    //位置更新
    public virtual void UpdatePos(Vector3 pos) { }
    //旋转
    public virtual void UpdateRotete(Vector3 dir) { }
    //透明度
    public  virtual  void ChangeTranslucence(bool isTrans) { }
    //设置装备
    public  virtual  void SetEquipDatas(List<int> equips) { }



}

public class roleModelSprite : BaseModelSprite
{


    public override void CreateModel(string path)
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
                GameObject go = GameObject.Instantiate(prefab, animator.transform);
                model = go;
                animator = go.GetComponent<Animator>();
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

    public override void UpdatePos(Vector3 pos)
    {
        model.transform.localPosition = pos;
    }

    public override void UpdateRotete(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        model.transform.localRotation = Quaternion.Euler(Vector3.up * angle);
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

