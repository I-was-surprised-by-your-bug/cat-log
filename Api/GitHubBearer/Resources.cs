using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
	internal static class Resources
	{
		private static ResourceManager s_resourceManager;

		internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(Resources)));

		internal static CultureInfo Culture
		{
			get;
			set;
		}

		internal static string Exception_OptionMustBeProvided => GetResourceString("Exception_OptionMustBeProvided");

		internal static string Exception_ValidatorHandlerMismatch => GetResourceString("Exception_ValidatorHandlerMismatch");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string GetResourceString(string resourceKey, string defaultValue = null)
		{
			return ResourceManager.GetString(resourceKey, Culture);
		}

		private static string GetResourceString(string resourceKey, string[] formatterNames)
		{
			string text = GetResourceString(resourceKey);
			if (formatterNames != null)
			{
				for (int i = 0; i < formatterNames.Length; i++)
				{
					text = text.Replace("{" + formatterNames[i] + "}", "{" + i.ToString() + "}");
				}
			}
			return text;
		}

		internal static string FormatException_OptionMustBeProvided(object p0)
		{
			return string.Format(Culture, GetResourceString("Exception_OptionMustBeProvided"), p0);
		}
	}

}
