namespace Mapbox.Unity.Platform
{
	using System;
	using Mapbox.Platform;

	public class WebFileSource : IFileSource
	{
		IAsyncRequestHandler _requestHandler;

		public WebFileSource()
		{
			// Build the links.
			var webHandler = new UnityCachingWebRequestAsyncHandler(null);

			// Chain them together.

			// Set the starting link.
			_requestHandler = webHandler;
		}

		public IAsyncRequest Request(string uri, Action<Response> callback, int timeout = 10)
		{
			return _requestHandler.Request(uri, callback, timeout);
		}
	}
}
