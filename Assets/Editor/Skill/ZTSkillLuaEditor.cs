using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using XLua;

public class ZtEdFrameData
{
    public int editorSel;
    public int frame = -1;
    public List<ZtEdSkillAction> actoinList;

    public ZtEdFrameData()
    {
        editorSel = 0;
        frame = -1;
        actoinList = new List<ZtEdSkillAction>();
    }
}

public class ZtEdSkillAction
{
    public int actionType;
    public ArrayList param;
    public ZtEdSkillAction()
    {
        actionType = -1;
        param = new ArrayList();
    }
}

public class ZTSkillLuaEditor
{
    public static bool IsUnsign(string value)
    {
        return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
    }


    LuaEnv luaenv = null;
    public List<string> SkillIdList;

    // Use this for initialization
    public ZTSkillLuaEditor()
    {
        luaenv = new LuaEnv();
        luaenv.AddLoader(CustomLoaderMethod);
        //luaenv.DoString("print('InMemory.ccc=', require('InMemory').ccc)");

    }

    private byte[] CustomLoaderMethod(ref string fileName)
    {
        fileName = PathManager.LuaPath + "/" + fileName.Replace('.', '/') + ".txt";
      
        //找到指定文件  
        return AssetManager.LoadLuaAsset(fileName);
    }

    public void DoString(string luaPath)
    {
        string lua = string.Format("package.loaded['{0}'] = nil", luaPath);
        luaenv.DoString(lua);
        lua = string.Format("require('{0}')", luaPath);
        luaenv.DoString(lua);
    }


    public string CurLuaKey = string.Empty;
    List<ZtEdFrameData> CurFrameList;
    LuaTable SkillConfigTab;
    public List<ZtEdFrameData> LoadSkillLua(string skillId)
    {
        //DoString("Battle.Skill.SkillDefine");
        DoString("ALuaConfig.SkillActionConfig");
        //DoString("Battle.skill.MoveAction.MoveActionConfig");
        SkillConfigTab = luaenv.Global.Get<LuaTable>("SkillActionConfig");//映射到LuaTable，by ref

        CurLuaKey = skillId;
        CurFrameList = GetSkillFrameList(skillId);
        if(SkillIdList == null)
        {
            SkillIdList = new List<string>();
            SkillConfigTab.ForEach<string, LuaTable>((string key, LuaTable value) =>
            {
                if(key != "TestSkillId")
                    SkillIdList.Add(key);
            });
            SkillIdList.Sort();
        }
        return CurFrameList;
    }

    public List<ZtEdFrameData> GetSkillFrameList(string skillId)
    {
        List<ZtEdFrameData>  frameList = new List<ZtEdFrameData>();
        GetGlobalKeyTab(skillId, frameList);
        return frameList;
        //SaveSkillTable();
    }

    public void GetGlobalKeyTab(string skillId, List<ZtEdFrameData> frameList)
    {
        LuaTable skillTab = SkillConfigTab.Get<LuaTable>(skillId);
        if(null != skillTab)
        {
            GetSkillTable(skillTab,frameList);
        }
    }

    public void GetSkillTable(LuaTable skillTab, List<ZtEdFrameData> frameList)
    {
        skillTab.ForEach<int, LuaTable>((int key, LuaTable value) =>
        {
            ZtEdFrameData framedata = new ZtEdFrameData();
            framedata.frame = key;
            frameList.Add(framedata);
            GetSkillParam(value, framedata);
        });
    }

    public void GetSkillParam(LuaTable actiontable, ZtEdFrameData framedata)
    {
        actiontable.ForEach<int, LuaTable>((int key, LuaTable value) =>
        {

            LuaTable paramTab = value.Get<LuaTable>("param");
            int actionType = value.Get<int>("actionType");
            ZtEdSkillAction skillAction = new ZtEdSkillAction();
            skillAction.actionType = actionType;
            paramTab.ForEach<int, string>((int paramKey, string paramVal) =>
            {
                skillAction.param.Add(paramVal);
            });
            framedata.actoinList.Add(skillAction);

        });
    }

    public void GetGlobalKeyVal(string tablename)
    {
        LuaTable table = luaenv.Global.Get<LuaTable>(tablename);//映射到LuaTable，by ref
       
    }
    string SkillTabSavePath = "Assets/LuaScript/ALuaConfig/SkillActionConfig.txt";
    public void SaveSkillTable()
    {
        if (File.Exists(SkillTabSavePath))
            File.Delete(SkillTabSavePath);

        string scriptStr = string.Empty;
        scriptStr = "SkillActionConfig = {\n";

        //排序
        SkillIdList.Sort();
        for (int i = 0; i < SkillIdList.Count; i++)
        {
            string key = SkillIdList[i];
            if (key == CurLuaKey)
            {
                scriptStr += GetFrameStr(key, CurFrameList);
            }
            else
            {
                List<ZtEdFrameData> frameList = GetSkillFrameList(key);
                frameList.Sort((x, y) => { return x.frame < y.frame ? -1 : 1; });
                scriptStr += GetFrameStr(key, frameList);
            }
        }

        scriptStr += "}\n\n";

        scriptStr += "SkillActionConfig.TestSkillId = " + "\"" + CurLuaKey + "\"";

        File.WriteAllText(SkillTabSavePath, scriptStr.Trim());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private string GetFrameStr(string key,List<ZtEdFrameData> frameList)
    {
        string scriptStr = string.Empty;
        scriptStr += "\t" + key + " = {\n";
        for (int index = 0; index < frameList.Count; index++)
        {
            ZtEdFrameData framedata = frameList[index];
            scriptStr += string.Format("\t\t[ {0} ] = ", framedata.frame) + "{\n";
            for (int i = 0; i < framedata.actoinList.Count; i++)
            {
                scriptStr += GetActcionStr(framedata.actoinList[i]);
            }
            scriptStr += "\t\t},\n";
        }
        //for (int i = 0; i < moduleList.Count; i++)
        //{
        //    scriptStr += moduleList[i].GetDefine();
        //}
        scriptStr += "\t},\n\n";
        return scriptStr;
    }

    private string GetActcionStr(ZtEdSkillAction action)
    {
        string result = string.Empty;
        string paramres = string.Empty;

        for (int k = 0; k < action.param.Count; k++)
        {
            bool isVal = IsUnsign(action.param[k] as string);
            System.Object val = isVal ? action.param[k] : "\"" + action.param[k] + "\"";
            if (k == 0)
            {
                paramres += "{ " + val;
            }
            else
            {
                paramres += string.Format(", {0}", val);

            }
        }
        result += "\t\t\t\t{ " + string.Format("actionType = {0}, param = {1}", action.actionType, paramres) + " }},\n";
        return result;
    }


    public void Destroy()
    {
        Debug.Log("ZTSkillLuaEditor Destroy");
        luaenv.Dispose();
        luaenv = null;
    }

}
