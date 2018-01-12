using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigDemo : MonoBehaviour {
    void Start()
    {
        AssetManager.LoadAsset(ConfigConst.ConfigResPath + "test.asset" , (obj, str) =>
        {
            ConfigAsset config = obj as ConfigAsset;
            for (int row = 0; row < config.MaxRow; row++)
            {
                for (int col = 0; col < config.MaxCol; col++)
                {
                    Debug.Log(string.Format("{0}:{1} =>{2}", row, col, config[row, col]));
                }
            }
        });
    }
}
