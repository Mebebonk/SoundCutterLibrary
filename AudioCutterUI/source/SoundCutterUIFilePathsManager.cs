using NAudio.Wave;
using SoundCutterLibrary;
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
		public readonly Dictionary<string, ObservableFile> _files = new();
		public readonly ObservableCollection<ObservableFile> _observableFiles = new();

		public void AddFilePaths(string[] filePaths)
		{
			foreach (var path in filePaths)
			{
				if (!_files.ContainsKey(path))
				{
					ObservableFile file = new(path);
					_observableFiles.Add(file);
					_files.Add(path, file);
				}
			}
		}

		public void LaunchFile(string filePath, CutterAPI api, ulong id)
		{
			_files[filePath].SetApi(api, id);
		}
		public void RemoveFilePath(string filePath) 
		{ 
			_files.Remove(filePath);
		}
	}
}
