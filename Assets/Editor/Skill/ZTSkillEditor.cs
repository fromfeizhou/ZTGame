using UnityEditor;
using UnityEngine;

public class ZTSkillEditor : EditorWindow
{
    private ZTSkillLuaEditor luaEditor;

    private static ZTSkillEditor GetSkillEditor()
    {
        var wnd = GetWindow<ZTSkillEditor>();
        wnd.Setup();

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
    public void Setup()
    {
        luaEditor = new ZTSkillLuaEditor();
        luaEditor.LoadSkillLua();
        /*
         * Another possible way to access the hierarchy window
         * 
        Assembly asm = typeof(UnityEditor.EditorWindow).Assembly;
        Type wndType = asm.GetType("UnityEditor.SceneHierarchyWindow");
        hierarchyWindow = EditorWindow.GetWindow(wndType);

        var treeViewVal = wndType.GetProperty("treeView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(hierarchyWindow, null);
        var stateVal = treeViewVal.GetType()
            .GetProperty("state")
            .GetValue(treeViewVal, null);
        */
    }

    public void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if(null != luaEditor)
        {
            luaEditor.Destroy();
            luaEditor = null;
        }
        
    }

}
