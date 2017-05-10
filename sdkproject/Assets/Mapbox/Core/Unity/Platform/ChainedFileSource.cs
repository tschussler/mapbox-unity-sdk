using Mapbox.Unity.Platform;
namespace Mapbox.Core.Unity.Platform
{
	using System;
	using Mapbox.Platform;
	using UnityEngine;

	// TODO: who owns this? Inject into MapboxAccess?

	// CHANGE this to a filesource factory, no instance, configuration settings from a json file configured through an editor window

	public class ChainedFileSource : IFileSource
	{
		static ChainedFileSource _instance = new ChainedFileSource();

		/// <summary>
		/// The singleton instance.
		/// </summary>
		public static ChainedFileSource Instance
		{
			get
			{
				return _instance;
			}
		}

		MemoryCacheAsyncRequestHandler _cachedMemoryHandler;
		public MemoryCacheAsyncRequestHandler CachedMemoryHandler
		{
			get
			{
				return _cachedMemoryHandler;
			}
		}

		ChainedFileSource()
		{
			_cachedMemoryHandler = new MemoryCacheAsyncRequestHandler();
			var webHandler = new UnityWebRequestAsyncHandler();
			webHandler.PreviousHandler = _cachedMemoryHandler;
			
			_cachedMemoryHandler.NextHandler = webHandler;
		}

		public IAsyncRequest Request(string uri, Action<Response> callback, int timeout = 10)
		{
			return _cachedMemoryHandler.Request(uri, callback, timeout);
		}
	}
}
