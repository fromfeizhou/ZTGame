using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
{
    private ICell _info;
    private Image _imgIcon;

    private bool _onDown = false;

	// Use this for initialization
	protected virtual void Awake () {
        _imgIcon = gameObject.GetComponent<Image>();
	}

    public ICell info
    {
        get { return _info; }
        set
        {
            if (null != info && null != value && _info.templateId == value.templateId)
                return;
            _info = value;
            UpdateImgView();
        }
    }

    // 刷新显示
	private void UpdateImgView(){
        if (null == _info)
        {
            _imgIcon.sprite = null;
            gameObject.SetActive(false);
        }
        else
        {
           gameObject.SetActive(true);
           AssetManager.LoadAsset(_info.iconPath, IconCallBack, typeof(Sprite));
        }
    }
    //资源加载回调
    private void IconCallBack(Object target, string path)
    {
        _imgIcon.sprite = target as Sprite;
    }

    //按钮按下回调
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //在这里监听按钮的点击事件
        if (!_onDown)
        {
            _onDown = true;
        }
    }

    // 当按钮抬起的时候自动调用此方法  
    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }

    // 当按钮失去焦点
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (_onDown)
        {
            _onDown = false;
        }
    }

    //按钮click触发
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (_onDown)
        {
            OnClick();
        }
    }
    // 当按钮点击回调
    private void OnClick()
    {
        Debug.Log("OnClick");
    }


    public virtual void OnDestroy()
    {
        _info = null;
    }
}
