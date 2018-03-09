using System;
using System.IO;
using System.Net;
using System.Text;
using XLua;
//
[LuaCallCSharp]
public class ByteBuffer
{
	private MemoryStream stream;
	private BinaryReader reader;
	private BinaryWriter writer;

	public ByteBuffer (byte[] buffer = null)
	{  
		if (buffer == null) {  
			this.stream = new MemoryStream ();  
		} else {  
			this.stream = new MemoryStream (buffer);  
		}  
  
		this.reader = new BinaryReader (this.stream);  
		this.writer = new BinaryWriter (this.stream);  
	}

	public void Close ()
	{  
		if (writer != null) writer.Close();
		if (reader != null) reader.Close();

		stream.Close();
		writer = null;
		reader = null;
		stream = null;
	}

	// -------------------------------------------------------------------------------
	public byte ReadByte ()
	{  
		return reader.ReadByte ();  
	}
	public byte[] ReadBytes (int len)
	{  
		return reader.ReadBytes (len);  
	}

	public int ReadInt(){
		return ReadInt32 ();
	}
	public uint ReadUInt(){
		return ReadUInt32 ();
	}

	public Int16 ReadInt16(){
		return reader.ReadInt16 ();
	}
	public UInt16 ReadUInt16(){
		return reader.ReadUInt16 ();
	}

	public Int32 ReadInt32(){
		return reader.ReadInt32 ();
	}
	public UInt32 ReadUInt32(){
		return reader.ReadUInt32 ();
	}

	public Int64 ReadInt64(){
		return reader.ReadInt64 ();
	}
	public UInt64 ReadUInt64(){
		return reader.ReadUInt64 ();
	}

	public string ReadString ()
	{  
		int len = ReadInt();
		byte[] buffer = new byte[len];
		buffer = reader.ReadBytes(len);
		return Encoding.UTF8.GetString(buffer);
	}

	// -------------------------------------------------------------------------------
	public void WriteByte (byte value)
	{  
		this.writer.Write (value);  
	}
	public void WriteBytes (byte[] value)
	{  
		this.writer.Write (value);  
	}

	public void WriteInt(int value){
		byte[] data = BitConverter.GetBytes (value);
		CheckBigLittleEndianFilp (data);
		this.writer.Write(data);  
	}
	public void WriteUInt(uint value){
		byte[] data = BitConverter.GetBytes (value);
		CheckBigLittleEndianFilp (data);
		this.writer.Write(data);  
	}

	public void WriteInt16 (Int16 value)
	{
		byte[] data = BitConverter.GetBytes (value);
		CheckBigLittleEndianFilp (data);
		this.writer.Write(data);  
	}
	public void WriteUInt16 (UInt16 value)
	{
		byte[] data = BitConverter.GetBytes (value);
		CheckBigLittleEndianFilp (data);
		this.writer.Write(data);  
	}

	public void WriteInt32 (Int32 value)
	{  
		byte[] data = BitConverter.GetBytes (value);
		CheckBigLittleEndianFilp (data);
		this.writer.Write(data); 
	}
	public void WriteUInt32 (UInt32 value)
	{  
		byte[] data = BitConverter.GetBytes (value);
		CheckBigLittleEndianFilp (data);
		this.writer.Write(data); 
	}

	public void WriteInt64 (Int64 value)
	{  
		byte[] data = BitConverter.GetBytes (value);
		CheckBigLittleEndianFilp (data);
		this.writer.Write(data); 
	}
	public void WriteUInt64 (UInt64 value)
	{  
		byte[] data = BitConverter.GetBytes (value);
		CheckBigLittleEndianFilp (data);
		this.writer.Write(data); 
	}

	public void WriteString (string value)
	{  
		byte[] bytes = Encoding.UTF8.GetBytes(value);
		WriteInt(bytes.Length);
		WriteBytes(bytes);
	}

	// -------------------------------------------------------------------------------

	public byte[] ToBytes ()
	{  
		return this.stream.ToArray ();  
	}

	public int GetLength ()
	{  
		return (int)this.stream.Length;  
	}


	// -------------------------------------------------------------------------------
	/** 大小端检查 */
	private void CheckBigLittleEndianFilp(byte[] data){
		if (BitConverter.IsLittleEndian)
			Array.Reverse (data);
	}

}