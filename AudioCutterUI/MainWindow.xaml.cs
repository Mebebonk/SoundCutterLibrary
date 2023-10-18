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

namespace SoundCutterUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly SoundCutterUIFilePathsManager filePathManager;

		public MainWindow()
		{
			InitializeComponent();
			filePathManager = new SoundCutterUIFilePathsManager();
			audioThreshold.Value = 50;
			fileList.ItemsSource = filePathManager._observableFiles;
		}

		private void SelectFiles(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "Audio|*.aac;*.mp3;*.wav"
			};
			if ((bool)openFileDialog.ShowDialog()) filePathManager.AddFilePaths(openFileDialog.FileNames);

		}

		private void StartProcessFiles(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
