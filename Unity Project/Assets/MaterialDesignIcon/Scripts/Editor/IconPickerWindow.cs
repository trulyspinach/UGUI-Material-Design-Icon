using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MaterialDesignIcon
{
	public class IconPickerWindow : EditorWindow
	{

		string myString = "Hello World";
		bool groupEnabled;
		bool myBool = true;
		float myFloat = 1.23f;

		static IconPickerWindow selfW;
		static GUIStyle labelStyle;
		static GUIStyle iconStyle;
		static GUIStyle iconTextStyle;
		static GUIStyle barStyle;
		static GUIStyle buttonStyle;

		string selected = "";
		int selectedID = -1;
		System.Action<string> onSelectionChanged;

		Vector2 scrollPosition;

		const int itemPerLine = 4;
		const int iconSize = 60;
		const int spacing = 30;

		static public void Show(string preSelect, System.Action<string> callback)
		{
			selfW = (IconPickerWindow)EditorWindow.GetWindow<IconPickerWindow>(true);

			selfW.title = "Pick an Icon";
			selfW.minSize = new Vector2(410, 529);
			selfW.maxSize = new Vector2(410, 529);

			labelStyle = new GUIStyle();
			var fontGUID = AssetDatabase.FindAssets("Roboto-Light")[0];
			labelStyle.font = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontGUID));

			iconStyle = new GUIStyle();
			fontGUID = AssetDatabase.FindAssets("MaterialIcons-Regular")[0];
			iconStyle.font = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontGUID));
			iconStyle.normal.textColor = MaterialDesignColorset.grey600;

			iconTextStyle = new GUIStyle();
			fontGUID = AssetDatabase.FindAssets("Roboto-Medium")[0];
			iconTextStyle.font = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontGUID));
			iconTextStyle.alignment = TextAnchor.LowerCenter;
			iconTextStyle.normal.textColor = MaterialDesignColorset.grey600;
			iconTextStyle.fontSize = 12;

			buttonStyle = new GUIStyle();

			selfW.onSelectionChanged = callback;
			List<string> tempList = new List<string>();
			foreach (string s in IconsData.iconsUnicode) tempList.Add(System.Text.RegularExpressions.Regex.Unescape(@"\u" + s));
			if (tempList.Contains(preSelect))
			{
				selfW.selected = preSelect;
				selfW.selectedID = tempList.IndexOf(selfW.selected);
				int lineOfSelection = selfW.selectedID / itemPerLine - 1;
				selfW.scrollPosition = new Vector2(0f, lineOfSelection * (iconSize + spacing));
			}

			selfW.ShowUtility();
		}

		private string DecodeUnicodeString(string s)
		{
			return System.Text.RegularExpressions.Regex.Unescape(s);
		}

		void OnGUI()
		{
			Rect windowRect = selfW.position;
			Rect titleRect = new Rect(0, 0, windowRect.width, windowRect.height * 0.13f);
			Rect contentRect = new Rect(0, windowRect.height * 0.13f, windowRect.width, windowRect.height * 0.87f);

			//Draw window background.
			EditorGUI.DrawRect(titleRect, MaterialDesignColorset.lightBlue400);
			EditorGUI.DrawRect(contentRect, MaterialDesignColorset.grey200);


			labelStyle.fontSize = 40;
			labelStyle.normal.textColor = MaterialDesignColorset.grey100;

			Rect titleLabelRect = titleRect;
			titleLabelRect.x = 15;
			titleLabelRect.y = (windowRect.height * 0.13f) / 2f - 20;
			if(selectedID >= 0) EditorGUI.LabelField(titleLabelRect, IconsData.iconsName[selectedID], labelStyle);
			else EditorGUI.LabelField(titleLabelRect, "Pick your icon here :",labelStyle);

			iconStyle.fontSize = iconSize;

			Rect iconRect = new Rect(10, 10, iconSize, iconSize);
			int lines = IconsData.iconsUnicode.Length / itemPerLine;

			Rect scrollArea = new Rect(0, 0, 10, lines * (iconSize + spacing));
			using (var scrollViewScope = new GUI.ScrollViewScope(contentRect, scrollPosition, scrollArea))
			{
				
				scrollPosition = scrollViewScope.scrollPosition;

				for (int l = 0; l < lines; l++)
				{
					int curLine = (int)((float)lines * (scrollPosition.y / scrollArea.height));
					iconRect.x = spacing;
					for (int r = 0; r < itemPerLine; r++)
					{
						if (curLine - l < -5 || curLine - l > 1) continue;

						int index2Pick = l * itemPerLine + r;

						string decoded = DecodeUnicodeString(@"\u" + IconsData.iconsUnicode[index2Pick]);

						if (decoded == selected)
						{
							selectedID = index2Pick;
							EditorGUI.DrawRect(iconRect, MaterialDesignColorset.grey400);
						}
						if (GUI.Button(iconRect, "", buttonStyle))
						{
							selected = decoded;
							onSelectionChanged(selected);
						}

						EditorGUI.LabelField(iconRect, decoded, iconStyle);
						Rect textRect = iconRect;
						textRect.yMax += 13;

						string name2Display = IconsData.iconsName[index2Pick];
						if (name2Display.Length > 14) name2Display = name2Display.Substring(0, 14);
						EditorGUI.LabelField(textRect, name2Display,iconTextStyle);

						iconRect.x += iconSize + spacing;
					}
					iconRect.y += iconSize + spacing;
				}

			}
		}
	}
}
