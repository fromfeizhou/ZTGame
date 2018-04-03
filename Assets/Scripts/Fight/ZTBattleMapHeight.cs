using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
[LuaCallCSharp]
public class ZTBattleMapHeight{
    private const float MapHeightGap = 0.2f;
    private const int MapDataMax = 8000;

    private Dictionary<int, Dictionary<int, float>> _mapHeightCacheA;
    private Dictionary<int, Dictionary<int, float>> _mapHeightCacheB;
    private int _heightCount = 0;
    private int _clearIndex = 0;

    //private TerrainData _terrainData;
    //初始化
    public void Init()
    {
        
        _heightCount = 0;
        _clearIndex = 0;
        ClearCache();

        //AssetManager.LoadAsset("Assets/Map/Model/MainTerrain.asset", (Object target, string path) =>
        // {
        //     TerrainData terrainData = (TerrainData)target;
        //     if(null != terrainData)
        //     {
        //         _terrainData = terrainData;
        //     }
        // });
    }

    public void Destroy()
    {
        _mapHeightCacheA = null;
        _mapHeightCacheB = null;
    }

    //获得高度
    public float GetHeight(Vector3 pos)
    {
        if (null == _mapHeightCacheA && null == _mapHeightCacheB) return 0;

        float tempHeight = 0;
        if (MapManager.GetInstance().GetCurMapBlockHeight(pos, ref tempHeight))
        {
            return tempHeight;
        }

        int row = Mathf.FloorToInt(pos.x / MapHeightGap);
        int column = Mathf.FloorToInt(pos.z / MapHeightGap);
        if (null != _mapHeightCacheA && _mapHeightCacheA.ContainsKey(row) && _mapHeightCacheA[row].ContainsKey(column))
        {
            return _mapHeightCacheA[row][column];
        }
        if (null != _mapHeightCacheB && _mapHeightCacheB.ContainsKey(row) && _mapHeightCacheB[row].ContainsKey(column))
        {
            return _mapHeightCacheB[row][column];
        }

        if(_clearIndex == 0)
        {
            return GetHeightInCache(_mapHeightCacheA, row, column, pos);
        }
        else
        {
            return GetHeightInCache(_mapHeightCacheB, row, column, pos);
        }
    }

    public float GetHeightInCache(Dictionary<int, Dictionary<int, float>> cache,int row,int column,Vector3 pos)
    {
        if (!cache.ContainsKey(row))
        {
            cache.Add(row, new Dictionary<int, float>());
        }
        if (!cache[row].ContainsKey(column))
        {
            RaycastHit hit;
            bool result = Physics.Raycast(pos + Vector3.up * 5, Vector3.down, out hit, 6f, LayerMask.GetMask("Terrain"));
            if (result)
            {
                cache[row].Add(column, hit.point.y);
            }
            else
            {
                cache[row].Add(column, 0);
            //    Debug.LogError("ZTBattleMapHeight:Error");
            }
            _heightCount++;
            if (_heightCount > MapDataMax)
            {
                _heightCount = 0;
                _clearIndex = _clearIndex == 0 ? 1 : 0;
                ClearCache();
            }
        }
        
        return cache[row][column];
    }


    private void ClearCache()
    {
        Debug.Log("ClearCache");
        if (_clearIndex == 0)
        {
            _mapHeightCacheA = null;
            _mapHeightCacheA = new Dictionary<int, Dictionary<int, float>>();
        }
        else
        {
            _mapHeightCacheB = null;
            _mapHeightCacheB = new Dictionary<int, Dictionary<int, float>>();
        }
    }
}
