/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2015/09/24
 * Note  : 继承MonoBehaviour的单例
***************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    /// <summary>
    ///   单例实例
    /// </summary>
	private static T _instance = null;
    public static T GetInstance()
	{
        if (_instance == null)
		{
            _instance = GameObject.FindObjectOfType(typeof(T)) as T;
            if (_instance == null)
                _instance = new GameObject("SingletonOf" + typeof(T).ToString(), typeof(T)).GetComponent<T>();

            DontDestroyOnLoad(_instance);
		}
        return _instance;
	}

    public virtual void Init()
    {
    }

    public virtual void Destroy()
    {
    }

	/// <summary>
    ///   确保在程序退出时销毁实例。
	/// </summary>
	protected virtual void OnApplicationQuit()
	{
        _instance = null;
	}
}