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
    private List<int> _effectKeys;      //非主动清理特效 记录 销毁时候清理

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
        _effectKeys = new List<int>() ;

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
                        DoAction(_targetList[i]);
                        _colliderDic[_targetList[i].BattleId] = curFrame;
                    }
                }
                else
                {
                    DoAction(_targetList[i]);
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
        _targetList = new List<ICharaBattle>();
        if (_colliderInfo.ColliderTarget == SkillDefine.ColliderTarget.SELF)
        {
            _targetList.Add(_skillPlayer as ICharaBattle);
            return;
        }

        List<CharaActorInfo> list = ZTSceneManager.GetInstance().GetCharaList();
        for (int i = 0; i < list.Count; i++)
        {
            ICharaBattle info = list[i] as ICharaBattle;
            if (null != info)
            {
                switch (_colliderInfo.ColliderTarget)
                {
                    case SkillDefine.ColliderTarget.TEAM:
                        if (_skillPlayer.Camp == info.Camp)
                        {
                            _targetList.Add(info);
                        }
                        break;
                    case SkillDefine.ColliderTarget.ENEMY:
                        if (_skillPlayer.Camp != info.Camp)
                        {
                            _targetList.Add(info);
                        }
                        break;
                    case SkillDefine.ColliderTarget.ALL:
                        _targetList.Add(info);
                        break;
                }
            }
        }
    }

    protected void DoAction(ICharaBattle player)
    {
        _colliderCount++;
        if (_colliderInfo.ColliderMax != -1 && _colliderCount >= _colliderInfo.ColliderMax)
        {
            Complete();
        }
        if (null != _colliderInfo.SelfActions && _colliderInfo.SelfActions.Count > 0)
        {
            for (int i = 0; i < _colliderInfo.SelfActions.Count; i++)
            {
                int actionId = _colliderInfo.SelfActions[i];
                _actionParser.ActionActivatebyId(actionId);
            }
        }
        if (null != _colliderInfo.TargetActions && _colliderInfo.TargetActions.Count > 0)
        {
            for (int i = 0; i < _colliderInfo.TargetActions.Count; i++)
            {
                int skillId = _colliderInfo.TargetActions[i];
                Vector3 dir = new Vector3(_collider.x, 0, _collider.y);
                dir = (dir - player.MovePos).normalized;
                SkillCommand command = FightDefine.GetSkillCommand(player.BattleId, skillId, dir, player.MovePos);
                SceneEvent.GetInstance().dispatchEvent(SceneEvents.ADD_COMMAND,new Notification(command));
            }
        }
    }

    protected override void Complete()
    {
        base.Complete();
        if (null != _effectKeys)
        {
            FightEffectManager.GetInstance().RemoveEffectInfo(_effectKeys);
            _effectKeys.Clear();
            _effectKeys = null;
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
            _colliderEffect.transform.parent = GameObject.Find("PlayerLayer").transform;
            _colliderEffect.transform.localRotation = Quaternion.Euler(Vector3.up * _collider.rotate);
            _colliderEffect.transform.localPosition = new Vector3(_collider.x, 0.1f, _collider.y);
        }
        if(null !=_colliderInfo.EffectInfos && _colliderInfo.EffectInfos.Count > 0){
            for (int i = 0; i < _colliderInfo.EffectInfos.Count; i++)
            {
                EffectInfo effectInfo = _colliderInfo.EffectInfos[i];
                int effectKey = FightEffectManager.GetInstance().AddEffectByInfo(effectInfo, _colliderEffect.transform);
                if (effectKey > 0)
                {
                    _effectKeys.Add(effectKey);
                }
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
        _colliderView.transform.parent = GameObject.Find("PlayerLayer").transform;
        _colliderView.transform.localRotation = Quaternion.Euler(Vector3.up * _collider.rotate);
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
