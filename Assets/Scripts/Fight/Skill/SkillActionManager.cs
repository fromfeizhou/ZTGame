using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActionManager : Singleton<SkillActionManager>
{
    private Dictionary<int, List<SkillActionParser>> _skillParserDic;
    public override void Init()
    {
        base.Init();
        _skillParserDic = new Dictionary<int, List<SkillActionParser>>();
        InitEvent();
    }

    public override void Destroy()
    {
        if (null != _skillParserDic)
        {
            foreach (int key in _skillParserDic.Keys)
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
        SceneEvent.GetInstance().addEventListener(SceneEvents.ADD_SKILL_PARSER, OnAddSkillParser);
    }

    private void RemoveEvent()
    {
        SceneEvent.GetInstance().removeEventListener(SceneEvents.ADD_SKILL_PARSER, OnAddSkillParser);
    }

    private void OnAddSkillParser(Notification data)
    {
        SkillCommand command = (data.param) as SkillCommand;
        if (null != command)
        {
            PlayerBattleInfo battleInfo = ZTSceneManager.GetInstance().GetCharaById(command.BattleId);

            if(null == battleInfo) return;

            if (!_skillParserDic.ContainsKey(command.BattleId)) _skillParserDic.Add(command.BattleId, new List<SkillActionParser>());
            SkillActionParser skillParser = new SkillActionParser(battleInfo, command);
            _skillParserDic[battleInfo.BattleId].Add(skillParser);
        }
    }
	
	// Update is called once per frame
	public void Update () {
        foreach (int key in _skillParserDic.Keys)
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
                        PlayerBattleInfo battleInfo = ZTSceneManager.GetInstance().GetCharaById(key);
                        if (null != battleInfo) battleInfo.ChangeState(BATTLE_STATE.NONE);
                    }
                }
            }
        }
        
	}
}
