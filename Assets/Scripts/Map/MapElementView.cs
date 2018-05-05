using System;
using System.Collections.Generic;
using UnityEngine;

public class MapElementView
{
    private MapElementLoader loader;


    public MapElementView(MapElementLoaderData data)
    {
        loader = new MapElementLoader(data);
    }

    public void LoaderElement(Dictionary<string, MapElement> data)
    {
        if (data == null) return;
        loader.LoadElement(data);
    }

}
