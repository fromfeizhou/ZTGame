using UnityEditor;
using UnityEngine;
using System.IO;
public class TerrainSlicing : Editor
{
    private const string MapShader = "Custom/Environment/LightedTerrain";
    private const string MapMaterialSavePath = "Assets/ResourcesLib/Map/MapMaterial.mat";

    //开始分割地形
    [MenuItem("Terrain/Slicing")]
	private static void Slicing()
	{
		EditorUtility.ClearProgressBar();
	    MapAsset mapAsset = new MapAsset();

		Terrain terrain = GameObject.Find("MainTerrain").GetComponent<Terrain>();

		if (terrain.terrainData.size.x % MapDefine.MAPITEMSIZE != 0) {
			Debug.LogErrorFormat("[TerrainSlicing]terrainSize:{0}, MapItemSize:{1}",terrain.terrainData.size,MapDefine.MAPITEMSIZE);
			return;
		}

		Transform mapRoot = (new GameObject ("MapRoot")).transform;
		mapRoot.position = Vector3.zero;
		mapRoot.eulerAngles = Vector3.zero;
		mapRoot.localScale = Vector3.one;

		if (terrain == null)
		{
			Debug.LogError("找不到地形!");
			return;
		}

		if (Directory.Exists(MapDefine.TERRAIN_ASSET_PATH))
			Directory.Delete(MapDefine.TERRAIN_ASSET_PATH, true);
		Directory.CreateDirectory(MapDefine.TERRAIN_ASSET_PATH);

		TerrainData terrainData = terrain.terrainData;

		int SlicingBlockCnt = (int)terrainData.size.x / MapDefine.MAPITEMSIZE;

	    if (Directory.Exists(MapDefine.TERRAIN_PREFAB_PATH))
	        Directory.Delete(MapDefine.TERRAIN_PREFAB_PATH, true);
	    Directory.CreateDirectory(MapDefine.TERRAIN_PREFAB_PATH);

        int heightmapResolution = (terrainData.heightmapResolution - 1) / SlicingBlockCnt;
		int alphamapResolution = terrainData.alphamapResolution / SlicingBlockCnt;
		int baseMapResolution = terrainData.baseMapResolution / SlicingBlockCnt;
		Vector3 size = new Vector3(terrainData.size.x / SlicingBlockCnt, terrainData.size.y,  terrainData.size.z / SlicingBlockCnt);

	    Material mat = new Material(Shader.Find(MapShader));
	    for (int i = 0; i < terrainData.splatPrototypes.Length; i++)
	    {
	        var sp = terrainData.splatPrototypes[i];
	        mat.SetTexture("_Texture" + i, sp.texture);
	    }
        AssetDatabase.CreateAsset(mat, MapMaterialSavePath);

        SplatPrototype[] splatProtos = terrainData.splatPrototypes;

	    EditorUtility.ClearProgressBar();

        for (int row = 0; row < SlicingBlockCnt; ++row)
	    {
            for (int col = 0; col < SlicingBlockCnt; ++col)
			{
			    EditorUtility.DisplayProgressBar("地形切割中", row+"_"+ col, (row * SlicingBlockCnt + col) / (SlicingBlockCnt * SlicingBlockCnt));

                TerrainData newData = new TerrainData();
				string terrainName = string.Format (MapDefine.MAPKEYNAME, row, col);
				newData.heightmapResolution = heightmapResolution;
				newData.alphamapResolution = alphamapResolution;
				newData.baseMapResolution = baseMapResolution;
				newData.size = size;
				SplatPrototype[] newSplats = new SplatPrototype[splatProtos.Length];
				for (int i = 0; i < splatProtos.Length;  ++i)
				{
					newSplats[i] = new SplatPrototype();
					newSplats[i].texture = splatProtos[i].texture;
					newSplats[i].tileSize = splatProtos[i].tileSize;

					float offsetX = (newData.size.x * col) % splatProtos[i].tileSize.x + splatProtos[i].tileOffset.x;
					float offsetY = (newData.size.z * row) % splatProtos[i].tileSize.y + splatProtos[i].tileOffset.y;
					newSplats[i].tileOffset = new Vector2(offsetX, offsetY);
				}
				newData.splatPrototypes = newSplats;

				float[,,] alphamap = new float[alphamapResolution, alphamapResolution, splatProtos.Length];
				alphamap = terrainData.GetAlphamaps(col * newData.alphamapWidth, row * newData.alphamapHeight, newData.alphamapWidth, newData.alphamapHeight);
				newData.SetAlphamaps(0, 0, alphamap);

				int xBase = terrainData.heightmapWidth / SlicingBlockCnt;
				int yBase = terrainData.heightmapHeight / SlicingBlockCnt;

				float[,] height = terrainData.GetHeights(xBase * col, yBase * row, xBase + 1, yBase + 1);
				newData.SetHeights(0, 0, height);

				GameObject terrainBase = new GameObject (terrainName, typeof(Terrain),typeof(TerrainCollider));
				terrainBase.transform.SetParent (mapRoot);

                Terrain terrainBlock = terrainBase.GetComponent<Terrain> ();
				terrainBlock.terrainData = newData;
				terrainBlock.materialType = terrain.materialType;
				terrainBlock.materialTemplate = terrain.materialTemplate;

				TerrainCollider terrainCollider = terrainBase.GetComponent<TerrainCollider> ();
				terrainCollider.terrainData = newData;

                Debug.Log("CreateMapTerrain:" + terrainName);
			    TerrainToMesh(terrainBase, mat, terrainName);
            }
        }

        //Collect Tree
        Transform TreeRoot = GameObject.Find ("[Trees]").transform;
		for (int i = 0; i < TreeRoot.childCount; i++) {
			Transform tree = TreeRoot.GetChild (i);
			int col = (int)tree.localPosition.x / MapDefine.MAPITEMSIZE;
			int row = (int)tree.localPosition.z / MapDefine.MAPITEMSIZE;
			string mapKey = string.Format (MapDefine.MAPKEYNAME, row, col);
		    MapItemInfoBase mapItemInfoBase = new MapItemInfoBase();
		    mapItemInfoBase.Pos = tree.localPosition - new Vector3(col * MapDefine.MAPITEMSIZE, 0, row * MapDefine.MAPITEMSIZE);
		    mapItemInfoBase.Angle = Vector3.zero;
		    mapItemInfoBase.Scale = tree.localScale;
		    mapAsset.AddMapItem(mapKey,eMapItemType.Tree, mapItemInfoBase);
        }

	    AssetDatabase.CreateAsset(mapAsset, MapDefine.MAPITEMINFOASSET);
		GameObject.DestroyImmediate (mapRoot.gameObject);
		EditorUtility.ClearProgressBar();
	}



    //地形转换为Mesh
    private static void TerrainToMesh(GameObject terrainObj,Material mat, string mapKey)
    {
        var terrain = terrainObj.GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.Log("terrain == null");
            return;
        }

        var terrainData = terrain.terrainData;
        if (terrainData == null)
        {
            Debug.Log("terrainData == null");
            return;
        }

        int vertexCountScale = 4;       // [dev] 将顶点数稀释 vertexCountScale*vertexCountScale 倍
        int w = terrainData.heightmapWidth;
        int h = terrainData.heightmapHeight;
        Vector3 size = terrainData.size;
        float[,,] alphaMapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        Vector3 meshScale = new Vector3(size.x / (w - 1f) * vertexCountScale, 1, size.z / (h - 1f) * vertexCountScale);
        Vector2 uvScale = new Vector2(1f / (w - 1f), 1f / (h - 1f)) * vertexCountScale * (size.x / terrainData.splatPrototypes[0].tileSize.x);     // [dev] 此处有问题，若每个图片大小不一，则出问题。日后改善

        w = (w - 1) / vertexCountScale + 1;
        h = (h - 1) / vertexCountScale + 1;
        Vector3[] vertices = new Vector3[w * h];
        Vector2[] uvs = new Vector2[w * h];
        Vector4[] alphasWeight = new Vector4[w * h];            // [dev] 只支持4张图片

        // 顶点，uv，每个顶点每个图片所占比重
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                int index = j * w + i;
                float z = terrainData.GetHeight(i * vertexCountScale, j * vertexCountScale);
                vertices[index] = Vector3.Scale(new Vector3(i, z, j), meshScale);
                uvs[index] = Vector2.Scale(new Vector2(i, j), uvScale);

                // alpha map
                int i2 = (int)(i * terrainData.alphamapWidth / (w - 1f));
                int j2 = (int)(j * terrainData.alphamapHeight / (h - 1f));
                i2 = Mathf.Min(terrainData.alphamapWidth - 1, i2);
                j2 = Mathf.Min(terrainData.alphamapHeight - 1, j2);
                var alpha0 = alphaMapData[j2, i2, 0];
                var alpha1 = alphaMapData[j2, i2, 1];
                var alpha2 = alphaMapData[j2, i2, 2];
                var alpha3 = alphaMapData[j2, i2, 3];
                alphasWeight[index] = new Vector4(alpha0, alpha1, alpha2, alpha3);
            }
        }

        int[] triangles = new int[(w - 1) * (h - 1) * 6];
        int triangleIndex = 0;
        for (int i = 0; i < w - 1; i++)
        {
            for (int j = 0; j < h - 1; j++)
            {
                int a = j * w + i;
                int b = (j + 1) * w + i;
                int c = (j + 1) * w + i + 1;
                int d = j * w + i + 1;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = b;
                triangles[triangleIndex++] = c;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = c;
                triangles[triangleIndex++] = d;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.tangents = alphasWeight;       // 将地形纹理的比重写入到切线中


        GameObject go = new GameObject(mapKey, typeof(MeshFilter), typeof(MeshRenderer));
        Transform t = go.transform;

        // 地形渲染
        MeshRenderer mr = t.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;

        t.parent = terrainObj.transform.parent;
        t.position = terrainObj.transform.position;
        t.gameObject.layer = terrainObj.layer;

        GameObject treeRoot = new GameObject("[Tree]");
        treeRoot.transform.SetParent(t);
        treeRoot.transform.localPosition = Vector3.zero;
        treeRoot.transform.localEulerAngles = Vector3.zero;
        treeRoot.transform.localScale = Vector3.one;
        GameObject prefabGo = PrefabUtility.CreatePrefab(MapDefine.TERRAIN_PREFAB_PATH + mapKey + ".prefab", t.gameObject);

        prefabGo.GetComponent<MeshFilter>().sharedMesh = mesh;
        AssetDatabase.CreateAsset(mesh, MapDefine.TERRAIN_ASSET_PATH + "Mesh_" + mapKey + ".asset");

        AssetDatabase.Refresh();
    }


   
}