//-----------------------------------------------------------------------
// <copyright file="HTTPRequest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Mapbox.Unity.Utilities
{

	using System;
	using UnityEngine.Networking;
	using System.Collections;
	using Mapbox.Platform;
	using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	internal sealed class HTTPRequest : IAsyncRequest
	{

		private UnityWebRequest _request;
		private int _timeout;
		private readonly Action<Response> _callback;

		public bool IsCompleted { get; private set; }

		public HTTPRequest(string url, Action<Response> callback, int timeout = 10)
		{
			IsCompleted = false;
			_timeout = timeout;
			_request = UnityWebRequest.Get(url);
			_callback = callback;

#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				Runnable.EnableRunnableInEditor();
			}
#endif
			Runnable.Run(DoRequest());
		}

		public void Cancel()
		{
			if (_request != null)
			{
				_request.Abort();
				Debug.Log("HTTPRequest: " + "ABORT: " + _request.url);
			}
		}

		private IEnumerator DoRequest()
		{
			_request.Send();
			while (!_request.isDone)
			{
				// TODO: implement timeout.
				yield return 0;
			}

			// This will occur even if cancelled. 
			var response = Response.FromWebResponse(this, _request, null);
			if (_request.isError)
			{
				response.AddException(new Exception(_request.error));
			}
			_callback(response);
			_request.Dispose();
			_request = null;
			IsCompleted = true;
		}
	}
}
