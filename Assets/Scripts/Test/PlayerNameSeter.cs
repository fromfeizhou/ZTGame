using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameSeter : MonoBehaviour {
	public Text txtPlayerName;

	void Update()
	{
		if (string.IsNullOrEmpty (txtPlayerName.text)) {
			txtPlayerName.text = PlayerModule.GetInstance ().RoleName;
		}
	}
}
