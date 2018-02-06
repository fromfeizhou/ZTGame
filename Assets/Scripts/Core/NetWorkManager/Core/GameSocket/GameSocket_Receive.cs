using System;
using System.Net.Sockets;
using com.game.client.network.utility;

namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
		    private DoubleBuff<byte[]> _receiveMsgQue;

			private int msgLen;
			private byte[] _receiveBuff;

			private byte[] _dataBuff;
			private int _dataLen;
			private string GetContent(byte[] data)
			{
				string output = string.Empty;
				for (int i = 0; i < data.Length; i++)
				{
					output += data[i].ToString() + " ";
				}
				return output.Trim();
			}
			private void onReceive ()
			{
                if (_socket == null || !_socket.Connected)
                {
					return;
				}
                NetworkStream ns = _socket.GetStream();
				while (ns.DataAvailable) {
					try {
						int receiveLen = ns.Read (_receiveBuff, 0, _receiveBuff.Length);
						if (receiveLen > 0) {
							
							EnsureCapacity (receiveLen);
							Array.Copy (_receiveBuff, 0, _dataBuff, _dataLen, receiveLen);

							byte[] aa = new byte[receiveLen];
							Array.Copy (_receiveBuff, 0, aa, 0, receiveLen);

							int readLen = 0;
							_dataLen += receiveLen;
							msgLen = GetMsgLen (0);
							while (msgLen > 0 && _dataLen >= msgLen) {
								byte[] msgData = new byte[msgLen];
								Array.Copy (_dataBuff, readLen, msgData, 0, msgLen);
								_receiveMsgQue.Push (msgData);
								readLen += msgLen;
								_dataLen -= msgLen;
								msgLen = GetMsgLen (readLen);
							}
							Array.Copy (_dataBuff, readLen , _dataBuff, 0, _dataLen);
						}
					} catch (Exception e) {
						Invoking_CallBack_OnError (eErrCode.ReadBuffEx, e);
						DisConnect ();
					}
				}
			}

			/** 待优化项：确保缓存容量 */
			private void EnsureCapacity (int readLen)
			{
				if (_dataBuff == null) {
					_dataBuff = new byte[_dataLen + readLen];
					return;
				}

				int _dataFreeLen = _dataBuff.Length - _dataLen;

				if (_dataFreeLen < readLen) {
					byte[] tmpDataBuff = new byte[_dataLen + readLen];
					Array.Copy (_dataBuff, 0, tmpDataBuff, 0, _dataLen);
					_dataBuff = tmpDataBuff;
				}
			}

			private int GetMsgLen (int readLen)
			{
				if (_dataLen <= MsgLen.SC_HEADLEN) {
					return 0;
				}

				byte[] msgLen = new byte[2];
				Array.Copy (_dataBuff, readLen, msgLen, 0, msgLen.Length);
				Array.Reverse (msgLen);
				return BitConverter.ToInt16 (msgLen,0) + MsgLen.PACKERLEN;
			}

			private void OnCheckReceiveQue ()
			{
				if (_receiveMsgQue.Count > 0) {
					Invoking_CallBack_OnReceive (_receiveMsgQue.Pop ());
				}
			}
		}
	}
}
