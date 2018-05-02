using System;
using System.Collections.Generic;
using UnityEngine;

public class PiecewiseLoaderMgr : Singleton<PiecewiseLoaderMgr>
{

    private List<PiecewiseLoader> loaders = new List<PiecewiseLoader>();

    public void AddLoaders(PiecewiseLoader loader)
    {
        if (!loaders.Contains(loader))
        {
           // Debug.LogError("Add PiecewiseLoader-----------------------------");
            loaders.Add(loader);
        }
    }

    public void RemoveLoader(PiecewiseLoader loader)
    {
        for (int index = 0; index < loaders.Count; index++)
        {
            if (loaders[index] == loader)
            {
                loader.Release();
                loaders.RemoveAt(index);
               // Debug.LogError("Remove PiecewiseLoader-----------------------------");
                break;
            }
        }
    }

    public void UpdateLoaders(float time)
    {
        for (int index = 0; index < loaders.Count; index++)
        {
            loaders[index].UpdateLoader(time);
        }
    }

}