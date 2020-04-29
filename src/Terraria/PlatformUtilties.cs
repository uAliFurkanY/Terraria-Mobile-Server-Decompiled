using System.Threading;
using System.Windows.Forms;

namespace Terraria
{
	public static class PlatformUtilties
	{
		public static string GetClipboard()
		{
			string clipboardText = "";
			Thread thread = new Thread((ThreadStart)delegate
			{
				clipboardText = Clipboard.GetText();
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			char[] array = new char[clipboardText.Length];
			int length = 0;
			for (int i = 0; i < clipboardText.Length; i++)
			{
				if (clipboardText[i] >= ' ' && clipboardText[i] != '\u007f')
				{
					array[length++] = clipboardText[i];
				}
			}
			return new string(array, 0, length);
		}

		public static void SetClipboard(string text)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				if (text.Length > 0)
				{
					Clipboard.SetText(text);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
		}
	}
}
