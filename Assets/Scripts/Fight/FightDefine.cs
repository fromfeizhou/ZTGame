using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightDefine
{
    
    public struct FightOperaEvents
    {
        public static readonly string PLAY_SKILL = "FightOperaEvents_PlaySkill";
    }

    public struct FightOpera
    {
        public static byte STOP = 1;     //停止

        public static byte MOVE_UP = 11;    //移动
        public static byte MOVE_RIGHT = 12;
        public static byte MOVE_DOWN = 13;
        public static byte MOVE_LEFT = 14;
        public static byte MOVE_UP_RIGHT = 15;
        public static byte MOVE_UP_LEFT = 16;
        public static byte MOVE_DOWN_RIGHT = 17;
        public static byte MOVE_DOWN_LEFT = 18;

        public static byte RUN_UP = 21;     //跑步
        public static byte RUN_RIGHT = 22;
        public static byte RUN_DOWN = 23;
        public static byte RUN_LEFT = 24;
        public static byte RUN_UP_RIGHT = 25;
        public static byte RUN_UP_LEFT = 26;
        public static byte RUN_DOWN_RIGHT = 27;
        public static byte RUN_DOWN_LEFT = 28;

        public static byte USE_SKILL = 101;     //使用技能
        public static byte USE_ITEM = 102;      //使用物品
    }
    //玩家操作
    public struct FightOperaData
    {
        public byte Opera;    //操作类型
        public int Frame;   //帧数 
        public int Param1;    //参数1 
    }


    // 获得操作结构
    public FightOperaData GetFightOperaData(byte opera = 0, int frame = 0, int param1 = 0)
    {
        FightOperaData operaData = new FightOperaData();
        operaData.Opera = opera;
        operaData.Frame = frame;
        operaData.Param1 = param1;
        return operaData;
    }


    public enum PLAYERDIR
    {
        NONE = 0,
        UP = 1,
        UP_LEFT = 2,
        LEFT = 3,
        DOWN_LEFT = 4,
        DOWN = 5,
        DOWN_RIGHT = 6,
        RIGHT = 7,
        UP_RIGHT = 8
    }

    public static Vector3 GetDirVec(PLAYERDIR dir){
        switch (dir)
        {
            case PLAYERDIR.UP:
                return Vector3.forward;
            case PLAYERDIR.DOWN:
                return Vector3.back;
            case PLAYERDIR.LEFT:
                return Vector3.left;
            case PLAYERDIR.RIGHT:
                return Vector3.right;
            case PLAYERDIR.UP_LEFT:
                return new Vector3(-1, 0, 1);
            case PLAYERDIR.UP_RIGHT:
                return new Vector3(1, 0, 1);
            case PLAYERDIR.DOWN_LEFT:
                return new Vector3(-1, 0, -1);
            case PLAYERDIR.DOWN_RIGHT:
                return new Vector3(1, 0, -1);
        }
        return Vector3.zero;
    }

    //旋转角度 欧拉角
    public static Quaternion GetDirEuler(PLAYERDIR dir)
    {
        Vector3 pos = Vector3.zero;
        switch (dir)
        {
            case PLAYERDIR.RIGHT:
                pos.x = 1;
                pos.z = 0;
                break;
            case PLAYERDIR.UP_RIGHT:
                pos.x = 1;
                pos.z = 1;
                break;
            case PLAYERDIR.UP:
                pos.x = 0;
                pos.z = 1;
                break;
            case PLAYERDIR.UP_LEFT:
                pos.x = -1;
                pos.z = 1;
                break;
            case PLAYERDIR.LEFT:
                pos.x = -1;
                pos.z = 0;
                break;
            case PLAYERDIR.DOWN_LEFT:
                pos.x = -1;
                pos.z = -1;
                break;
            case PLAYERDIR.DOWN:
                pos.x = 0;
                pos.z = -1;
                break;
            case PLAYERDIR.DOWN_RIGHT:
                pos.x = 1;
                pos.z = -1;
                break;
        }
        float angle = Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg;
        return Quaternion.Euler(Vector3.up * angle);
    }

}
