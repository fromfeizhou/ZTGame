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
    public void CreateAnimatorView(string path)
    {
        if (modelSprite == null)
            modelSprite = new roleModelSprite();
        modelSprite.CreateModel(path);
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
        if (modelSprite == null) return;
        modelSprite.UpdatePos(pos);
    }

    //更新角度
    public void UpdateRotate(Vector3 dir)
    {
        if (modelSprite == null) return;
        modelSprite.UpdateRotete(dir);
    }

    //改变透明度
    public void ChangeTranslucence(bool isTrans)
    {
        if (modelSprite == null) return;
        modelSprite.ChangeTranslucence(isTrans);

    }

    
    
}
