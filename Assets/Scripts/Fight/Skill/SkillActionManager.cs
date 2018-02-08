using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActionManager : Singleton<SkillActionManager>
{
    private Dictionary<uint, List<SkillActionParser>> _skillParserDic;
    private List<SkillCommand> _addSkillCommand;
    public override void Init()
    {
        base.Init();
        _skillParserDic = new Dictionary<uint, List<SkillActionParser>>();
        _addSkillCommand = new List<SkillCommand>();
        InitEvent();
    }

    public override void Destroy()
    {
        if (null != _skillParserDic)
        {
            foreach (uint key in _skillParserDic.Keys)
            {
                List<SkillActionParser> list = _skillParserDic[key];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    list[i].Destroy();
                }
                list.Clear();
            }
            _skillParserDic.Clear();
            _skillParserDic = null;

            RemoveEvent();
        }
        base.Destroy();
    }

    private void InitEvent()
    {
        SceneEvent.GetInstance().addEventListener(SCENE_EVENT.ADD_SKILL_PARSER, OnAddSkillParser);
    }

    private void RemoveEvent()
    {
        SceneEvent.GetInstance().removeEventListener(SCENE_EVENT.ADD_SKILL_PARSER, OnAddSkillParser);
    }

    private void OnAddSkillParser(Notification data)
    {
        SkillCommand command = (data.param) as SkillCommand;
        _addSkillCommand.Add(command);
       
    }
	
	// Update is called once per frame
	public void Update () {
        foreach (uint key in _skillParserDic.Keys)
        {
            List<SkillActionParser> list = _skillParserDic[key];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                list[i].UpdateAction();
                if (list[i].IsComplete)
                {
                    list.RemoveAt(i);
                    if (list.Count == 0)
                    {
                        ICharaBattle battleInfo = ZTSceneManager.GetInstance().GetCharaById(key) as ICharaBattle;
                        if (null != battleInfo) battleInfo.ChangeState(BATTLE_STATE.NONE);
                    }
                }
            }
        }

        UpdateLate();
	}

    public void UpdateLate()
    {
        if (null == _addSkillCommand || _addSkillCommand.Count == 0) return;
        while (_addSkillCommand.Count > 0)
        {
            SkillCommand command = _addSkillCommand[0];
            _addSkillCommand.RemoveAt(0);
            if (null != command)
            {
                ICharaBattle battleInfo = ZTSceneManager.GetInstance().GetCharaById(command.BattleId) as ICharaBattle;

                if (null == battleInfo) return;

                if (!_skillParserDic.ContainsKey(command.BattleId)) _skillParserDic.Add(command.BattleId, new List<SkillActionParser>());
                SkillActionParser skillParser = new SkillActionParser(battleInfo, command);
                _skillParserDic[battleInfo.BattleId].Add(skillParser);
            }
        }
    }
}
