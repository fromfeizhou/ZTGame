using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZTSkillEditor : EditorWindow
{
    private ZTSkillLuaEditor luaEditor;

    private static ZTSkillEditor GetSkillEditor()
    {
        var wnd = GetWindowWithRect<ZTSkillEditor>(new Rect(600, 800, 1200, 700));
        wnd.Show();

        return wnd;
    }

    [MenuItem("ZTTool/Skill/ZTSkillEditorWindow")]
    static void ImportZTSkillEditorWindow()
    {
        GetSkillEditor();
        //if (PsdAssetSelected)
        //{
        //    window.Image = (Texture2D)Selection.objects[0];
        //    EditorUtility.SetDirty(window);
        //}
    }

    public void OnEnable()
    {
        if (null != luaEditor)
        {
            luaEditor.Destroy();
            luaEditor = null;
        }
        Setup();
    }

    private List<ZtEdFrameData> frameList;
    public void Setup()
    {
        luaEditor = new ZTSkillLuaEditor();
        LoadSkillConfig();
    }

    public void LoadSkillConfig(string skillId = "id_10001")
    {
        frameList = luaEditor.LoadSkillLua(skillId);
        SortFrameData();
    }

    public void OnGUI()
    {
        SetupStyles();
        DrawZtEdFrameDataList();
        DrawButton();
    }

    private bool styleIsSetup = false;
    private GUIStyle styleHeader, styleLabelLeft, styleBoldFoldout, styleLayerSelected, styleLayerNormal, stylePop;
    void SetupStyles()
    {
        if (styleIsSetup)
            return;

        styleHeader = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
           
        };

        styleLabelLeft = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(0, 0, 0, 0),
            fontSize = 22,
        };

        styleBoldFoldout = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold
        };

        styleLayerSelected = new GUIStyle(GUI.skin.box)
        {
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            contentOffset = new Vector2(0.5f, 0.5f)
        };

        stylePop = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = 30,
        };

        styleLayerNormal = new GUIStyle();

        styleIsSetup = true;
    }

    int skillselIndex;
    void DrawButton()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        if (GUILayout.Button("添加帧", GUILayout.Width(100), GUILayout.Height(30)))
        {
            AddFrameList();
        }
        GUILayout.FlexibleSpace();

        skillselIndex = EditorGUILayout.Popup(skillselIndex, luaEditor.SkillIdList.ToArray(), stylePop,GUILayout.Width(100),GUILayout.Height(30));
        if (GUILayout.Button("加载技能配置", GUILayout.Width(100), GUILayout.Height(30)))
        {
            LoadSkillConfig(luaEditor.SkillIdList[skillselIndex]);
        }

        if (GUILayout.Button("排序", GUILayout.Width(100), GUILayout.Height(30)))
        {
            SortFrameData();
        }
        if (GUILayout.Button("保存动作记录", GUILayout.Width(100), GUILayout.Height(30)))
        {
            SortFrameData();
            luaEditor.SaveSkillTable();
        }
        GUILayout.Space(30);
        GUILayout.EndHorizontal();
    }

    private Vector2 scrollPos = Vector2.zero;
    void DrawZtEdFrameDataList()
    {
        if (null == frameList) return;

        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("当前编辑id: " + luaEditor.CurLuaKey, styleLabelLeft);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(1200), GUILayout.Height(620));

        for (int i = 0; i < frameList.Count; i++)
        {
            ZtEdFrameData framedata = frameList[i];
            DrawFrameData(framedata);
        }
        
        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
    }


    void DrawFrameData(ZtEdFrameData framedata)
    {
        #region 垂直窗口
        GUILayout.BeginVertical("Frame" + framedata.frame, "window", GUILayout.Width(1180), GUILayout.MinHeight(50));

        #region 水平标题
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("—", GUILayout.Width(25), GUILayout.Height(15)))
        {
            RemoveFrameData(framedata);
            return;
        }
        GUILayout.Label("触发帧:", GUILayout.Width(50));
        framedata.frame = EditorGUILayout.IntField(framedata.frame, GUILayout.Width(50));

        GUILayout.FlexibleSpace();
        framedata.editorSel = GUILayout.Toolbar(framedata.editorSel, ZTSkillEditorDefine.TypeDes);
        //framedata.editorSel = EditorGUILayout.Popup(framedata.editorSel, ZTSkillEditorDefine.TypeDes,GUILayout.Width(100),GUILayout.Height(20));

        if (GUILayout.Button("添加action", GUILayout.Width(100),GUILayout.Height(20))) {
            ZtEdSkillAction actoin = new ZtEdSkillAction();
            actoin.actionType = framedata.editorSel;
            int count = 1;
            if(ZTSkillEditorDefine.TypeList != null && ZTSkillEditorDefine.TypeList.ContainsKey(actoin.actionType))
            {
                count = ZTSkillEditorDefine.TypeList[actoin.actionType].Length;
            }
            for(int i = 0; i < count; i++)
            {
                actoin.param.Add("-1");
            }
            framedata.actoinList.Add(actoin);
        }
        GUILayout.Space(30);

        GUILayout.EndHorizontal();
        #endregion

        for (int i = 0; i < framedata.actoinList.Count; i++)
        {
            bool isRemove = DrawActoin(framedata.actoinList[i], framedata);
            if (isRemove) break;
        }

        GUILayout.EndVertical();
        GUILayout.Space(10);

        #endregion
    }

    void RemoveFrameData(ZtEdFrameData framedata)
    {
        if (null != frameList)
        {
            frameList.Remove(framedata);
        }
    }

    void SortFrameData()
    {
        if (null != frameList)
        {
            frameList.Sort((x, y) => { return x.frame < y.frame ? -1 : 1; });
        }
    }
    
   
    bool DrawActoin(ZtEdSkillAction action,ZtEdFrameData framedata)
    {
        GUILayout.BeginVertical("HelpBox");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("—", GUILayout.Width(25), GUILayout.Height(15)))
        {
            framedata.actoinList.Remove(action);
            return true;
        }

        GUILayout.Label(ZTSkillEditorDefine.TypeDes[action.actionType] + "：");
        for (int i = 0; i < action.param.Count; i++)
        {
            if (ZTSkillEditorDefine.TypeList.ContainsKey(action.actionType))
            {
                string labelName = ZTSkillEditorDefine.TypeList[action.actionType][i];
                GUILayout.Label(labelName);
                switch (labelName) {
                    case "方向":
                        action.param[i] = DrawFacePop(action.param[i] as string);
                        break;
                    case "层次":
                        action.param[i] = DrawLayerPop(action.param[i] as string);
                        break;
                    case "目标类型":
                        action.param[i] = DrawTargetPop(action.param[i] as string);
                        break;
                    default:
                        action.param[i] = (string)EditorGUILayout.TextField(action.param[i] as string);
                        break;
                }
                
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        return false;
    }

    //方向调整
    string DrawFacePop(string value)
    {
        int key = ZTSkillEditorDefine.FaceType.IndexOf(value);
        key = key >= 0 ? key : 0;
        key = EditorGUILayout.Popup(key, ZTSkillEditorDefine.FaceTypeNames, GUILayout.Width(100));
        return ZTSkillEditorDefine.FaceType[key] as string;
    }
    //层次选择
    string DrawLayerPop(string value)
    {
        int key = ZTSkillEditorDefine.LayerType.IndexOf(value);
        key = key >= 0 ? key : 0;
        key = EditorGUILayout.Popup(key, ZTSkillEditorDefine.LayerTypeNames, GUILayout.Width(60));
        return ZTSkillEditorDefine.LayerType[key] as string;
    }
    //目标类型选择
    string DrawTargetPop(string value)
    {
        int key = ZTSkillEditorDefine.TargetType.IndexOf(value);
        key = key >= 0 ? key : 0;
        key = EditorGUILayout.Popup(key, ZTSkillEditorDefine.TargetTypeNames, GUILayout.Width(60));
        return ZTSkillEditorDefine.TargetType[key] as string;
    }


    void AddFrameList()
    {
        if (null == frameList)
        {
            return;
        }

        frameList.Add(new ZtEdFrameData());
    }

    public void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (null != luaEditor)
        {
            luaEditor.Destroy();
            luaEditor = null;
        }

    }

}
