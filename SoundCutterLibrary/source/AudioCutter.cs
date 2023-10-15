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
		private int _lastPosition = 0;
		private bool lastEmpty = true;

		public float threshold = 0.1f;

		public AudioCutter(WaveStream audioInput, Stream audioOutput)
		{
			_audioInput = audioInput;
			_audioOutput = audioOutput;
		}

		public void Process()
		{

			int foundPosition;
			while (true)
			{
				if (lastEmpty)
				{
					foundPosition = AudioSearch();
					if (foundPosition == -1)
					{
						break;
					}
					lastEmpty = false;
				}
				else
				{
					foundPosition = AudioSearch(true);
					CaptureAudioIsland(_lastPosition, foundPosition);
					lastEmpty = true;
				}
				_lastPosition = foundPosition;
			}
		}

		private void CaptureAudioIsland(int startPosition, int endPosition)
		{
			_audioInput.Position = startPosition;
			byte[] buffer = new byte[1024];
			while (_audioInput.Position < endPosition)
			{
				int bytesRequired = (int)(endPosition - _audioInput.Position);
				if (bytesRequired > 0)
				{
					int bytesToRead = Math.Min(bytesRequired, buffer.Length);
					int bytesRead = _audioInput.Read(buffer, 0, bytesToRead);
					if (bytesRead > 0)
					{
						_audioOutput.Write(buffer, 0, bytesRead);
					}
					else
					{
						return;
					}
				}

			}
		}


		private int AudioSearch(bool isSearhingForEmpty = false)
		{
			byte[] buffer = new byte[1024];
			WaveBuffer nbuffer = new(buffer);
			
			while (true)
			{

				int bytesComplitedRead = _audioInput.Read(buffer, 0, buffer.Length);
				if (bytesComplitedRead > 0)
				{

					float midSignal = 0.0f;
					for (int i = 0; i < bytesComplitedRead / 4; i++)
					{
						float sample = nbuffer.FloatBuffer[i];

						midSignal += sample;
					}
					if ((Math.Abs(midSignal) / bytesComplitedRead >= threshold && !isSearhingForEmpty) || (Math.Abs(midSignal) / bytesComplitedRead < threshold && isSearhingForEmpty))
					{
						return (int)_audioInput.Position;
					}
				}
				else
				{
					break;
				}
				

			}
			return -1;
		}
		public float State => _audioInput.Position / _audioInput.Length;

		~AudioCutter()
		{
			_audioInput.Dispose();
			_audioOutput.Dispose();
		}
	}

	
}
