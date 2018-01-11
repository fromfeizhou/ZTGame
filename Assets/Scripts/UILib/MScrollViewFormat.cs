using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class MScrollViewFormat : MonoBehaviour
{
    // Use this for initialization
    private int _initIndex;     //当前创建元素
    private int _initMax;  //最大创建元素
    private int _row;       //行数
    private int _column;    //列数
    private GridLayoutGroup.Axis _startAxis;     //排列顺序（通常与滚动方向相反）
    private MScrollRect _scrollRect; //滚动控制器
    private Transform _container;   //滚动容器
    private GridLayoutGroup _layoutGroup;   //布局对象 
    private List<ICell> _infoList;   //数据池
    private int _scrollIndex; //滚动队列下标
    private int _scrollMax;    //滚动队列最大值

    public delegate GameObject InitFuncAction(ICell info);
    private InitFuncAction _initFunc;
    private UnityAction<GameObject, ICell> _updateFunc;
    private bool _canInit = false;
    void Start()
    {
        _scrollRect = gameObject.GetComponent<MScrollRect>();
        _container = transform.Find("Container");

        //计算容器大小
        CalculateRC();
        //监听滚动事件
        _scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(OnScrollEvent));
    }

    // Update is called once per frame
    void Update()
    {
        if (_canInit)
        {
            InitItem();
        }
    }
    //设置数据列表 构建函数 更新函数
    public void SetCellFunc(List<ICell> infoList = null, InitFuncAction initFunc = null, UnityAction<GameObject, ICell> updateFunc = null)
    {
        ClearItem();
        _initFunc = initFunc;
        _updateFunc = updateFunc;
        _infoList = infoList;
        UpdateScrollMax();

        _initIndex = 0;
        _scrollIndex = 0;
        _canInit = true;
    }

    //更新数据列表
    public void UpdateInfoList()
    {
        UpdateScrollMax();

        //列表元素增加
        if (_initIndex < _initMax && _initIndex < _infoList.Count)
        {
            UpdateCellInfo();
            _canInit = true;
            return;
        }
        UpdateCellInfo();
    }

    //初始化Item 超出数据列表不在创建
    private void InitItem()
    {
        if (_initIndex >= _infoList.Count || _initIndex >= _initMax)
        {
            _canInit = false;
            return;
        }
        GameObject cell = _initFunc(_infoList[_initIndex]);
        cell.transform.SetParent(_container);
        _initIndex++;
        UpdateContainerSize();
    }

    //计算容器行列
    private void CalculateRC()
    {
        RectTransform sForm = gameObject.GetComponent<RectTransform>();
        _layoutGroup = _container.gameObject.GetComponent<GridLayoutGroup>();
        _startAxis = _layoutGroup.startAxis;

        float height = sForm.sizeDelta.y;
        float width = sForm.sizeDelta.x;

        if (IsAxisHorizontal())
        {
            //水平排列 垂直滚动 左右
            height += _layoutGroup.spacing.y;

            width = width - _layoutGroup.padding.left - _layoutGroup.padding.right;
            width = width > _layoutGroup.cellSize.x ? width : _layoutGroup.cellSize.x;
        }
        else
        {
            //垂直排列 水平滚动 高度上下扣除
            width += _layoutGroup.spacing.x;

            height = height - _layoutGroup.padding.top - _layoutGroup.padding.bottom;
            height = height > _layoutGroup.cellSize.y ? height : _layoutGroup.cellSize.y;
        }

        _row = (int)Mathf.Ceil(height * 1.0f / (_layoutGroup.cellSize.y + _layoutGroup.spacing.y));


        _column = (int)Mathf.Ceil(width * 1.0f / (_layoutGroup.cellSize.x + _layoutGroup.spacing.x));

        if (IsAxisHorizontal())
        {
            //水平方向铺排（放置满一行 换行）
            _row++;
        }
        else
        {
            //垂直方向铺排
            _column++;
        }
        _initMax = _row * _column;
    }

    //更新滚动队列
    private void UpdateScrollMax()
    {
        if (_infoList.Count < _initMax)
        {
            _scrollMax = 0;
            return;
        }
        if (IsAxisHorizontal())
        {
            _scrollMax = Mathf.CeilToInt(1.0f * (_infoList.Count - _initMax) / _column);
        }
        else
        {
            _scrollMax = Mathf.CeilToInt(1.0f * (_infoList.Count - _initMax) / _row);
        }
    }

    //更新滚动容器size
    private void UpdateContainerSize(bool isConst = false)
    {
        int itemCount = _initIndex < _infoList.Count ? _initIndex : _infoList.Count;
        if (IsAxisHorizontal())
        {
            //每行起始位 计算变动
            if (isConst || itemCount % _column == 1)
            {
                //实际位置需要数据队列下标+1
                int row = (int)Mathf.Ceil((itemCount) * 1.0f / _column);
                RectTransform tForm = _container.gameObject.GetComponent<RectTransform>();
                float height = _layoutGroup.padding.top + _layoutGroup.padding.bottom + row * _layoutGroup.cellSize.y + (row - 1) * _layoutGroup.spacing.y;
                RectTransform sForm = gameObject.GetComponent<RectTransform>();
                height = height > sForm.sizeDelta.y ? height : sForm.sizeDelta.y;

                tForm.sizeDelta = new Vector2(tForm.sizeDelta.x, height);
            }
        }
        else
        {
            //每列起始位 计算变动
            if (isConst || itemCount % _row == 1)
            {
                //实际位置需要数据队列下标+1
                int column = (int)Mathf.Ceil((itemCount) * 1.0f / _row);
                RectTransform tForm = _container.gameObject.GetComponent<RectTransform>();
                float width = _layoutGroup.padding.left + _layoutGroup.padding.right + column * _layoutGroup.cellSize.x + (column - 1) * _layoutGroup.spacing.x;

                RectTransform sForm = gameObject.GetComponent<RectTransform>();
                width = width > sForm.sizeDelta.x ? width : sForm.sizeDelta.x;
                tForm.sizeDelta = new Vector2(width, tForm.sizeDelta.y);
            }
        }
    }

    //滚动监听
    private void OnScrollEvent(Vector2 pos)
    {

        //Debug.Log("V:\t" + _scrollRect.verticalNormalizedPosition + "\tH:\t" + _scrollRect.horizontalNormalizedPosition);
        if (IsAxisHorizontal())
        {
            //水平排列 垂直滚动
            RectTransform tForm = _container.gameObject.GetComponent<RectTransform>();
            float max = _layoutGroup.padding.top + _layoutGroup.cellSize.y + _layoutGroup.spacing.y;
            float min = _layoutGroup.padding.top;
            if (_scrollIndex < _scrollMax && tForm.anchoredPosition.y > max)
            {
                ScrollItemView(true);
                _scrollRect.ResetDragState();
            }
            else if (_scrollIndex > 0 && tForm.anchoredPosition.y < min)
            {
                ScrollItemView(false);
                _scrollRect.ResetDragState();
            }
        }
        else
        {
            //垂直排列 水平滚动
            RectTransform tForm = _container.gameObject.GetComponent<RectTransform>();
            float max = -(_layoutGroup.padding.left + _layoutGroup.cellSize.x + _layoutGroup.spacing.x);//左边
            float min = -(_layoutGroup.padding.left);//右边
            if (_scrollIndex < _scrollMax && tForm.anchoredPosition.x < max)
            {
                ScrollItemView(true);
                _scrollRect.ResetDragState();
            }
            else if (_scrollIndex > 0 && tForm.anchoredPosition.x > min)
            {
                ScrollItemView(false);
                _scrollRect.ResetDragState();
            }
        }
    }

    //队列滚动
    private void ScrollItemView(bool isFront)
    {
        if (IsAxisHorizontal())
        {
            //水平排列 垂直滚动
            RectTransform tForm = _container.gameObject.GetComponent<RectTransform>();
            float max = _layoutGroup.padding.top + _layoutGroup.cellSize.y + _layoutGroup.spacing.y;
            float min = _layoutGroup.padding.top;
            if (isFront)
            {
                tForm.anchoredPosition = new Vector2(tForm.anchoredPosition.x, min);
                for (int i = 0; i < _column; i++)
                {
                    GameObject cell = _container.GetChild(0).gameObject;
                    cell.transform.SetSiblingIndex(_initMax - 1);
                    UpdateCellInfoByIndex(cell, _initMax + _scrollIndex * _column + i);
                }
                //滚动队列
                _scrollIndex++;
            }
            else
            {
                tForm.anchoredPosition = new Vector2(tForm.anchoredPosition.x, max);
                for (int i = 0; i < _column; i++)
                {
                    GameObject cell = _container.GetChild(_initMax - 1).gameObject;
                    cell.transform.SetSiblingIndex(0);
                    UpdateCellInfoByIndex(cell, _scrollIndex * _column - i - 1);
                }
                //滚动队列
                _scrollIndex--;
            }
        }
        else
        {
            //垂直排列 水平滚动
            RectTransform tForm = _container.gameObject.GetComponent<RectTransform>();
            float max = -(_layoutGroup.padding.left + _layoutGroup.cellSize.x + _layoutGroup.spacing.x);//左边
            float min = -(_layoutGroup.padding.left);//右边
            if (isFront)
            {
                tForm.anchoredPosition = new Vector2(min, tForm.anchoredPosition.y);
                for (int i = 0; i < _row; i++)
                {
                    GameObject cell = _container.GetChild(0).gameObject;
                    cell.transform.SetSiblingIndex(_initMax - 1);
                    UpdateCellInfoByIndex(cell, _initMax + _scrollIndex * _row + i);
                }
                //滚动队列
                _scrollIndex++;
            }
            else
            {
                tForm.anchoredPosition = new Vector2(max, tForm.anchoredPosition.y);
                for (int i = 0; i < _row; i++)
                {
                    GameObject cell = _container.GetChild(_initMax - 1).gameObject;
                    cell.transform.SetSiblingIndex(0);
                    UpdateCellInfoByIndex(cell, _scrollIndex * _row - i - 1);
                }
                //滚动队列
                _scrollIndex--;
            }
        }
    }

    //更新数据
    private void UpdateCellInfoByIndex(GameObject cell, int index)
    {
        if (index < _infoList.Count)
        {
            _updateFunc(cell, _infoList[index]);
        }
        else
        {
            _updateFunc(cell, null);
        }
    }

    private void UpdateCellInfo()
    {
        //计算当前显示数据 起始位
        int dataGap = IsAxisHorizontal() ? _column : _row;

        int dataIndex = _scrollIndex * dataGap;

        for (int i = 0; i < _container.childCount; i++)
        {
            if (dataIndex < _infoList.Count)
            {
                _updateFunc(_container.GetChild(i).gameObject, _infoList[dataIndex]);
                dataIndex++;
            }
            else
            {
                _updateFunc(_container.GetChild(i).gameObject, null);
            }
        }
        UpdateContainerSize(true);
        if (_infoList.Count < _initIndex - 2 * dataGap)
        {
            if (IsAxisHorizontal())
            {
                _scrollRect.verticalNormalizedPosition = 0;
            }
            else
            {
                _scrollRect.horizontalNormalizedPosition = 0;
            }
        }
    }

    //排列顺序
    private bool IsAxisHorizontal()
    {
        return _startAxis == GridLayoutGroup.Axis.Horizontal;
    }

    protected virtual void OnDestroy()
    {
        ClearItem();
    }

    private void ClearItem()
    {
        if (_container != null && _container.childCount > 0)
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                GameObject cell = _container.GetChild(i).gameObject;
                Destroy(cell);
            }
        }
        _infoList = null;
    }
}
