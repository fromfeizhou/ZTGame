/*
 * ----------------------------------------------------------------
 * 网络消息数据包 => Client -> Server
 * 格式:[包长][序号][模块][指令][数据]
 * ----------------------------------------------------------------
 * [包长]：  2byte value:（2+1+1+N=4）	=> 整条数据长度
 * [序号]：  2byte value:Seq				=> 数据序号
 * [模块]：  1byte value:Module			=> 模块号
 * [指令]：  1byte value:Command			=> 指令号
 * [数据]:   Nbyte value:ValueObject		=> VO数据
 * ----------------------------------------------------------------
 * 
 * 
 * 网络消息数据包 => Server -> Client
 * 格式:[包长][门面][指令][数据]
 * ----------------------------------------------------------------
 * [包长]：  2byte value:（1+1+N=4）	 	=> 整条数据长度
 * [模块]：  1byte value:Module			=> 模块号
 * [指令]：  1byte value:Command			=> 指令号
 * [数据]:   Nbyte value:ValueObject		=> VO数据
 * ----------------------------------------------------------------
 * 
 */
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;

public enum DisType {
    Exception,
    Disconnect,
}

public class SocketClient {
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];
    public static bool loggedIn = false;

    // Use this for initialization
    public SocketClient() {
		Debug.Log ("[SocketClient] is Create");
    }

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister() {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
		Debug.Log ("[SocketClient] OnRegister");

    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove() {
        this.Close();
        reader.Close();
        memStream.Close();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer(string host, int port) {
        client = null;
        try {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0) {
                Debug.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6) {
                client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else {
                client = new TcpClient(AddressFamily.InterNetwork);
            }
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        } catch (Exception e) {
            Close(); Debug.LogError(e.Message);
        }
    }
	private int msgLen;
	private byte[] _receiveBuff = new byte[8 * 1024];

	private byte[] _dataBuff;
	private int _dataLen;
	public void OnReadZt ()
	{
		NetworkStream ns = client.GetStream();
		while (ns.DataAvailable) {
			try {
				int receiveLen = ns.Read (_receiveBuff, 0, _receiveBuff.Length);
				if (receiveLen > 0) {

					EnsureCapacity (receiveLen);
					Array.Copy (_receiveBuff, 0, _dataBuff, _dataLen, receiveLen);

					int readLen = 0;
					_dataLen += receiveLen;
					msgLen = GetMsgLen (0);
					while (msgLen > 0 && _dataLen >= msgLen) {
						byte[] msgData = new byte[msgLen-2];
						Array.Copy (_dataBuff, readLen+2, msgData, 0, msgData.Length);
						NetWorkManager.AddEvent(Protocal.NetMessage,new ByteBuffer(msgData));

						readLen += msgLen;
						_dataLen -= msgLen;
						msgLen = GetMsgLen (readLen);
					}
					Array.Copy (_dataBuff, readLen , _dataBuff, 0, _dataLen);
				}
			} catch (Exception e) {
				Debug.LogError ("NetWorkManager:" + e.Message);
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


	public class MsgLen
	{
		//Server To Client
		public static int SC_HEADLEN {
			get {
				return MODULE + COMMAND;
			}
		}

		//Client To Server
		public static int CS_HEADLEN {
			get {
				return SEQNUM + MODULE + COMMAND;
			}
		}

		//包长
		public const int PACKERLEN = 2;
		//序号
		private const int SEQNUM = 2;
		//门面
		private const int MODULE = 1;
		//指令
		private const int COMMAND = 1;
	}


    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr) {
        outStream = client.GetStream();
		isConnect = true;
        //client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
        NetWorkManager.AddEvent(Protocal.Connect, new ByteBuffer());
    }
	public bool isConnect = false;
    /// <summary>
    /// 写数据
    /// </summary>
    public void WriteMessage(byte[] message) {
        if (client != null && client.Connected) {
			ByteBuffer byteBuffer = new ByteBuffer ();
			byteBuffer.WriteUInt16 ((ushort)message.Length);
			byteBuffer.WriteBytes (message);
			byte[] payload = byteBuffer.ToBytes();
			byteBuffer.Close ();
			//PrintBytes (payload);
	        outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
        } else {
            Debug.LogError("client.connected----->>false");
        }
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr) {
        int bytesRead = 0;
        try {
            lock (client.GetStream()) {         //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1) {                //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }
            OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
            lock (client.GetStream()) {         //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        } catch (Exception ex) {
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg) {
        Close();   //关掉客户端链接

		int protocal = 0;
		ByteBuffer buffer = new ByteBuffer();
		if (dis == DisType.Disconnect) {
			protocal = Protocal.Disconnect;
		} else {
			protocal = Protocal.Exception;
			buffer.WriteString ("Connection was closed by the server:>" + msg + ", Distype:>" + dis);
		}
        NetWorkManager.AddEvent(protocal, buffer);
    }

    /// <summary>
    /// 打印字节
    /// </summary>
    /// <param name="bytes"></param>
	void PrintBytes(byte[] data = null) {

		if (data == null)
			data = byteBuffer;

        string returnStr = string.Empty;
		for (int i = 0; i < data.Length; i++) {
			returnStr += data[i].ToString("X2") + " ";
        }
        Debug.LogError(returnStr);
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r) {
        try {
            outStream.EndWrite(r);
        } catch (Exception ex) {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes, int length) {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > 4) {
			byte[] msgLenData = reader.ReadBytes (2);
			Array.Reverse (msgLenData);
			ushort messageLen = BitConverter.ToUInt16 (msgLenData,0);
            if (RemainingBytes() >= messageLen) {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(messageLen));
                ms.Seek(0, SeekOrigin.Begin);
                OnReceivedMessage(ms);
            } else {
                //Back up the position two bytes
                memStream.Position = memStream.Position - 2;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes() {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms) {
        BinaryReader r = new BinaryReader(ms);
        byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));
		ByteBuffer buff = new ByteBuffer (message);
		NetWorkManager.AddEvent(Protocal.NetMessage, buff);
    }



    /// <summary>
    /// 关闭链接
    /// </summary>
    public void Close() {
        if (client != null) {
            if (client.Connected) 
				client.Close();
            client = null;
        }
        loggedIn = false;
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
	public void SendConnect(string ip,int port) {
		ConnectServer(ip, port);
    }
}
