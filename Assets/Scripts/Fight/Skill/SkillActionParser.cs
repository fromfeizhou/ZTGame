using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技能解析器
/// </summary>
public class SkillActionParser
{
    public PlayerBase SkillPlayer;     //使用技能者
    public SkillOpera Operate;      //操作
    public bool IsComplete = false;
    private Dictionary<int, List<SkillActionBase>> _skillActionDic;     //技能action列表

    public SkillActionParser(PlayerBase playerBase, SkillOpera opera)
    {
        SkillPlayer = playerBase;
        Operate = opera;
        _skillActionDic = new Dictionary<int, List<SkillActionBase>>();
        IsComplete = false;
        ParseSkillAction();
    }

    //解析配置
    private void ParseSkillAction()
    {
        AssetManager.LoadAsset(PathManager.GetResPathByName("SkillAsset", Operate.ActionId.ToString() + ".asset"), LoadAssetHandler);
    }

    private void LoadAssetHandler(Object target, string path)
    {
        SkillAsset skillAsset = target as SkillAsset;
        if (null != skillAsset)
        {
            for (int i = 0; i < skillAsset.ListSkillGroup.Count; i++)
            {
                for (int k = 0; k < skillAsset.ListSkillGroup[i].ListSkillInfo.Count; k++)
                {
                    int frame = Operate.StartFrame + skillAsset.ListSkillGroup[i].FrameTime;
                    if (skillAsset.ListSkillGroup[i].FrameTime < 0)
                    {
                        frame = skillAsset.ListSkillGroup[i].FrameTime;
                    }
                    if (!_skillActionDic.ContainsKey(frame))
                    {
                        _skillActionDic.Add(frame, new List<SkillActionBase>());
                    }
                    _skillActionDic[frame].Add(GetSkillActionBase(frame, skillAsset.ListSkillGroup[i].ListSkillInfo[k]));
                }
            }
        }

    }

    //对应actor类型 创建
    private SkillActionBase GetSkillActionBase(int frame, SkillAssetInfo skillInfo)
    {
        switch (skillInfo.actionType)
        {
            case SkillDefine.SkillActionType.PLAY_ANIM:
                return new SAPlayAnim(skillInfo.animName, this, frame);
            case SkillDefine.SkillActionType.PLAY_MOVE:
                MoveInfo moveInfo = new MoveInfo(skillInfo.moveType, 0, skillInfo.frameCount, skillInfo.speedX);
                return new SAPlayerMove(moveInfo, this, frame);
            case SkillDefine.SkillActionType.COLLIDER:
                CollBase collider = GetOperaColliderInfo(skillInfo);
                ColliderData data = new ColliderData(collider, skillInfo.interval, skillInfo.colliderTime, skillInfo.colliderMax, skillInfo.isPenetrate, skillInfo.collEffect, skillInfo.colliderActions);
                return new SACollider(skillInfo.colliderTarget, data, this, frame);

            case SkillDefine.SkillActionType.COLLIDER_MOVE:
                CollBase colliderMove = GetOperaColliderInfo(skillInfo);
                moveInfo = new MoveInfo(skillInfo.moveType, 0, skillInfo.frameCount, skillInfo.speedX);
                data = new ColliderData(colliderMove, skillInfo.interval, skillInfo.colliderTime, skillInfo.colliderMax, skillInfo.isPenetrate, skillInfo.collEffect, skillInfo.colliderActions);
                return new SAColliderMove(moveInfo, skillInfo.colliderTarget, data, this, frame);
            case SkillDefine.SkillActionType.ADD_EFFECT:
                return new SAEffect(skillInfo.effectInfo, this, frame);

        }
        return new SkillActionBase(this, frame);
    }

    private CollBase GetOperaColliderInfo(SkillAssetInfo skillInfo)
    {
        CollBase collider = null;
        float startX = SkillPlayer.PlayerPos.x + skillInfo.csX;
        float startZ = SkillPlayer.PlayerPos.z + skillInfo.csZ;
        float angle = skillInfo.csA;
        if (skillInfo.collPosType == CollBase.PosType.SKILL || skillInfo.collPosType == CollBase.PosType.SKILL_ROTATE)
        {
            startX = Operate.TargetPos.x + skillInfo.csX;
            startZ = Operate.TargetPos.z + skillInfo.csZ;
            angle = skillInfo.csA;
            //附加操作旋转
            if (skillInfo.collPosType == CollBase.PosType.SKILL_ROTATE)
            {
                // 方向盘角度 与场景角度差异修正 修正符号(场景y轴 逆时针旋转 与方向盘顺时针旋转 一致)
                angle = Mathf.Atan2(-Operate.SkillDir.z, Operate.SkillDir.x) * Mathf.Rad2Deg + skillInfo.csA;
            }
        }
        switch (skillInfo.colliderType)
        {
            case CollBase.ColType.RECTANGLE:
                collider = new CollRectange(startX, startZ, angle, skillInfo.width, skillInfo.height);
                break;
            case CollBase.ColType.SECTOR:
                collider = new CollSector(startX, startZ, angle, skillInfo.inCircle, skillInfo.outCircle, skillInfo.cAngle);
                break;
            default:
                collider = new CollRadius(startX, startZ, angle, skillInfo.radius);
                break;
        }

        return collider;
    }


    public void UpdateAction()
    {
        bool allIsDone = true;
        if (null != _skillActionDic)
        {

            int curFrame = ZTSceneManager.GetInstance().SceneFrame;
            //帧率溢出 补足
            if (curFrame < Operate.StartFrame)
            {
                curFrame += int.MaxValue - Operate.StartFrame;
            }
            foreach (int key in _skillActionDic.Keys)
            {
                for (int i = _skillActionDic[key].Count - 1; i >= 0; i--)
                {
                    SkillActionBase skillActoin = _skillActionDic[key][i];
                    if (!skillActoin.IsStart && curFrame >= skillActoin.ActFrame)
                    {
                        skillActoin.IsStart = true;
                    }
                    if (skillActoin.IsStart && !skillActoin.IsComplete)
                    {
                        skillActoin.UpdateActoin(curFrame);
                    }

                    if (skillActoin.ActFrame > 0 && !skillActoin.IsComplete)
                    {
                        allIsDone = false;
                    }
                }
            }

            if (allIsDone)
            {
                Destroy();
            }

        }
    }


    //激活行为
    public void ActionActivatebyId(int id)
    {
        if (_skillActionDic.ContainsKey(id))
        {
            for (int i = _skillActionDic[id].Count - 1; i >= 0; i--)
            {
                SkillActionBase skillActoin = _skillActionDic[id][i];

                skillActoin.IsStart = true;
                skillActoin.ActFrame = ZTSceneManager.GetInstance().SceneFrame;

            }
        }

    }

    public void Destroy()
    {
        if (null != _skillActionDic)
        {
            _skillActionDic = null;
        }
        IsComplete = true;
        GameTool.Log("SkillActionParser Destory");
    }
}
