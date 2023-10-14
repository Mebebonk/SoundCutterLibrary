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

		public AudioCutter(WaveStream audioInput, Stream audioOutput)
		{
			_audioInput = audioInput;
			_audioOutput = audioOutput;
		}

		public void Process()
		{
			/*
			 * while input.Pos < final pos
			 * search for spaces
			 * save inbetveens to out
			 * 
			 */
			while(true)
			{
				if(FindEmptyAudio() > _lastPosition)
				{

				}
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
				}
			}
		}

		private int FindEmptyAudio()
		{
			return 0;
		}

		public float State => _lastPosition / _audioInput.Length;

	}
}
