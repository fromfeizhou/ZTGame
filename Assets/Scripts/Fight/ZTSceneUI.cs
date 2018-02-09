using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZTSceneUI : Singleton<ZTSceneUI>
{
    private struct TextInfo
    {
        public Text text;
        public float time;
        public Vector3 oriPos;
        public float horSpeed;
        public float extent;
        public TextInfo(Text value, Vector3 point, float timemax, float min, float max, float extentmax)
        {
            text = value;
            time = timemax;
            extent = extentmax;
            oriPos = point;
            horSpeed = Random.Range(min, max);
        }
    }

    private Transform _hpTransform;
    private Canvas _canvas;
    private List<TextInfo> _hurtTs = new List<TextInfo>();
    private GameObject _hurtGo;
    private GameObject _battleHeadGo;

    public override void Init()
    {
        base.Init();
        _hpTransform = GameObject.Find("HpPanel").transform;
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _hurtTs = new List<TextInfo>();
        LoadHurtGo();
        InitEvent();
    }

    //控制伤害值动画的字段
    private float _tiTime = 0.9f;//消失时间
    //private float _tiHeight = 0.8f;//弹得高度
    private float _tiSpeed = 1.2f;//抛物线速度
    private float _tiHorSpeedMin = 0.8f;//水平速度随机值的最小值
    private float _tiHorSpeedMax = 1.3f;//水平速度随机值的最大值
    private float _tiHorSpeedReduce = 0.007f;//水平减速度
    //private float _tiVerSpeed = 3;//垂直速度
    private float _tiExtent = 4;//抛物线系数
    public void Update()
    {
        float dt = 0.025f;
        for (int i = _hurtTs.Count - 1; i >= 0; --i)
        {
            TextInfo ti = _hurtTs[i];
            ti.time = ti.time - dt;
            ti.extent = ti.extent - dt * _tiSpeed;
            ti.horSpeed -= _tiHorSpeedReduce;
            if (ti.time < 0.5f && ti.time + dt > 0.5f)
                ti.text.CrossFadeAlpha(0, 1f, true);
            if (ti.time <= 0)
            {
                GameObject.Destroy(ti.text.gameObject);
                _hurtTs.RemoveAt(i);
                return;
            }
            Vector3 worldToScreenPoint = Camera.main.WorldToScreenPoint(ti.oriPos);
            Vector3 screenToWorldPoint = _canvas.worldCamera.ScreenToWorldPoint(worldToScreenPoint);
            ti.text.transform.position = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y + 0.2f + _tiTime - ti.time, 0);
            _hurtTs[i] = ti;
        }
    }

    private void InitEvent()
    {
        SceneEvent.GetInstance().addEventListener(SCENE_EVENT.ADD_UI_HURT_VALUE, OnAddUIHurtValue);
        SceneEvent.GetInstance().addEventListener(SCENE_EVENT.ADD_UI_HEAD, OnAddUIHead);
    }

    private void RemoveEvent()
    {
        SceneEvent.GetInstance().removeEventListener(SCENE_EVENT.ADD_UI_HURT_VALUE, OnAddUIHurtValue);
        SceneEvent.GetInstance().removeEventListener(SCENE_EVENT.ADD_UI_HEAD, OnAddUIHead);
    }

    private void OnAddUIHead(Notification data)
    {
        CharaActorInfo info = (CharaActorInfo)data.param;
        if (null != info)
        {
            if (null != _battleHeadGo)
            {
                CreateBattleHead(info);
            }
            else
            {
                AssetManager.LoadAsset(PathManager.GetFullPathByName("BattleUIPrefab", "BattleHead.prefab"), (Object target, string path) =>
                {
                    _battleHeadGo = target as GameObject;
                    if (null != _battleHeadGo)
                    {
                        CreateBattleHead(info);
                    }
                });
            }
            
        }
    }

    private void OnAddUIHurtValue(Notification data)
    {
        HurtInfo info = (HurtInfo)data.param;
        if(null != _hurtGo){
            Vector3 worldToScreenPoint = Camera.main.WorldToScreenPoint(info.Pos);
            Vector3 screenToWorldPoint = _canvas.worldCamera.ScreenToWorldPoint(worldToScreenPoint);

            GameObject go = GameObject.Instantiate(_hurtGo);
            go.transform.SetParent(_hpTransform,false);
            go.transform.position = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y + 0.2f, 0);
            go.transform.localScale = new Vector3(1, 1, 1);
            Text text = go.GetComponent<Text>();
            text.text = info.Value.ToString();
            TextInfo ti = new TextInfo(text, info.Pos, _tiTime, _tiHorSpeedMin, _tiHorSpeedMax, _tiExtent);
            _hurtTs.Add(ti);
        }
    }

    private void LoadHurtGo()
    {
        AssetManager.LoadAsset(PathManager.GetFullPathByName("BattleUIPrefab", "HurtText.prefab"), (Object target, string path) =>
        {
            _hurtGo = target as GameObject;
        });
    }

    private void CreateBattleHead(CharaActorInfo info)
    {
        ICharaBattle battleInfo = info as ICharaBattle;
        if (null == battleInfo) return;
     
        GameObject go = GameObject.Instantiate(_battleHeadGo);
        go.transform.SetParent(_hpTransform, false);
        go.transform.localScale = new Vector3(1, 1, 1);
        go.GetComponent<BattleHead>().SetInfo(info);
    }

    public override void Destroy()
    {
        RemoveEvent();
        base.Destroy();
    }
}
