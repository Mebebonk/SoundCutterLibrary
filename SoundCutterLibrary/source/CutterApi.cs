using NAudio.Wave;
using System.Collections.Concurrent;

namespace SoundCutterLibrary
{
	public class CutterAPI
	{
		public ulong ProcessFile(string inputPath, string outputPath, string cutPath, float threshold, Action<float> callback)
		{
			WaveStream inputStream = new MediaFoundationReader(inputPath);
			TaskInformation task = new
			(
				new
				(
					inputStream,
					new WaveFileWriter(File.Open(outputPath, FileMode.Create), inputStream.WaveFormat),
					new WaveFileWriter(File.Open(cutPath, FileMode.Create), inputStream.WaveFormat),
					threshold,
					callback
				)
			);

			_tasks.TryAdd(_index, task);

			return _index++;
		}

		public bool IsCompleted(ulong index)
		{
			return _tasks[index].task.IsCompleted;
		}

		public void Remove(ulong index)
		{
			_tasks.Remove(index, out TaskInformation _);
		}

		private readonly ConcurrentDictionary<ulong, TaskInformation> _tasks = new();
		private ulong _index = 0;

		private struct TaskInformation
		{
			public AudioCutter cutter;
			public Task task;

			public TaskInformation(AudioCutter cutter)
			{
				this.cutter = cutter;
				task = Task.Run(cutter.Process);
			}
		}
	}
}
