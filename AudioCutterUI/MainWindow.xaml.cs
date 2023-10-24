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
		private readonly FilePathsManager filePathManager;

		[Setting]
		private float _threshold = 0.2f;

		[Setting]
		private string _prefix = "new_";

		[Setting]
		private string _outPath = "...";

		[Setting]
		private float _dbThreshold = 100;
		
		public MainWindow()
		{
			InitializeComponent();
			GetType().GetCustomAttribute<SettingReflectorAttribute>().LoadSettings(this);
			filePathManager = new(UpdateMainProgress, UpdateOutPath);

			audioThreshold.Value = _threshold;
			prefixBox.Text = _prefix;
			outPathText.Text = _outPath;
			fileList.ItemsSource = filePathManager.Files;
		}

		private void SelectFolder(object sender, RoutedEventArgs e)
		{
			filePathManager.SelectFolder(ref _outPath);
		}

		private void SelectFiles(object sender, RoutedEventArgs e)
		{
			filePathManager.SelectFiles();
		}

		private void StartProcessFiles(object sender, RoutedEventArgs e)
		{
			filePathManager.ProcessFiles(ref _outPath, _threshold, _dbThreshold, _prefix);
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
			mainProgress.Value = filePathManager.GetTotalProgress(); 
		}

		private void UpdateOutPath()
		{
			outPathText.Text = _outPath;
		}
	}
}
