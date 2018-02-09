using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightModule : Singleton<FightModule> {

	private int _curSkillId;
	public int CurSkillId{
		get{ 
			return _curSkillId;
		}
	}

	public void SelectSkill(int skillId)
	{
		_curSkillId = skillId;
	}
}
