using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技能解析器
/// </summary>
public class SkillActionParser
{
    public ICharaBattle SkillPlayer;     //使用技能者
    public SkillCommand Command;      //操作
    public bool IsComplete = false;
    private Dictionary<int, List<SkillActionBase>> _skillActionDic;     //技能action列表

    public SkillActionParser(ICharaBattle playerBase, SkillCommand command)
    {
        SkillPlayer = playerBase;
        Command = command;
        _skillActionDic = new Dictionary<int, List<SkillActionBase>>();
        IsComplete = false;
        ParseSkillAction();
    }

    //解析配置(技能配置 必需预加载好，立即返回，当前创建帧执行 第一帧)
    private void ParseSkillAction()
    {
        AssetManager.LoadAsset(PathManager.GetResPathByName("SkillAsset", Command.SkillId.ToString() + ".asset"), LoadAssetHandler);
    }

    private void LoadAssetHandler(Object target, string path)
    {
        SkillAsset gameObject = target as SkillAsset;
        if (null == gameObject) return;
        SkillAsset skillAsset = GameObject.Instantiate(gameObject);
        if (null != skillAsset)
        {
            for (int i = 0; i < skillAsset.ListSkillGroup.Count; i++)
            {
                for (int k = 0; k < skillAsset.ListSkillGroup[i].ListSkillInfo.Count; k++)
                {
                    int frame = Command.StartFrame + skillAsset.ListSkillGroup[i].FrameTime;
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
            case SkillDefine.SkillActionType.PLAY_CONTROL:
                return new SAPlayerControl(skillInfo.isCtrl, this, frame);
            case SkillDefine.SkillActionType.PLAY_ANIM:
                return new SAPlayAnim(skillInfo.animName, this, frame);
            case SkillDefine.SkillActionType.PLAY_MOVE:
                MoveInfo moveInfo = skillInfo.moveInfo;
                return new SAPlayerMove(moveInfo, this, frame);
            case SkillDefine.SkillActionType.COLLIDER:
                CollBase collider = GetOperaColliderInfo(skillInfo.colliderInfo);
                return new SACollider(collider, skillInfo.colliderInfo, this, frame);
            case SkillDefine.SkillActionType.COLLIDER_MOVE:
                CollBase colliderMove = GetOperaColliderInfo(skillInfo.colliderInfo);
                moveInfo = skillInfo.moveInfo;
                return new SAColliderMove(moveInfo, colliderMove, skillInfo.colliderInfo, this, frame);
            case SkillDefine.SkillActionType.ADD_EFFECT:
                return new SAEffect(skillInfo.effectInfo, this, frame);

        }
        return new SkillActionBase(this, frame);
    }

    private CollBase GetOperaColliderInfo(ColliderInfo colliderInfo)
    {
        CollBase collider = null;
        float startX = SkillPlayer.MovePos.x + colliderInfo.StartX;
        float startZ = SkillPlayer.MovePos.z + colliderInfo.StartZ;
        float angle = colliderInfo.StartAngle;
        if (colliderInfo.CollPosType == CollBase.PosType.SKILL || colliderInfo.CollPosType == CollBase.PosType.SKILL_ROTATE)
        {
            startX += Command.TargetPos.x;
            startZ += Command.TargetPos.z;
            //附加操作旋转
            if (colliderInfo.CollPosType == CollBase.PosType.SKILL_ROTATE)
            {
                // 方向盘角度 与场景角度差异修正 修正符号(场景y轴 逆时针旋转 与方向盘顺时针旋转 一致)
                angle = Mathf.Atan2(-Command.SkillDir.z, Command.SkillDir.x) * Mathf.Rad2Deg + colliderInfo.StartAngle;
            }
        }
        switch (colliderInfo.ColliderType)
        {
            case CollBase.ColType.RECTANGLE:
                collider = new CollRectange(startX, startZ, angle, colliderInfo.Width, colliderInfo.Height);
                break;
            case CollBase.ColType.SECTOR:
                collider = new CollSector(startX, startZ, angle, colliderInfo.InCircle, colliderInfo.OutCircle, colliderInfo.Angle);
                break;
            default:
                collider = new CollRadius(startX, startZ, angle, colliderInfo.Radius);
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
            if (curFrame < Command.StartFrame)
            {
                curFrame += int.MaxValue - Command.StartFrame;
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
