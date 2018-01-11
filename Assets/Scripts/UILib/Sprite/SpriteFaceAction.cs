using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class SpriteFaceAction : MonoBehaviour
{

    SpriteInforGroup _infoGroup;
    public int m_index = 0;
    public string m_actoin = "play";

    private float fTime = 0.0f;
    private float tickTime = 1f;
    void Start()
    {
        _infoGroup = SpriteFaceCache.GetAsset(m_index, m_actoin);
        tickTime = SpriteFaceCache.GetAsset(m_index).tickTime;
    }
    // Update is called once per frame
    void Update()
    {
        fTime += Time.deltaTime;

        if (fTime >= tickTime)
        {
            GetComponent<Image>().sprite = _infoGroup.curSprteInfo.sprite;
            _infoGroup.run();
            fTime = 0.0f;
        }
    }

    //设置动画 动作
    public void setIndexAction(int index,string action)
    {
        if (m_index == index && m_actoin == action)
        {
            return;
        }
        m_index = index;
        m_actoin = action;
        _infoGroup = SpriteFaceCache.GetAsset(m_index, m_actoin);
        tickTime = SpriteFaceCache.GetAsset(m_index).tickTime;
    }
}
