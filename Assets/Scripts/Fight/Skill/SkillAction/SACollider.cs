using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 碰撞
/// </summary>
public class SACollider : SkillActionBase
{
    protected ColliderInfo _colliderInfo = null;    //碰撞信息
    protected CollBase _collider = null;    //碰撞结构

    public bool ColliderDestroy = false;    //碰撞消失
    private int _colliderCount = 0; //碰撞次数

    private List<ICharaBattle> _targetList = null;
    private Dictionary<int, int> _colliderDic = null; //已碰撞队列

    private GameObject _colliderView;     //碰撞显示
    private GameObject _colliderEffect;     //特效容器
    private EffectCounter _effectCounter;      //非主动清理特效 记录 销毁时候清理

    public override bool IsStart
    {
        get { return _isStart; }
        set
        {
            _isStart = value;
#if UNITY_EDITOR
            CreateColliderView();
#endif
            CreateColliderEffect();
        }
    }

    public override int ActFrame
    {
        get { return _actFrame; }
        set
        {
            _curFrame = value;
            _actFrame = value;
            _frameMax = _actFrame + _colliderInfo.LifeTime;
        }
    }

    public SACollider(CollBase collider, ColliderInfo collidInfo, SkillActionParser actionParser, int actFrame)
        : base(actionParser, actFrame)
    {
        _collider = collider;
        _colliderInfo = collidInfo;
        _effectCounter = new EffectCounter();

        _colliderCount = 0;
        ColliderDestroy = false;

        ActionType = SkillDefine.SkillActionType.COLLIDER;
        _frameMax = _actFrame + collidInfo.LifeTime;

        CheckTargetList();


    }
    //刷新对象
    public override void UpdateActoin(int curFrame = 0)
    {
        base.UpdateActoin(curFrame);

#if UNITY_EDITOR
        UpdateColliderView();
#endif
        CheckCollider(curFrame);
        UpdateColliderEffect();
    }

    protected void CheckCollider(int curFrame)
    {
        //指定目标 不需要判断 在运动结束 调用 
        if (_colliderInfo.ColliderType == CollBase.ColType.TARGET)
        {
            return;
        }

        for (int i = 0; i < _targetList.Count; i++)
        {
            //碰撞结束 （碰撞次数已满）
            bool isCollider = ZTCollider.CheckCollision(_targetList[i].Collider, _collider);
            if (isCollider)
            {
                if (_colliderDic.ContainsKey(_targetList[i].BattleId))
                {
                    if (_colliderInfo.Interval > 0 && (curFrame - _colliderDic[_targetList[i].BattleId]) > _colliderInfo.Interval)
                    {
                        DoColliderAction(_targetList[i]);
                        _colliderDic[_targetList[i].BattleId] = curFrame;
                    }
                }
                else
                {
                    DoColliderAction(_targetList[i]);
                    _colliderDic[_targetList[i].BattleId] = curFrame;
                }

                if (ColliderDestroy)
                {
                    Complete();
                }
            }
        }
    }

    //过滤碰撞对象 
    private void CheckTargetList()
    {
        _colliderDic = new Dictionary<int, int>();
        _targetList = SkillMethod.GetTargetList(_skillPlayer, _colliderInfo.ColliderTarget);
    }

    public void DoColliderAction(ICharaBattle player)
    {
        _colliderCount++;
        if (_colliderInfo.ColliderMax != -1 && _colliderCount >= _colliderInfo.ColliderMax)
        {
            Complete();
            return;
        }
        if (null != _colliderInfo.FightEffectList && _colliderInfo.FightEffectList.Count > 0)
        {
            Vector3 dir = new Vector3(_collider.x, 0, _collider.y);
            dir = (player.MovePos - dir).normalized;

            for (int i = 0; i < _colliderInfo.FightEffectList.Count; i++)
            {
                FightEffectInfo info = _colliderInfo.FightEffectList[i];
                if (info.EffectTarget == FIGHT_EF_TARGET.SELF)
                {
                    FightEffectDefine.ParseEffect(_skillPlayer, info, _skillPlayer.BattleId, dir);
                }
                else
                {
                    SkillCommand command = null;
                    if (info.EffectType == FIGHT_EF_TPYE.ACTION)
                    {
                        command = FightDefine.GetSkillCommand(player.BattleId, info.Param1, dir, player.MovePos);
                    }
                    FightEffectDefine.ParseEffect(player, info, _skillPlayer.BattleId, dir, command);
                }
            }
        }
        //if (null != _colliderInfo.SelfActions && _colliderInfo.SelfActions.Count > 0)
        //{
        //    for (int i = 0; i < _colliderInfo.SelfActions.Count; i++)
        //    {
        //        int actionId = _colliderInfo.SelfActions[i];
        //        _actionParser.ActionActivatebyId(actionId);
        //    }
        //}
        //if (null != _colliderInfo.TargetActions && _colliderInfo.TargetActions.Count > 0)
        //{
        //    for (int i = 0; i < _colliderInfo.TargetActions.Count; i++)
        //    {
        //        int skillId = _colliderInfo.TargetActions[i];
        //        Vector3 dir = new Vector3(_collider.x, 0, _collider.y);
        //        dir = (dir - player.MovePos).normalized;
        //        SkillCommand command = FightDefine.GetSkillCommand(player.BattleId, skillId, dir, player.MovePos);
        //        SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_COMMAND,new Notification(command));
        //    }
        //}
    }

    protected override void Complete()
    {
        base.Complete();
        if (null != _effectCounter)
        {
            _effectCounter.Destroy();
            _effectCounter = null;
        }
        if (null != _colliderEffect)
        {
            GameObject.Destroy(_colliderEffect);
            _colliderEffect = null;
        }
#if UNITY_EDITOR
        if (null != _colliderView)
        {
            GameObject.Destroy(_colliderView);
            _colliderView = null;
        }
#endif
    }

    //特效创建
    private void CreateColliderEffect()
    {
        if (null == _colliderEffect)
        {
            _colliderEffect = new GameObject();
            _colliderEffect.name = "EffectCon";
            _colliderEffect.transform.localRotation = Quaternion.Euler(Vector3.up * _collider.rotate);
            _colliderEffect.transform.localPosition = new Vector3(_collider.x, 0.1f, _collider.y);
            _colliderEffect.transform.SetParent(GameObject.Find("PlayerLayer").transform,false);
        }
        if(null !=_colliderInfo.EffectInfos && _colliderInfo.EffectInfos.Count > 0){
            for (int i = 0; i < _colliderInfo.EffectInfos.Count; i++)
            {
                EffectInfo effectInfo = _colliderInfo.EffectInfos[i];
                _effectCounter.AddEffect(effectInfo, _colliderEffect.transform);
            }
        }
    }
    //特效刷新
    private void UpdateColliderEffect()
    {
        if (null != _colliderEffect)
        {
            _colliderEffect.transform.localPosition = new Vector3(_collider.x, 0.1f, _collider.y);
        }
    }
    //碰撞创建 debug
    private void CreateColliderView()
    {
        _colliderView = ZColliderView.CreateColliderView(_collider);
        _colliderView.transform.localRotation = Quaternion.Euler(Vector3.up * _collider.rotate);
        _colliderView.transform.transform.SetParent(GameObject.Find("PlayerLayer").transform, false);
        UpdateColliderView();
    }
    //碰撞刷新 debug
    private void UpdateColliderView()
    {
        if (null != _colliderView)
        {
            _colliderView.transform.localPosition = new Vector3(_collider.x, 0.1f, _collider.y);
        }
    }

}
