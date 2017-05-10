using Mapbox.Unity.Platform;
namespace Mapbox.Core.Unity.Platform
{
	using System;
	using System.Collections.Generic;
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

		IAsyncRequestHandler _requestHandler;

		ChainedFileSource()
		{
			// Build the links.
			var memoryCachingAsyncRequestHandler = new MemoryCachingAsyncRequestHandler(100);
			var webHandler = new UnityCachingWebRequestAsyncHandler(new List<IResponseCachingStrategy> { memoryCachingAsyncRequestHandler });

			// Chain them together.
			memoryCachingAsyncRequestHandler.NextHandler = webHandler;

			// Set the starting link.
			_requestHandler = memoryCachingAsyncRequestHandler;
		}

		public IAsyncRequest Request(string uri, Action<Response> callback, int timeout = 10)
		{
			return _requestHandler.Request(uri, callback, timeout);
		}
	}
}
