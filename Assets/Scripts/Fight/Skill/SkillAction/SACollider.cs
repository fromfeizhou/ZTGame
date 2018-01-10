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
    private List<PlayerBase> _targetList = null;
    private Dictionary<int, int> _colliderDic = null; //已碰撞队列

    private GameObject _colliderView;

    public override int ActFrame
    {
        get { return _actFrame; }
        set
        {
            _actFrame = value;
            _curFrame = value;
            _frameMax = ActFrame + _collider.LifeTime;
        }
    }

    public SACollider(SkillDefine.ColliderTarget colliderTarget, ColliderData collider, SkillActionParser actionParser, int actFrame)
        : base(actionParser, actFrame)
    {
        ActionType = SkillDefine.SkillActionType.COLLIDER;
        _colliderTarget = colliderTarget;
        _collider = collider;

        _frameMax = ActFrame + _collider.LifeTime;
        CheckTargetList();

#if UNITY_EDITOR
        CreateColliderView();
#endif
    }
    //刷新对象
    public override void UpdateActoin(int curFrame = 0)
    {
        base.UpdateActoin(curFrame);

#if UNITY_EDITOR
        UpdateColliderView();
#endif
        CheckCollider(curFrame);
    }

    protected void CheckCollider(int curFrame)
    {
        for (int i = 0; i < _targetList.Count; i++)
        {
            //碰撞结束 （碰撞次数已满）
            if (IsComplete)
            {
                return;
            }
           bool isCollider = ZTCollider.CheckCollision(_targetList[i].Collider, _collider.Collider);
           if (isCollider)
           {
               if(_colliderDic.ContainsKey(_targetList[i].Id)){
                   if((curFrame -  _colliderDic[_targetList[i].Id]) >Interval) {
                       DoAction(_targetList[i]);
                       _colliderDic[_targetList[i].Id] = curFrame;
                   }
               }else{
                    DoAction(_targetList[i]);
                    _colliderDic[_targetList[i].Id] = curFrame;
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
        _targetList = new List<PlayerBase>();
        if (_colliderTarget == SkillDefine.ColliderTarget.SELF)
        {
            _targetList.Add(_skillPlayer);
            return;
        }

        List<PlayerBase> list = SceneManager.GetInstance().PlayerList;
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

    protected void DoAction(PlayerBase player)
    {
        _colliderCount++;
        if (ColliderMax != -1 && _colliderCount >= ColliderMax)
        {
            Complete();
        }
        if (null != _collider.SelfActoins && _collider.SelfActoins.Count > 0)
        {
            for(int i =0; i < _collider.SelfActoins.Count;i++){
                int actionId = _collider.SelfActoins[i];
                _actionParser.ActionActivatebyId(actionId);
            }
        }
        if (null != _collider.TargetActions && _collider.TargetActions.Count > 0)
        {
            for (int i = 0; i < _collider.TargetActions.Count; i++)
            {
                int actionId = _collider.TargetActions[i];
                SceneManager.GetInstance().PlayerUseSkill(player.Id, new SkillOpera(actionId, SceneManager.GetInstance().SceneFrame, FightDefine.GetDirVec(_skillPlayer.MoveDir)));
            }
        }
    }

    protected override void Complete()
    {
        base.Complete();
#if UNITY_EDITOR
        if (null != _colliderView)
        {
            GameObject.Destroy(_colliderView);
            _colliderView = null;
        }
#endif
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
