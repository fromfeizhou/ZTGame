using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;

namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
			private static bool IsConnectionSuccessful = false;  
			private static Exception socketexception;  
			private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);  
			public void Connect ()
			{
                if(IsActive())
                    return;

				if (_state != eConnectState.None) {
					Invoking_CallBack_OnError (eErrCode.ConnectStateErr,null);
					return;
				}

				_state = eConnectState.Connecting;
				AddressFamily family = GetFamily();
				_socket = new TcpClient(family);
				_socket.NoDelay = true;

				UnityEngine.Debug.Log("[" + System.DateTime.Now + "]" + "[" + this.GetType().Name + "]RequestServer. Ip:" + _ip + ", Port:" + _port);
				IsConnectionSuccessful = false;
				_socket.BeginConnect(_ip, _port, new AsyncCallback(CallBack_Connect), _socket);

				if (TimeoutObject.WaitOne(_connectTimeOut, false))  
				{  
					if (IsConnectionSuccessful)  
					{  
						_state = eConnectState.Connected;
						Invoking_CallBack_OnConnect();
						isCheckReceiveQue = true;
						isCheckSendQue = true;
						isOnReceiver = true;
					}  
					else  
					{  
						_state = eConnectState.ConnectionRefused;
						Invoking_CallBack_OnError (eErrCode.ConnectFail, null);
						Dispose ();
					}  
				}  
				else  
				{  
					Invoking_CallBack_OnError (eErrCode.ConnectOutTime, null);
					Dispose ();
				}  
			}


			private void CallBack_Connect(IAsyncResult ar){
				try 
				{
					IsConnectionSuccessful = false;
					TcpClient client = (TcpClient)ar.AsyncState;
					if(client.Client != null)
					{
						client.EndConnect (ar);
						IsConnectionSuccessful = true;
					}
				}
				catch (System.Exception e) {
					IsConnectionSuccessful = false;
					_state = eConnectState.ConnectionRefused;
					Invoking_CallBack_OnError(eErrCode.ConnectRefused, e);
					Dispose ();
				}
				finally  
				{  
					TimeoutObject.Set();  
				}  
			}


		    private AddressFamily GetFamily()
		    {
		        IPAddress[] socketAddress = Dns.GetHostAddresses(_ip);
		        AddressFamily family = AddressFamily.InterNetwork;
		        if (socketAddress[0].AddressFamily == AddressFamily.InterNetwork)
		        {
		            family = AddressFamily.InterNetwork;
		        }
		        else if (socketAddress[0].AddressFamily == AddressFamily.InterNetworkV6)
		        {
		            family = AddressFamily.InterNetworkV6;
		        }
		        return family;
		    }
		}
	}
}