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
using com.game.client.utility;
using System;
using com.game.client.network.utility;

namespace com.game.client
{
	namespace network
	{
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

		public class Message : IPoolObject
		{
			/** 序号 */
			public Int16 Seq;

			/** 门面 */
			public byte module;

			/** 指令 */
			public byte command;

			/** VO */
			public byte[] voData;

			private byte[] _allBytes;
			public byte[] AllBytes
			{
				get{
					if (_allBytes == null) {

						StreamBuff writer = new StreamBuff ();
						int packLen = MsgLen.CS_HEADLEN + voData.Length;
						writer.WriteInt16 ((Int16)packLen);
						writer.WriteInt16 (Seq);
						writer.WriteByte (module);
						writer.WriteByte (command);
						writer.WriteBytes (voData);
						_allBytes = writer.GetBuffer ();
					}
					return _allBytes;
				}
			}

			public void Reset()
			{
				Seq = 0;
				_allBytes = null;
			}

			public void Parse(byte[] data)
			{
				StreamBuff reader = new StreamBuff (data);
				int packgerLen = reader.ReadInt16 ();
				module = reader.ReadByte ();
				command	= reader.ReadByte ();
				voData	= reader.ReadBytes (packgerLen - MsgLen.SC_HEADLEN);
				reader.Close ();
			}
		}
	}
}