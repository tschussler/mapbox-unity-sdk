namespace Mapbox.Unity.Platform
{
	using System;
	using Mapbox.Platform;

	public class UnityWebRequestAsyncHandler : AbstractAyncRequestHandler
	{
		public override IAsyncRequest Request(string uri, Action<Response> callback, int timeout = 10)
		{
			return HandleHere(uri, callback, timeout);
		}

		protected internal override IAsyncRequest HandleHere(string uri, Action<Response> callback, int timeout = 10)
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

			return new Utilities.HTTPRequest(uriBuilder.ToString(), (response) =>
				{
					PreviousHandler.CacheResponse(uri, response);
					callback(response);
				}, timeout);
		}
	}
}
