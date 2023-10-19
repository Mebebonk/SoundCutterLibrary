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

namespace SoundCutterUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly SoundCutterUIFilePathsManager filePathManager;
		private readonly CutterAPI _api = new();


		[Settings<float>("threshold", float.MaxValue * 0.0001f)]
		public float _threshold;

		[Settings<string>("prefix", "new_")]
		public string _prefix;


		public MainWindow()
		{
			InitializeComponent();

			_prefix = GetType()!.GetField("_prefix")!.GetCustomAttribute<SettingsAttribute<string>>()!.Value;
			_threshold = GetType()!.GetField("_threshold")!.GetCustomAttribute<SettingsAttribute<float>>()!.Value;

			audioThreshold.Value = _threshold;
			prefixBox.Text = _prefix;

			filePathManager = new SoundCutterUIFilePathsManager();
			fileList.ItemsSource = filePathManager._observableFiles;
		}

		private void SelectFiles(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new()
			{
				Multiselect = true,
				Filter = "Audio|*.aac;*.mp3;*.wav"
			};
			if ((bool)openFileDialog.ShowDialog()) filePathManager.AddFilePaths(openFileDialog.FileNames);

		}

		private void StartProcessFiles(object sender, RoutedEventArgs e)
		{
			foreach (var file in filePathManager._files.Keys)
			{
				string newName = "gug_" + file.Split('\\').Last();
				string oldName = file.Split('\\').Last();
				filePathManager.LaunchFile(file, _api, _api.ProcessFile(file, $"{file.Replace(oldName, newName)}", $"{file.Replace(oldName, $"silent_{newName}")}", _threshold));
			}

		}

		void MainWindow_Close(object sender, CancelEventArgs e)
		{
			SettingsAssistant settingsData = new()
			{
				prefix = _prefix,
				threshold = _threshold
			};

			string json = JsonSerializer.Serialize(settingsData);
			var file = File.Open("settings.json", FileMode.Create);
			StreamWriter fileStream = new(file);
			fileStream.Write(json);
			fileStream.Close();

		}

		private void PrefixBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_prefix = prefixBox.Text;
		}

		private void AudioThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_threshold = (float)audioThreshold.Value;
		}
	}
}
