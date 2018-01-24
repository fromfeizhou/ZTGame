using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameDefine{

    public static bool IsPointerOverUIObject(Vector2 screenPosition)
    {
        //判断是否点击的是UI，有效应对安卓没有反应的情况，true为UI  
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }  
    
}
/// <summary>
/// 加载事件
/// </summary>
public struct GameLoadStepEvents
{
    public static string LOAD_COM = "GameLoadStepEvents_LoadCom";
    public static string LOAD_PATH = "GameLoadStepEvents_LoadPath";
    public static string LOAD_WORD = "GameLoadStepEventsp_LoadWord";
    public static string LOAD_FACE_ASSET = "GameLoadStepEvents_LoadFaceAsset";
}

/// <summary>
/// 控制事件
/// </summary>
public struct GameTouchEvents
{
    public static string JOY_MOVE = "GameTouchEvents_JoyMove";       //摇杆移动
}

/// <summary>
/// 控制事件
/// </summary>
public struct SceneEvents
{
    public static readonly string ADD_PLAYER = "SceneEvents_AddPlayer";       //添加玩家
    public static readonly string ADD_COMMAND = "SceneEvents_AddCommand";     //命令刷新
    public static readonly string ADD_SKILL_PARSER = "SceneEvents_AddSkillParser";  //添加技能解析 
}
