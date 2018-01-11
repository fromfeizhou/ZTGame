using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


//按钮预定义
public struct MButtonData
{
    public int width;
    public int height;
}

public class ButtonFormatDefine
{
    public static MButtonData GetFormat(string strFormat)
    {
        MButtonData data = new MButtonData();
        string[] arr = strFormat.Split(',');
        if (arr.Length == 2 && arr[0] !="" && arr[1] != "")
        {
            data.width = int.Parse(arr[0]);
            data.height = int.Parse(arr[1]);
        }
        else
        {
            data.width = 220;
            data.height = 100;
        }
        return data;
    }
}

//文本预定义
public struct MTextFormatData
{
    public Color color;
    public int fontSize;
    public bool isOutline;
    public Color effectColor;
}

public class TextFormatDefine  {

    private static Dictionary<string, Color> colorDic = null;
    private static Color defColor = Color.white;
    private static int defSize = 18;

    //获取文本格式
    public static MTextFormatData GetFormat(string strFormat)
    {
        if (colorDic == null)
        {
            initColorDic();
        }

        MTextFormatData data = new MTextFormatData();

        Regex reg = new Regex("C[0-9]+");
        //返回一个结果集
        MatchCollection result = reg.Matches(strFormat);
        bool isStandard = true;
        //是否符合规则颜色
        if (result.Count > 0 && result.Count <= 2)
        {
            foreach (Match m in result)
            {
                if (!colorDic.ContainsKey(m.ToString()))
                {
                    isStandard = false;
                }
            }
        }
        else
        {
            isStandard = false;
        }
        if (strFormat == "" || !isStandard)
        {
            data.color = defColor;
            data.fontSize = defSize;
            data.isOutline = false;
            data.effectColor = defColor;
        }
        else
        {
            data.color = colorDic[result[0].ToString()];
            if (result.Count == 2)
            {
                data.isOutline = true;
                data.effectColor = colorDic[result[1].ToString()];
            }
            else
            {
                data.isOutline = false;
                data.effectColor = defColor;
            }
            int fontSize = defSize;
            Regex regF = new Regex("F[0-9]+");
            Regex regNum = new Regex("[0-9]+");
            Match resultF = regF.Match(strFormat, 0);
            if (resultF.Success)
            {
                Match resultNum = regNum.Match(resultF.ToString(), 0);
                if (resultNum.Success)
                {
                    fontSize = System.Convert.ToInt32(resultNum.ToString());
                }
            }
            data.fontSize = fontSize;
        }

        return data;
    }

    //获取颜色
    public static Color GetColor(string colorKey)
    {
        if (colorDic == null)
        {
            initColorDic();
        }
        if (!colorDic.ContainsKey(colorKey))
        {
            return defColor;
        }
        return colorDic[colorKey];
    }

    //初始化颜色列表
    private static void initColorDic()
    {
        colorDic = new Dictionary<string, Color>();
        colorDic.Add("C1", new Color(0, 0, 0));
        colorDic.Add("C2", new Color(255, 255, 255));
        colorDic.Add("C3", new Color(255, 0, 0));
        colorDic.Add("C4", new Color(0, 255, 0));
        colorDic.Add("C5", new Color(0, 0, 255));

        colorDic.Add("C6", new Color(255, 236, 193));
        colorDic.Add("C7", new Color(0, 255, 0));
    }
	
}
