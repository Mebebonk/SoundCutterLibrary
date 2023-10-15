using NAudio.MediaFoundation;
using NAudio.Wave;
using System;

namespace SoundCutterLibrary
{
	public class CutterAPI
	{
		private struct TaskInformation
		{
			public AudioCutter cutter;
			public Task task;
		}

		public ulong ProcessFile(string inputPath, string outputPath)
		{
			WaveStream inputStream = new MediaFoundationReader(inputPath);
			TaskInformation task = new TaskInformation();

			task.cutter = new(inputStream, new WaveFileWriter(File.Open(outputPath, FileMode.Create), inputStream.WaveFormat));
			task.task = Task.Run(task.cutter.Process);

			_tasks.Add(_index, task);

			return _index++;
		}

		public bool IsCompleted(ulong index)
		{
			return _tasks[index].task.IsCompleted;
		}

		public void Remove(ulong index)
		{
			_tasks.Remove(index);
		}

		public float GetProgress(ulong index)
		{
			return _tasks[index].cutter.State;
		}

		private readonly Dictionary<ulong, TaskInformation> _tasks = new();
		private ulong _index = 0;
	}
}
