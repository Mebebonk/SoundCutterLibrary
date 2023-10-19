using SoundCutterLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCutterUI
{
	internal class ObservableFile
	{
		public string FileName { get; set; }

		private CutterAPI _api;
		private ulong _id;

		public ObservableFile(string fileName)
		{
			this.FileName = fileName.Split('\\').Last();
		}
		public float ProgressValue 
		{
			get
			{
				if (_api == null)
				{
					return 0.0f;
				}
				return _api.GetProgress(_id) * 100;
			}
			set { }
		}

		public void SetApi(CutterAPI api, ulong id)
		{
			_api = api;
			_id = id;
		}

	}
}

