using System.Collections.Generic;
using System.Diagnostics;
using Terraria.GameContent.UI.States;

namespace Terraria.World.Generation
{
	internal class WorldGenerator
	{
		private List<GenPass> _passes = new List<GenPass>();

		private float _totalLoadWeight;

		public void Append(GenPass pass)
		{
			_passes.Add(pass);
			_totalLoadWeight += pass.Weight;
		}

		public void GenerateWorld(GenerationProgress progress = null)
		{
			Stopwatch stopwatch = new Stopwatch();
			float num = 0f;
			foreach (GenPass pass in _passes)
			{
				num += pass.Weight;
			}
			if (progress == null)
			{
				progress = new GenerationProgress();
			}
			progress.TotalWeight = num;
			string text = "";
			Main.MenuUI.SetState(new UIWorldLoad(progress));
			Main.menuMode = 888;
			foreach (GenPass pass2 in _passes)
			{
				stopwatch.Start();
				progress.Start(pass2.Weight);
				pass2.Apply(progress);
				progress.End();
				string text2 = text;
				text = text2 + "Pass - " + pass2.Name + " : " + stopwatch.Elapsed.TotalMilliseconds + ",\n";
				stopwatch.Reset();
			}
		}
	}
}
