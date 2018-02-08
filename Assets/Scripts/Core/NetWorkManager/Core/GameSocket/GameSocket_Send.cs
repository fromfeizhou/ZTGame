using System;
using System.Net.Sockets;
using com.game.client.network.utility;

namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
		    private DoubleBuff<byte[]> _sendMsgQue;

			public void WriteData(byte[] data){
                if (_socket == null || _socket.Connected == false || data == null || data.Length == 0)
			    {
			        return;
			    }

                _sendMsgQue.Push(data);
			
			}

            private void OnCheckSendQue()
		    {
                for (int i = 0; i < NetWorkConst.MxSendMsgNum; i++)
		        {
		            if (_sendMsgQue.Count > 0)
		            {
		                byte[] data = _sendMsgQue.Pop();
		                try
		                {
                            NetworkStream ns = _socket.GetStream();
                            ns.BeginWrite(data, 0, data.Length, Send_CallBack, _socket);
		                }
		                catch (Exception e)
		                {
		                    Invoking_CallBack_OnError(eErrCode.SendMsgEx, e);
		                    DisConnect();
		                }
		            }
		        }
		    }

		    private void Send_CallBack (IAsyncResult ar)
			{
				var client = (TcpClient)ar.AsyncState;
				try {
					NetworkStream ns = client.GetStream ();
					ns.EndWrite (ar);
					Invoking_CallBack_OnSend();
				} catch (Exception e) {
					Invoking_CallBack_OnError(eErrCode.SendMsgFail, e);
				}
			}
		}
	}
}
