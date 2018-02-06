using com.game.client.network.facade;
using UnityEngine;


namespace com.game.client
{
    namespace network
    {
        public partial class NetWorkManager
        {
            private bool _isHeart = false;
            public bool IsHeart {
                get { return _isHeart; }
            }

            private float curHeartTime;

			public void HeartSwitch(bool isActive)
            {
                _isHeart = isActive;
				ResetHeartTime();
            }

            private void ResetHeartTime()
            {
				curHeartTime = NetWorkConst.Heart_Time;
            }

			private long sendTime;

            private void CheckHeart()
            {
                if (_isHeart)
                {
					curHeartTime += Time.deltaTime;
					if (curHeartTime >= NetWorkConst.Heart_Time) {
						curHeartTime = 0.0f;
						SendHeart ();
					}
                }
            }

			gprotocol.login_heart_c2s heartVo = new gprotocol.login_heart_c2s();
			private void SendHeart()
			{
				NetWorkManager.Instace.SendNetMsg(Module.login, Command.login_heart, heartVo);
			}
        }
    }
}