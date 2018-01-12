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

    private string _textStr = "";
    public string TextStr
    {
        get { return _textStr; }
        set
        {
            _textStr = value;
            if (null != _text)
            {
                _text.text = _textStr;
            }
        }
    }

    private Text _text;

    public void Awake()
    {

        _text = gameObject.GetComponent<Text>();
        _textStr = _text.text;
    }


}
