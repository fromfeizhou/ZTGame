using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//C#在类定义外可以声明方法的签名（Delegate，代理或委托），但是不能声明真正的方法。
public delegate void OnNotificationDelegate(Notification note);

public class NotificationDelegate
{

    private Dictionary<string, OnNotificationDelegate> eventListerners;

    //Single 
    public NotificationDelegate()
    {
        eventListerners = new Dictionary<string, OnNotificationDelegate>();
    }

    /*
     * 监听事件
     */

    //添加监听事件
    public void addEventListener(string type, OnNotificationDelegate listener)
    {
        if (!eventListerners.ContainsKey(type))
        {
            OnNotificationDelegate deleg = null;
            eventListerners[type] = deleg;
        }
        eventListerners[type] += listener;
    }

    //移除监听事件
    public void removeEventListener(string type, OnNotificationDelegate listener)
    {
        if (!eventListerners.ContainsKey(type))
        {
            return;
        }
        eventListerners[type] -= listener;
    }

    //移除某一类型所有的监听事件
    public void removeEventListener(string type)
    {
        if (eventListerners.ContainsKey(type))
        {
            eventListerners.Remove(type);
        }
    }

    /*
     * 派发事件
     */

    //派发数据
    public void dispatchEvent(string type, Notification note)
    {
        if (eventListerners.ContainsKey(type))
        {
            eventListerners[type](note);
        }
    }

    //派发无数据
    public void dispatchEvent(string type)
    {
        dispatchEvent(type, null);
    }

    //查找是否有当前类型事件监听
    public bool HasEventListener(string type)
    {
        return eventListerners.ContainsKey(type);
    }
}

public class Notification
{
    /// <summary>
    /// 通知发送者
    /// </summary>
    public GameObject sender;

    /// <summary>
    /// 通知内容
    /// 备注：在发送消息时需要装箱、解析消息时需要拆箱
    /// </summary>
    public object param;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="param"></param>
    public Notification(object param, GameObject sender = null)
    {
        this.sender = sender;
        this.param = param;
    }

    /// <summary>
    /// 实现ToString方法
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Format("sender={0},param={1}", this.sender, this.param);
    }
}
