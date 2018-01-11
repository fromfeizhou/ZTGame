using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : CellInfo
{
    /**数量**/
    private int _count = 0;
    public int count
    {
        set { _count = value; }
        get { return _count; }
    }

    // Use this for initialization
    public ItemInfo(int id = 10001,int count = 0):base(id)
    {
        _count = count;
    }

    public string getCountStr()
    {
        return _count > 0 ? _count.ToString() : "";
    }

    public override string iconPath
    {
        get { return PathManager.GetResPathByName("ImgShopItem", templateId + ".png"); }
    }
}
