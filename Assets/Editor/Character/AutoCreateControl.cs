using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;

/**
* 功 能： 
* ───────────────────────────────────
* V0.01 2017/8/10 14:55:13 leeming 
* Copyright (c) 2017 edo Corporation. All rights reserved.
*/
public class AutoCreateControl
{
    #region 创建animator
    //动作模型 父文件夹
    private static string m_parentPath = "";
    private static AnimatorStateMachine m_stateMachine;
    private static AnimatorController m_curAnimCtrl;
    private static Dictionary<string, AnimatorState> m_allState = new Dictionary<string, AnimatorState>();
    private static string[] m_states = { "idle1", "item_boots", "item_pants", "item_shirt", "walk" };
    public static AnimatorController CreateControl(string filePath)
    {
        m_parentPath = filePath;
        string foldPath = filePath + "Control/";
        if (!Directory.Exists(foldPath))
        {
            Directory.CreateDirectory(foldPath);
        }
        string ctrlPath = PathManager.CombinePath(foldPath, "animation.controller");
        AnimatorController ac = AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);
        m_curAnimCtrl = ac;
        AddParam(ac);

        var animLayer = ac.layers[0];
        m_stateMachine = animLayer.stateMachine;
        CreateAllState();
        CreateAllRelation();

        return ac;
    }

    /************************************************************************/
    /* 增加参数                                                                     */
    /************************************************************************/
    private static string[] ParamNamesFloat = { "rotation" };
    private static string[] ParamNamesInt = { "action" };

    public static void AddParam(AnimatorController ac)
    {
        foreach (string name in ParamNamesFloat)
        {
            var parm2 = new UnityEngine.AnimatorControllerParameter();
            parm2.type = UnityEngine.AnimatorControllerParameterType.Float;
            parm2.name = name;
            parm2.defaultFloat = 0;
            ac.AddParameter(parm2);
        }

        foreach (string name in ParamNamesInt)
        {
            var parm2 = new UnityEngine.AnimatorControllerParameter();
            parm2.type = UnityEngine.AnimatorControllerParameterType.Int;
            parm2.name = name;
            parm2.defaultFloat = 0;
            ac.AddParameter(parm2);
        }
    }

    //关联 state
    public static void CreateAllRelation()
    {
        AddTransition("", m_states[0], "action", AnimatorConditionMode.Equals, 0);
        AddTransition(m_states[0], m_states[1], "action", AnimatorConditionMode.Equals, 1);
        AddTransition(m_states[0], m_states[2], "action", AnimatorConditionMode.Equals, 2);
        AddTransition(m_states[0], m_states[3], "action", AnimatorConditionMode.Equals, 3);
        AddTransition(m_states[0], m_states[4], "action", AnimatorConditionMode.Equals, 4);
    }

    private static AnimatorStateTransition AddTransition(string fromName, string toName,
        params object[] conditions)
    {
        AnimatorState from = null;
        if (m_allState.ContainsKey(fromName))
            from = m_allState[fromName];

        AnimatorState to = null;
        if (m_allState.ContainsKey(toName))
            to = m_allState[toName];

        AnimatorStateTransition transition = (from == null ? m_stateMachine.AddAnyStateTransition(to) : from.AddTransition(to));

        for (int i = 0; i < conditions.Length; )
        {
            var name = (string)conditions[i++];
            var type = (AnimatorConditionMode)conditions[i++];
            if (type == AnimatorConditionMode.If || type == AnimatorConditionMode.IfNot)
            {
                i++;
                transition.AddCondition(type, 0, name);
            }
            else
            {
                var arg = (int)conditions[i++];
                transition.AddCondition(type, arg, name);
            }
        }
        if (!fromName.StartsWith("idle1"))
        {
            transition.canTransitionToSelf = false;
        }
        if (fromName.StartsWith("attack"))
        {
            transition.hasExitTime = true;
        }
        return transition;
    }

    #region 创建状态
    public static void CreateAllState()
    {
        m_allState.Clear();
        
        AnimatorState aState;
        int index = 0;
        for (int i = 0; i < m_states.Length; i++)
        {
            string state = m_states[i];
            if (i == 0)
                aState = CreateState(state, new Vector3(0, -100, 0));
            else
                aState = CreateState(state, new Vector3(250, -m_states.Length * 30 + 60 * index, 0));
            if (aState != null)
                index++;
        }
    }
    //创建state （判断动作是否有混合动作 分别加载）
    public static AnimatorState CreateState(string name, Vector3 pos)
    {
        string fbxPath = PathManager.CombinePath(m_parentPath, CreatePrefabs.m_ModeName + "@" + name + ".fbx");
        AnimatorState state = null;
        if (IsBlendTree(name))
        {
            AnimationClip a1c = GetAnimationClip(fbxPath);
            if (a1c == null)
            {
                Debug.LogError("--------动作丢失---------name=" + (fbxPath));
                return null;
            }
            BlendTree bt;
            //创建混合树(名字为name)
            state = m_curAnimCtrl.CreateBlendTreeInController(name, out bt);
            bt.hideFlags = HideFlags.HideInHierarchy;
            string[] btMotion = { "_up", "_rightup", "_right", "_rightdown" };
            for (int i = 0; i < btMotion.Length; i++)
            {
                string blendFbxPath = PathManager.CombinePath(m_parentPath, CreatePrefabs.m_ModeName + "@" + name + btMotion[i] + ".fbx");
                a1c = GetAnimationClip(blendFbxPath);
                bt.AddChild(a1c);
            }
            bt.useAutomaticThresholds = true;
            bt.blendParameter = "rotation";
            bt.name = "Blend Tree";
            state.motion = bt;
        }
        else
        {
            AnimationClip anim = GetAnimationClip(fbxPath);

            if (anim == null)
            {
                Debug.LogError("--------动作丢失---------name=" + (fbxPath));
                return null;
            }

            state = m_stateMachine.AddState(name, pos);
            state.motion = anim;
        }
        m_allState[name] = state;
        return state;
    }

    private static bool IsBlendTree(string name)
    {
        //暂时没有 sample::   treeStates = { "atstand","attack01", "attack02", "attack03", "attack04", "attack05" };
        string[] treeStates = { };
        if (treeStates.Contains(name))
            return true;
        return false;
    }

    private static AnimationClip GetAnimationClip(string fbxPath)
    {
        AnimationClip a1c = AssetDatabase.LoadAssetAtPath(fbxPath, typeof(AnimationClip)) as AnimationClip;
        return a1c;
    }

    #endregion
    #endregion  -- 创建animator
}
