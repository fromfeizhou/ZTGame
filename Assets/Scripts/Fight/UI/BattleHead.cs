using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHead : MonoBehaviour
{
    private CharaActorInfo _charaActor;
    private ICharaFight _fightInfo;
    private ICharaBattle _battleInfo;
    private Slider _slider;
    private MTextFormat _name;
    private MTextFormat _hpText;
    private Canvas _canvas;

    void Awake()
    {
        _slider = this.transform.Find("HpBar").gameObject.GetComponent<Slider>();
        _name = this.transform.Find("Name").gameObject.GetComponent<MTextFormat>();
        _hpText = this.transform.Find("HpText").gameObject.GetComponent<MTextFormat>();
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        StartCoroutine(EndOfFrameHandler());
    }

    IEnumerator EndOfFrameHandler()
    {
        yield return new WaitForEndOfFrame();
        if (null != _battleInfo)
        {
            this.OnUpdatePos(new Notification(_battleInfo.MovePos));
        }

    }

    public void SetInfo(CharaActorInfo info)
    {
        if (null == _slider) return;
        if (null != _charaActor)
        {
            RemoveEvent();
        }
        _charaActor = info;
        _fightInfo = info as ICharaFight;
        _battleInfo = info as ICharaBattle;
        if (null == _charaActor || null == _fightInfo || null == _battleInfo) return;

        _slider.maxValue = _fightInfo.MaxHp;
        _slider.value = _fightInfo.Hp;
        _hpText.TextStr = _fightInfo.Hp.ToString() + "/" + _fightInfo.MaxHp.ToString();
        _name.TextStr = "测试";
        InitEvent();
    }

    public void InitEvent()
    {
        if (null == _charaActor) return;
        _charaActor.addEventListener(CHARA_EVENT.ADD_HURT, OnHurt);
        _charaActor.addEventListener(CHARA_EVENT.UPDATE_POS, OnUpdatePos);
    }

    public void RemoveEvent()
    {
        if (null == _charaActor) return;
        _charaActor.removeEventListener(CHARA_EVENT.ADD_HURT, OnHurt);
        _charaActor.removeEventListener(CHARA_EVENT.UPDATE_POS, OnUpdatePos);
    }

    public void OnHurt(Notification data)
    {
        _slider.value = _fightInfo.Hp;
        _hpText.TextStr = _fightInfo.Hp.ToString() + "/" + _fightInfo.MaxHp.ToString();
    }

    public void OnUpdatePos(Notification data)
    {
        Vector3 worldToScreenPoint = Camera.main.WorldToScreenPoint(_battleInfo.MovePos);
        Vector3 screenToWorldPoint = _canvas.worldCamera.ScreenToWorldPoint(worldToScreenPoint);
        this.transform.position = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y + CharaDefine.BATTLE_HEAD_OFFSET, 0);

    }

    void OnDestroy()
    {
        if (null != _charaActor)
        {
            RemoveEvent();
            _charaActor = null;
            _fightInfo = null;
            _battleInfo = null;
        }
    }
}
