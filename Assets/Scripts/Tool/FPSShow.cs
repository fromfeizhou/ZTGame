using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSShow : MonoBehaviour
{
   
    private float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;  

    private float m_UpdateShowDeltaTime = 0.1f;//更新帧率的时间间隔;  

    private int m_FrameUpdate = 0;//帧数;  

    private float m_FPS = 0;

	private Ping ping;

	private GUIStyle fontStyle = new GUIStyle();
    void Awake()
    {
        //Application.targetFrameRate = 30;
    }

    // Use this for initialization  
    void Start()
    {
		fontStyle.normal.background = null;    //这是设置背景填充的
		fontStyle.normal.textColor = Color.white;
		fontStyle.fontSize = 40;       //当然，这是字体颜色

		transform.SetSiblingIndex(10000);
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
        SendPing();
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
        }

		if (pingIp == string.Empty || ping == null)
			return;

		if (null != ping && ping.isDone)
		{
			delayTime = ping.time;
			ping.DestroyPing();
			ping = null;
			Invoke("SendPing", 1.0F);//每秒Ping一次
		}
    }

    public static string pingIp = string.Empty;
    float delayTime;
    void OnGUI()
    {
		GUILayout.Label ("ping: " + delayTime + "ms",fontStyle);
		GUILayout.Label ("fps:" + Mathf.Floor (m_FPS),fontStyle);
    }

    public void SendPing()
    {
        if (pingIp == string.Empty)
            return;

        if (ping != null)
        {
            ping.DestroyPing();
            ping = null;
        }
        ping = new Ping(pingIp);
    }
}
