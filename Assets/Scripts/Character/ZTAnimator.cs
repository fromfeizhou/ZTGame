using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class ZTAnimator : MonoBehaviour {
     
    private Animation _animator; //动画
    private GameObject _animatorGo; //动画对象
    private bool _isTrans;  //是否半透明

    public void Start()
    {
        _animator = null;
        _animatorGo = null;
        _isTrans = false;
    }

   

    public void OnDestroy()
    {
        //if (null != _animatorGo)
        //{
        //    GameObject.Destroy(_animatorGo);
        //}
        _animatorGo = null;
        _animator = null;
    }

    //创建形象
    public void CreateAnimatorView(string path)
    {
        if (null != _animator)
        {
            this.gameObject.SetActive(true);
            return;
        }

        AssetManager.LoadAsset(path, (Object target, string comePath) => {
            GameObject prefab = target as GameObject;
            if (null != prefab)
            {
                GameObject go = GameObject.Instantiate(prefab);
                _animatorGo = go;
                _animator = go.GetComponent<Animation>();
            }
        });
    }

    //移除形象
    public void RemoveAnimatorView()
    {
        if (null != _animator)
        {
            this.gameObject.SetActive(false);
            this.StartCoroutine(timeOut());
        }
    }

    IEnumerator timeOut()
    {
        yield return new WaitForSeconds(60);
        //非活动状态 清理东西对象
        if (!this.gameObject.activeSelf)
        {
            if (null != _animatorGo)
            {
                GameObject.Destroy(_animatorGo);
            }
            _animatorGo = null;
            _animator = null;
        }
    }

    //播放动作
    public void Play(string actoinName)
    {
        if (null == _animator) return;
        _animator.Play(actoinName);
    }

    //刷新位置
    public void UpdatePos(Vector3 pos)
    {
        this.gameObject.transform.localPosition = pos;
    }

    //更新角度
    public void UpdateRotate(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        this.gameObject.transform.localRotation = Quaternion.Euler(Vector3.up * angle);
    }

    //改变透明度
    public void ChangeTranslucence(bool isTrans)
    {
        if(_isTrans != isTrans)
        {
            _isTrans = isTrans;
            UpdateTranslucence();
        }
       
    }
    //刷新透明度
    private void UpdateTranslucence()
    {
        if (null == _animator) return;
        if (_isTrans)
        {
            _animator.gameObject.SetActive(true);
            SkinnedMeshRenderer render = _animator.gameObject.transform.Find("equitPos").GetComponent<SkinnedMeshRenderer>();

            if (!_isTrans)
            {
                render.material.shader = Shader.Find("Custom/PengLuOccTransVF");
            }
            else
            {
                render.material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
                Color color = render.material.color;
                render.material.color = new Color(color.r, color.g, color.b, 125);
            }

        }
        else
        {
            _animator.gameObject.SetActive(false);
        }
    }
}
