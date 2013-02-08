using System;
using System.Collections.Generic;
using System.IO;
using CodeEditor.Composition;

namespace Debugger.Unity.Standalone
{
	[Export (typeof (ISourcesProvider))]
	class SourcesProvider : ISourcesProvider
	{
		private FileSystemWatcher fsw = null;
		private IList<string> sources = new List<string> ();
		public int Port { get; set; }
		public string Path { get; set; }
		public IList<string> Sources { get { return sources; } }

		public event Action<string> FileChanged;
		public event Action SourcesChanged;

		public void StartRefreshingSources ()
		{
			RescanFS (null, null);
			fsw = new FileSystemWatcher (Path, "*.cs") { IncludeSubdirectories = true };
			fsw.Changed += FSChanged;
			fsw.Created += RescanFS;
		}

		public void StopRefreshingSources ()
		{
			fsw.Changed -= FSChanged;
			fsw.Created -= RescanFS;
			fsw = null;
		}

		private void RescanFS (object sender, FileSystemEventArgs e)
		{
			sources = Directory.GetFiles (Path, "*.cs", SearchOption.AllDirectories);
		}

		private void FSChanged (object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			if (FileChanged != null)
				FileChanged (fileSystemEventArgs.FullPath);
		}
	}

/*
	// keep this in sync with the server
	enum ServiceRequestType : ushort
	{
		Unknown = 0,
		Sources = 1
	}

 * [Export (typeof (ISourcesProvider))]
	class ServiceClient : Client, ISourcesProvider
	{
		private bool refreshing = false;
		Action<object, object> callback;
		object state;

		private List<string> sources = new List<string> ();
		public IList<string> Sources
		{
			get { return sources; }
		}

		public ServiceClient ()
		{
			Port = 12346;
		}

		private void OnConnected ()
		{
			RequestSourceRefresh (callback, state);
		}

		public void StartRefreshingSources (Action<object, object> callback, object state)
		{
			OnConnect += OnConnected;
			this.callback = callback;
			this.state = state;
			Start ();
			refreshing = true;
		}

		public void StopRefreshingSources ()
		{
			refreshing = false;
			Stop ();
		}

		void RequestSourceRefresh (Action<object, object> callback, object state)
		{
			var req = RequestData.Create (RequestType.Service, (ushort)ServiceRequestType.Sources, null);
			SendRequestAsync (req, ar => OnRefreshSources (ar, callback), state);
			BeginReceive ();
		}

		void OnRefreshSources (IAsyncResult ar, Action<object, object> callback)
		{
			Console.WriteLine ("OnRefreshSources ");
			try
			{
				var response = ((AsyncRequestResult)ar).Response;
				var sources = Serializer.Unpack<string[]> (response);
				this.sources = new List<string> (sources);
				if (callback != null)
					callback (this.sources, ar.AsyncState);
			}
			catch (Exception ex)
			{
				Console.WriteLine (ex);
			}
		}

		protected override bool HandleData (System.Net.EndPoint sender, byte[] data)
		{
			base.HandleData (sender, data);
			return refreshing;
		}

		public override string ToString ()
		{
			return "Service Client provider on port " + Port;
		}
	}
*/
}