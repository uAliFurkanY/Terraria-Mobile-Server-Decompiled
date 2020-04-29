using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NATUPNPLib
{
	[ComImport]
	[TypeIdentifier]
	[Guid("B171C812-CC76-485A-94D8-B6B3A2794E99")]
	[CompilerGenerated]
	public interface IUPnPNAT
	{
		IStaticPortMappingCollection StaticPortMappingCollection
		{
			[DispId(1)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}
	}
}
