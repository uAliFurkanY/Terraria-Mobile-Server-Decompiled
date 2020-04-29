using System;
using System.IO;

namespace Terraria.IO
{
	public class FileMetadata
	{
		public const ulong MAGIC_NUMBER = 27981915666277746uL;

		public const int SIZE = 20;

		public FileType Type;

		public uint Revision;

		public bool IsFavorite;

		private FileMetadata()
		{
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0x6369676F6C6572L | ((ulong)Type << 56));
			writer.Write(Revision);
			writer.Write((ulong)(IsFavorite.ToInt() & 1));
		}

		public void IncrementAndWrite(BinaryWriter writer)
		{
			Revision++;
			Write(writer);
		}

		public static FileMetadata FromCurrentSettings(FileType type)
		{
			FileMetadata fileMetadata = new FileMetadata();
			fileMetadata.Type = type;
			fileMetadata.Revision = 0u;
			fileMetadata.IsFavorite = false;
			return fileMetadata;
		}

		public static FileMetadata Read(BinaryReader reader, FileType expectedType)
		{
			FileMetadata fileMetadata = new FileMetadata();
			fileMetadata.Read(reader);
			if (fileMetadata.Type != expectedType)
			{
				throw new FileFormatException("Expected type \"" + Enum.GetName(typeof(FileType), expectedType) + "\" but found \"" + Enum.GetName(typeof(FileType), fileMetadata.Type) + "\".");
			}
			return fileMetadata;
		}

		private void Read(BinaryReader reader)
		{
			ulong num = reader.ReadUInt64();
			if ((num & 0xFFFFFFFFFFFFFFL) != 27981915666277746L)
			{
				throw new FileFormatException("Expected Re-Logic file format.");
			}
			byte b = (byte)((num >> 56) & 0xFF);
			FileType fileType = FileType.None;
			FileType[] array = (FileType[])Enum.GetValues(typeof(FileType));
			for (int i = 0; i < array.Length; i++)
			{
				if ((uint)array[i] == b)
				{
					fileType = array[i];
					break;
				}
			}
			if (fileType == FileType.None)
			{
				throw new FileFormatException("Found invalid file type.");
			}
			Type = fileType;
			Revision = reader.ReadUInt32();
			ulong num2 = reader.ReadUInt64();
			IsFavorite = ((num2 & 1) == 1);
		}
	}
}
