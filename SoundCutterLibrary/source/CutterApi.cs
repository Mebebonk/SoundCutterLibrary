using NAudio.MediaFoundation;
using NAudio.Wave;
using System;

namespace SoundCutterLibrary
{
	public class CutterAPI
	{
		public Task ProcessFile(string inputPath, string outputPath, Action<float> progressCallback, out Action callProgressCallback)
		{
			WaveStream inputStream = new MediaFoundationReader(inputPath);
			IProgress<AudioCutter> progress = new Progress<AudioCutter>((AudioCutter cutter) => progressCallback(cutter.State));
			AudioCutter cutter = new(inputStream, new WaveFileWriter(File.OpenWrite(outputPath), inputStream.WaveFormat));

			callProgressCallback = () => progress.Report(cutter);

			return Task.Run(cutter.Process);
		}
	}
}
