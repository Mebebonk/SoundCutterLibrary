using SoundCutterLibrary;
using SoundCutterUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AudioCutterUI
{
	internal class ObservableFile : INotifyPropertyChanged
	{
		public string FileName { get; set; }

		private float _progressValue;
		public float ProgressValue
		{
			get { return _progressValue; }

			set
			{
				_progressValue = value;
				this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(ProgressValue)));
				Application.Current.Dispatcher.Invoke(new Action(() => _callback() ));						
			}

		}

		private CutterAPI _api;
		private ulong _id;
		private readonly Action _callback;

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableFile(string fileName, Action callback)
		{
			FileName = fileName.Split('\\').Last();
			_callback = callback;
		}

		public void UpdateProgress(float value)
		{
			ProgressValue = value;
		}

		public void SetApi(CutterAPI api, ulong id)
		{
			_api = api;
			_id = id;
		}

		public bool IsFileRunning() { return _api != null; }

	}
}

