using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameDefine{
    //是否点击在ui上
    public static bool IsPointerOverUIObject(Vector2 screenPosition)
    {
        //判断是否点击的是UI，有效应对安卓没有反应的情况，true为UI  
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //是否已经释放了
    public static bool IsDestroyed(GameObject gameObject)
    {
        return gameObject == null && !ReferenceEquals(gameObject, null);
    }
    
}
/// <summary>
/// 加载事件
/// </summary>
public struct GAME_LOAD_SETP_EVENT
{
    public static string LOAD_COM = "GAME_LOAD_SETP_EVENT_LOAD_COM";
    public static string LOAD_PATH = "GAME_LOAD_SETP_EVENT_LOAD_PATH";
    public static string LOAD_WORD = "GAME_LOAD_SETP_EVENT_LOAD_WORD";
    public static string LOAD_FACE_ASSET = "GAME_LOAD_SETP_EVENT_LOAD_FACE_ASSET";
}

/// <summary>
/// 控制事件
/// </summary>
public struct GAME_TOUCH_EVENT
{
    public static string JOY_MOVE = "GAME_TOUCH_EVENT_JOY_MOVE";       //摇杆移动
}

/// <summary>
/// 控制事件
/// </summary>
public struct SCENE_EVENT
{
    public static readonly string ADD_PLAYER = "SCENE_EVENT_ADD_PLAYER";       //添加玩家
    public static readonly string ADD_COMMAND = "SCENE_EVENT_ADD_COMMAND";     //命令刷新
    public static readonly string ADD_SKILL_PARSER = "SCENE_EVENT_ADD_SKILL_PARSER";  //添加技能解析 
    public static readonly string ADD_UI_HURT_VALUE = "SCENE_EVENT_ADD_UI_HURT_VALUE";  //ui数字
    public static readonly string UPDATE_GRASS_ID = "SCENE_EVENT_UPDATE_GRASS_ID";  //ui数字
}
