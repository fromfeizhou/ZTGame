using UnityEngine;
using System.Collections;
//引入库
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using LuaFramework;

public class ZTTcpClient
{
    string editString = "hello wolrd"; //编辑框文字

    Socket serverSocket; //服务器端socket
   
    string recvStr; //接收的字符串
    string sendStr; //发送的字符串
    int recvLen; //接收的数据长度
    Thread connectThread; //连接线程

    public void ConnectServer(string ip, int port)
    {
        SocketQuit();

        //定义套接字类型,必须在子线程中定义
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //连接
        IPAddress ipAddress = IPAddress.Parse(ip); 
        IPEndPoint ipEnd = new IPEndPoint(ipAddress, port);
        serverSocket.Connect(ipEnd);

        //输出初次连接收到的字符串
        //recvLen = serverSocket.Receive(recvData);
        //recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);

        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();

        NetWorkManager.AddEvent(Protocal.Connect, new ByteBuffer());

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

    }

    public void OnRemove()
    {
        SocketQuit();
    }
}
