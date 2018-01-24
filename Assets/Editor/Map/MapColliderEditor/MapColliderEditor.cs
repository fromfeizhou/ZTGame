using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class MapColliderEditor : EditorWindow
{
    [MenuItem("Window/MyWindow")]//在unity菜单Window下有MyWindow选项
    private static void MyWindow()
    {
        MapColliderEditor myWindow = GetWindowWithRect<MapColliderEditor>(new Rect(0,0,1920,800),false,"地图热区编辑器");
        myWindow.Show();//展示
    }
    private Vector2 MapViewSize = new Vector2(640, 640);

    private Camera _cameraMain;
    private Camera _cameraPort;
    private Camera _cameraEdit;

    private Vector2 _curSelectGridPos_Main;
    private Vector2 _curSelectGridPos_Port;
    private Vector2 _curSelectGridPos_Edit;

    private Color _curSelectGridColor_Main
    {
        get { return new Color(1, 0, 0, 0.2f); }
    }
    private Color _curSelectGridColor_Port
    {
        get { return new Color(1, 0, 0, 0.2f); }
    }
    private Color _curSelectGridColor_Edit
    {
        get { return MapDefine.MapBlockTypeColor[(int)MapBlockType]; }
    }

    private Vector3 _cameraPos_Port;
    private Vector3 _cameraPos_Edit;

    private float _cameraGridPort_Size
    {
        get { return MapDefine.MAPITEMTOTALSIZE / (MapViewSize.x / MapDefine.GridSize_Main); }
    }
    private float _cameraGridEdit_Size
    {
        get { return _cameraGridPort_Size / (MapViewSize.x / MapDefine.GridSize_Port); }
    }

    private int _gridCnt_Main
    {
        get { return (int)(MapViewSize.x / MapDefine.GridSize_Main); }
    }
    private int _gridCnt_Port
    {
        get { return (int)(MapViewSize.x / MapDefine.GridSize_Port); }
    }
    private int _gridCnt_Edit
    {
        get { return (int)(MapViewSize.x / MapDefine.GridSize_Edit); }
    }

    private List<MapBlockData> _mapBlockData;
    private eMapBlockType MapBlockType = eMapBlockType.None;

    void OnEnable()
    {
        if (_cameraPort == null)
        {
            _cameraPort = GameObject.Find("CameraPort").GetComponent<Camera>();
            _cameraPort.targetTexture = new RenderTexture(4096, 4096, 0);
        }
        if (_cameraMain == null)
        {
            _cameraMain = GameObject.Find("CameraMain").GetComponent<Camera>();
            _cameraMain.targetTexture = new RenderTexture(4096, 4096, 0);
        }
        if (_cameraEdit == null)
        {
            _cameraEdit = GameObject.Find("CameraEdit").GetComponent<Camera>();
            _cameraEdit.targetTexture = new RenderTexture(4096, 4096, 0);
        }

        if (_mapBlockData == null || _mapBlockData.Count == 0)
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

    }
    void OnGUI()
    {
        UserInput();
        MenuInput();
        DrawMap();
        UpdateCameraPos();
        DrawData();
        Repaint();
    }

    void UserInput()
    {
        Event e = Event.current;
        /*
        if (e.isKey)
        {
            if (e.keyCode >= KeyCode.Alpha1 && (int)e.keyCode < (int)KeyCode.Alpha1 + (int)eMapBlockType.Count)
            {
                eMapBlockType tmpMapBlockType = (eMapBlockType)(e.keyCode - KeyCode.Alpha1);
                if (MapBlockType != tmpMapBlockType)
                    MapBlockType = tmpMapBlockType;
            }

            if (e.control && _mapBlockData != null)
            {
                int index = _mapBlockData.FindIndex(a => a.row == lastRow && a.col == lastCol);
                if (index >= 0)
                    _mapBlockData[index].type = MapBlockType;
                else
                    _mapBlockData.Add(new MapBlockData { row = lastRow, col = lastCol, type = MapBlockType });
            }
        }*/

        if (e.isMouse)
        {
            if (e.button == 1)
            {
                if (e.mousePosition.x >= 0 && e.mousePosition.y >= 0)
                {
                    if (e.mousePosition.x < MapViewSize.x && e.mousePosition.y < MapViewSize.y)
                    {
                        _cameraPos_Port.x = (int)(e.mousePosition.x / MapDefine.GridSize_Main);
                        _cameraPos_Port.y = (int)(e.mousePosition.y / MapDefine.GridSize_Main);
                        _curSelectGridPos_Main = _cameraPos_Port * MapDefine.GridSize_Main;
                    }
                }
                if (e.mousePosition.x >= MapViewSize.x && e.mousePosition.y >= 0)
                {
                    if (e.mousePosition.x < MapViewSize.x * 2 && e.mousePosition.y < MapViewSize.y)
                    {
                        _cameraPos_Edit.x = (int)(e.mousePosition.x / MapDefine.GridSize_Port);
                        _cameraPos_Edit.y = (int)(e.mousePosition.y / MapDefine.GridSize_Port);
                        _curSelectGridPos_Port = _cameraPos_Edit * MapDefine.GridSize_Port;
                    }
                }
                if (e.mousePosition.x >= MapViewSize.x * 2 && e.mousePosition.y >= 0)
                {
                    if (e.mousePosition.x < MapViewSize.x * 3 && e.mousePosition.y < MapViewSize.y)
                    {
                        _curSelectGridPos_Edit.x = (int)(e.mousePosition.x / MapDefine.GridSize_Edit) * MapDefine.GridSize_Edit;
                        _curSelectGridPos_Edit.y = (int)(e.mousePosition.y / MapDefine.GridSize_Edit) * MapDefine.GridSize_Edit;
                    }
                }
            }
        }
    }

    private void UpdateCameraPos()
    {
        Vector3 localPos_Port;
        localPos_Port.x = _cameraPos_Port.x * _cameraGridPort_Size + _cameraGridPort_Size * 0.5f;
        localPos_Port.y = 200;
        localPos_Port.z = (15 - _cameraPos_Port.y) * _cameraGridPort_Size + _cameraGridPort_Size * 0.5f;
        _cameraPort.transform.localPosition = localPos_Port;


        Vector3 localPos_Edit;
        localPos_Edit.x = (_cameraPos_Edit.x + 0.5f) * _cameraGridEdit_Size - _cameraGridPort_Size * 1.5f;
        localPos_Edit.y = 200;
        localPos_Edit.z = (15 - _cameraPos_Edit.y + 0.5f) * _cameraGridEdit_Size - _cameraGridPort_Size * 0.5f;
        _cameraEdit.transform.localPosition = localPos_Edit + localPos_Port;
    }

    void MenuInput()
    {
        if (GUI.Button(new Rect(0, 670, 100, 20), "保存"))
        {
            if (_mapBlockData != null && _mapBlockData.Count > 0)
            {
                string mapData = string.Empty;
                for (int i = 0; i < _mapBlockData.Count; i++)
                {
                    if (_mapBlockData[i].type == eMapBlockType.None)
                        continue;

                    mapData += _mapBlockData[i] + "\n";
                }
                if (File.Exists(MapDefine.MapDataSavePath))
                    File.Delete(MapDefine.MapDataSavePath);
                File.WriteAllText(MapDefine.MapDataSavePath, mapData.Trim());
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("没有数据");
            }
        }

    }

    void DrawMap()
    {
        GUI.DrawTexture(new Rect(Vector2.zero, MapViewSize), _cameraMain.targetTexture);
        GUI.DrawTexture(new Rect(new Vector2(MapViewSize.x, 0), MapViewSize), _cameraPort.targetTexture);
        GUI.DrawTexture(new Rect(new Vector2(MapViewSize.x * 2, 0), MapViewSize), _cameraEdit.targetTexture);

        Handles.DrawSolidRectangleWithOutline(new Rect(_curSelectGridPos_Main, MapDefine.GridSize_Main * Vector2.one), _curSelectGridColor_Main, _curSelectGridColor_Main);
        Handles.DrawSolidRectangleWithOutline(new Rect(_curSelectGridPos_Port, MapDefine.GridSize_Port * Vector2.one), _curSelectGridColor_Port, _curSelectGridColor_Port);
        Handles.DrawSolidRectangleWithOutline(new Rect(_curSelectGridPos_Edit, MapDefine.GridSize_Edit * Vector2.one), _curSelectGridColor_Edit, _curSelectGridColor_Edit);

        for (int row = 0; row <= _gridCnt_Main; row++)
        {
            Handles.DrawLine(new Vector2(0, MapDefine.GridSize_Main * row), new Vector2(MapDefine.GridSize_Main * _gridCnt_Main, MapDefine.GridSize_Main * row));
            Handles.DrawLine(new Vector2(MapDefine.GridSize_Main * row, 0), new Vector2(MapDefine.GridSize_Main * row, MapDefine.GridSize_Main * _gridCnt_Main));
        }

        for (int row = 0; row <= _gridCnt_Port; row++)
        {
            Handles.DrawLine(new Vector2(MapViewSize.x + 0, MapDefine.GridSize_Port * row), new Vector2(MapViewSize.x + MapDefine.GridSize_Port * _gridCnt_Port, MapDefine.GridSize_Port * row));
            Handles.DrawLine(new Vector2(MapViewSize.x + MapDefine.GridSize_Port * row, 0), new Vector2(MapViewSize.x + MapDefine.GridSize_Port * row, MapDefine.GridSize_Port * _gridCnt_Port));
        }

        for (int row = 0; row <= _gridCnt_Edit; row++)
        {
            Handles.DrawLine(new Vector2(MapViewSize.x * 2 + 0, MapDefine.GridSize_Edit * row), new Vector2(MapViewSize.x * 2 + MapDefine.GridSize_Edit * _gridCnt_Edit, MapDefine.GridSize_Edit * row));//row
            Handles.DrawLine(new Vector2(MapViewSize.x * 2 + MapDefine.GridSize_Edit * row, 0), new Vector2(MapViewSize.x * 2 + MapDefine.GridSize_Edit * row, MapDefine.GridSize_Edit * _gridCnt_Edit));//col
        }
    }

    void DrawData()
    {
        float gridIndex_Main_X = _curSelectGridPos_Main.x / MapDefine.GridSize_Main * _gridCnt_Main;
        float gridIndex_Main_Y = (_gridCnt_Main - 1 - _curSelectGridPos_Main.y / MapDefine.GridSize_Main) * _gridCnt_Main;

        float gridIndex_Port_X = (_curSelectGridPos_Port.x - MapViewSize.x) / MapDefine.GridSize_Port;
        float gridIndex_Port_Y = _gridCnt_Port - 1 - _curSelectGridPos_Port.y / MapDefine.GridSize_Port;

        int DSRow = (int)((gridIndex_Port_X + gridIndex_Main_X) * _gridCnt_Edit);
        int DSCol = (int)((gridIndex_Port_Y + gridIndex_Main_Y) * _gridCnt_Edit);

        List<MapBlockData> tmpBlockData = _mapBlockData.FindAll(a => a.row >= DSRow && a.row < DSRow + _gridCnt_Edit && a.col >= DSCol && a.col < DSCol + _gridCnt_Edit);
        for (int i = 0; i < tmpBlockData.Count; i++)
        {
            if (tmpBlockData[i].type != eMapBlockType.None)
            {
                Vector2 pos = new Vector2(tmpBlockData[i].row % _gridCnt_Edit * MapDefine.GridSize_Edit + MapViewSize.x * 2, (_gridCnt_Edit-1 - tmpBlockData[i].col % _gridCnt_Edit) * MapDefine.GridSize_Edit);
                Handles.DrawSolidRectangleWithOutline(new Rect(pos, MapDefine.GridSize_Edit * Vector2.one), MapDefine.MapBlockTypeColor[(int)tmpBlockData[i].type], MapDefine.MapBlockTypeColor[(int)tmpBlockData[i].type]);
            }
        }
    }
}