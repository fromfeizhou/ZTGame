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

    public void OnEnable()
    {
        skillAsset = (SkillAsset)target;
        skillAsset.ListSkillGroup.Sort((x, y) => { return x.FrameTime < y.FrameTime ?-1:1; });
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
                        case SkillDefine.SkillActionType.PLAY_CONTROL:
                            UpdatePlayerControl(skillInfo);
                            break;
                        case SkillDefine.SkillActionType.PLAY_ANIM:
                            UpdatePlayerAnimItem(skillInfo);
                            break;
                        case SkillDefine.SkillActionType.PLAY_MOVE:
                            UpdatePlayerMoveItem(skillInfo.moveInfo);
                            break;
                        case SkillDefine.SkillActionType.COLLIDER:
                            UpdateColliderItem(skillInfo.colliderInfo);
                            break;
                        case SkillDefine.SkillActionType.COLLIDER_MOVE:
                            UpdateColliderItem(skillInfo.colliderInfo);
                            UpdatePlayerMoveItem(skillInfo.moveInfo);
                            break;
                        case SkillDefine.SkillActionType.ADD_EFFECT:
                            UpdateEffectItem(skillInfo.effectInfo);
                            break;
                        case SkillDefine.SkillActionType.FIGHT_EFFECT:
                            UpdateFightEffect(skillInfo.fightEffects);
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
    private void UpdatePlayerControl(SkillAssetInfo skillInfo)
    {

        GUILayout.BeginHorizontal();
        skillInfo.isCtrl = EditorGUILayout.Toggle("IsControl", skillInfo.isCtrl);
        GUILayout.EndHorizontal();

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
    private void UpdatePlayerMoveItem(MoveInfo moveInfo)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("MoveType:", GUILayout.Width(100));
        moveInfo.MoveType = (SkillDefine.MoveType)EditorGUILayout.EnumPopup("", moveInfo.MoveType);
        GUILayout.EndHorizontal();
        switch (moveInfo.MoveType)
        {
            case SkillDefine.MoveType.LINE:
                GUILayout.BeginHorizontal();
                GUILayout.Label("FrameCount:", GUILayout.Width(100));
                moveInfo.FrameCount = EditorGUILayout.IntField(moveInfo.FrameCount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Speed:", GUILayout.Width(100));
                moveInfo.SpeedX = EditorGUILayout.FloatField(moveInfo.SpeedX);
                GUILayout.EndHorizontal();
                break;
            case SkillDefine.MoveType.LINE_TARGET:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Speed:", GUILayout.Width(100));
                moveInfo.SpeedX = EditorGUILayout.FloatField(moveInfo.SpeedX);
                GUILayout.EndHorizontal();
                break;
            case SkillDefine.MoveType.ROTATE:
                GUILayout.BeginHorizontal();
                GUILayout.Label("FrameCount:", GUILayout.Width(100));
                moveInfo.FrameCount = EditorGUILayout.IntField(moveInfo.FrameCount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("StartAngle:", GUILayout.Width(100));
                moveInfo.Angle = EditorGUILayout.FloatField(moveInfo.Angle);

                GUILayout.Label("Rotate:", GUILayout.Width(100));
                moveInfo.Rotate = EditorGUILayout.FloatField(moveInfo.Rotate);
                GUILayout.EndHorizontal();
                break;
        }
        GUILayout.EndVertical();

    }

    private void UpdateColliderItem(ColliderInfo colliderInfo)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("PosType:", GUILayout.Width(100));
        colliderInfo.CollPosType = (CollBase.PosType)EditorGUILayout.EnumPopup("", colliderInfo.CollPosType);
        GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("EffectId:", GUILayout.Width(100));
        //colliderInfo.EffectId = EditorGUILayout.TextField("", colliderInfo.EffectId);
        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ColliderType:", GUILayout.Width(100));
        colliderInfo.ColliderType = (CollBase.ColType)EditorGUILayout.EnumPopup("", colliderInfo.ColliderType);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("TargetType:", GUILayout.Width(100));
        colliderInfo.ColliderTarget = (SkillDefine.ColliderTarget)EditorGUILayout.EnumPopup("", colliderInfo.ColliderTarget);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("startX:", GUILayout.Width(50));
        colliderInfo.StartX = EditorGUILayout.FloatField(colliderInfo.StartX);
        GUILayout.Label("startY:", GUILayout.Width(50));
        colliderInfo.StartZ = EditorGUILayout.FloatField(colliderInfo.StartZ);
        GUILayout.Label("startAngle:", GUILayout.Width(100));
        colliderInfo.StartAngle = EditorGUILayout.FloatField(colliderInfo.StartAngle);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Interval:", GUILayout.Width(100));
        colliderInfo.Interval = EditorGUILayout.IntField(colliderInfo.Interval);
        GUILayout.Label("ColliderMax:", GUILayout.Width(100));
        colliderInfo.ColliderMax = EditorGUILayout.IntField(colliderInfo.ColliderMax);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("LifeTime:", GUILayout.Width(100));
        colliderInfo.LifeTime = EditorGUILayout.IntField(colliderInfo.LifeTime);
        GUILayout.FlexibleSpace();
        colliderInfo.IsPenetrate = EditorGUILayout.Toggle("IsPenetrate:", colliderInfo.IsPenetrate);
        GUILayout.EndHorizontal();

        switch (colliderInfo.ColliderType)
        {
            case CollBase.ColType.CIRCLE:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Radius:", GUILayout.Width(100));
                colliderInfo.Radius = EditorGUILayout.FloatField(colliderInfo.Radius);
                GUILayout.EndHorizontal();
                break;
            case CollBase.ColType.RECTANGLE:
                GUILayout.BeginHorizontal();

                GUILayout.Label("Width:", GUILayout.Width(100));
                colliderInfo.Width = EditorGUILayout.FloatField(colliderInfo.Width);

                GUILayout.Label("Heigh:", GUILayout.Width(100));
                colliderInfo.Height = EditorGUILayout.FloatField(colliderInfo.Height);
                GUILayout.EndHorizontal();
                break;
            case CollBase.ColType.SECTOR:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Angle:", GUILayout.Width(40));
                colliderInfo.Angle = EditorGUILayout.FloatField(colliderInfo.Angle);

                GUILayout.Label("InCircle:", GUILayout.Width(50));
                colliderInfo.InCircle = EditorGUILayout.FloatField(colliderInfo.InCircle);

                GUILayout.Label("OutCircle:", GUILayout.Width(50));
                colliderInfo.OutCircle = EditorGUILayout.FloatField(colliderInfo.OutCircle);
                GUILayout.EndHorizontal();
                break;
        }
        //GUILayout.BeginHorizontal();
        ////GUILayout.Label("ColliderActions:", GUILayout.Width(100));
        ////colliderInfo.colliderActions = EditorGUILayout.TextField(colliderInfo.colliderActions);
        ////使用当前类初始化
        //SerializedObject _serializedObject = new SerializedObject(colliderInfo);
        //////获取当前类中可序列话的属性
        //SerializedProperty assetListProperty = _serializedObject.FindProperty("SelfActions");
        //SerializedProperty assetListProperty2 = _serializedObject.FindProperty("TargetActions");
        //SerializedProperty assetListProperty3 = _serializedObject.FindProperty("EffectInfos");
        //////更新
        //_serializedObject.Update();
        //////开始检查是否有修改
        //EditorGUI.BeginChangeCheck();

        //////显示属性
        //////第二个参数必须为true，否则无法显示子节点即List内容
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("SelfActions:", GUILayout.Width(100));
        //EditorGUILayout.PropertyField(assetListProperty, true);
        //GUILayout.EndHorizontal();
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("TargetActions:", GUILayout.Width(100));
        //EditorGUILayout.PropertyField(assetListProperty2, true);
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("EffectInfos:", GUILayout.Width(100));
        //EditorGUILayout.PropertyField(assetListProperty3, true);
        //GUILayout.EndHorizontal();
        //////结束检查是否有修改
        //if (EditorGUI.EndChangeCheck())
        //{//提交修改
        //    _serializedObject.ApplyModifiedProperties();
        //}
        ////GUILayout.EndHorizontal();

        if (GUILayout.Button("Add Effect", GUILayout.Width(100)))
        {

            colliderInfo.EffectInfos.Add(new EffectInfo());
        }
        
        for (int i = 0; i < colliderInfo.EffectInfos.Count; i++)
        {
            GUILayout.BeginHorizontal("Effect" + i, "window");
            GUILayout.BeginVertical();
            UpdateEffectItem(colliderInfo.EffectInfos[i]);
            if (GUILayout.Button("Clear Effect", GUILayout.Width(100)))
            {
                colliderInfo.EffectInfos.RemoveAt(i);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
           
        }


        GUILayout.EndVertical();
    }

    private void UpdateEffectItem(EffectInfo effectInfo){
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("EffectId:", GUILayout.Width(100));
        effectInfo.Id = EditorGUILayout.TextField(effectInfo.Id);
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

        GUILayout.BeginHorizontal();
        GUILayout.Label("LifeTime:", GUILayout.Width(100));
        effectInfo.LifeTime = EditorGUILayout.IntField(effectInfo.LifeTime);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void UpdateFightEffect(List<FightEffectInfo> list)
    {
        if (GUILayout.Button("Add FightEffect", GUILayout.Width(100)))
        {

            list.Add(new FightEffectInfo());
        }

        for (int i = 0; i < list.Count; i++)
        {
            GUILayout.BeginHorizontal("FightEffect" + i, "window");
            GUILayout.BeginVertical();
            UpdateFightEffectItem(list[i]);
            if (GUILayout.Button("Clear FightEffect", GUILayout.Width(120)))
            {
                list.RemoveAt(i);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        }
    }

    private void UpdateFightEffectItem(FightEffectInfo effectInfo)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("EffectType:", GUILayout.Width(100));
        effectInfo.EffectType = (FIGHT_EF_TPYE)EditorGUILayout.EnumPopup("", effectInfo.EffectType);
        GUILayout.EndHorizontal();

        if (effectInfo.EffectType == FIGHT_EF_TPYE.ARRTIBUTE)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ATTRIBUTE:", GUILayout.Width(100));
            effectInfo.Param1 = (int)(ATTRIBUTE)EditorGUILayout.EnumPopup("", ((ATTRIBUTE)effectInfo.Param1));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ATT_ALTER_TYPE:", GUILayout.Width(150));
            effectInfo.Param2 = (int)(ATT_ALTER_TYPE)EditorGUILayout.EnumPopup("", ((ATT_ALTER_TYPE)effectInfo.Param2));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Param:", GUILayout.Width(100));
            effectInfo.Param3 = EditorGUILayout.IntField(effectInfo.Param3);
            GUILayout.EndHorizontal();
        }
        else if (effectInfo.EffectType == FIGHT_EF_TPYE.SHARK)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("SharkTime:", GUILayout.Width(100));
            effectInfo.Param1 = EditorGUILayout.IntField(effectInfo.Param1);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Offset:", GUILayout.Width(100));
            effectInfo.Param2 = EditorGUILayout.IntField(effectInfo.Param2);
            GUILayout.EndHorizontal();
        }
        else if (effectInfo.EffectType == FIGHT_EF_TPYE.RE_BUFF)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("RemoveType:", GUILayout.Width(100));
            effectInfo.Param1 = (int)(BUFF_REMOVE_TYPE)EditorGUILayout.EnumPopup("", ((BUFF_REMOVE_TYPE)effectInfo.Param1));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string label = "BuffId:";
            if ((BUFF_REMOVE_TYPE)effectInfo.Param1 == BUFF_REMOVE_TYPE.TYPE)
            {
                label = "BuffType:";
            }

            GUILayout.Label(label, GUILayout.Width(100));
            effectInfo.Param2 = EditorGUILayout.IntField(effectInfo.Param2);
            GUILayout.EndHorizontal();
        }
        else if (effectInfo.EffectType == FIGHT_EF_TPYE.ACTION || effectInfo.EffectType == FIGHT_EF_TPYE.ADD_BUFF)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Param:", GUILayout.Width(100));
            effectInfo.Param1 = EditorGUILayout.IntField(effectInfo.Param1);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }
}