using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZTSkillEditor : EditorWindow
{
    private ZTSkillLuaEditor luaEditor;

    private static ZTSkillEditor GetSkillEditor()
    {
        var wnd = GetWindowWithRect<ZTSkillEditor>(new Rect(500, 500, 1100, 630));
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
        frameList = luaEditor.LoadSkillLua();
    }

    public void OnGUI()
    {
        SetupStyles();
        DrawZtEdFrameDataList();
        DrawButton();
    }

    private bool styleIsSetup = false;
    private GUIStyle styleHeader, styleLabelLeft, styleBoldFoldout, styleLayerSelected, styleLayerNormal;
    void SetupStyles()
    {
        if (styleIsSetup)
            return;

        styleHeader = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        styleLabelLeft = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(0, 0, 0, 0)
        };

        styleBoldFoldout = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold
        };

        styleLayerSelected = new GUIStyle(GUI.skin.box)
        {
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            contentOffset = new Vector2(0, 0)
        };

        styleLayerNormal = new GUIStyle();

        styleIsSetup = true;
    }

    void DrawButton()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        if (GUILayout.Button("添加帧", GUILayout.Width(100), GUILayout.Height(30)))
        {
            AddFrameList();
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("排序", GUILayout.Width(100), GUILayout.Height(30)))
        {
            SortFrameData();
        }
        if (GUILayout.Button("保存动作记录", GUILayout.Width(100), GUILayout.Height(30)))
        {
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

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(1100), GUILayout.Height(580));

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
        GUILayout.BeginVertical("Frame" + framedata.frame, "window", GUILayout.Width(1100), GUILayout.MinHeight(50));

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
        framedata.editorSel = GUILayout.Toolbar(framedata.editorSel, typeDes);
        if (GUILayout.Button("添加action", GUILayout.Width(100),GUILayout.Height(20))) {
            ZtEdSkillAction actoin = new ZtEdSkillAction();
            actoin.actionType = framedata.editorSel;
            int count = 1;
            if(typeList != null && typeList.ContainsKey(actoin.actionType))
            {
                count = typeList[actoin.actionType].Length;
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

    void DrawFrameData()
    {

    }

    private string[] dirDes = { "方向" };
    private string[] playDes = { "动作名" };
    private string[] soundDes = { "声音" };
    private string[] colliderDes = { "半径", "运动id", "层次", "偏移x", "偏移y","目标类型", "存在时间","碰撞总数","特效名字" };
    private string[] typeDes = { "朝向","动作播放","声音播放","碰撞"};
    private Dictionary<int,string[]> typeList;
    bool DrawActoin(ZtEdSkillAction action,ZtEdFrameData framedata)
    {
        if(null == typeList)
        {
            typeList = new Dictionary<int, string[]>();
            typeList.Add(0, dirDes);
            typeList.Add(1,playDes);
            typeList.Add(2, soundDes);
            typeList.Add(3,colliderDes);
        }

        GUILayout.BeginVertical("HelpBox");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("—", GUILayout.Width(25), GUILayout.Height(15)))
        {
            framedata.actoinList.Remove(action);
            return true;
        }


        GUILayout.Label(typeDes[action.actionType] + "：");
        for (int i = 0; i < action.param.Count; i++)
        {
            if (typeList.ContainsKey(action.actionType))
            {
                GUILayout.Label(typeList[action.actionType][i]);
                action.param[i] = (string)EditorGUILayout.TextField(action.param[i] as string);
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        return false;
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
