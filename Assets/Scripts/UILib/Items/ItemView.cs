using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : CellView {
    private ItemInfo _itemInfo;
    private Text _textNum;
	// Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
        _textNum = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
    }
    public ItemInfo itemInfo
    {
        get { return _itemInfo; }
        set
        {
            if (null != _itemInfo && null != value && _itemInfo.templateId == value.templateId)
                return;
            //父类数据更新
            info = value as ICell;
            _itemInfo = value;
            UpdateTextNumView();
        }
    }
    // 刷新显示
    private void UpdateTextNumView()
    {
        if (null != _itemInfo)
        {
            _textNum.text = _itemInfo.getCountStr();
        }
    }

}
