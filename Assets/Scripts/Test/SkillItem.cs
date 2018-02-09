using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour {
	public Image SkillImage;
	public int SkillId;

	public GameObject light;
	void Start()
	{
		if (SkillId == 1001)
			SetSkill ();
		
		transform.Find("BtnSkill").GetComponent<Button> ().onClick.AddListener (()=>{
			SetSkill();
		});
	}

	private void SetSkill()
	{
		SkillImage.sprite = transform.Find("BtnSkill").GetComponent<Image>().sprite;
		FightModule.GetInstance().SelectSkill(SkillId);
	}

	void Update()
	{
		light.SetActive (FightModule.GetInstance().CurSkillId == SkillId);
	}
}
