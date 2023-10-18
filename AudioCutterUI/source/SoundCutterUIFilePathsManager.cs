using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCutterUI
{
	internal class SoundCutterUIFilePathsManager
	{
		public readonly Dictionary<string, ObservableFile> _files = new Dictionary<string, ObservableFile>();
		public readonly ObservableCollection<ObservableFile> _observableFiles = new ObservableCollection<ObservableFile>();

		public void AddFilePaths(string[] filePaths)
		{
			foreach (var path in filePaths)
			{
				if (!_files.Keys.Contains<string>(path))
				{
					ObservableFile file = new ObservableFile(path, 0.0f);
					_observableFiles.Add(file);
					_files.Add(path, file);
				}
			}
		}
	}
}
