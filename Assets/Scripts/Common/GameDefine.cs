using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDefine{

    
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
public struct ScenePlayerEvents
{
    public static string ADD_PLAYER = "GameSceneEvent_AddPlayer";       //添加玩家
}
