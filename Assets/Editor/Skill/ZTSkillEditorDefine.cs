using System.Collections;
using System.Collections.Generic;

public class ZTSkillEditorDefine {
    //行为类型(说明)
    public static string[] TypeDes = { "方向调整", "动作播放", "声音播放", "碰撞", "特效", "位移" };
    //行为类型详细参数(说明)
    public static Dictionary<int, string[]> TypeList = new Dictionary<int, string[]>() {
        { 0 ,new string[]{ "方向" } },
        { 1 ,new string[]{ "动作名" } },
        { 2 ,new string[]{ "声音" } },
        { 3 ,new string[]{ "半径", "运动id", "层次", "偏移x", "偏移y", "目标类型", "存在时间-帧", "碰撞总数", "特效名字" } },
        { 4 ,new string[]{ "特效名字", "层次", "存在时间-帧", "偏移x", "偏移y" } },
        { 5 ,new string[]{ "运动id" } },
    };

    //朝向(说明)
    public static ArrayList FaceType = new ArrayList() { "0", "1"};
    public static string[] FaceTypeNames = { "朝向操作点", "背向操作点"};

    //层次(说明)
    public static ArrayList LayerType = new ArrayList() { "1", "2", "3" };
    public static string[] LayerTypeNames = { "玩家", "场景", "操作点" };

    //目标选择(说明)Self = 0,	--自己
    public static ArrayList TargetType = new ArrayList() { "0", "1", "2", "3","4" };
    public static string[] TargetTypeNames = { "自己", "队友", "敌人", "所有人", "指定目标",};
}
