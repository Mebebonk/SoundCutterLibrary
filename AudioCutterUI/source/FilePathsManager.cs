using NAudio.Wave;
using SoundCutterLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace AudioCutterUI
{
	internal class FilePathsManager
	{
		private readonly Dictionary<string, ObservableFile> _files = new();
		private readonly ObservableCollection<ObservableFile> _observableFiles = new();
		private readonly CutterAPI _api = new();

		private readonly Action callback;
		private readonly Action updateOutPath;

		public ObservableCollection<ObservableFile> Files { get { return _observableFiles; } }

		public FilePathsManager(Action callback, Action updateOutPath)
		{
			this.callback = callback;
			this.updateOutPath = updateOutPath;
		}
		public void AddFilePaths(string[] filePaths)
		{
			foreach (var path in filePaths)
			{
				if (!_files.ContainsKey(path))
				{
					ObservableFile file = new(path, callback);
					_observableFiles.Add(file);
					_files.Add(path, file);
				}
			}
		}

		public void LaunchFile(string filePath, CutterAPI api, ulong id)
		{
			_files[filePath].SetApi(api, id);
		}
		public void RemoveFilePath(ObservableFile observableFile)
		{
			var a = _files.FirstOrDefault(x => x.Value == observableFile).Key;
			if (a != null)
			{
				_files.Remove(a);
				_observableFiles.Remove(observableFile);
			}
		}

		public bool SelectFiles()
		{
			Microsoft.Win32.OpenFileDialog openFileDialog = new()
			{
				Multiselect = true,
				Filter = "Audio|*.aac;*.mp3;*.wav"
			};
			if ((bool)openFileDialog.ShowDialog())
			{
				AddFilePaths(openFileDialog.FileNames);
				return true;
			}
			return false;
		}

		public void ProcessFiles(ref string outPath, float threshold, float dbThreshold, string prefix)
		{
			if (_files.Count == 0)
			{
				SelectFiles();
			}
			foreach (var file in _files.Keys)
			{
				if (!_files[file].IsFileRunning())
				{
					string oldName = file.Split('\\').Last();
					string newName = prefix + oldName;
					string silentName = $"silent_{newName}";
					if (outPath == "...")
					{
						SelectFolder(ref outPath);
					}
					string path = outPath + "\\";

					LaunchFile(file, _api, _api.ProcessFile(file, path + newName, path + silentName, threshold, dbThreshold, _files[file].UpdateProgress));
				}
			}
		}

		public bool SelectFolder(ref string outPath)
		{
			using var fbd = new FolderBrowserDialog();
			fbd.SelectedPath = outPath;
			DialogResult result = fbd.ShowDialog();
			if (result == DialogResult.OK)
			{
				outPath = fbd.SelectedPath;
				updateOutPath();
				return true;
			}
			return false;
		}

		public float GetTotalProgress()
		{
			List<float> progresses = new();
			foreach (var file in _observableFiles)
			{
				progresses.Add(file.ProgressValue);
			}
			float summ = 0;
			foreach (float a in progresses)
			{
				summ += a;
			}

			return summ / progresses.Count;
		}
	}
}
