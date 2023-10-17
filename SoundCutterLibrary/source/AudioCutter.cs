using NAudio.Wave;
using System.Reflection;

namespace SoundCutterLibrary
{
	internal class AudioCutter
	{
		private readonly WaveStream _audioInput;
		private readonly Stream _audioOutput;
		private readonly Stream _cutAudio;

		[Settings<float>("threshold", float.MaxValue * 0.0001f)]
		public readonly float _threshold;

		public AudioCutter(WaveStream audioInput, Stream audioOutput, Stream cutAudio)
		{
			_audioInput = audioInput;
			_audioOutput = audioOutput; 
			_cutAudio = cutAudio;

			_threshold = GetType()!.GetField("_threshold")!.GetCustomAttribute<SettingsAttribute<float>>()!.Value;
		}

		public void Process()
		{
			while (SearchAudio())
			_audioInput.Dispose();
			_audioOutput.Dispose();
			_cutAudio.Dispose();
		}

		private bool SearchAudio()
		{

			byte[] buffer = new byte[_audioInput.WaveFormat.SampleRate / 4];
			WaveBuffer sampleBuffer = new(buffer);
			float midSignal;

			while (true)
			{

				int bytesComplitedRead = _audioInput.Read(buffer);
				if (bytesComplitedRead > 0)
				{

					midSignal = 0.0f;
					float sample;

					for (int i = 0; i < bytesComplitedRead / 4; i++)
					{

						sample = sampleBuffer.FloatBuffer[i];

						if (sample != sample)
						{
							midSignal += 0;
						}
						else
						{
							midSignal += Math.Abs(sample) / bytesComplitedRead;
						}
					}
					if (midSignal >= _threshold)
					{
						_audioOutput.Write(buffer);
					}
					else
					{
						_cutAudio.Write(buffer);
					}
				}
				else
				{
					break;
				}

			}
			return false;
		}

		public float State => (float)_audioInput.Position / _audioInput.Length;

	}


}
