using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingZTCollider : MonoBehaviour
{

    public bool isShowCollider = false;
    List<CollBase> colliders = new List<CollBase>();

    private Renderer[] tempRenderers;
    private CollBase tempTarget;
    private GameObject tempDing;
    void Start()
    {
        InitCollider();
        tempDing = transform.GetChild(0).gameObject;
        if (tempDing == null)
        {
            Debug.LogError("Model is error!!!");
        }
        Transform rendererObj = transform.GetChild(1);
        tempRenderers = rendererObj.GetComponentsInChildren<Renderer>();
        SetColor(true);
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
                    CollRectange collider = new CollRectange(temp.position.x, temp.position.z, temp.rotation.y, tempCollider.bounds.size.x, tempCollider.bounds.size.z);
                    colliders.Add(collider);
                    tempObj.Add(temp);
                }
            }
        }
    }

    bool isShow = true;
    void Update()
    {

        isShow = true;
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
        {
            tempDing.SetActive(isShow);
            SetColor(isShow);
        }
    }

    public void SetColor(bool isShow)
    {
        if (tempRenderers == null) return;
        for (int index = 0; index < tempRenderers.Length; index++)
        {
            Color obj_color = tempRenderers[index].material.color;
            obj_color.a = isShow ? 1f : 0.5f;
            Debug.LogError(obj_color + " " + tempRenderers[index].name);
            tempRenderers[index].material.SetColor("_Color", obj_color);
        }
    }

}
