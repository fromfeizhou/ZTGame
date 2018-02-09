using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAPlayerControl : SkillActionBase {

	public bool IsControl = false;
    public bool IsSkillDir = false;

    public SAPlayerControl(bool isCtrl, bool isSkillDir, SkillActionParser actionParser, uint actFrame)
        : base(actionParser,actFrame)
    {
        IsControl = isCtrl;
        IsSkillDir = isSkillDir;
        ActionType = SkillDefine.SkillActionType.PLAY_CONTROL;
    }

     //播放动作
    public override void UpdateActoin(uint curFrame = 0)
    {
        base.UpdateActoin(curFrame);
        DoAction();
    }

    protected override void DoAction()
    {
        if (IsControl)
        {
            _actionParser.OldBattleState = _skillPlayer.BattleState;
            _skillPlayer.ChangeState(BATTLE_STATE.SKILL);
        }
        else
        {
            if (_skillPlayer.BattleState == BATTLE_STATE.SKILL)
            {
                _skillPlayer.ChangeState(BATTLE_STATE.NONE);
                _skillPlayer.ChangeState(_actionParser.OldBattleState);
            }
        }

        if (IsSkillDir)
        {
            ICharaActor chara = _skillPlayer as ICharaActor;
            if (null != chara)
            {
                chara.ChangeRotate(_actionParser.Command.SkillDir);
            }
        }

        Complete();
    }
}
