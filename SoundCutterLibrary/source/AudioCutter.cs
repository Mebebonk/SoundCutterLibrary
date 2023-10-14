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

		public float threshold = 0.0f;

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
					foundPosition = FindAudio();
					if (foundPosition == -1)
					{
						break;
					}
					lastEmpty = false;
				}
				else
				{
					foundPosition = FindEmptyAudio();
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
				}
			}
		}

		private int FindEmptyAudio()
		{
			byte[] buffer = new byte[1024];
			while (_audioInput.Position < _audioInput.Length)
			{
				int bytesRequired = (int)(_audioInput.Length - _audioInput.Position);
				if (bytesRequired > 0)
				{
					int bytesToRead = Math.Min(bytesRequired, buffer.Length);
					int bytesRead = _audioInput.Read(buffer, 0, bytesToRead);
					if (bytesRead > 0)
					{
						int mid = 0;
						foreach (byte b in buffer)
						{
							mid += Math.Abs(b);
						}
						if (mid / bytesRead < threshold)
						{
							return (int)_audioInput.Position;
						}
					}
				}
				
			}
			return -1;
		}

		private int FindAudio()
		{
			byte[] buffer = new byte[1024];
			while (_audioInput.Position < _audioInput.Length)
			{
				int bytesRequired = (int)(_audioInput.Length - _audioInput.Position);
				if (bytesRequired > 0)
				{
					int bytesToRead = Math.Min(bytesRequired, buffer.Length);
					int bytesRead = _audioInput.Read(buffer, 0, bytesToRead);
					if (bytesRead > 0)
					{
						int mid = 0;
						foreach (byte b in buffer)
						{
							mid += Math.Abs(b);
						}
						if (mid / bytesRead >= threshold)
						{
							return (int)_audioInput.Position;
						}
					}
				}
				
			}
			return -1;
		}
		public float State => _audioInput.Position / _audioInput.Length;

	}
}
