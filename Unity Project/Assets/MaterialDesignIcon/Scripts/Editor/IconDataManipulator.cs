using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MaterialDesignIcon
{
	public class IconDataManipulator : Editor
	{
		
		[MenuItem("Assets/UI/Create Icon Dataset")]
		static public void CreateIconDataset()
		{
			string codepointsFile = EditorUtility.OpenFilePanel("Select codepoint File", Application.dataPath, "");
			string[] codepoints = File.ReadAllLines(codepointsFile);

			string scriptContent = "";

			string[] enumContents = new string[codepoints.Length];
			string[] unicodeContents = new string[codepoints.Length];

			int index = 0;
			foreach (string s in codepoints)
			{
				string[] e = s.Split(' ');
				enumContents[index] = e[0];
				unicodeContents[index] = e[1];
				index++;
			}

			scriptContent += "\nnamespace MaterialDesignIcon";
			scriptContent += "\n{";

			scriptContent += "\n\tpublic enum Icons";
			scriptContent += "\n\t{";

			for (int i = 0; i < codepoints.Length; i++)
			{
				string c = enumContents[i];//System.Text.RegularExpressions.Regex.Replace(enumContents[i], "[0-9]{2,}", "");
				if (System.Text.RegularExpressions.Regex.IsMatch(c, @"^\d")) c = "_" + c;
				scriptContent += "\n\t\t" + c.ToUpper() + ",";
			}
			scriptContent += "\n\t}";

			scriptContent += "\n\tpublic class IconsData";
			scriptContent += "\n\t{";
			scriptContent += "\n\t\tstatic public string[] iconsUnicode = new string[]{";
			for (int i = 0; i < codepoints.Length; i++)
			{
				scriptContent += "\n\t\t\t\"" + unicodeContents[i] + "\",";
			}
			scriptContent += "\n\t\t};";

			scriptContent += "\n\t\tstatic public string[] iconsName = new string[]{";
			for (int i = 0; i < codepoints.Length; i++)
			{
				scriptContent += "\n\t\t\t\"" + enumContents[i] + "\",";
			}
			scriptContent += "\n\t\t};";

			scriptContent += "\n\t}";

			scriptContent += "\n}";

			string datasetPath = EditorUtility.SaveFilePanel("Save Created .cs File", Application.dataPath, "IconDataset", "cs");
			File.WriteAllText(datasetPath, scriptContent);
		}
	}
}
