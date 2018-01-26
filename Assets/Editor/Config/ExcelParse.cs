using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

public class ExcelParse
{
    [MenuItem("MyTool/ConfigParse")]
    private static void ConfigParse()
    {
        IWorkbook workbook = null;  
        List<string> tmpDataList = new List<string>();
        string[] excelFiles = Directory.GetFiles(ConfigConst.ExcelResPath,"*.xlsx");
        ConfigAsset configAsset = ScriptableObject.CreateInstance<ConfigAsset>();
        
        for (int i = 0; i < excelFiles.Length; i++)
        {
            string fileName = excelFiles[i];
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本  
            {
                workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook  
            }
            else if (fileName.IndexOf(".xls") > 0) // 2003版本  
            {
                workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook  
            }

            ISheet sheet = workbook.GetSheetAt(0);  //获取第一个工作表  
            int rowNum = sheet.LastRowNum + 1;
            IRow iRow;
            for (int row = 0; row < rowNum; row++)  //对工作表每一行  
            {
                iRow = sheet.GetRow(row);   //row读入第i行数据  
                if (iRow != null)
                {
                    for (int col = 0; col < iRow.LastCellNum; col++)  //对工作表每一列  
                    {
                        tmpDataList.Add(iRow.GetCell(col).ToString());
                    }
                }
            }

            configAsset.Name = sheet.SheetName;
            configAsset.MaxRow = rowNum;
            configAsset.MaxCol = tmpDataList.Count / rowNum;
            configAsset.Datas = tmpDataList.ToArray();

            if (File.Exists(ConfigConst.ExcelResPath))
                File.Delete(ConfigConst.ExcelResPath);

            AssetDatabase.CreateAsset(configAsset, ConfigConst.ConfigResPath + configAsset.Name + ".asset");
            fileStream.Close();
            workbook.Close();
        }
        AssetDatabase.Refresh();
    }
}
