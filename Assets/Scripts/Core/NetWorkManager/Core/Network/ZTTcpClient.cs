using UnityEngine;
using System.Collections;
//引入库
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using LuaFramework;
using System.IO;

public class ZTTcpClient
{

    Socket serverSocket; //服务器端socket
    Thread connectThread; //连接线程
    byte[] clientBuffer = new byte[8192];
    private MemoryStream memStream;
    private BinaryReader reader;

    public void ConnectServer(string ip, int port)
    {
        SocketQuit();
      

        //定义套接字类型,必须在子线程中定义
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //连接
        IPAddress ipAddress = IPAddress.Parse(ip); 
        IPEndPoint ipEnd = new IPEndPoint(ipAddress, port);
        serverSocket.ReceiveBufferSize = 8192;
        serverSocket.NoDelay = true;
        serverSocket.Connect(ipEnd);

        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Priority = System.Threading.ThreadPriority.Highest;
        connectThread.Start();

        //ZTSceneManager.GetInstance().StartCoroutine(OnReceiveCoroutine());
        //serverSocket.BeginReceive(clientBuffer, 0, this.clientBuffer.Length, SocketFlags.None,
        //   new System.AsyncCallback(clientReceive), this.serverSocket);
        NetWorkManager.AddEvent(Protocal.Connect, new ByteBuffer());

    }

    IEnumerator OnReceiveCoroutine()
    {
        while (true)
        {
            int byteCount = 0;
            try
            {
                //结束接受数据 完成储存  
                byteCount = serverSocket.Receive(clientBuffer);

            }
            catch (SocketException ex)
            {
            }
            Debug.Log(byteCount);
            if (byteCount > 0)
            {
                //发送数据  
                OnReceiveSync(clientBuffer, byteCount);
                Array.Clear(clientBuffer, 0, clientBuffer.Length);   //清空数组
            }
            yield return null;
        }
    }

    void clientReceive(System.IAsyncResult ar)
    {
        //获取一个客户端正在接受数据的对象  
        Socket workingSocket = ar.AsyncState as Socket;
        int byteCount = 0;
        string content = "";
        try
        {
            //结束接受数据 完成储存  
            byteCount = workingSocket.EndReceive(ar);

        }
        catch (SocketException ex)
        {
            ////如果接受消息失败  
            //clientReceiveCallBack(ex.ToString());
        }
        if (byteCount > 0)
        {
            //发送数据  
            OnReceiveSync(clientBuffer, byteCount);
            Array.Clear(clientBuffer, 0, clientBuffer.Length);   //清空数组
        }
       
        //接受下一波数据  
        serverSocket.BeginReceive(clientBuffer, 0, this.clientBuffer.Length, SocketFlags.None,
           new System.AsyncCallback(clientReceive), this.serverSocket);

    }
    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceiveSync(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > 4)
        {
            byte[] msgLenData = reader.ReadBytes(2);
            Array.Reverse(msgLenData);
            ushort messageLen = BitConverter.ToUInt16(msgLenData, 0);
            if (RemainingBytes() >= messageLen)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(messageLen));
                ms.Seek(0, SeekOrigin.Begin);
                BinaryReader r = new BinaryReader(ms);
                byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));

                OnReceive(message);
            }
            else
            {
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
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    //public void SocketSend(string sendStr)
    //{
    //    //清空发送缓存
    //    byte[] sendData = new byte[1024];
    //    //数据类型转换
    //    sendData = Encoding.ASCII.GetBytes(sendStr);
    //    //发送
    //    serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
    //}

    /// <summary>
    /// 写数据
    /// </summary>
    public void WriteMessage(byte[] message)
    {
        if (serverSocket != null && serverSocket.Connected)
        {
            ByteBuffer byteBuffer = new ByteBuffer();
            byteBuffer.WriteUInt16((ushort)message.Length);
            byteBuffer.WriteBytes(message);
            byte[] payload = byteBuffer.ToBytes();
            byteBuffer.Close();
            //PrintBytes (payload);
            serverSocket.Send(payload, payload.Length, SocketFlags.None);
        }
        else
        {
            Debug.LogError("client.connected----->>false");
        }
    }

    void SocketReceive()
    {
        try
        {
            OnRead();
        }
        catch (Exception ex)
        {
            OnDisconnected(DisType.Exception, ex.Message);
        }

       
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        SocketQuit();   //关掉客户端链接

        int protocal = 0;
        ByteBuffer buffer = new ByteBuffer();
        if (dis == DisType.Disconnect)
        {
            protocal = Protocal.Disconnect;
        }
        else
        {
            protocal = Protocal.Exception;
            buffer.WriteString("Connection was closed by the server:>" + msg + ", Distype:>" + dis);
        }
        NetWorkManager.AddEvent(protocal, buffer);
    }

    void OnRead()
    {
        while (true)
        {
            //接受消息头（消息长度2字节 = 2字节）  
            int HeadLength = 2;
            //存储消息头的所有字节数  
            byte[] recvBytesHead = new byte[HeadLength];
            //如果当前需要接收的字节数大于0，则循环接收  
            while (HeadLength > 0)
            {
                byte[] recvBytes1 = new byte[HeadLength];
                //将本次传输已经接收到的字节数置0  
                int iBytesHead = 0;
                //如果当前需要接收的字节数大于缓存区大小，则按缓存区大小进行接收，相反则按剩余需要接收的字节数进行接收  
                if (HeadLength >= recvBytes1.Length)
                {
                    iBytesHead = serverSocket.Receive(recvBytes1, recvBytes1.Length, 0);
                }
                else
                {
                    iBytesHead = serverSocket.Receive(recvBytes1, HeadLength, 0);
                }
                //将接收到的字节数保存  
                recvBytes1.CopyTo(recvBytesHead, recvBytesHead.Length - HeadLength);
                //减去已经接收到的字节数  
                HeadLength -= iBytesHead;
            }
            //接收消息体（消息体的长度存储）
            Array.Reverse(recvBytesHead);
            int BodyLength = BitConverter.ToUInt16(recvBytesHead, 0);
            //存储消息体的所有字节数  
            byte[] recvBytesBody = new byte[BodyLength];
            //如果当前需要接收的字节数大于0，则循环接收  
            while (BodyLength > 0)
            {
                byte[] recvBytes2 = new byte[BodyLength < 1024 ? BodyLength : 1024];
                //将本次传输已经接收到的字节数置0  
                int iBytesBody = 0;
                //如果当前需要接收的字节数大于缓存区大小，则按缓存区大小进行接收，相反则按剩余需要接收的字节数进行接收  
                if (BodyLength >= recvBytes2.Length)
                {
                    iBytesBody = serverSocket.Receive(recvBytes2, recvBytes2.Length, 0);
                }
                else
                {
                    iBytesBody = serverSocket.Receive(recvBytes2, BodyLength, 0);
                }
                //将接收到的字节数保存  
                recvBytes2.CopyTo(recvBytesBody, recvBytesBody.Length - BodyLength);
                //减去已经接收到的字节数  
                BodyLength -= iBytesBody;
            }
            //一个消息包接收完毕，解析消息包  
            OnReceive(recvBytesBody);
        }
    }


    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes)
    {
        ByteBuffer buff = new ByteBuffer(bytes);
        NetWorkManager.AddEvent(Protocal.NetMessage, buff);
    }



    public void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭服务器
        if (serverSocket != null)
            serverSocket.Close();
        connectThread = null;
        serverSocket = null;
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
	public void SendConnect(string ip, int port)
    {
        ConnectServer(ip, port);
    }

    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    public void OnRemove()
    {
        SocketQuit();

        reader.Close();
        memStream.Close();
    }
}
