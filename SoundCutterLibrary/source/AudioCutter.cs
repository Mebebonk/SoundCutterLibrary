using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SoundCutterLibrary
{
	internal class AudioCutter
	{
		private WaveStream _audioInput;
		private Stream _audioOutput;
		private Stream _cutAudio;

		public float threshold = float.MaxValue * 0.0001f;

		public AudioCutter(WaveStream audioInput, Stream audioOutput, Stream cutAudio)
		{
			_audioInput = audioInput;
			_audioOutput = audioOutput;
			_cutAudio = cutAudio;
		}

		public void Process()
		{
			while (SearchAudio())
			_audioInput.Dispose();
			_audioOutput.Dispose();
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
					if (midSignal >= threshold)
					{
						_audioOutput.Write(buffer);
					}
					else if (midSignal < threshold)
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
