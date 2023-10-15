using NAudio.MediaFoundation;
using NAudio.Wave;
using System;

namespace SoundCutterLibrary
{
	public class CutterAPI
	{
		public Task ProcessFile(string inputPath, string outputPath, Action<float> progressCallback, out Action callProgressCallback)
		{
			IProgress<AudioCutter> progress = new Progress<AudioCutter>((AudioCutter cutter) => progressCallback(cutter.State));
			WaveStream inputStream = new MediaFoundationReader(inputPath);
			AudioCutter cutter = new(inputStream, new WaveFileWriter(File.Open(outputPath, FileMode.Create), inputStream.WaveFormat));

			callProgressCallback = () => progress.Report(cutter);

			return Task.Run(cutter.Process);
		}
	}
}
