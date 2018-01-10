using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SkillAreaType
{
    OuterCircle = 0,
    OuterCircle_InnerCube = 1,
    OuterCircle_InnerSector = 2,
    OuterCircle_InnerCircle = 3,
}

public class SkillArea : MonoBehaviour {

    enum SKillAreaElement
    {
        OuterCircle,    // 外圆
        InnerCircle,    // 内圆
        Cube,           // 矩形 
        Sector60,        // 扇形
        Sector120,        // 扇形
    }

    SkillJoystick joystick;

    public GameObject player;      

    public SkillAreaType areaType;      // 设置指示器类型

    Vector3 deltaVec;

    float outerRadius = 6;      // 外圆半径
    float innerRadius = 2f;     // 内圆半径
    float cubeWidth = 2f;       // 矩形宽度 （矩形长度使用的外圆半径）
    int angle = 60;             // 扇形角度

    bool isPressed = false;


    string path = "Assets/ResourcesLib/Skill/Prefabs/";  // 路径
    string circle = "quan_hero";    // 圆形
    string cube = "chang_hero";     // 矩形
    string sector60 = "shan_hero_60";    // 扇形60度
    string sector120 = "shan_hero_120";    // 扇形120度

    Dictionary<SKillAreaElement, string> allElementPath;
    Dictionary<SKillAreaElement, Transform> allElementTrans;

    private string _elementName;
    private List<SKillAreaElement> _elementNames;
    private bool _inCreate;

    // Use this for initialization
    void Start()
    {
        joystick = GetComponent<SkillJoystick>();

        joystick.onJoystickDownEvent += OnJoystickDownEvent;
        joystick.onJoystickMoveEvent += OnJoystickMoveEvent;
        joystick.onJoystickUpEvent += OnJoystickUpEvent;

        InitSkillAreaType();

        _elementNames = new List<SKillAreaElement>();
        _elementNames.Add(SKillAreaElement.OuterCircle);
        _elementNames.Add(SKillAreaElement.InnerCircle);
        _elementNames.Add(SKillAreaElement.Cube);
        _elementNames.Add(SKillAreaElement.Sector60);
        _elementNames.Add(SKillAreaElement.Sector120);
        _inCreate = false;
    }

    void OnDestroy()
    {
        joystick.onJoystickDownEvent -= OnJoystickDownEvent;
        joystick.onJoystickMoveEvent -= OnJoystickMoveEvent;
        joystick.onJoystickUpEvent -= OnJoystickUpEvent;
    }

    void InitSkillAreaType()
    {
        allElementPath = new Dictionary<SKillAreaElement, string>();
        allElementPath.Add(SKillAreaElement.OuterCircle, circle);
        allElementPath.Add(SKillAreaElement.InnerCircle, circle);
        allElementPath.Add(SKillAreaElement.Cube, cube);
        allElementPath.Add(SKillAreaElement.Sector60, sector60);
        allElementPath.Add(SKillAreaElement.Sector120, sector120);

        allElementTrans = new Dictionary<SKillAreaElement, Transform>();
        allElementTrans.Add(SKillAreaElement.OuterCircle, null);
        allElementTrans.Add(SKillAreaElement.InnerCircle, null);
        allElementTrans.Add(SKillAreaElement.Cube, null);
        allElementTrans.Add(SKillAreaElement.Sector60, null);
        allElementTrans.Add(SKillAreaElement.Sector120, null);
    }

    void AutoCreateArea()
    {
        if (!_inCreate && null != _elementNames && _elementNames.Count > 0)
        {
            CreateElement(_elementNames[0]);
            _elementNames.RemoveAt(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        AutoCreateArea();
    }


    void OnJoystickDownEvent(Vector2 deltaVec)
    {
        isPressed = true;
        this.deltaVec = new Vector3(deltaVec.x, 0, deltaVec.y);
        CreateSkillArea();
    }

    void OnJoystickUpEvent(Vector2 deltaVec)
    {
        isPressed = false;
        HideElements();
    }

    void OnJoystickMoveEvent(Vector2 deltaVec)
    {
        if (_inCreate)
            return;

        this.deltaVec = new Vector3(deltaVec.x, 0, deltaVec.y);
    }

    void LateUpdate()
    {
        if (isPressed && !_inCreate)
            UpdateElement();
    }

    /// <summary>
    /// 创建技能区域展示
    /// </summary>
    void CreateSkillArea()
    {
        switch (areaType)
        {
            case SkillAreaType.OuterCircle:
                CreateElement(SKillAreaElement.OuterCircle);
                break;
            case SkillAreaType.OuterCircle_InnerCube:
                //CreateElement(SKillAreaElement.OuterCircle);
                CreateElement(SKillAreaElement.Cube);
                break;
            case SkillAreaType.OuterCircle_InnerSector:
                //CreateElement(SKillAreaElement.OuterCircle);
                switch (angle)
                {
                    case 60:
                        CreateElement(SKillAreaElement.Sector60);
                        break;
                    case 120:
                        CreateElement(SKillAreaElement.Sector120);
                        break;
                    default:
                        break;
                }
                break;
            case SkillAreaType.OuterCircle_InnerCircle:
                CreateElement(SKillAreaElement.OuterCircle);
                CreateElement(SKillAreaElement.InnerCircle);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 创建技能区域展示元素
    /// </summary>
    /// <param name="element"></param>
	void CreateElement(SKillAreaElement element)
    {
        Transform elementTrans = GetElement(element);
        if (elementTrans == null) return;
        allElementTrans[element] = elementTrans;
        switch (element)
        {
            case SKillAreaElement.OuterCircle:
                elementTrans.localScale = new Vector3(outerRadius * 2, 1, outerRadius * 2) / player.transform.localScale.x;
                elementTrans.gameObject.SetActive(true);
                break;
            case SKillAreaElement.InnerCircle:
                elementTrans.localScale = new Vector3(innerRadius * 2, 1, innerRadius * 2) / player.transform.localScale.x;
                break;
            case SKillAreaElement.Cube:
                elementTrans.localScale = new Vector3(cubeWidth, 1, outerRadius) / player.transform.localScale.x;
                break;
            case SKillAreaElement.Sector60:
            case SKillAreaElement.Sector120:
                elementTrans.localScale = new Vector3(outerRadius, 1, outerRadius) / player.transform.localScale.x;
                break;
            default:
                break;
        }
    }

    Transform elementParent;
    /// <summary>
    /// 获取元素的父对象
    /// </summary>
    /// <returns></returns>
    Transform GetParent()
    {
        if (elementParent == null)
        {
            elementParent = player.transform.Find("SkillArea");
        }
        if (elementParent == null)
        {
            elementParent = new GameObject("SkillArea").transform;
            elementParent.parent = player.transform;
            elementParent.localEulerAngles = Vector3.zero;
            elementParent.localPosition = Vector3.zero;
            elementParent.localScale = Vector3.one;
        }
        return elementParent;
    }

    /// <summary>
    /// 获取元素物体
    /// </summary>
    Transform GetElement(SKillAreaElement element)
    {
        if (player == null) return null;
        string name = element.ToString();
        Transform parent = GetParent();
        Transform elementTrans = parent.Find(name);
        _elementName = name;
        if (elementTrans == null)
        {
            _inCreate = true;
            AssetManager.LoadAsset(path + allElementPath[element] + ".prefab", AssetLoadCallBack);
            return null;
        }
        elementTrans.localEulerAngles = Vector3.zero;
        elementTrans.localPosition = Vector3.zero;
        elementTrans.localScale = Vector3.one;
        return elementTrans;
    }
    //localstring加载回调
    private void AssetLoadCallBack(Object target, string path)
    {
        _inCreate = false;
        if (null == target)
        {
            return;
        }
        GameObject prefab = target as GameObject;
        GameObject elementGo = GameObject.Instantiate(prefab);
        elementGo.transform.SetParent(GetParent());
        elementGo.gameObject.SetActive(false);
        elementGo.name = _elementName;
        elementGo.transform.localEulerAngles = Vector3.zero;
        elementGo.transform.localPosition = Vector3.zero;
        elementGo.transform.localScale = Vector3.one;
    }



    /// <summary>
    /// 隐藏所有元素
    /// </summary>
    void HideElements()
    {
        if (player == null) return;
        Transform parent = GetParent();
        for (int i = 0, length = parent.childCount; i < length; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 隐藏指定元素
    /// </summary>
    /// <param name="element"></param>
    void HideElement(SKillAreaElement element)
    {
        if (player == null) return;
        Transform parent = GetParent();
        Transform elementTrans = parent.Find(element.ToString());
        if (elementTrans != null)
            elementTrans.gameObject.SetActive(false);
    }

    /// <summary>
    /// 每帧更新元素
    /// </summary>
    void UpdateElement()
    {
        switch (areaType)
        {
            case SkillAreaType.OuterCircle:
                break;
            case SkillAreaType.OuterCircle_InnerCube:
                UpdateElementPosition(SKillAreaElement.Cube);
                break;
            case SkillAreaType.OuterCircle_InnerSector:
                switch (angle)
                {
                    case 60:
                        UpdateElementPosition(SKillAreaElement.Sector60);
                        break;
                    case 120:
                        UpdateElementPosition(SKillAreaElement.Sector120);
                        break;
                    default:
                        break;
                }
                break;
            case SkillAreaType.OuterCircle_InnerCircle:
                UpdateElementPosition(SKillAreaElement.InnerCircle);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 每帧更新元素位置
    /// </summary>
    /// <param name="element"></param>
    void UpdateElementPosition(SKillAreaElement element)
    {
        if (allElementTrans[element] == null)
            return;
        switch (element)
        {
            case SKillAreaElement.OuterCircle:
                break;
            case SKillAreaElement.InnerCircle:
                allElementTrans[element].transform.position = GetCirclePosition(outerRadius);
                break;
            case SKillAreaElement.Cube:
            case SKillAreaElement.Sector60:
            case SKillAreaElement.Sector120:
                allElementTrans[element].transform.LookAt(GetCubeSectorLookAt());
                break;
            default:
                break;
        }
        if (!allElementTrans[element].gameObject.activeSelf)
            allElementTrans[element].gameObject.SetActive(true);
    }

    /// <summary>
    /// 获取InnerCircle元素位置
    /// </summary>
    /// <returns></returns>
    Vector3 GetCirclePosition(float dist)
    {
        if (player == null) return Vector3.zero;

        Vector3 targetDir = deltaVec * dist;

        float y = Camera.main.transform.rotation.eulerAngles.y;
        targetDir = Quaternion.Euler(0, y, 0) * targetDir;

        return targetDir + player.transform.position;
    }

    /// <summary>
    /// 获取Cube、Sector元素朝向
    /// </summary>
    /// <returns></returns>
    Vector3 GetCubeSectorLookAt()
    {
        if (player == null) return Vector3.zero;
        
        Vector3 targetDir = deltaVec;

        float y = Camera.main.transform.rotation.eulerAngles.y;
        targetDir = Quaternion.Euler(0, y, 0) * targetDir;

        return targetDir + player.transform.position;
    }
}
