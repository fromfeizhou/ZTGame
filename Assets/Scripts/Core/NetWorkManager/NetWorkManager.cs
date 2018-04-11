using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XLua;
using UnityEngine.Networking;

[LuaCallCSharp]
public class NetWorkManager : MonoSingleton<NetWorkManager>
{
	private string _host;
	private int _port;
	
	private SocketClient _socketClient;
	static readonly object m_lockObject = new object ();
	static Queue<KeyValuePair<int, ByteBuffer>> mEvents = new Queue<KeyValuePair<int, ByteBuffer>> ();

	private Action<int,ByteBuffer> _onReceiveMsg;
	    

	public void InitServerList(string serverList,Action<bool,string> callback){
		StartCoroutine (_initServerList(serverList,callback));
	}

	private IEnumerator _initServerList(string serverList,Action<bool,string> callback){
		if (string.IsNullOrEmpty (serverList) || callback == null) {
			Debug.LogError ("[" + this.GetType().Name + "]" + "serverList or callback is null");
			yield break;
		}
		   
		UnityWebRequest webRequest = UnityWebRequest.Get(serverList);
		yield return webRequest.Send ();
		if (!string.IsNullOrEmpty(webRequest.error)) {
			callback (false, "[webRequest] Error");
		} else {
			if (webRequest.responseCode == 200) {
				callback (true, webRequest.downloadHandler.text);
			} else {
				callback (false, "[webRequest] responseCode:" + webRequest.responseCode);
			}
		}
	}

	public void SetDelegateReceiveMsg(Action<int,ByteBuffer> onReceiveMsg){
		this._onReceiveMsg = onReceiveMsg;
	}

	public void SetNetWorkAddress(string host,int port)
	{
		_host = host;
		_port = port;
	}

	public override void Init ()
	{
		_socketClient = new SocketClient();
		_socketClient.OnRegister ();
	}
      
	///------------------------------------------------------------------------------------
	public static void AddEvent (int _event, ByteBuffer data)
	{
		lock (m_lockObject) {
			mEvents.Enqueue (new KeyValuePair<int, ByteBuffer> (_event, data));
		}
	}

	/// <summary>
	/// 交给Command，这里不想关心发给谁。
	/// </summary>
	void FixedUpdate()
	{
		if (mEvents.Count > 0) {
			while (mEvents.Count > 0) {
				KeyValuePair<int, ByteBuffer> _event = mEvents.Dequeue ();
				if(_onReceiveMsg != null)
				{
					_onReceiveMsg(_event.Key,_event.Value);
				}
			}
		}
	}

	/// <summary>
	/// 发送链接请求
	/// </summary>
	public void RequestConnect ()
	{
		_socketClient.SendConnect (_host, _port);
	}

	/// <summary>
	/// 发送SOCKET消息
	/// </summary>
	public void SendSocketMessage (byte[] msgData)
	{
		_socketClient.WriteMessage(msgData);
	}
	         
	/// <summary>
	/// 析构函数
	/// </summary>
	void OnDestroy ()
	{
		_socketClient.OnRemove ();
		Debug.Log ("~NetworkManager was destroy");
	}
}