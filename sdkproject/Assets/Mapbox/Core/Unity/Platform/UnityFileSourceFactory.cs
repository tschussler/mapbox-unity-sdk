namespace Mapbox.Unity.Platform
{
	using Mapbox.Platform;

	public class UnityFileSourceFactory
	{
		public static IFileSource MemoryDiskWebFileSource
		{
			get { return new MemoryDiskWebFileSource(); }
		}

		public static IFileSource WebFileSource
		{
			get { return new WebFileSource(); }
		}
	}
}
