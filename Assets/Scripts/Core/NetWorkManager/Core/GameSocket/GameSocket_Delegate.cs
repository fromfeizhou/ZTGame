namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
			/** 设置委托 - 连接超时 */
			private void delegate_TimeOut (bool active)
			{
				if (active) {
                    if (OnUpdate != OnTimeOut)
                        OnUpdate += OnTimeOut;
				} else {
                    if (OnUpdate == OnTimeOut)
                        OnUpdate -= OnTimeOut;    
				}
			}

		    /** 设置委托 - 检测发送队列 */
		    private void delegate_CheckSendQue(bool active)
		    {
		        if (active)
		        {
                    if (OnUpdate != OnCheckSendQue)
                        OnUpdate += OnCheckSendQue;
		        }
		        else
		        {
                    if (OnUpdate == OnCheckSendQue)
                        OnUpdate -= OnCheckSendQue;
		        }
		    }


		    /** 设置委托 - 检测接收队列 */
			private void delegate_CheckReceiveQue (bool active)
			{
				if (active) {
                    if (OnUpdate != OnCheckReceiveQue)
                        OnUpdate += OnCheckReceiveQue;
				} else {
                    if (OnUpdate == OnCheckReceiveQue)
                        OnUpdate -= OnCheckReceiveQue;
				}
			}

			/** 设置委托 - 设置数据接收者 */
			private void delegate_OnReceiver (bool active)
			{
				if (active) {
                    if (OnUpdate != onReceive)
					    OnUpdate += onReceive;
				} else {
                    if (OnUpdate == onReceive)
					    OnUpdate -= onReceive;
				}
			}
		}
	}
}