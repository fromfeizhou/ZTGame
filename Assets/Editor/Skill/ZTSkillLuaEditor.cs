using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using XLua;

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
        return Regex.IsMatch(value, @"^\d*[.]?\d*$");
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

    public void LoadSkillLua()
    {
        DoString("Battle.Skill.SkillDefine");
        DoString("ALuaConfig.SkillActoinConfig");
        GetLuaData();

    }
    public void DoString(string luaPath)
    {
        string lua = string.Format("require('{0}')", luaPath);
        luaenv.DoString(lua);
    }
    string LuaKey = string.Empty;
    LuaTable LuaTab = null;
    Dictionary<int,List<ZtEdSkillAction>> framList;

    public class ZtEdSkillAction{
        public int actionType;
        public ArrayList param;
        public ZtEdSkillAction()
        {
            actionType = -1;
            param = new ArrayList();
        }
    }

    public void GetLuaData()
    {
        framList = new Dictionary<int, List<ZtEdSkillAction>>();

        GetGlobalKeyVal("SkillActionType");
        GetGlobalKeyVal("SkillLayerType");
        GetGlobalKeyVal("SkillFaceType");
        GetGlobalKeyVal("SkillTargetType");
        GetGlobalKeyVal("SkillEffectType");
        GetGlobalKeyVal("AttributeType");
        GetGlobalKeyVal("BuffType");

        Debug.Log("Tabel>>>>>>>>>>>>>>>>>>>>>>>");
        GetGlobalKeyTab("SkillActoinConfig");

        SaveSkillTable();
    }
    
    public void GetGlobalKeyTab(string tablename)
    {
        //Debug.Log("Tabel: " + tablename);
        LuaTable table = luaenv.Global.Get<LuaTable>(tablename);//映射到LuaTable，by ref
        table.ForEach<string, LuaTable>((string key, LuaTable value) =>
        {
            //Debug.Log(key + " = ");
            GetSkillTable(value);
            LuaKey = key;
            LuaTab = value;
        });

    }

    public void GetSkillTable(LuaTable table)
    {
        table.ForEach<int, LuaTable>((int key, LuaTable value) =>
        {
            List<ZtEdSkillAction> frame = new List<ZtEdSkillAction>();
            framList.Add(key,frame);

            //Debug.Log("frame:" + key + " =>>>>>>>>>");
            GetSkillParam(value,frame);
        });
    }

    public void GetSkillParam(LuaTable table, List<ZtEdSkillAction> frame)
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
            frame.Add(skillAction);

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
    string SkillTabSavePath = "Assets/LuaScript/ALuaConfig/SkillActoinConfigTmp.txt";
    public void SaveSkillTable()
    {
        if (File.Exists(SkillTabSavePath))
            File.Delete(SkillTabSavePath);

        string scriptStr = string.Empty;
        scriptStr =LuaKey + " = {\n";
        foreach (int frame in framList.Keys)
        {
            List<ZtEdSkillAction> list = framList[frame];
            scriptStr += string.Format("\t[ {0} ] = ",frame) + "{\n";
            for(int i = 0; i < list.Count; i++)
            {
                scriptStr += GetActcionStr(list[i]);
            }
            scriptStr += " \t\t},\n";
        }
            //for (int i = 0; i < moduleList.Count; i++)
            //{
            //    scriptStr += moduleList[i].GetDefine();
            //}
            scriptStr += " }";

        File.WriteAllText(SkillTabSavePath, scriptStr.Trim());
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
        result += "\t\t{ " + string.Format("actionType = {0}, param = {1}", action.actionType, paramres) + " }},\n";
        return result;
    }


    public void Destroy()
    {
        luaenv.Dispose();
        luaenv = null;
    }

}
