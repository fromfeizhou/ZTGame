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

			private gprotocol.login_auth_key_s2c authData = new gprotocol.login_auth_key_s2c ();
            private GameSocket _gameSocket;
			private Int16 _curMsgSeq;
			public Int16 CurMsgSeq
            {
                get
				{
					_curMsgSeq += (Int16)authData.unique_add;
					if (_curMsgSeq > authData.max_unique_id)
						_curMsgSeq = (Int16)authData.unique_id;
					//Debug.Log ("_curMsgSeq:" + _curMsgSeq);
					return _curMsgSeq;
				}
            }

            private ObjectPool<Message> _msgPool;
			private Dictionary<uint,string> errDic;

			private bool _needReConnect = false;

			public void Init(System.Action<bool> _action_LockScreen)
            {
                RegisterFacades();
                _gameSocket = new GameSocket();
                _gameSocket.Init(NetWorkConst.Ip, NetWorkConst.Port, NetWorkConst.ConnectTimeOut);

                _msgPool = new ObjectPool<Message>(32);

                _gameSocket.CallBack_OnConnect = OnConnect;
                _gameSocket.CallBack_OnSend = OnSend;
                _gameSocket.CallBack_OnDisConnect = OnDisConnect;
                _gameSocket.CallBack_OnReceive = OnReceive;
                _gameSocket.CallBack_OnError = OnError;

				errDic = new Dictionary<uint, string> ();
				TextAsset errCodeStr = Resources.Load<TextAsset> (PathManager.NetWorkErrCodeFilePath);
				Debug.Log (errCodeStr);
				string[] errorStr = errCodeStr.text.Trim().Split ('\n');
				for (int i = 0; i < errorStr.Length; i++) {
					string str = errorStr [i];
					if (str.StartsWith ("-define")) {
						str = str.Substring (8).Split (',') [1].Trim();
						string key = str.Split ('%') [0].Trim ();
						errDic[uint.Parse(key.Substring (0, key.Length - 2))] = str.Split ('%') [1].Trim ();
					}
				}
            }

            public void Connect()
            {
                _gameSocket.Connect();
            }

			public void SendNetMsg(byte facade, byte command, global::ProtoBuf.IExtensible vo)
            {
				Message message = _msgPool.Talk();
				message.Seq = CurMsgSeq;
				message.module = facade;
				message.command = command;
				using (System.IO.MemoryStream m = new System.IO.MemoryStream())
				{
					ProtoBuf.Serializer.Serialize(m, vo);
					message.voData = m.ToArray();
				}
                _gameSocket.WriteData(message.AllBytes);

				//Debug.Log ("[SendSocketMsg]Module:" + facade + ", Command:" + command);
				_msgPool.Recovery(message);

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
				_gameSocket.DisConnect(eErrCode.Null);
            }

			private void OnDisConnect(eErrCode errCode)
            {
				Debug.Log ("连接断开:" + errCode);
				HeartSwitch (false);

				if (errCode == eErrCode.SendMsgFail) {
					_needReConnect = true;
				}

				Connect ();
            }

            private void OnError(string error)
            {
				LoginModule.GetInstance ().LoginPanel.LockPanel.Show ("[网络发生错误],请重启启动客户端");
				Debug.LogError ("[网络发生错误]"+ error);
            }

			/** 锁屏 */
			private void OnLockScreen(){
				
			}

            private void OnConnect()
            {
				Debug.Log ("[" + System.DateTime.Now + "]" + "[" + this.GetType().Name + "]连接IP" + NetWorkConst.Ip + ":" + NetWorkConst.Port + " 成功. 并发送第一个心跳包。");
				SendHeart ();
				if (_needReConnect)
					OnReConnect ();
            }


			private void OnReConnect(){
				gprotocol.login_relogin_c2s vo = new gprotocol.login_relogin_c2s ()
				{
					id = PlayerModule.GetInstance ().RoleID,
					key = authData.relogin_key,
				};
				Debug.Log ("断线重连请求。ID:" + vo.id + ", reLoginKey:" + vo.key);
				SendNetMsg(Module.login,Command.login_relogin,vo);
			}

            private void OnSend()
            {
				
            }

            private void OnReceive(byte[] data)
            {
                Message message = _msgPool.Talk();
                message.Parse(data);
				FacadeInvoking(message);
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
            }

			public void CheckErrCode(uint code)
			{
				if (code != 0 && errDic.ContainsKey (code)) {
					Debug.LogError ("[NetWorkManager][OnReceiveData]ErrCode:" + code + ", Content:" + errDic[code]);
				}
			}
        }
    }
}