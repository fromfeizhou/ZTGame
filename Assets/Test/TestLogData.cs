using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLogData : MonoBehaviour
{
    private Terrain terrain;
	// Use this for initialization
	void Start ()
	{

	    terrain = transform.GetComponent<Terrain>();
	    TerrainData data = terrain.terrainData;
        Debug.LogError(data.size);
        Debug.LogError(data.heightmapHeight);
	    Debug.LogError(data.heightmapWidth);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
