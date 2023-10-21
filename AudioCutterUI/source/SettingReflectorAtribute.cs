using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AudioCutterUI
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class SettingReflectorAttribute : Attribute
	{
		private readonly BindingFlags filter = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		private readonly string fileName;

		public SettingReflectorAttribute(string fileName)
		{
			this.fileName = fileName;
		}
		public void LoadSettings(object caller)
		{
			try
			{
				using FileStream file = File.OpenRead($"{fileName}.json");
				JsonDocument json = JsonDocument.Parse(file);

				MemberInfo[] members = caller.GetType().GetMembers(filter);

				foreach (MemberInfo member in members)
				{
					if (member.GetCustomAttribute<SettingAttribute>() != null)
					{

						if (member.MemberType == MemberTypes.Property)
						{
							Type propertyType = caller.GetType().GetProperty(member.Name, filter).PropertyType;
							var propertyValue = json.RootElement.GetProperty(member.Name.Replace("_", "")).Deserialize(propertyType);

							if (propertyValue != null) caller.GetType().GetProperty(member.Name, filter).SetValue(caller, propertyValue);
						}
						else if (member.MemberType == MemberTypes.Field)
						{
							Type fieldType = caller.GetType().GetField(member.Name, filter).FieldType;
							var fieldValue = json.RootElement.GetProperty(member.Name.Replace("_", "")).Deserialize(fieldType);

							if (fieldValue != null) caller.GetType().GetField(member.Name, filter).SetValue(caller, fieldValue);
						}

					}
				}
			}
			catch (Exception)
			{
				return;
			}
		}

		public void SaveSettings(object caller)
		{
			MemberInfo[] members = caller.GetType().GetMembers(filter);
			Dictionary<string, object> settings = new();

			foreach (MemberInfo member in members)
			{
				if (member.GetCustomAttribute<SettingAttribute>() != null)
				{
					string memberName = member.Name.Replace("_", "");
					if (member.MemberType == MemberTypes.Property)
					{
						settings.Add(memberName, caller.GetType().GetProperty(member.Name, filter).GetValue(caller));
					}
					else if (member.MemberType == MemberTypes.Field)
					{
						settings.Add(memberName, caller.GetType().GetField(member.Name, filter).GetValue(caller));
					}
				}
			}

			string json = JsonSerializer.Serialize(settings);
			using FileStream file = File.Open($"{fileName}.json", FileMode.Create);
			using StreamWriter fileStream = new(file);
			 
			fileStream.Write(json);
			fileStream.Close();
		}
	}
}
