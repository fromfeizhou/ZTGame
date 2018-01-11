using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MTextBtnFormat : MBaseBtnFormat
{
    /**
     * oColor 描边颜色
     * oSize 描边大小
     */
    [HideInInspector]
    public string textKey = "";
    //文本预定义style
    [HideInInspector]
    public string style = "";

    public override void Start()
    {
        base.Start();
        gameObject.GetComponentInChildren<Text>().text = LocalString.GetWord(textKey);
    }
	
}
