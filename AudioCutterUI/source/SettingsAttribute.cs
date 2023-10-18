using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoundCutterLibrary
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class SettingsAttribute<T> : Attribute
	{
		private readonly T _value;

		public SettingsAttribute(string name, T defaultValue)
		{
			try
			{
				using FileStream file = File.OpenRead("settings.json");
				JsonDocument json = JsonDocument.Parse(file);
				T? temp = json.RootElement.GetProperty(name).Deserialize<T>();

				if (temp != null)
				{
					_value = temp;
				}
				else
				{
					_value = defaultValue;
				}
			}
			catch (Exception)
			{
				_value = defaultValue;
			}
		}

		public T Value => _value;
	}
}
