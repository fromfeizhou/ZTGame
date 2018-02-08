using System.Collections.Generic;
using UnityEngine;
using com.game.client.network.gamescoket;
using com.game.client.utility;
using System;

namespace com.game.client
{
    namespace network
    {
        public partial class NetWorkManager
        {
            private static NetWorkManager _instace;
            public static NetWorkManager Instace
            {
                get
                {
                    if (_instace == null) _instace = new NetWorkManager();
                    return _instace;
                }
            }

            private GameSocket _gameSocket;
			private Int16 _curMsgSeq;
			public Int16 CurMsgSeq
            {
                get { return _curMsgSeq; }
            }

            //private Dictionary<uint, Message> _waitMsgReceive;
            /*public int WaitMsgCnt {
                get
                {
                    if (_waitMsgReceive == null)
                        return 0;
                    return _waitMsgReceive.Count;
                }
            }*/

            private ObjectPool<Message> _msgPool;

			public void Init(System.Action<bool> _action_LockScreen)
            {
                RegisterFacades();
                _gameSocket = new GameSocket();
                _gameSocket.Init(NetWorkConst.Ip, NetWorkConst.Port, NetWorkConst.ConnectTimeOut);

                //_waitMsgReceive = new Dictionary<uint, Message>();
                _msgPool = new ObjectPool<Message>(32);

                _gameSocket.CallBack_OnConnect = OnConnect;
                _gameSocket.CallBack_OnSend = OnSend;
                _gameSocket.CallBack_OnDisConnect = OnDisConnect;
                _gameSocket.CallBack_OnReceive = OnReceive;
                _gameSocket.CallBack_OnError = OnError;
            }


            public void Connect()
            {
                _gameSocket.Connect();
            }

			public void SendNetMsg(byte facade, byte command, global::ProtoBuf.IExtensible vo)
            {
				Message message = _msgPool.Talk();
				message.Seq = _curMsgSeq++;
				message.module = facade;
				message.command = command;

				using (System.IO.MemoryStream m = new System.IO.MemoryStream())
				{
					ProtoBuf.Serializer.Serialize(m, vo);
					message.voData = m.ToArray();
				}


				/*
                if (_waitMsgReceive.ContainsKey(message.Seq))
                {
                    Debug.LogError("发送队列重复");
                }
                else
                {
                    _waitMsgReceive[message.Seq] = message;
                }*/

                _gameSocket.WriteData(message.AllBytes);
            }
			private string GetContent(byte[] data)
			{
				string output = string.Empty;
				for (int i = 0; i < data.Length; i++)
				{
					output += data[i].ToString("X2") + " ";
				}
				return output.Trim();
			}
            public void DisConnect()
            {
                _gameSocket.DisConnect();
            }

			private int curAutoReConnectTimes;
            private void OnDisConnect()
            {
				Debug.Log ("连接断开");
				HeartSwitch (false);

				//if (NetWorkConst.ReConnectTimes == 0 || curAutoReConnectTimes++ < NetWorkConst.ReConnectTimes) {
				//	Connect ();
				//}
            }

            private void OnError(string error)
            {
				Debug.LogError ("[网络发生错误]"+ error);
            }

			/** 锁屏 */
			private void OnLockScreen(){
				
			}

            private void OnConnect()
            {
				Debug.Log ("[" + System.DateTime.Now + "]" + "[" + this.GetType().Name + "]连接IP" + NetWorkConst.Ip + ":" + NetWorkConst.Port + " 成功. 并发送第一个心跳包。");
				SendHeart ();
                //CheckSendFailMsg();
            }

			/*
            public void CheckSendFailMsg()
            {
                List<Message> waitSendMsgList = new List<Message>();
                foreach (var msg in _waitMsgReceive)
                    waitSendMsgList.Add(msg.Value);

                waitSendMsgList.Sort((left, right) => { return right.Seq.CompareTo(left.Seq); });

                for (int i = 0; i < waitSendMsgList.Count; i++)
                {
                    _gameSocket.WriteData(waitSendMsgList[i].AllBytes);
                }
            }
*/
            private void OnSend()
            {
				
            }

            private void OnReceive(byte[] data)
            {
                Message message = _msgPool.Talk();
				//Debug.Log ("[Client]ReceiveData:" + GetContent(data));
                message.Parse(data);
				FacadeInvoking(message);
				/*
                if (_waitMsgReceive.ContainsKey(message.Seq))
                {
                    Message waitReceiveMsg = _waitMsgReceive[message.Seq];
                    _msgPool.Recovery(waitReceiveMsg);
                    _waitMsgReceive.Remove(message.Seq);
                }
                else
                {
                    Debug.LogError("没有找到对应序号：" + message.Seq);
                }
				*/
                _msgPool.Recovery(message);
            }

            public void Update()
            {
                CheckHeart();

                if (_gameSocket != null)
                    _gameSocket.Update();
            }


            public void Dispose()
            {
                //_waitMsgReceive.Clear();
				curAutoReConnectTimes = 0;
            }
        }
    }
}