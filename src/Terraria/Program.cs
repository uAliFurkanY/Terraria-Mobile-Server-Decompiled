using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Initializers;
using Terraria.Social;

namespace Terraria
{
	public static class Program
	{
		public const bool IsServer = true;

		public static Dictionary<string, string> LaunchParameters = new Dictionary<string, string>();

		public static void LaunchGame(string[] args)
		{
			LaunchParameters = Utils.ParseArguements(args);
			using (Main main = new Main())
			{
				try
				{
					SocialAPI.Initialize();
					LaunchInitializer.LoadParameters(main);
					main.DedServ();
				}
				catch (Exception e)
				{
					DisplayException(e);
				}
			}
		}

		private static void DisplayException(Exception e)
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true))
				{
					streamWriter.WriteLine(DateTime.Now);
					streamWriter.WriteLine(e);
					streamWriter.WriteLine("");
				}
				Console.WriteLine("Server crash: " + DateTime.Now);
				Console.WriteLine(e);
				Console.WriteLine("");
				Console.WriteLine("Please send crashlog.txt to support@terraria.org");
			}
			catch
			{
			}
		}
	}
}
