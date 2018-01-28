using System.Collections.Generic;
using UnityEngine;

public class LocalString
{
    private static Dictionary<string, string> m_localWord;

    /**
     * @brief 本地化文字 解析
     */
    public static void ParseWord(){
        if (m_localWord != null) return;
        Debug.Log("LocalString ParseWord");
        //初始化 列表
        m_localWord = new Dictionary<string, string>();
        //加载资源
        AssetManager.LoadAsset(PathManager.GetLocalStringPath(),AssetLoadCallBack);
    }

    //localstring加载回调
    private static void AssetLoadCallBack(Object target,string path)
    {
        if (null == target)
        {
            return;
        }
        TextAsset txt = target as TextAsset;
        string[] lines = txt.text.Split(";"[0]);
        for (int i = 0; i < lines.Length; i++) {
            string strLine = lines[i];
            if (strLine != "")
            {
                string[] keyValue = strLine.Split(':');
                if (keyValue.Length >= 2)
                {
                    m_localWord[keyValue[0]] = keyValue[1].Replace("\n", ";").Replace("\\t", "\t");
                }
                else
                {
                    m_localWord[keyValue[0]] = "";
                }
            }
        }

        GameStartEvent.GetInstance().dispatchEvent(GAME_LOAD_SETP_EVENT.LOAD_WORD);
    }
    
    /**
     * @brief 获取本地化文字
     */
    public static string GetWord(string key)
    {
        if (m_localWord == null)
        {
            ParseWord();
        }
        if (m_localWord.ContainsKey(key))
        {
            return m_localWord[key];
        }
        return "";
    }

    /**
     * @brief 清空本地化文字记录
     */
    public static void Destroy()
    {
        m_localWord = null;
    }

}
