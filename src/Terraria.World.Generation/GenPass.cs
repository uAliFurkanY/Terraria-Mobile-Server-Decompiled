namespace Terraria.World.Generation
{
	internal abstract class GenPass : GenBase
	{
		public string Name;

		public float Weight;

		public GenPass(string name, float loadWeight)
		{
			Name = name;
			Weight = loadWeight;
		}

		public abstract void Apply(GenerationProgress progress);
	}
}
