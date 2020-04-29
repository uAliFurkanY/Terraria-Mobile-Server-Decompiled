using System.IO;
using Terraria.DataStructures;

namespace Terraria.Net
{
	public struct NetPacket
	{
		public ushort Id;

		public int Length;

		public CachedBuffer Buffer;

		public BinaryWriter Writer => Buffer.Writer;

		public BinaryReader Reader => Buffer.Reader;

		public NetPacket(ushort id, int size)
		{
			Id = id;
			Buffer = BufferPool.Request(size);
			Length = size;
		}

		public void Recycle()
		{
			Buffer.Recycle();
		}
	}
}
