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

		public float threshold = 0.2f;

		public AudioCutter(WaveStream audioInput, Stream audioOutput)
		{
			_audioInput = audioInput;
			_audioOutput = audioOutput;
		}

		public void Process()
		{
			while (true)
			{
				int foundPosition;
				if (lastEmpty)
				{
					foundPosition = FindAudio(_lastPosition);
					if (foundPosition == -1)
					{
						break;
					}
				}
				else
				{
					foundPosition = FindEmptyAudio(_lastPosition);
					CaptureAudioIsland(_lastPosition, foundPosition);
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
				}
			}
		}

		private int FindEmptyAudio(int searchStartPosition)
		{
			byte[] buffer = new byte[1024];
			long audioLength = _audioInput.Length;
			while (_audioInput.Position < audioLength - 1)
			{
				int bytesRequired = (int)(audioLength - _audioInput.Position);
				if (bytesRequired > 0)
				{
					int bytesToRead = Math.Min(bytesRequired, buffer.Length);
					int bytesRead = _audioInput.Read(buffer, 0, bytesToRead);
					if (bytesRead > 0)
					{
						int mid = 0;
						foreach (byte b in buffer)
						{
							mid += b;
						}
						if (mid / bytesRead < threshold)
						{
							return (int)_audioInput.Position - (buffer.Length - 1);
						}
					}					
				}
			}
			return -1;
		}

		private int FindAudio(int searchStartPosition)
		{
			byte[] buffer = new byte[1024];
			long audioLength = _audioInput.Length;
			while (_audioInput.Position < audioLength - 1)
			{
				int bytesRequired = (int)(audioLength - _audioInput.Position);
				if (bytesRequired > 0)
				{
					int bytesToRead = Math.Min(bytesRequired, buffer.Length);
					int bytesRead = _audioInput.Read(buffer, 0, bytesToRead);
					if (bytesRead > 0)
					{
						int mid = 0;
						foreach (byte b in buffer)
						{
							mid += b;
						}
						if (mid / bytesRead >= threshold)
						{
							return (int)_audioInput.Position - (buffer.Length - 1);
						}
					}
				}
			}
			return -1;
		}
		public float State => _lastPosition / _audioInput.Length;

	}
}
