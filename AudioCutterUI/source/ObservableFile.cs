using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCutterUI
{
	internal class ObservableFile
	{
		public string fileName {get;set;}
		public float progressValue {get;set;}

		public ObservableFile(string fileName, float progressValue)
		{
			this.fileName = fileName.Split('\\').Last();
			this.progressValue = progressValue;
		}
	}
}
