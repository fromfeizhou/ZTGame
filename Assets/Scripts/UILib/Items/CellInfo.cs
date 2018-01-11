using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellInfo:ICell
{
    public CellInfo(int id = 10001) {
        _templateId = id;
    }
    /**模板id**/
    private int _templateId = 0;
    public int templateId
    {
        set { _templateId = value; }
        get { return _templateId; }
    }

    /**资源路径**/
    public virtual string iconPath
    {
        get { return PathManager.GetResPathByName("ImgShopItem", templateId + ".png"); }
    }
}
