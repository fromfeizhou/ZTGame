using System.IO;
using System;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections;

// 压缩或解压文件  
public class CompressHelper
{
	public class CompressTask : IEnumerator
	{
		public float Progress;
		public object Current
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool MoveNext()
		{
			return !IsDone;
		}

		public void Reset()
		{
			throw new System.NotImplementedException();
		}

		public string Error;

		public bool IsDone
		{
			get { return Progress >= 1.0f; }
		}

	}
	public static CompressTask UnCompressAsync(string zipPath, string outPath)
	{
		CompressTask task = new CompressTask();
		Loom.RunAsync(() => UnZipFile(zipPath, outPath, out task.Error, out task.Progress));
		return task;
	}

	/// <summary>
	/// 功能：压缩文件（暂时只压缩文件夹下一级目录中的文件，文件夹及其子级被忽略）
	/// </summary>
	/// <param name="dirPath">被压缩的文件夹夹路径</param>
	/// <param name="zipFilePath">生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名+.zip</param>
	/// <param name="err">出错信息</param>
	/// <returns>是否压缩成功</returns>
	public static bool ZipFile(string dirPath, string zipFilePath, out string err, out float progress)
	{
		progress = 0.0f;
		err = "";
		if (dirPath == string.Empty)
		{
			err = "要压缩的文件夹不能为空！";
			return false;
		}
		if (!Directory.Exists(dirPath))
		{
			err = "要压缩的文件夹不存在！";
			return false;
		}
		//压缩文件名为空时使用文件夹名＋.zip
		if (zipFilePath == string.Empty)
		{
			if (dirPath.EndsWith("//"))
			{
				dirPath = dirPath.Substring(0, dirPath.Length - 1);
			}
			zipFilePath = dirPath + ".zip";
		}

		try
		{
			using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
			{
				s.SetLevel(1);
				byte[] buffer = new byte[4096];

				DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

				FileInfo[] fileInfos = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
				long totalSize = 0;
				for (int i = 0; i < fileInfos.Length; i++)
				{
					totalSize += fileInfos[i].Length;
				}
				long curSize = 0;
				for (int i = 0; i < fileInfos.Length; i++)
				{
					FileInfo file = fileInfos[i];

					ZipEntry entry = new ZipEntry(file.FullName.Replace(dirInfo.FullName, string.Empty));
					entry.DateTime = DateTime.Now;
					s.PutNextEntry(entry);
					using (FileStream fs = File.OpenRead(file.FullName))
					{
						int sourceBytes;
						do
						{
							sourceBytes = fs.Read(buffer, 0, buffer.Length);
							s.Write(buffer, 0, sourceBytes);
							curSize += sourceBytes;
							progress = (float) ((double) curSize / (double) totalSize);
						} while (sourceBytes > 0);
					}
				}
				s.Finish();
				s.Close();
			}
		}
		catch (Exception ex)
		{
			err = ex.Message;
			return false;
		}
		return true;
	}

	/// <summary>
	/// 功能：解压zip格式的文件。
	/// </summary>
	/// <param name="zipFilePath">压缩文件路径</param>
	/// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
	/// <param name="err">出错信息</param>
	/// <returns>解压是否成功</returns>
	public static bool UnZipFile(string zipFilePath, string unZipDir, out string err, out float progress)
	{
		progress = 0.0f;
		err = "";
		if (zipFilePath == string.Empty)
		{
			err = "压缩文件不能为空！";
			return false;
		}
		if (!File.Exists(zipFilePath))
		{
			err = "压缩文件不存在！";
			return false;
		}
		//解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
		if (unZipDir == string.Empty)
			unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath),
				Path.GetFileNameWithoutExtension(zipFilePath));
		if (!unZipDir.EndsWith("//"))
			unZipDir += "//";
		if (!Directory.Exists(unZipDir))
			Directory.CreateDirectory(unZipDir);

		try
		{
			long totalSize = 0;
			using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
			{
				ZipEntry theEntry;
				while ((theEntry = s.GetNextEntry()) != null)
				{
					totalSize += theEntry.Size;
				}
				s.Close();
			}

			using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
			{
				ZipEntry theEntry;
				long curUnZipSize = 0;
				while ((theEntry = s.GetNextEntry()) != null)
				{
					string directoryName = Path.GetDirectoryName(theEntry.Name);
					string fileName = Path.GetFileName(theEntry.Name);
					if (directoryName.Length > 0)
					{
						Directory.CreateDirectory(unZipDir + directoryName);
					}
					if (!directoryName.EndsWith("//"))
						directoryName += "//";
					if (fileName != String.Empty)
					{
						string destPath = unZipDir + theEntry.Name;
						using (FileStream streamWriter = File.Create(destPath))
						{
							int size = 2048;
							byte[] data = new byte[2048];
							while (true)
							{
								size = s.Read(data, 0, data.Length);
								if (size > 0)
								{
									streamWriter.Write(data, 0, size);
								}
								else
								{
									break;
								}

								curUnZipSize += (long) size;
								progress = (float) ((double) curUnZipSize / (double) totalSize);
							}
						}
					}
				} //while
				s.Close();
			}
		}
		catch (Exception ex)
		{
			err = ex.Message;
			return false;
		}
		return true;
	} //解压结束
}