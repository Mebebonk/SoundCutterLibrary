using AudioCutterUI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SoundCutterLibrary;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Security.Principal;

namespace SoundCutterUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	[SettingReflector("settings")]
	public partial class MainWindow : Window
	{
		private readonly FilePathsManager filePathManager = new();
		private readonly CutterAPI _api = new();

		[Setting]
		private float _threshold = 0.2f;

		[Setting]
		private string _prefix = "new_";

		[Setting]
		private string _outPath = "";

		public MainWindow()
		{
			InitializeComponent();
			GetType().GetCustomAttribute<SettingReflectorAttribute>().LoadSettings(this);

			audioThreshold.Value = _threshold;
			prefixBox.Text = _prefix;
			outPathText.Text = _outPath;
			fileList.ItemsSource = filePathManager._observableFiles;
		}

		private void SelectFolder(object sender, RoutedEventArgs e)
		{
			using var fbd = new FolderBrowserDialog();
			fbd.SelectedPath = _outPath;
			DialogResult result = fbd.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				_outPath = fbd.SelectedPath;
				outPathText.Text = _outPath;
			}
		}

		private void SelectFiles(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog openFileDialog = new()
			{
				Multiselect = true,
				Filter = "Audio|*.aac;*.mp3;*.wav"
			};
			if ((bool)openFileDialog.ShowDialog()) filePathManager.AddFilePaths(openFileDialog.FileNames, UpdateMainProgress);

		}

		private void StartProcessFiles(object sender, RoutedEventArgs e)
		{
			if (filePathManager._files.Keys.Count == 0)
			{
				System.Windows.Forms.MessageBox.Show("Добавь файлы");
				return;
			}
			foreach (var file in filePathManager._files.Keys)
			{
				if (!filePathManager._files[file].IsFileRunning())
				{
					string oldName = file.Split('\\').Last();
					string newName = _prefix + oldName;
					string silentName = $"silent_{newName}";
					while (_outPath == "")
					{
						SelectFolder(sender, e);
					}
					string path = _outPath + "\\";

					filePathManager.LaunchFile(file, _api, _api.ProcessFile(file, path + newName, path + silentName, _threshold, filePathManager._files[file].UpdateProgress));
				}
			}

		}

		void MainWindow_Close(object sender, CancelEventArgs e)
		{
			GetType().GetCustomAttribute<SettingReflectorAttribute>().SaveSettings(this);
		}

		private void PrefixBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_prefix = prefixBox.Text;
			cutDesc.Text = $"{_prefix}названиефайла";
			silentDesc.Text = $"silent_{_prefix}названиефайла";
		}

		private void AudioThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_threshold = (float)audioThreshold.Value;
		}

		private void DeleteFileFromList(object sender, RoutedEventArgs e)
		{
			filePathManager.RemoveFilePath((sender as FrameworkElement).DataContext as ObservableFile);
		}

		private void UpdateMainProgress()
		{
			List<float> progresses = new();
			foreach (var file in filePathManager._observableFiles)
			{
				progresses.Add(file.ProgressValue);
			}
			float summ = 0;
			foreach (float a in progresses)
			{
				summ += a;
			}

			mainProgress.Value = summ / progresses.Count;

		}
	}
}
