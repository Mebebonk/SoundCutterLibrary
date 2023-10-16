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

		public float threshold = float.MaxValue * 0.01f;

		public AudioCutter(WaveStream audioInput, Stream audioOutput)
		{
			_audioInput = audioInput;
			_audioOutput = audioOutput;
		}

		public void Process()
		{
			while (true)
			{

				if (SearchAudioIsland(out long islandStart, out long islandEnd))
				{
					CaptureAudioIsland(islandStart, islandEnd);
				}
				else
				{
					break;
				}

			}
			_audioInput.Dispose();
			_audioOutput.Dispose();
		}

		private void CaptureAudioIsland(long startPosition, long endPosition)
		{
			_audioInput.Position = startPosition;
			long wtf = _audioInput.Position;
			byte[] buffer = new byte[1024];
			while (true)
			{
				int bytesRequired = (int)(endPosition - _audioInput.Position);
				if (bytesRequired > 0)
				{
					int bytesToRead = Math.Min(bytesRequired, buffer.Length);
					int bytesComplitedRead = _audioInput.Read(buffer, 0, bytesToRead);
					if (bytesComplitedRead > 0)
					{
						_audioOutput.Write(buffer, 0, bytesComplitedRead);
					}
					else
					{
						throw new Exception("File write reached _audioInput end");
					}
				}
				else
				{
					return;
				}


			}
		}


		private long SearchAudio(bool isSearhingForEmpty = false)
		{
			byte[] buffer = new byte[_audioInput.WaveFormat.SampleRate / 4];
			WaveBuffer sampleBuffer = new(buffer);
			float midSignal;

			while (true)
			{

				int bytesComplitedRead = _audioInput.Read(buffer, 0, buffer.Length);
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
					if (midSignal >= threshold && !isSearhingForEmpty)
					{
						return _audioInput.Position;
					}
					else if (midSignal < threshold && isSearhingForEmpty)
					{
						return _audioInput.Position;
					}
				}
				else
				{
					break;
				}

			}
			return -1;
		}

		private bool SearchAudioIsland(out long islandStartPos, out long islandEndpos)
		{
			long islandStart = SearchAudio(), islandEnd = SearchAudio(true);
			if (islandStart != -1 && islandEnd != -1)
			{
				islandStartPos = islandStart;
				islandEndpos = islandEnd;
				return true;
			}
			else
			{
				islandEndpos = 0;
				islandStartPos = 0;
				return false;
			}

		}

		private byte[] Test(bool da)
		{
			if (Array.Empty<byte>().Length > 0)
			return Array.Empty<byte>();
		}
		public float State => (float)_audioInput.Position / _audioInput.Length;

	}


}
