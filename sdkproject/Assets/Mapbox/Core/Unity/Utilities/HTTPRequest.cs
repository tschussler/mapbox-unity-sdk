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
	using UnityEditor;

#if UNITY_EDITOR
	using UnityEngine;
#endif

	internal sealed class HTTPRequest : IAsyncRequest
	{

		private UnityWebRequest _request;
		private int _timeout;
		private Action<Response> _callback;

		int _id;
		bool _didAbort;

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
			_id = Runnable.Run(DoRequest());

		}

		public void Cancel()
		{
			if (_request != null)
			{
				_request.Abort();
				Runnable.Stop(_id);
				Debug.Log("HTTPRequest: " + "ABORT: " + _request.url);
				_didAbort = true;
				//_request.Dispose();
				//_request = null;
			}
		}

		private IEnumerator DoRequest()
		{
			_request.Send();
			while (!_request.isDone)
			{
				Debug.Log("HTTPRequest: " + _didAbort);
				yield return 0;
			}

			if (_didAbort)
			{
				Debug.Log("HTTPRequest: " + "DID NOT ABORT PROPERLY: " + _request.url);
			}
			var response = Response.FromWebResponse(this, _request, null);
			_callback(response);
			_request.Dispose();
			_request = null;
			IsCompleted = true;
		}
	}
}
