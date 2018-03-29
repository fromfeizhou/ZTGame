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
    //public static bool IsNumeric(string value)
    //{
    //    return Regex.IsMatch(value, "^[+-]?/d*[.]?/d*$");
    //}
    //public static bool IsInt(string value)
    //{
    //    return Regex.IsMatch(value, "^[+-]?/d*$");
    //}

    public static bool IsUnsign(string value)
    {
        return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
    }


    LuaEnv luaenv = null;
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
        TextAsset txtAsset = AssetManager.LoadLuaAsset(fileName) as TextAsset;
        if (null != txtAsset)
        {
            return txtAsset.bytes;
        }
        //找到指定文件  
        return null;
    }

    public List<ZtEdFrameData> LoadSkillLua()
    {
        DoString("Battle.Skill.SkillDefine");
        DoString("ALuaConfig.SkillActionConfigTmp");
        return GetLuaData();

    }
    public void DoString(string luaPath)
    {
        string lua = string.Format("require('{0}')", luaPath);
        luaenv.DoString(lua);
    }
   

   

    string LuaKey = string.Empty;
    List<ZtEdFrameData> framList;

    public List<ZtEdFrameData> GetLuaData()
    {
        framList = new List<ZtEdFrameData>();

        GetGlobalKeyVal("SkillActionType");
        GetGlobalKeyVal("SkillLayerType");
        GetGlobalKeyVal("SkillFaceType");
        GetGlobalKeyVal("SkillTargetType");
        GetGlobalKeyVal("SkillEffectType");
        GetGlobalKeyVal("AttributeType");
        GetGlobalKeyVal("BuffType");

        Debug.Log("Tabel>>>>>>>>>>>>>>>>>>>>>>>");
        GetGlobalKeyTab("SkillActionConfig");
        return framList;
        //SaveSkillTable();
    }
    
    public void GetGlobalKeyTab(string tablename,string skillId = "id_template")
    {
        //Debug.Log("Tabel: " + tablename);
        LuaTable table = luaenv.Global.Get<LuaTable>(tablename);//映射到LuaTable，by ref
        LuaTable skillTab = table.Get<LuaTable>(skillId);
        if(null != skillTab)
        {
            GetSkillTable(skillTab);
            LuaKey = skillId;
        }
        //table.ForEach<string, LuaTable>((string key, LuaTable value) =>
        //{
        //    //Debug.Log(key + " = ");
        //    GetSkillTable(value);
        //    LuaKey = key;
        //});

    }

    public void GetSkillTable(LuaTable table)
    {
        table.ForEach<int, LuaTable>((int key, LuaTable value) =>
        {
            ZtEdFrameData framedata = new ZtEdFrameData();
            framedata.frame = key;
            framList.Add(framedata);

            //Debug.Log("frame:" + key + " =>>>>>>>>>");
            GetSkillParam(value, framedata);
        });
    }

    public void GetSkillParam(LuaTable table, ZtEdFrameData framedata)
    {
        table.ForEach<int, LuaTable>((int key, LuaTable value) =>
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
        //table.ForEach<string, int>((string key, int value) =>
        //{
        //    Debug.Log(key + " = " + value);
        //});
    }
    string SkillTabSavePath = "Assets/LuaScript/ALuaConfig/SkillActionConfigTmp.txt";
    public void SaveSkillTable()
    {
        if (File.Exists(SkillTabSavePath))
            File.Delete(SkillTabSavePath);

        string scriptStr = string.Empty;
        scriptStr = "SkillActionConfig = {\n";
        scriptStr += "\t" + LuaKey + " = {\n";
        for(int index = 0; index < framList.Count; index++)
        {
            ZtEdFrameData framedata = framList[index];
            scriptStr += string.Format("\t\t[ {0} ] = ", framedata.frame) + "{\n";
            for(int i = 0; i < framedata.actoinList.Count; i++)
            {
                scriptStr += GetActcionStr(framedata.actoinList[i]);
            }
            scriptStr += "\t\t},\n";
        }
            //for (int i = 0; i < moduleList.Count; i++)
            //{
            //    scriptStr += moduleList[i].GetDefine();
            //}
            scriptStr += "\t},\n}";

        File.WriteAllText(SkillTabSavePath, scriptStr.Trim());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
