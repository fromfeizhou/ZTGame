using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class MapColliderEditor : EditorWindow
{
    private Camera _cameraEditorPort;
    private Camera _cameraMain;
    private Camera _cameraEditor;
    [MenuItem("Window/MyWindow")]//在unity菜单Window下有MyWindow选项
    private static void MyWindow()
    {
        MapColliderEditor myWindow = GetWindowWithRect<MapColliderEditor>(new Rect(0,0,1920,800),false,"地图热区编辑器");
        myWindow.Init();
        myWindow.Show();//展示
    }

    private void Init()
    {
        if (_cameraEditorPort == null)
        {
            _cameraEditorPort = GameObject.Find("CameraEditorPort").GetComponent<Camera>();
            _cameraEditorPort.targetTexture = new RenderTexture(4096, 4096, 0);
        }
        if (_cameraMain == null)
        {
            _cameraMain = GameObject.Find("CameraMain").GetComponent<Camera>(); 
            _cameraMain.targetTexture = new RenderTexture(4096, 4096, 0);
        }
        if (_cameraEditor == null)
        {
            _cameraEditor = GameObject.Find("CameraEditor").GetComponent<Camera>();
            _cameraEditor.targetTexture = new RenderTexture(4096, 4096, 0);
        }

        InitMapData();
    }

    private const float MaxScale = 640;
    private const float MinScale = 1.0f;

    private float _viewPortPosX = 0.0f;
    private float _viewPortPosY = 0.0f;

    private float _progress;

    private void InitMapData()
    {
        if (File.Exists(MapDefine.MapDataSavePath))
        {
            _mapBlockData = new List<MapBlockData>();
            string[] contents = File.ReadAllLines(MapDefine.MapDataSavePath);
            for (int i = 0; i < contents.Length; i++)
            {
                if (!string.IsNullOrEmpty(contents[i]))
                {
                    _mapBlockData.Add(MapBlockData.Parse(contents[i]));
                }
            }

        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 670, 100, 20), "保存"))
        {
            if (_mapBlockData != null && _mapBlockData.Count > 0)
            {
                string mapData = string.Empty;
                for (int i = 0; i < _mapBlockData.Count; i++)
                {
                    if(_mapBlockData[i].type == eMapBlockType.None)
                        continue;

                    mapData += _mapBlockData[i] + "\n";
                }
                if(File.Exists(MapDefine.MapDataSavePath))
                    File.Delete(MapDefine.MapDataSavePath);
                File.WriteAllText(MapDefine.MapDataSavePath, mapData.Trim());
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("没有数据");
            }
        }

        EditorGUI.LabelField(new Rect(0, 690, 400, 20), "curMousePos:" + curMousePos.x + ", " + (15- curMousePos.y));
        EditorGUI.LabelField(new Rect(0, 710, 400, 20), "curSelectMapBlockPos:" + curSelectMapBlockPos);
        EditorGUI.LabelField(new Rect(0, 730, 400, 20), "curMousePosGrid:" + curSelectMapBlockPos.x / 40 + ", " + (15- curSelectMapBlockPos.y / 40));
        EditorGUI.LabelField(new Rect(0, 750, 400, 20), "editorGridPortPos:" + editorGridPortPos);

        EditorGUI.LabelField(new Rect(640, 690, 400, 20), "editorGridPortPo1s:" + (editorGridPortPos.x - 640) / 40 + ", " + (15 - editorGridPortPos.y / 40));
        EditorGUI.LabelField(new Rect(1280, 690, 400, 20), "editosdfsdfrGridPortPos2:" + (editorGridPortPos2.x - 1280) / 32 + ", " + (19 - editorGridPortPos2.y / 32));

        float dddddx = (editorGridPortPos.x - 640) / 40 + curSelectMapBlockPos.x / 40 * 16;
        float dddddy = 15 - editorGridPortPos.y / 40 + (15 - curSelectMapBlockPos.y / 40) * 16;

        EditorGUI.LabelField(new Rect(640, 710, 400, 20), "editorGridP2ortPos:" + dddddx + ", " + dddddy);

        lastRow = (int)((editorGridPortPos2.x - 1280) / 32 + dddddx * 20);
        lastCol = (int)(19 - editorGridPortPos2.y / 32 + dddddy * 20);
        EditorGUI.LabelField(new Rect(1280, 710, 400, 20), "editorGridPortPos22:" + lastRow + ", " + lastCol);
        if (keyCodeColor == null)
            InitKeyCodeColor();
        EditorGUI.DrawRect(new Rect(1280, 730, 16, 16), keyCodeColor[MapBlockType]);
        EditorGUI.LabelField(new Rect(1280 + 18, 730, 400, 20), "MapBlockType:" + MapBlockType);

        EditorGUI.LabelField(new Rect(1280, 750, 400, 20), "DSRow:" + DSRow + ",  DSCol:" + DSCol);


        //MapBlockType
        UserInput();
        DrawMapTex();
        DrawGridPrev();
        Repaint();
    }

    private int lastRow;
    private int lastCol;

    private Vector2 MapViewPos = new Vector2(0,0);
    private Vector2 MapViewSize = new Vector2(640, 640);


    private Vector2 oldPos;
    private float tmpCameraSize;

    private Vector2 MoveOldPos;
    private float MoveOldPos_X;
    private float MoveOldPos_Y;

    private Dictionary<KeyCode, eMapBlockType> keyCodeDic;
    private Dictionary<eMapBlockType, Color> keyCodeColor;

    private void InitKeyCodeDic()
    {
        keyCodeDic = new Dictionary<KeyCode, eMapBlockType>();
        keyCodeDic[KeyCode.Alpha1] = eMapBlockType.None;
        keyCodeDic[KeyCode.Alpha2] = eMapBlockType.Collect;
        keyCodeDic[KeyCode.Alpha3] = eMapBlockType.Hide;
        keyCodeDic[KeyCode.Alpha4] = eMapBlockType.Event;

    }

    private void InitKeyCodeColor()
    {
        keyCodeColor = new Dictionary<eMapBlockType, Color>();
        keyCodeColor[eMapBlockType.None] = new Color(1, 0, 0, 0.2f);
        keyCodeColor[eMapBlockType.Collect] = new Color(0, 1, 0, 0.2f);
        keyCodeColor[eMapBlockType.Hide] = new Color(1, 1, 1, 0.2f);
        keyCodeColor[eMapBlockType.Event] = new Color(0, 0, 1, 0.2f);

    }

    private void UserInput()
    {
        if(keyCodeDic == null)
            InitKeyCodeDic();
        Event e = Event.current;
        if (e.mousePosition.x < MapViewSize.x && e.mousePosition.y < MapViewSize.y)
        {
            if (e.alt && e.button == 1)
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                    {
                        tmpCameraSize = _cameraEditor.orthographicSize;
                        oldPos = e.mousePosition;
                        break;
                    }
                    case EventType.MouseDrag:
                    {
                        Vector2 tmpValue = e.mousePosition - oldPos;

                        if (_editorModel)
                            tmpValue *= 0.01f;
                        _cameraEditor.orthographicSize = tmpCameraSize + tmpValue.x + tmpValue.y;
                        break;
                    }
                }
            }

        }
    }

    private float _editorModel_MinSize = 2.0f;
    private float _editorModel_ManSize = 16.0f;

    private bool _editorModel;

    private eMapBlockType MapBlockType = eMapBlockType.None;
    private void DrawMapTex()
    {
        Vector2 startPos = new Vector2(MapViewSize.x, 0);
        Vector2 endPos = startPos + MapViewSize;

        Vector2 editorStartPos = new Vector2(startPos.x + MapViewSize.x, 0);
        Vector2 editorEndPos = editorStartPos + MapViewSize;

        GUI.DrawTexture(new Rect(Vector2.zero, MapViewSize), _cameraMain.targetTexture); 
        GUI.DrawTexture(new Rect(startPos, MapViewSize ), _cameraEditorPort.targetTexture);
        GUI.DrawTexture(new Rect(new Vector2(MapViewSize.x * 2.0f,0), MapViewSize), _cameraEditor.targetTexture);
      
        Event e = Event.current;
        if (e.mousePosition.x > startPos.x && e.mousePosition.y > startPos.y)
        {
            if (e.mousePosition.x < endPos.x && e.mousePosition.y < endPos.y)
            {
                if (e.button == 1)
                    editorPortPos = e.mousePosition;
            }
        }

        if (e.mousePosition.x > editorStartPos.x && e.mousePosition.y > editorStartPos.y)
        {
            if (e.mousePosition.x < editorEndPos.x && e.mousePosition.y < editorEndPos.y)
            {
                if (e.button == 1)
                    editorPos = e.mousePosition;

                if (e.isKey)
                {
                    if (keyCodeDic.ContainsKey(e.keyCode))
                    {
                        if(MapBlockType != keyCodeDic[e.keyCode])
                            MapBlockType = keyCodeDic[e.keyCode];
                    }

                    if (e.control)
                    {
                        if (_mapBlockData != null)
                        {
                            int index = _mapBlockData.FindIndex(a => a.row == lastRow && a.col == lastCol);
                            if (index >= 0)
                                _mapBlockData[index].type = MapBlockType;
                            else
                            {
                                _mapBlockData.Add(new MapBlockData { row = lastRow, col = lastCol, type = MapBlockType });
                            }
                        }
                    }
                }
            }
        }


        Color color = new Color(1, 0, 0, 0.2f);
        float editorGridSize = 40f;
        int row = (int) (editorPortPos.x / editorGridSize);
        int col = (int) (editorPortPos.y / editorGridSize);
        editorGridPortPos = new Vector2(row, col) * editorGridSize;
        Handles.DrawSolidRectangleWithOutline(new Rect(editorGridPortPos, editorGridSize * Vector2.one), color, color);
        _cameraEditor.transform.localPosition =
            new Vector3(row * 5 + 2.5f - editorGridSize * 2, 200, (15 - col) * 5 + 2.5f) +
            _cameraEditorPort.transform.localPosition - _cameraGridSize * 0.5f;

        int eerow = (int)(editorPos.x / editorGridSize2);
        int eecol = (int)(editorPos.y / editorGridSize2);
        editorGridPortPos2 = new Vector2(eerow, eecol) * editorGridSize2;
        Handles.DrawSolidRectangleWithOutline(new Rect(editorGridPortPos2, editorGridSize2 * Vector2.one), keyCodeColor[MapBlockType], keyCodeColor[MapBlockType]);
    }
        float editorGridSize2 = 32;

    private Vector2 editorGridPortPos;
    private Vector2 editorGridPortPos2;

    private Vector2 editorPortPos;
    private Vector2 editorPos;

    private float GridSize
    {
        get { return MapDefine.GridSize;}
    }

    private Vector2 curMousePos;
    private Vector2 CameraEditorPos;

    private void DrawGridEditor()
    {
        int gridCnt = (int)(640.0f / 32);
        for (int row = 0; row <= gridCnt; row++)
        {
            Handles.DrawLine(new Vector2(1280 + 0, 32 * row), new Vector2(1280 + 32 * gridCnt, 32 * row));//row
            Handles.DrawLine(new Vector2(1280 + 32 * row, 0), new Vector2(1280 + 32 * row, 32 * gridCnt));//col
        }
    }
    private void DrawGridEditorPort()
    {
        int gridCnt = (int)(640.0f / 40);
        for (int row = 0; row <= gridCnt; row++){
            Handles.DrawLine(new Vector2(640 + 0, 40 * row), new Vector2(640 + 40 * gridCnt, 40 * row));//row
            Handles.DrawLine(new Vector2(640 + 40 * row, 0), new Vector2(640 + 40 * row, 40 * gridCnt));//col
        }
    }

    private void DrawGridPrev()
    {
        int gridCnt = (int)(640.0f / GridSize);

        Handles.color = Color.red;
        Handles.BeginGUI();
        for (int row = 0; row <= gridCnt; row++)
        {
            Handles.DrawLine(new Vector2(0, GridSize * row), new Vector2(GridSize * gridCnt, GridSize * row));//row
            Handles.DrawLine(new Vector2(GridSize * row, 0), new Vector2(GridSize * row, GridSize * gridCnt));//col
        }

        Event e = Event.current;
        DrawSelectGrid();
        if (Event.current.mousePosition.x < 640 && Event.current.mousePosition.y < 640)
        {
            if (Event.current.mousePosition.x >= 0 && Event.current.mousePosition.y >= 0)
            {
                curMousePos.x = (int) (Event.current.mousePosition.x / GridSize);
                curMousePos.y = (int) (Event.current.mousePosition.y / GridSize);

                Handles.color = Color.blue;

                Vector3[] points =
                {
                    curMousePos * GridSize,
                    curMousePos * GridSize + new Vector2(GridSize, 0),
                    curMousePos * GridSize + new Vector2(GridSize, GridSize),
                    curMousePos * GridSize + new Vector2(0, GridSize),
                    curMousePos * GridSize
                };

                Handles.DrawAAPolyLine(4.0f, points.Length, points);

                if (e.button == 1)
                {
                    CurSelectMapBlockPos = curMousePos * GridSize;
                }
            }
        }
        DrawGridEditor();
        DrawGridEditorPort();
        DrawData();
        Handles.EndGUI();
    }

    private int DSRow;
    private int DSCol;
    private List<MapBlockData> _mapBlockData;
    private void DrawData()
    {
        DSRow = (int)(((editorGridPortPos.x - 640) / 40 + curSelectMapBlockPos.x / 40 * 16) * 20);
        DSCol = (int)((15 - editorGridPortPos.y / 40 + (15 - curSelectMapBlockPos.y / 40) * 16) * 20);

        int maxDSRow = DSRow + 20;
        int maxDSCol = DSCol + 20;

        List<MapBlockData> tmpBlockData = _mapBlockData.FindAll(a => a.row >= DSRow && a.row < maxDSRow && a.col >= DSCol && a.col < maxDSCol);
        for (int i = 0; i < tmpBlockData.Count; i++)
        {
            if (tmpBlockData[i].type != eMapBlockType.None)
            {
                Vector2 pos = new Vector2(tmpBlockData[i].row%20 * editorGridSize2 + MapViewSize.x * 2, (19- tmpBlockData[i].col % 20) * editorGridSize2);
                Handles.DrawSolidRectangleWithOutline(new Rect(pos, editorGridSize2 * Vector2.one), keyCodeColor[tmpBlockData[i].type], keyCodeColor[tmpBlockData[i].type]);
            }
        }
    }

    private Vector2 curSelectMapBlockPos;

    private Vector2 CurSelectMapBlockPos
    {
        get { return curSelectMapBlockPos; }
        set
        {
            if (curSelectMapBlockPos != value)
                UpdateEditorCameraPos();
            curSelectMapBlockPos = value;
        }
    }

    private Vector3 _cameraGridSize = new Vector3(80,0,80);
    private void UpdateEditorCameraPos()
    {
        float cameraPosX = curMousePos.x * _cameraGridSize.x;
        float cameraPosY = (15- curMousePos.y) * _cameraGridSize.z;
        _cameraEditorPort.transform.localPosition = new Vector3(cameraPosX,200, cameraPosY) + _cameraGridSize * 0.5f;
    }

    private void DrawSelectGrid()
    {
        Color color = new Color(1, 0, 0, 0.2f);
        Handles.DrawSolidRectangleWithOutline(new Rect(CurSelectMapBlockPos, GridSize * Vector2.one), color, color);
    }
    
    

    //将RenderTexture保存成一张png图片  
    public bool SaveRenderTextureToPNG(RenderTexture rt, string contents, string pngName)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToPNG();
        if (!Directory.Exists(contents))
            Directory.CreateDirectory(contents);
        FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
        Texture2D.DestroyImmediate(png);
        png = null;
        RenderTexture.active = prev;
        return true;

    }
}