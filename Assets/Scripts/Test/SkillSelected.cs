using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelected : MonoBehaviour
{
    public static int SelectIndex = 1001;

    private GameObject _btnCut;
    private GameObject _btnAdd;

    private MTextFormat _textFormat;
    private int _startIndex;
    private int _maxIndex;
    void Start()
    {
        _startIndex = 1001;
        _maxIndex = 3001;
        SelectIndex = 3001;

        _btnCut = GameObject.Find("BtnCut");
        _btnCut.GetComponent<MBaseBtnFormat>().OnBtnClick.AddListener(Click);

        _btnAdd = GameObject.Find("BtnAdd");
        _btnAdd.GetComponent<MBaseBtnFormat>().OnBtnClick.AddListener(Click2);

        _textFormat = GameObject.Find("MText").GetComponent<MTextFormat>();
        _textFormat.TextStr = SelectIndex.ToString();
    }

    private void Click(string val)
    {
        int index = int.Parse(_textFormat.TextStr);
        index -= 1000;
        index = index < _startIndex ? _maxIndex : index;
        SelectIndex = index;
        _textFormat.TextStr = index.ToString();
    }

    private void Click2(string val)
    {
        int index = int.Parse(_textFormat.TextStr);
        index += 1000;
        index = index > _maxIndex ? _startIndex : index;
        SelectIndex = index;
        _textFormat.TextStr = index.ToString();
    }
}
