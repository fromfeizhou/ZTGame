using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingZTCollider : MonoBehaviour {

    public bool isShowCollider = false;
    List<CollBase> colliders = new List<CollBase>();
    List<GameObject> colliderViews;
	// Use this for initialization

    CollBase tempTarget;
    GameObject tempDing;
	void Start () {
        InitCollider();
        tempDing = transform.GetChild(0).gameObject;
        if (tempDing == null)
        {
            Debug.LogError(">>>>");
        }
	}

    public void SetTarget(CollBase coll)
    {
        tempTarget = coll;
    }
    private void InitCollider()
    {
        List<Transform> tempObj = new List<Transform>();
        Transform colliderRoot = transform.Find("Colliders");
        foreach (Transform temp in colliderRoot)
        {
            if (temp.name.Contains("GameObject"))
            {
                BoxCollider tempCollider = temp.GetComponent<BoxCollider>();
                if (tempCollider != null)
                {
                    CollRectange collider = new CollRectange(temp.position.x, temp.position.z , temp.rotation.y, tempCollider.bounds.size.x, tempCollider.bounds.size.z);
                    colliders.Add(collider);
                    tempObj.Add(temp);
                }
            }
        }
    }

    bool isShow = true;
	void Update () {
      
        isShow=true;
        if (tempTarget != null)
        {
            for (int index = 0; index < colliders.Count; index++)
            {
                if (ZTCollider.CheckCollision(tempTarget, colliders[index]))
                {
                    isShow = false;
                    break;
                }
            }
        }
        if (isShow != tempDing.activeSelf)
            tempDing.SetActive(isShow);
	}
}
