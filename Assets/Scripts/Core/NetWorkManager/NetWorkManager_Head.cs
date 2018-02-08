using com.game.client.network.facade;
using UnityEngine;


namespace com.game.client
{
    namespace network
    {
        public partial class NetWorkManager
        {
			private int _ping;
			public int Ping{
				get{ 
					return _ping;
				}
			}


			private System.DateTime _lastSendTime;
			private byte[] _heartBytes;

            private bool _isHeart = false;
            public bool IsHeart {
                get { return _isHeart; }
            }

			private float _maxHeartInterval;
            private float curHeartTime;

			public void SetHeartInterval(float interval)
			{
				_maxHeartInterval = interval;
			}

			public void HeartSwitch(bool isActive)
            {
                _isHeart = isActive;
				ResetHeartTime();
            }

            private void ResetHeartTime()
            {
				curHeartTime = _maxHeartInterval;
            }

			private long sendTime;

            private void CheckHeart()
            {
                if (_isHeart)
                {
					curHeartTime += Time.deltaTime;
					if (curHeartTime >= _maxHeartInterval) {
						SendHeart ();
						curHeartTime = 0.0f;
					}
                }
            }

			private void SendHeart()
			{
				if (_heartBytes == null) {
					Message message = _msgPool.Talk();
					message.Seq = _curMsgSeq++;
					message.module = Module.login;
					message.command = Command.login_heart;

					using (System.IO.MemoryStream m = new System.IO.MemoryStream())
					{
						ProtoBuf.Serializer.Serialize(m, new gprotocol.login_heart_c2s());
						message.voData = m.ToArray();
					}
					_heartBytes = message.AllBytes;
					_msgPool.Recovery(message);
				}

				_lastSendTime = System.DateTime.Now;
				_gameSocket.WriteData(_heartBytes);
			}

			private void OnReceive_Heart(gprotocol.login_heart_s2c vo)
			{
				_ping = (int)(System.DateTime.Now - _lastSendTime).TotalMilliseconds;
			}
        }
    }
}