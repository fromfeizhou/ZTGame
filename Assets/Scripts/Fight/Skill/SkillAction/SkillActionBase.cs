using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActionBase
{
    public SkillDefine.SkillActionType ActionType = SkillDefine.SkillActionType.NONE;   //行为类型
    protected int _actFrame = 0;     //行动帧
    public virtual int ActFrame
    {
        get { return _actFrame; }
        set {
            _curFrame = value;
            _actFrame = value;
            _frameMax = value;
        }
    }

    public bool IsComplete = false; //是否完成

    protected bool _isStart;
    public virtual bool IsStart
    {
        get { return _isStart; }
        set
        {
            _isStart = value;
        }
    }
    

    protected SkillActionParser _actionParser = null;
    protected PlayerBattleInfo _skillPlayer = null;
    protected int _curFrame = -1; //当前已执行的帧数
    protected int _frameMax = 0;
    protected int _dtFrame = 0;//执行间隔

    /// <summary>
    /// 行为基类 构造函数
    /// </summary>
    /// <param name="player">技能使用对象</param>
    /// <param name="actionId">行为编号</param>
    /// <param name="actionType">行为类型</param>
    public SkillActionBase(SkillActionParser actionParser,int actFrame = 0)
    {
        _actionParser = actionParser;
        _skillPlayer = _actionParser.SkillPlayer;
        
        ActionType = SkillDefine.SkillActionType.NONE;

        _curFrame = actFrame;
        _actFrame = actFrame;
        _frameMax = actFrame;
    }
    //处理对象
    protected virtual void DoAction()
    {
        
    }

    protected virtual void Complete()
    {
        IsComplete = true;
    }

    //刷新对象
    public virtual void UpdateActoin(int curFrame = 0)
    {
        if (ActionType == SkillDefine.SkillActionType.NONE)
        {
            Complete();
        }
        else
        {
            _dtFrame = curFrame - _curFrame;
            _curFrame = curFrame;

            //有最大帧限制
            CheckMaxFrame();
        }
    }

    private void CheckMaxFrame()
    {
        if (_frameMax > 0)
        {
            bool isDone = false;
            if (_curFrame >= _frameMax)
            {
                //减去溢出部分 
                _dtFrame = _dtFrame -  (_curFrame - _frameMax);
                _curFrame = _frameMax;
                isDone = true;
            }

            if (isDone)
            {
                Complete();
            }
        }
    }

    

}