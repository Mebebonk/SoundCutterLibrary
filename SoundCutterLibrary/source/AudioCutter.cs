using NAudio.Wave;
using System.Reflection;

namespace SoundCutterLibrary
{
	internal class AudioCutter
	{
		private readonly WaveStream _audioInput;
		private readonly Stream _audioOutput;
		private readonly Stream _cutAudio;
		private readonly float _threshold;
		private readonly Action<float> _callback;

		public AudioCutter(WaveStream audioInput, Stream audioOutput, Stream cutAudio, float threshold, float dbThreshold, Action<float> callback)
		{
			_audioInput = audioInput;
			_audioOutput = audioOutput;
			_cutAudio = cutAudio;
			_threshold = (float)Math.Pow(10, ((20 * Math.Log10(float.MaxValue)) - (dbThreshold * (1 - threshold))) / 20);
			_callback = callback;
		}

		public void Process()
		{
			SearchAudio();
			_audioInput.Dispose();
			_audioOutput.Dispose();
			_cutAudio.Dispose();
		}

		private void SearchAudio()
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

						if (float.IsNaN(sample))
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
					_callback((float)_audioInput.Position / _audioInput.Length);
				}
				else
				{
					break;
				}

			}
		}

	}


}
