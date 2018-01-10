using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SkillAsset))]
public class SkillAssetEditor : Editor
{
    private SkillAsset skillAsset;
    private Vector2 ve2ScorllView;

    //当前展开的标签索引
    private int showIndex;

    //添加的标签的名称
    private int addTagFrame;

    private int index;
    private SkillDefine.SkillActionType skillActionType;
    public void OnEnable()
    {
        skillAsset = (SkillAsset)target;
        addTagFrame = 0;
        Init();
    }

    public void OnDisable()
    {

    }

    public override void OnInspectorGUI()
    {

        ve2ScorllView = GUILayout.BeginScrollView(ve2ScorllView);

        #region 标题栏
        GUILayout.BeginVertical("HelpBox");
        GUILayout.BeginHorizontal();
        GUILayout.Label("创建触发帧：", GUILayout.Width(100));
        addTagFrame = EditorGUILayout.IntField(addTagFrame, GUILayout.Width(50));
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Tag", GUILayout.Width(100), GUILayout.Height(30)))
        {
            SkillAssetInforGroup spriteInforGroup = skillAsset.ListSkillGroup.Find(
                delegate(SkillAssetInforGroup sig)
                {
                    return sig.FrameTime == addTagFrame;
                });

            if (spriteInforGroup != null)
            {
                Debug.Log("该标签已存在！");
            }
            else
            {
                AddTagSure(addTagFrame);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        #endregion

        for (int i = 0; i < skillAsset.ListSkillGroup.Count; i++)
        {
            GUILayout.BeginHorizontal("HelpBox");
            #region 展开与收缩按钮
            if (GUILayout.Button("触发帧:  ", showIndex == i ? "OL Minus" : "OL Plus"))
            {
                if (showIndex == i)
                {
                    showIndex = -1;
                }
                else
                {
                    showIndex = i;
                }
            }
            #endregion

            skillAsset.ListSkillGroup[i].FrameTime = EditorGUILayout.IntField(skillAsset.ListSkillGroup[i].FrameTime, GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("AddSKillInfo", GUILayout.Width(100)))
            {
                AddSubTagSure(i);
            }
            if (GUILayout.Button("Clear Tag", GUILayout.Width(100)))
            {
                ClearTagByIndex(i);
            }
            GUILayout.EndHorizontal();

            #region 展开的Skill组，显示所有Skill属性
            if (showIndex == i)
            {

                //index = EditorGUILayout.Popup(index, new string[]{"1","2","3"});
                for (int j = 0; j < skillAsset.ListSkillGroup[i].ListSkillInfo.Count; j++)
                {
                    GUILayout.BeginHorizontal("SkillAsset" + j, "window");
                    //skillAsset.listSkillGroup[i].listSkillInfor[j].sprite = EditorGUILayout.ObjectField("", skillAsset.listSkillGroup[i].listSkillInfor[j].sprite, typeof(Skill), false, GUILayout.Width(80)) as Skill;

                    //GUILayout.FlexibleSpace();
                    SkillAssetInfo skillInfo = skillAsset.ListSkillGroup[i].ListSkillInfo[j];

                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("SkillActionType:", GUILayout.Width(100));
                    skillInfo.actionType = (SkillDefine.SkillActionType)EditorGUILayout.EnumPopup("", skillInfo.actionType);

                    if (GUILayout.Button("Clear Action", GUILayout.Width(100)))
                    {
                        ClearSubTagByIndex(i, j);
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    switch (skillInfo.actionType)
                    {
                        case SkillDefine.SkillActionType.PLAY_ANIM:
                            UpdatePlayerAnimItem(skillInfo);
                            break;
                        case SkillDefine.SkillActionType.PLAY_MOVE:
                            UpdatePlayerMoveItem(skillInfo);
                            break;
                        case SkillDefine.SkillActionType.COLLIDER:
                            UpdateColliderItem(skillInfo);
                            break;
                        case SkillDefine.SkillActionType.COLLIDER_MOVE:
                            UpdateColliderItem(skillInfo);
                            UpdatePlayerMoveItem(skillInfo);
                            GUILayout.BeginVertical("HelpBox");
                            if (GUILayout.Button("AddEffectInfo", GUILayout.Width(100)))
                            {
                                EffectInfo effectInfo = new EffectInfo();
                                skillInfo.collMoveEffList.Add(effectInfo);
                                EditorUtility.SetDirty(skillAsset);
                            }

                            for (int effId = 0; effId < skillInfo.collMoveEffList.Count;effId++)
                            {
                                GUILayout.BeginVertical("HelpBox");
                                UpdateEffectItem(skillInfo.collMoveEffList[i]);
                                if (GUILayout.Button("Clear Effect", GUILayout.Width(100)))
                                {
                                    skillInfo.collMoveEffList.RemoveAt(effId);
                                    EditorUtility.SetDirty(skillAsset);
                                }
                                GUILayout.EndVertical();
                            }
                            GUILayout.EndVertical();
                            break;
                        case SkillDefine.SkillActionType.ADD_EFFECT:
                            UpdateEffectItem(skillInfo.effectInfo);
                            break;
                    }
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }
            }
            #endregion
        }

        GUILayout.EndScrollView();
        //unity
        EditorUtility.SetDirty(skillAsset);
    }

    private void Init()
    {
        showIndex = -1;
    }


    /// <summary>
    /// 新增标签
    /// </summary>
    private void AddTagSure(int frame)
    {
        SkillAssetInforGroup sig = new SkillAssetInforGroup();
        sig.FrameTime = frame;
        sig.ListSkillInfo = new List<SkillAssetInfo>();

        skillAsset.ListSkillGroup.Add(sig);

        Init();

        EditorUtility.SetDirty(skillAsset);
    }

    private void ClearTagByIndex(int i)
    {
        skillAsset.ListSkillGroup.RemoveAt(i);
        Init();
    }


    private void AddSubTagSure(int index)
    {
        SkillAssetInfo skillInfo = new SkillAssetInfo();
        skillAsset.ListSkillGroup[index].ListSkillInfo.Add(skillInfo);
        EditorUtility.SetDirty(skillAsset);
    }




    private void ClearSubTagByIndex(int i, int j)
    {
        skillAsset.ListSkillGroup[i].ListSkillInfo.RemoveAt(j);
    }

    //播放动作
    private void UpdatePlayerAnimItem(SkillAssetInfo skillInfo)
    {

        GUILayout.BeginHorizontal();
        GUILayout.Label("AnimaName:", GUILayout.Width(100));
        skillInfo.animName = EditorGUILayout.TextField(skillInfo.animName);
        GUILayout.EndHorizontal();

    }

    //移动位置
    private void UpdatePlayerMoveItem(SkillAssetInfo skillInfo)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("MoveType:", GUILayout.Width(100));
        skillInfo.moveType = (SkillDefine.MoveType)EditorGUILayout.EnumPopup("", skillInfo.moveType);
        GUILayout.EndHorizontal();
        switch (skillInfo.moveType)
        {
            case SkillDefine.MoveType.LINE:
                GUILayout.BeginHorizontal();
                GUILayout.Label("FrameCount:", GUILayout.Width(100));
                skillInfo.frameCount = EditorGUILayout.IntField(skillInfo.frameCount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Speed:", GUILayout.Width(100));
                skillInfo.speedX = EditorGUILayout.FloatField(skillInfo.speedX);
                GUILayout.EndHorizontal();
                break;
            case SkillDefine.MoveType.LINE_TARGET:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Speed:", GUILayout.Width(100));
                skillInfo.speedX = EditorGUILayout.FloatField(skillInfo.speedX);
                GUILayout.EndHorizontal();
                break;
            case SkillDefine.MoveType.ROTATE:
                GUILayout.BeginHorizontal();
                GUILayout.Label("FrameCount:", GUILayout.Width(100));
                skillInfo.frameCount = EditorGUILayout.IntField(skillInfo.frameCount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("StartAngle:", GUILayout.Width(100));
                skillInfo.angle = EditorGUILayout.FloatField(skillInfo.angle);

                GUILayout.Label("Rotate:", GUILayout.Width(100));
                skillInfo.rotate = EditorGUILayout.FloatField(skillInfo.rotate);
                GUILayout.EndHorizontal();
                break;
        }
        GUILayout.EndVertical();

    }

    private void UpdateColliderItem(SkillAssetInfo skillInfo)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("PosType:", GUILayout.Width(100));
        skillInfo.collPosType = (CollBase.PosType)EditorGUILayout.EnumPopup("", skillInfo.collPosType);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ColliderType:", GUILayout.Width(100));
        skillInfo.colliderType = (CollBase.ColType)EditorGUILayout.EnumPopup("", skillInfo.colliderType);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("TargetType:", GUILayout.Width(100));
        skillInfo.colliderTarget = (SkillDefine.ColliderTarget)EditorGUILayout.EnumPopup("", skillInfo.colliderTarget);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("startX:", GUILayout.Width(50));
        skillInfo.csX = EditorGUILayout.FloatField(skillInfo.csX);
        GUILayout.Label("startY:", GUILayout.Width(50));
        skillInfo.csZ = EditorGUILayout.FloatField(skillInfo.csZ);
        GUILayout.Label("startAngle:", GUILayout.Width(100));
        skillInfo.csA = EditorGUILayout.FloatField(skillInfo.csA);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Interval:", GUILayout.Width(100));
        skillInfo.interval = EditorGUILayout.IntField(skillInfo.interval);
        GUILayout.Label("ColliderMax:", GUILayout.Width(100));
        skillInfo.colliderMax = EditorGUILayout.IntField(skillInfo.colliderMax);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("LifeTime:", GUILayout.Width(100));
        skillInfo.colliderTime = EditorGUILayout.IntField(skillInfo.colliderTime);
        GUILayout.FlexibleSpace();
        skillInfo.isPenetrate = EditorGUILayout.Toggle("IsPenetrate:", skillInfo.isPenetrate);
        GUILayout.EndHorizontal();

        switch (skillInfo.colliderType)
        {
            case CollBase.ColType.CIRCLE:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Radius:", GUILayout.Width(100));
                skillInfo.radius = EditorGUILayout.FloatField(skillInfo.radius);
                GUILayout.EndHorizontal();
                break;
            case CollBase.ColType.RECTANGLE:
                GUILayout.BeginHorizontal();

                GUILayout.Label("Width:", GUILayout.Width(100));
                skillInfo.width = EditorGUILayout.FloatField(skillInfo.width);

                GUILayout.Label("Heigh:", GUILayout.Width(100));
                skillInfo.height = EditorGUILayout.FloatField(skillInfo.height);
                GUILayout.EndHorizontal();
                break;
            case CollBase.ColType.SECTOR:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Angle:", GUILayout.Width(40));
                skillInfo.cAngle = EditorGUILayout.FloatField(skillInfo.cAngle);

                GUILayout.Label("InCircle:", GUILayout.Width(50));
                skillInfo.inCircle = EditorGUILayout.FloatField(skillInfo.inCircle);

                GUILayout.Label("OutCircle:", GUILayout.Width(50));
                skillInfo.outCircle = EditorGUILayout.FloatField(skillInfo.outCircle);
                GUILayout.EndHorizontal();
                break;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("ColliderActions:", GUILayout.Width(100));
        skillInfo.colliderActions = EditorGUILayout.TextField(skillInfo.colliderActions);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void UpdateEffectItem(EffectInfo effectInfo){
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("EffectId:", GUILayout.Width(100));
        effectInfo.Id = EditorGUILayout.IntField( effectInfo.Id);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Scale:", GUILayout.Width(100));
        effectInfo.Scale = EditorGUILayout.FloatField(effectInfo.Scale);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Offset:", GUILayout.Width(100));
        effectInfo.Offset.x = EditorGUILayout.FloatField(effectInfo.Offset.x);
        effectInfo.Offset.y = EditorGUILayout.FloatField(effectInfo.Offset.y);
        effectInfo.Offset.z = EditorGUILayout.FloatField(effectInfo.Offset.z);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}