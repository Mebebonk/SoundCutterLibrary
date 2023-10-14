using NAudio.MediaFoundation;
using NAudio.Wave;
using System;

namespace SoundCutterLibrary
{
	public class CutterAPI
	{
		public Task ProcessFile(string inputPath, string outputPath, Action<float> progressCallback, out Action callProgressCallback)
		{
			using WaveStream inputStream = new MediaFoundationReader(inputPath);
			using Stream outputStream = File.OpenWrite(outputPath);
			IProgress<AudioCutter> progress = new Progress<AudioCutter>((AudioCutter cutter) => progressCallback(cutter.State));
			AudioCutter cutter = new AudioCutter(inputStream, new WaveFileWriter(outputStream, inputStream.WaveFormat));

			callProgressCallback = () => progress.Report(cutter);

			return Task.Run(cutter.Process);
		}
	}
}
