using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MTextFormat : MonoBehaviour
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

    public void Start()
    {
        //gameObject.GetComponent<Text>().text = LocalString.GetWord(textKey);
    }
}
