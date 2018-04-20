using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoiller : MonoBehaviour {

    private GameObject _colliderView;
    public CollRectange collider;
	// Use this for initialization
	void Start () {
        ZTCollider aa = new ZTCollider();
         collider = new CollRectange(1, 1, 0, CharaDefine.PLAYER_RADIUS, 1f);
         

        _colliderView = ZColliderView.CreateColliderView(collider);
        _colliderView.transform.localPosition = new Vector3(collider.x, 0.1f, collider.y);
        _colliderView.transform.SetParent(this.gameObject.transform, false);
        _colliderView.transform.localRotation = Quaternion.Euler(Vector3.up * collider.rotate);
	}

    private void ClearColliderView()
    {
        if (null != _colliderView)
        {
            GameObject.Destroy(_colliderView);
            _colliderView = null;
        }
    }
	
	// Update is called once per frame
	void Update () {

        collider.MovePos = _colliderView.transform.position;
	}
}
