
using System.Data;
using System.IO;
using Excel;
using UnityEditor;
using UnityEngine;


public class ExcelParse
{
    [MenuItem("MyTool/ConfigParse")]
    private static void ConfigParse()
    {
        string[] excelFiles = Directory.GetFiles(ConfigConst.ExcelResPath,"*.xlsx");

        ConfigAsset configAsset = new ConfigAsset();
        
        for (int i = 0; i < excelFiles.Length; i++)
        {
            FileStream stream = File.Open(excelFiles[i], FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet();


            configAsset.Name = Path.GetFileNameWithoutExtension(excelFiles[i]);
            configAsset.MaxRow = result.Tables[0].Rows.Count;
            configAsset.MaxCol = result.Tables[0].Columns.Count;
            configAsset.Datas = new string[configAsset.MaxRow * configAsset.MaxCol];

            for (int row = 0; row < configAsset.MaxRow; row++)
            {
                for (int col = 0; col < configAsset.MaxCol; col++)
                {
                    configAsset.Datas[row * configAsset.MaxCol + col] = result.Tables[0].Rows[row][col].ToString();
                    Debug.Log(string.Format("{0} - {1}:{2} =>{3}", row * configAsset.MaxCol + col, row,col, configAsset[row, col]));
                }
            }

            if (File.Exists(ConfigConst.ExcelResPath))
                File.Delete(ConfigConst.ExcelResPath);

            AssetDatabase.CreateAsset(configAsset, ConfigConst.ConfigResPath + configAsset.Name + ".asset");
        }

        AssetDatabase.Refresh();
    }
}
