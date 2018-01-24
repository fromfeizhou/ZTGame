using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 碰撞
/// </summary>
public class SACollider : SkillActionBase
{
    public int Interval = 0;    //碰撞间隙
    public int ColliderMax = 1;    //碰撞最大次数
    private int _colliderCount = 0;
    public bool ColliderDestroy = false;    //碰撞消失

    protected ColliderData _collider = null;    //碰撞信息
    private SkillDefine.ColliderTarget _colliderTarget = SkillDefine.ColliderTarget.SELF;     //碰撞对象
    private List<PlayerBattleInfo> _targetList = null;
    private Dictionary<int, int> _colliderDic = null; //已碰撞队列

    private GameObject _colliderView;
    private GameObject _colliderEffect;

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
            _frameMax = _actFrame + _collider.LifeTime;
        }
    }

    public SACollider(SkillDefine.ColliderTarget colliderTarget, ColliderData collider, SkillActionParser actionParser, int actFrame)
        : base(actionParser, actFrame)
    {
        _colliderTarget = colliderTarget;
        _collider = collider;

        ActionType = SkillDefine.SkillActionType.COLLIDER;
        _frameMax = _actFrame + _collider.LifeTime;

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
        if (_dtFrame < 0)
            return;

        for (int i = 0; i < _targetList.Count; i++)
        {
            //碰撞结束 （碰撞次数已满）
            bool isCollider = ZTCollider.CheckCollision(_targetList[i].Collider, _collider.Collider);
            if (isCollider)
            {
                if (_colliderDic.ContainsKey(_targetList[i].BattleId))
                {
                    if (Interval > 0 && (curFrame - _colliderDic[_targetList[i].BattleId]) > Interval)
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
        _targetList = new List<PlayerBattleInfo>();
        if (_colliderTarget == SkillDefine.ColliderTarget.SELF)
        {
            _targetList.Add(_skillPlayer);
            return;
        }

        List<PlayerBattleInfo> list = ZTSceneManager.GetInstance().GetCharaList();
        for (int i = 0; i < list.Count; i++)
        {
            switch (_colliderTarget)
            {
                case SkillDefine.ColliderTarget.TEAM:
                    if (_skillPlayer.Camp == list[i].Camp)
                    {
                        _targetList.Add(list[i]);
                    }
                    break;
                case SkillDefine.ColliderTarget.ENEMY:
                    if (_skillPlayer.Camp != list[i].Camp)
                    {
                        _targetList.Add(list[i]);
                    }
                    break;
                case SkillDefine.ColliderTarget.ALL:
                    _targetList.Add(list[i]);
                    break;
            }
        }
    }

    protected void DoAction(PlayerBattleInfo player)
    {
        _colliderCount++;
        if (ColliderMax != -1 && _colliderCount >= ColliderMax)
        {
            Complete();
        }
        if (null != _collider.SelfActoins && _collider.SelfActoins.Count > 0)
        {
            for (int i = 0; i < _collider.SelfActoins.Count; i++)
            {
                int actionId = _collider.SelfActoins[i];
                _actionParser.ActionActivatebyId(actionId);
            }
        }
        if (null != _collider.TargetActions && _collider.TargetActions.Count > 0)
        {
            for (int i = 0; i < _collider.TargetActions.Count; i++)
            {
                int skillId = _collider.TargetActions[i];
                //ZTSceneManager.GetInstance().PlayerUseSkill(player.Id, new SkillOpera(actionId, ZTSceneManager.GetInstance().SceneFrame, FightDefine.GetDirVec(_skillPlayer.MoveDir)));
                Vector3 dir = new Vector3( _collider.Collider.x,0,_collider.Collider.y);
                dir = (dir - player.MovePos).normalized;
                SkillCommand command = FightDefine.GetSkillCommand(player.BattleId, skillId, dir, player.MovePos);
                SceneEvent.GetInstance().dispatchEvent(SceneEvents.ADD_COMMAND,new Notification(command));
            }
        }
    }

    protected override void Complete()
    {
        base.Complete();
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

    private void CreateColliderEffect()
    {
        if (_collider.EffectId == null) return;

        FightEffectLib.GetEffectByName(_collider.EffectId, (Object target, string path) =>
       {
           GameObject go = target as GameObject;
           if (null != go)
           {
               Transform effect = GameObject.Instantiate(go).transform;
               effect.transform.parent = GameObject.Find("PlayerLayer").transform;
               effect.transform.localRotation = Quaternion.Euler(Vector3.up * _collider.Collider.rotate);
               effect.transform.localPosition = new Vector3(_collider.Collider.x, 0.1f, _collider.Collider.y);
               ParticleDestroy pds = go.GetComponent<ParticleDestroy>();
               if (null != pds)
               {
                   return;
               }
               _colliderEffect = effect.gameObject;
           }
       });
    }

    private void UpdateColliderEffect()
    {
        if (_collider.EffectId == null || _colliderEffect == null) return;
        if (null != _colliderEffect)
        {
            _colliderEffect.transform.localPosition = new Vector3(_collider.Collider.x, 0.1f, _collider.Collider.y);
        }
    }

    private void CreateColliderView()
    {
        _colliderView = ZColliderView.CreateColliderView(_collider.Collider);
        _colliderView.transform.parent = GameObject.Find("PlayerLayer").transform;
        _colliderView.transform.localRotation = Quaternion.Euler(Vector3.up * _collider.Collider.rotate);
        UpdateColliderView();
    }

    private void UpdateColliderView()
    {
        if (null != _colliderView)
        {
            _colliderView.transform.localPosition = new Vector3(_collider.Collider.x, 0.1f, _collider.Collider.y);
        }
    }

}
