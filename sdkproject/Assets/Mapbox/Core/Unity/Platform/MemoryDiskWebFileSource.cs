namespace Mapbox.Unity.Platform
{
	using System;
	using System.Collections.Generic;
	using Mapbox.Platform;

	public class MemoryDiskWebFileSource : IFileSource
	{
		IAsyncRequestHandler _requestHandler;

		public MemoryDiskWebFileSource()
		{
			// Build the links.
			var memoryCachingAsyncRequestHandler = new MemoryCachingAsyncRequestHandler(100);
			// TODO: implement disk caching handler!
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
