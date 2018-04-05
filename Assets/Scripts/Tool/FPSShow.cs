using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class FPSShow : MonoBehaviour
{
    private float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;  

    private float m_UpdateShowDeltaTime = 0.1f;//更新帧率的时间间隔;  

    private int m_FrameUpdate = 0;//帧数;  

    private float m_FPS = 0;

    void Awake()
    {
        //Application.targetFrameRate = 30;
    }

    // Use this for initialization  
    void Start()
    {
        transform.SetSiblingIndex(10000);
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame  
    void Update()
    {
        m_FrameUpdate++;
        if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
        {
            m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
            m_FrameUpdate = 0;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;

            GetComponent<Text>().text = "FPS: " + Mathf.Floor(m_FPS);
        }
    }
}
