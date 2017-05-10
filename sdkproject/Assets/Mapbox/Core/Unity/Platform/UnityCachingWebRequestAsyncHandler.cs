namespace Mapbox.Unity.Platform
{
	using System;
	using Mapbox.Platform;
	using System.Collections.Generic;

	public class UnityCachingWebRequestAsyncHandler : AbstractCachingAyncRequestHandler
	{
		List<IResponseCachingStrategy> _cachingStrategies;

		public UnityCachingWebRequestAsyncHandler(List<IResponseCachingStrategy> cachingStrategies)
		{
			_cachingStrategies = cachingStrategies;
		}

		protected override bool CanHandle(string url)
		{
			return true;
		}

		protected override IAsyncRequest Handle(string uri, Action<Response> callback, int timeout = 10)
		{
			var uriBuilder = new UriBuilder(uri);
			string accessTokenQuery = "access_token=" + MapboxAccess.Instance.AccessToken;

			if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
			{
				uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + accessTokenQuery;
			}
			else
			{
				uriBuilder.Query = accessTokenQuery;
			}

			return new Utilities.HTTPRequest(uriBuilder.ToString(), callback, timeout);
		}
		
		public override bool ShouldCache(string key, Response response)
		{
			return !response.HasError && _cachingStrategies != null;
		}

		public override void Cache(string key, Response response)
		{
			foreach (var cachingStrategy in _cachingStrategies)
			{
				if (cachingStrategy.ShouldCache(key, response))
				{
					cachingStrategy.Cache(key, response);
				}
			}
		}
	}
}
