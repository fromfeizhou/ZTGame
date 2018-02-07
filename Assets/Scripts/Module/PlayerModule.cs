using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModule : Singleton<PlayerModule>
{
	private gprotocol.p_role _roleInfo;

	public uint RoleID
	{
		get{ 
			return _roleInfo.id;
		}
	}
	public void SetRoleInfo(gprotocol.p_role roleInf){
		_roleInfo = roleInf;
	}
}
