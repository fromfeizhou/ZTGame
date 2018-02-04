using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class MapColliderEditor : EditorWindow
{
    [MenuItem("Window/MyWindow")] //在unity菜单Window下有MyWindow选项
    private static void MyWindow()
    {
        MapColliderEditor myWindow = GetWindowWithRect<MapColliderEditor>(new Rect(0, 0, 1920, 800), false, "地图热区编辑器");
        myWindow.Show(); //展示
    }

    private Vector2 MapViewSize = new Vector2(640, 640);
    private Vector2 MenuViewPort_Size = new Vector2(640, 200);
    private float terrainWidth = 1000f;

    private TerrainCollider _mapCollider;

    private Camera _cameraMain;
    private Camera _cameraPort;
    private Camera _cameraEdit;

    private Vector2 _selectPos_Main;
    private Vector2 _selectPos_Port;
    private Vector2 _selectPos_Edit;

    private Vector2 _selectGridPos_Main;
    private Vector2 _selectGridPos_Port;
    private Vector2 _selectGridPos_Edit;

    private Vector2 _curMouseGridPos_Main;
    private Vector2 _curMouseGridPos_Port;
    private Vector2 _curMouseGridPos_Edit;

    private Vector3 _cameraPos_Port;
    private Vector3 _cameraPos_Edit;

    private float GridSize_Port = 40f;
    

    private enum eMapViewType
    {
        None, Main, Port, Edit
    }
    private eMapViewType MapViewType;


    Vector2 curMouseGridPos;
    float curMouseGridWidth;

    private Vector2 tmpPosMain;
    private Vector2 tmpPosPort;
    private Vector2 tmpPosEdit;

    private float _cameraGridPort_Size
    {
        get { return terrainWidth / (MapViewSize.x / MapDefine.GridSize_Main); }
    }

    private float _cameraGridEdit_Size
    {
        get { return _cameraGridPort_Size / (MapViewSize.x / GridSize_Port); }
    }

    private int _gridCnt_Main
    {
        get { return (int)(MapViewSize.x / MapDefine.GridSize_Main); }
    }

    private int _gridCnt_Port
    {
        get { 
            float  aa = MapViewSize.x / GridSize_Port;
            return (int)(aa); }
    }

    private int _gridCnt_Edit
    {
        get { return Mathf.RoundToInt(MapViewSize.x / MapDefine.GridSize_Edit); }
    }

    private List<MapBlockData> _mapBlockData = new List<MapBlockData>();
    private eMapBlockType MapBlockType = eMapBlockType.None;
    private MapColliderHelper.eMapEditHelper _mapEditHelper = MapColliderHelper.eMapEditHelper.None;


    void OnEnable()
    {
        InitCamera();

        if (_mapBlockData == null || _mapBlockData.Count == 0)
        {
            _mapBlockData = new List<MapBlockData>();
            if (File.Exists(MapDefine.MapDataSavePath))
            {
                string[] contents = File.ReadAllLines(MapDefine.MapDataSavePath);
                for (int i = 0; i < contents.Length; i++)
                {
                    if (!string.IsNullOrEmpty(contents[i]))
                    {
                        _mapBlockData.Add(MapBlockData.Parse(contents[i]));
                    }
                }
            }
            else
            {
                MapColliderHelper.SaveMapBlockFile(_mapBlockData);
            }
        }
    }

    void OnGUI()
    {
        UserInput();
        MenuInput();
        DrawMap();
        DrawGrid();
        UpdateCameraPos();
        DrawData();
        DrawCurState();
        Repaint();
    }

    private void InitCamera()
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

        if (_mapCollider == null)
            _mapCollider = GameObject.Find("MainTerrain").GetComponent<TerrainCollider>();
        terrainWidth = _mapCollider.bounds.size.x;
        float tempWidth = _mapCollider.bounds.size.x / 2;
        float tempHeight = _mapCollider.bounds.size.z / 2;
        _cameraMain.orthographicSize = tempWidth;
        _cameraMain.transform.localPosition = new Vector3(tempWidth, 100, tempHeight);
        //float portViewItemCount = MapViewSize.x / MapDefine.GridSize_Port;
        //float portCameraOrz = terrainWidth / portViewItemCount;
        //_cameraPort.orthographicSize = portCameraOrz * 0.5f;

        //float editViewItemCount = MapViewSize.x / MapDefine.GridSize_Edit;
        //float editCameraOrz = portCameraOrz / editViewItemCount;
        //_cameraEdit.orthographicSize = editCameraOrz * 0.5f;

        float mainViewItemCount = MapViewSize.x / MapDefine.GridSize_Main;
        float portCameraOrz = terrainWidth / mainViewItemCount;
        _cameraPort.orthographicSize = portCameraOrz * 0.5f;

       int tempPortCount = (int)portCameraOrz / 4;//editView 编辑为5米
       GridSize_Port = MapViewSize.x / tempPortCount;
        float portViewItemCount = MapViewSize.x / GridSize_Port;
        float editCameraOrz = portCameraOrz / portViewItemCount;
        _cameraEdit.orthographicSize = editCameraOrz * 0.5f;
    }

    private void UserInput()
    {
        Event e = Event.current;
        if (e.mousePosition.x >= 0 && e.mousePosition.y >= 0 && e.mousePosition.x < MapViewSize.x && e.mousePosition.y < MapViewSize.y)
        {
            MapViewType = eMapViewType.Main;
            curMouseGridWidth = MapDefine.GridSize_Main;
            tmpPosMain.x = (int)(e.mousePosition.x / MapDefine.GridSize_Main);
            tmpPosMain.y = (int)(e.mousePosition.y / MapDefine.GridSize_Main);
            curMouseGridPos = tmpPosMain * MapDefine.GridSize_Main;

            _curMouseGridPos_Main = curMouseGridPos / MapDefine.GridSize_Main;
            _curMouseGridPos_Main.y = _gridCnt_Main - 1 - _curMouseGridPos_Main.y;

            if (e.button == 0 && e.isMouse)
            {
                _cameraPos_Port = tmpPosMain;
                _selectPos_Main = curMouseGridPos;
                _selectGridPos_Main = _curMouseGridPos_Main;
            }
        }
        else if (e.mousePosition.x >= MapViewSize.x && e.mousePosition.y >= 0 && e.mousePosition.x < MapViewSize.x * 2 && e.mousePosition.y < MapViewSize.y)
        {
            MapViewType = eMapViewType.Port;
            curMouseGridWidth = GridSize_Port;
            tmpPosPort.x = (int)(e.mousePosition.x / GridSize_Port);
            tmpPosPort.y = (int)(e.mousePosition.y / GridSize_Port);
            curMouseGridPos = tmpPosPort * GridSize_Port;

            _curMouseGridPos_Port = (curMouseGridPos - MapViewSize.x * Vector2.right) / GridSize_Port;
            _curMouseGridPos_Port.x = Mathf.RoundToInt(_curMouseGridPos_Port.x);
            _curMouseGridPos_Port.y = _gridCnt_Port - 1 - (int)_curMouseGridPos_Port.y;

            if (e.button == 0 && e.isMouse)
            {
                _cameraPos_Edit = tmpPosPort;
                _selectPos_Port = curMouseGridPos;
                _selectGridPos_Port = _curMouseGridPos_Port;
            }
        }
        else if (e.mousePosition.x >= MapViewSize.x * 2 && e.mousePosition.y >= 0 && e.mousePosition.x < MapViewSize.x * 3 && e.mousePosition.y < MapViewSize.y)
        {
            MapViewType = eMapViewType.Edit;
            curMouseGridWidth = MapDefine.GridSize_Edit;
            tmpPosEdit.x = (int)(e.mousePosition.x / MapDefine.GridSize_Edit);
            tmpPosEdit.y = (int)(e.mousePosition.y / MapDefine.GridSize_Edit);
            curMouseGridPos = tmpPosEdit * MapDefine.GridSize_Edit;

            _curMouseGridPos_Edit = (curMouseGridPos - MapViewSize.x * 2 * Vector2.right) / MapDefine.GridSize_Edit;
            _curMouseGridPos_Edit.y = _gridCnt_Edit - 1 - _curMouseGridPos_Edit.y;

            if (e.button == 0 && e.isMouse)
            {
                _selectPos_Edit = curMouseGridPos;
                _selectGridPos_Edit = _curMouseGridPos_Edit;
            }
        }
        else
        {
            MapViewType = eMapViewType.None;
        }

        int row = (int)((int)_selectGridPos_Main.x * _gridCnt_Port * _gridCnt_Edit + (int)_selectGridPos_Port.x * _gridCnt_Edit + (int)_selectGridPos_Edit.x);
        int col = (int)((int)_selectGridPos_Main.y * _gridCnt_Port * _gridCnt_Edit + (int)_selectGridPos_Port.y * _gridCnt_Edit + (int)_selectGridPos_Edit.y);

        if (_isControl && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
        {
            AddCollider(row, col, MapBlockType);
        }

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            int tmpaaRolw  = (int)((int)_selectGridPos_Main.x * _gridCnt_Port * _gridCnt_Edit + (int)_selectGridPos_Port.x * _gridCnt_Edit + (int)_selectGridPos_Edit.x);
            int tmpaaColw = (int)((int)_selectGridPos_Main.y * _gridCnt_Port * _gridCnt_Edit + (int)_selectGridPos_Port.y * _gridCnt_Edit + (int)_selectGridPos_Edit.y);

            curSelectMapBlockData = GetCollider(row, col);
            isEditMapEvent = curSelectMapBlockData != null;
        }
    }

    private bool _isControl
    {
        get
        {
            return Event.current.control;
        }
    }


    private void UpdateCameraPos()
    {
        Vector3 localPos_Port;
        localPos_Port.x = _cameraPos_Port.x * _cameraGridPort_Size + _cameraGridPort_Size * 0.5f;
        localPos_Port.y = 200;
        int tempRow = (int)(MapViewSize.x / MapDefine.GridSize_Main) - 1;
        localPos_Port.z = (tempRow - _cameraPos_Port.y) * _cameraGridPort_Size + _cameraGridPort_Size * 0.5f;
        _cameraPort.transform.localPosition = localPos_Port;


        Vector3 localPos_Edit;
        localPos_Edit.x = (_cameraPos_Edit.x + 0.5f) * _cameraGridEdit_Size - _cameraGridPort_Size * 1.5f;
        localPos_Edit.y = 200;
        float aaaaaRow = MapViewSize.x / GridSize_Port;
        tempRow = (int)(aaaaaRow) - 1;
        localPos_Edit.z = (tempRow - _cameraPos_Edit.y + 0.5f) * _cameraGridEdit_Size - _cameraGridPort_Size * 0.5f;
        _cameraEdit.transform.localPosition = localPos_Edit + localPos_Port;
    }

    private bool isEditMapEvent;
    private void MenuInput()
    {
        if (GUI.Button(new Rect(10, 650, 50, 18), "保存"))
        {
            _mapEditHelper = MapColliderHelper.eMapEditHelper.SaveMapBlockFile;
        }

        MapBlockType = (eMapBlockType)EditorGUI.EnumPopup(new Rect(0, 670, 200, 18), "事件类型选择", MapBlockType);
        if (isEditMapEvent)
        {
            string labelName = string.Format("参数设置:当前坐标 => x:{0}, y:{1}", curSelectMapBlockData.row, curSelectMapBlockData.col);
            GUI.Label(new Rect(0, 690, 200, 18), labelName);
            curSelectMapBlockData.param = EditorGUI.TextField(new Rect(0, 710, 200, 18), curSelectMapBlockData.param);
        }
    }

    void Update()
    {
        if (_mapEditHelper == MapColliderHelper.eMapEditHelper.None)
            return;

        switch (_mapEditHelper)
        {
            case MapColliderHelper.eMapEditHelper.SaveMapBlockFile:
                {
                    MapColliderHelper.SaveMapBlockFile(_mapBlockData);
                    break;
                }
        }
        _mapEditHelper = MapColliderHelper.eMapEditHelper.None;
    }

    private MapBlockData curSelectMapBlockData;

    private void DrawMap()
    {
        GUI.DrawTexture(new Rect(Vector2.zero, MapViewSize), _cameraMain.targetTexture);
        GUI.DrawTexture(new Rect(new Vector2(MapViewSize.x, 0), MapViewSize), _cameraPort.targetTexture);
        GUI.DrawTexture(new Rect(new Vector2(MapViewSize.x * 2, 0), MapViewSize), _cameraEdit.targetTexture);
    }

    private void DrawGrid()
    {
        Handles.color = Color.white;
        for (int row = 0; row <= _gridCnt_Main; row++)//16
        {
            Handles.DrawLine(new Vector2(0, MapDefine.GridSize_Main * row), new Vector2(MapDefine.GridSize_Main * _gridCnt_Main, MapDefine.GridSize_Main * row));
            Handles.DrawLine(new Vector2(MapDefine.GridSize_Main * row, 0), new Vector2(MapDefine.GridSize_Main * row, MapDefine.GridSize_Main * _gridCnt_Main));
        }

        for (int row = 0; row <= _gridCnt_Port; row++)
        {
            Handles.DrawLine(new Vector2(MapViewSize.x + 0, GridSize_Port * row), new Vector2(MapViewSize.x + GridSize_Port * _gridCnt_Port, GridSize_Port * row));
            Handles.DrawLine(new Vector2(MapViewSize.x + GridSize_Port * row, 0), new Vector2(MapViewSize.x + GridSize_Port * row, GridSize_Port * _gridCnt_Port));
        }

        for (int row = 0; row <= _gridCnt_Edit; row++)
        {
            Handles.DrawLine(new Vector2(MapViewSize.x * 2 + 0, MapDefine.GridSize_Edit * row), new Vector2(MapViewSize.x * 2 + MapDefine.GridSize_Edit * _gridCnt_Edit, MapDefine.GridSize_Edit * row)); //row
            Handles.DrawLine(new Vector2(MapViewSize.x * 2 + MapDefine.GridSize_Edit * row, 0), new Vector2(MapViewSize.x * 2 + MapDefine.GridSize_Edit * row, MapDefine.GridSize_Edit * _gridCnt_Edit)); //col
        }
    }

    private void DrawCurState()
    {
        EditorGUI.DrawRect(new Rect(_selectPos_Main, MapDefine.GridSize_Main * Vector2.one), new Color(1, 0, 0, 0.4f));
        EditorGUI.DrawRect(new Rect(_selectPos_Port, GridSize_Port * Vector2.one), new Color(1, 0, 1, 0.4f));

        if (MapViewType != eMapViewType.None)
        {
            Vector2 mouseLabelNotice = Vector2.zero;
            switch (MapViewType)
            {
                case eMapViewType.Main:
                    {
                        mouseLabelNotice = _curMouseGridPos_Main;
                        break;
                    }
                case eMapViewType.Port:
                    {
                        mouseLabelNotice = _curMouseGridPos_Port;
                        break;
                    }
                case eMapViewType.Edit:
                    {
                        mouseLabelNotice = _curMouseGridPos_Edit;
                        break;
                    }
            }

            if (_isControl)
                EditorGUI.DrawRect(new Rect(curMouseGridPos, MapDefine.GridSize_Edit * Vector2.one), MapDefine.MapBlockTypeColor[(int)MapBlockType]);

            {//鼠标当前选择框
                Vector3[] _gridPoints =
                {
                    curMouseGridPos,
                    curMouseGridPos + Vector2.right * curMouseGridWidth,
                    curMouseGridPos + Vector2.one * curMouseGridWidth,
                    curMouseGridPos + Vector2.up * curMouseGridWidth,
                    curMouseGridPos
                };
                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(4.0f, _gridPoints);
            }

            {//鼠标坐标提示
                string labelStr = string.Format("({0},{1})", mouseLabelNotice.x, mouseLabelNotice.y);
                Vector2 labelSize = new Vector2(46, 18);
                Vector2 mousePos = Event.current.mousePosition;
                if (MapViewType == eMapViewType.Main)
                    mousePos -= Vector2.left * labelSize.x;

                EditorGUI.DrawRect(new Rect(mousePos - labelSize, labelSize), Color.gray);
                EditorGUI.LabelField(new Rect(mousePos - labelSize, labelSize), labelStr);
            }


        }
    }

    private void DrawData()
    {
        float gridIndex_Main_X = _selectPos_Main.x / MapDefine.GridSize_Main * _gridCnt_Port;//_gridCnt_Main;
        float gridIndex_Main_Y = (_gridCnt_Main - 1 - _selectPos_Main.y / MapDefine.GridSize_Main) * _gridCnt_Port;

        float gridIndex_Port_X = (_selectPos_Port.x - MapViewSize.x) / GridSize_Port;
        float gridIndex_Port_Y = _gridCnt_Port - 1 - _selectPos_Port.y / GridSize_Port;

        int ScrRow = (Mathf.RoundToInt(gridIndex_Port_X + gridIndex_Main_X) * _gridCnt_Edit);
        int ScrCol = (Mathf.RoundToInt(gridIndex_Port_Y + gridIndex_Main_Y) * _gridCnt_Edit);

       
        List<MapBlockData> tmpBlockData = _mapBlockData.FindAll(a => a.IsInBlock(_gridCnt_Edit, ScrRow, ScrCol));
        for (int i = 0; i < tmpBlockData.Count; i++)
        {
            if (tmpBlockData[i].type != eMapBlockType.None)
            {
                Vector2 pos = new Vector2(tmpBlockData[i].row % _gridCnt_Edit * MapDefine.GridSize_Edit + MapViewSize.x * 2, (_gridCnt_Edit - 1 - tmpBlockData[i].col % _gridCnt_Edit) * MapDefine.GridSize_Edit);
                EditorGUI.DrawRect(new Rect(pos, MapDefine.GridSize_Edit * Vector2.one),
                    MapDefine.MapBlockTypeColor[(int)tmpBlockData[i].type]);
            }
        }
    }

    private void AddCollider(int row, int col, eMapBlockType mapBlockType)
    {
        int index = _mapBlockData.FindIndex(a => a.row == row && a.col == col);
        if (index >= 0)
            _mapBlockData[index].type = mapBlockType;
        else
            _mapBlockData.Add(new MapBlockData { row = row, col = col, type = mapBlockType });
    }

    private MapBlockData GetCollider(int row, int col)
    {
        int index = _mapBlockData.FindIndex(a => a.row == row && a.col == col);
        if (index >= 0 && _mapBlockData[index].type == eMapBlockType.Event)
            return _mapBlockData[index];
        return null;

    }
}