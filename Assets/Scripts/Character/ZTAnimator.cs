using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class ZTAnimator : MonoBehaviour {
     

    private BaseModelSprite modelSprite;

    public void Start()
    {

    }

    public void OnDestroy()
    {
        if (modelSprite == null) return;
        modelSprite.RemoveAnimatorView();
    }

    //创建形象
    public void CreateAnimatorView(string path,int type=1)
    {
        if (modelSprite == null)
            modelSprite = new RoleModelSprite();
        modelSprite.CreateModel(path,this.transform,type);
    }

    //创建形象
    public void CreateAnimatorView(int modelType,int animationType, string rolePath,string equipPath ,string necklace,string ring,string spirit)
    {
        if (modelSprite == null)
            modelSprite = new RoleModelSprite();

        Dictionary<EquipType, string> tempPathDic = new Dictionary<EquipType, string>()
        {
            {EquipType.Main, rolePath},
            {EquipType.Equip, equipPath}
        };
        modelSprite.CreateModel(this.transform, animationType,tempPathDic);
    }




    public void SetEquips(List<int> equips)
    {
        if (modelSprite == null) return;
        modelSprite.SetEquipDatas(equips);
    }

    //移除形象
    public void RemoveAnimatorView()
    {
        if (modelSprite == null) return;
        modelSprite.RemoveAnimatorView();
    }

    //播放动作
    public void Play(string actoinName)
    {
        if (modelSprite == null) return;
        modelSprite.Play(actoinName);
    }

    //刷新位置
    public void UpdatePos(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }

    //更新角度
    public void UpdateRotate(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        this.transform.localRotation = Quaternion.Euler(Vector3.up * angle);
    }

    //改变透明度
    public void ChangeTranslucence(int transLv)
    {
        if (modelSprite == null) return;
        modelSprite.ChangeTranslucence(transLv);

    }

    
    
}
