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

			ulong index = api.ProcessFile(args[0], args[1]);

            while (!api.IsCompleted(index))
            {
                Console.WriteLine(api.GetProgress(index));

                Thread.Sleep(1000);
            }

			Console.WriteLine(1.0f);

            api.Remove(index);
		}
    }
}
