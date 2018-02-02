using System;
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
        if (Directory.Exists(ConfigConst.ConfigResPath))
            Directory.Delete(ConfigConst.ConfigResPath, true);
        Directory.CreateDirectory(ConfigConst.ConfigResPath);

        string[] excelFiles = Directory.GetFiles(ConfigConst.ExcelResPath, "*.xlsx");
        for (int i = 0; i < excelFiles.Length; i++)
        {
            FileStream fileStream = new FileStream(excelFiles[i], FileMode.Open, FileAccess.Read);
            IWorkbook workbook = new XSSFWorkbook(fileStream);
            fileStream.Close();

            ISheet sheet = workbook.GetSheetAt(0);  //获取第一个工作表  
            IRow iRowType = sheet.GetRow(0);
            IRow iRowName = sheet.GetRow(2);
            int colNum = iRowType.LastCellNum;
            string[] dataType = new string[colNum];
            string[] dataName = new string[colNum];
            for (int col = 0; col < colNum; col++)
            {
                dataType[col] = iRowType.GetCell(col).ToString();
                dataName[col] = iRowName.GetCell(col).ToString();
            }
            string tableData = "{0} = {{\n{1}}}";
            string rowKey = "  {0}_{1} = {{\n{2}  }},\n";

            string rowDataStr = string.Empty;
            for (int row = 3; row < sheet.LastRowNum + 1; row++)  //对工作表每一行  
            {
                IRow iRow = sheet.GetRow(row);   //row读入第i行数据  
                if (iRow != null)
                {
                    string rowData = string.Empty;
                    string dataFormat = string.Empty;

                    for (int col = 0; col < colNum; col++)  //对工作表每一列  
                    {
                        var obj = iRow.GetCell(col);
                        string value = obj != null ? obj.ToString() : string.Empty;
                        switch (dataType[col])
                        {
                            case "int":
                                {
                                    if (string.IsNullOrEmpty(value))
                                        value = "0";

                                    dataFormat = "    {0} = {1},\n";
                                    break;
                                }
                            case "float":
                                {
                                    if (string.IsNullOrEmpty(value))
                                        value = "0.0";

                                    dataFormat = "    {0} = {1:0.0000},\n";
                                    break;
                                }
                            case "string":
                                {
                                    if (string.IsNullOrEmpty(value))
                                        value = "";

                                    dataFormat = "    {0} = '{1}',\n";
                                    break;
                                }
                            case "table":
                                {
                                    if (string.IsNullOrEmpty(value))
                                        value = "{}";

                                    dataFormat = "    {0} = {1},\n";
                                    break;
                                }
                        }

                        rowData += String.Format(dataFormat, dataName[col], value);
                    }
                    rowDataStr += string.Format(rowKey, dataName[0], iRow.GetCell(0), rowData);
                }
            }
            string luaPath = ConfigConst.ConfigResPath + sheet.SheetName + ".lua";
            string luaContent = string.Format(tableData, sheet.SheetName, rowDataStr).Trim();
            File.WriteAllText(luaPath, luaContent);
            workbook.Close();
        }
        AssetDatabase.Refresh();
    }
}
