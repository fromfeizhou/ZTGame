
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ConfigAsset : ScriptableObject {
    public string Name;
    public int MaxRow;
    public int MaxCol;
    public string[] Datas;
    public string this[int row,int col]
    {
        get
        {
            int index = row * MaxCol + col;
            return Datas[index];
        }
    }
}
