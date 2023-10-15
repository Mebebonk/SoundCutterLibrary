using System;
using System.Diagnostics;
using SoundCutterLibrary;

namespace SoundCutterCLI
{
    internal class SoundCutterCLI
    {
        static void Main(string[] args)
        {
            CutterAPI api = new();
			args = new string[2]
			{
				"REC20230902203639.mp3",
				"test.wav"
			};

			Task task = api.ProcessFile(args[0], args[1], (float progress) => Console.WriteLine(progress), out Action callback);

            while (!task.IsCompleted)
            {
                callback();

                Thread.Sleep(1000);
            }

			callback();
		}
    }
}
