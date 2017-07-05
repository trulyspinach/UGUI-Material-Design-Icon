using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MaterialDesignIcon
{
	public class ColorPickerWindow : EditorWindow
	{

		string myString = "Hello World";
		bool groupEnabled;
		bool myBool = true;
		float myFloat = 1.23f;

		static ColorPickerWindow selfW;
		static GUIStyle labelStyle;
		static GUIStyle iconTextStyle;
		static GUIStyle barStyle;
		static GUIStyle buttonStyle;

		//Color cannot be null, so default would be pink. Just because I like pink.
		//the default never matters.
		Color selected = MaterialDesignColorset.pink500;
		int selectedID = -1;
		System.Action<Color> onSelectionChanged;

		Vector2 scrollPosition;

		const int itemPerLine = 1;
		const int iconSize = 40;
		const int spacing = 20;

		static public void Show(Color preSelect, System.Action<Color> callback)
		{
			selfW = (ColorPickerWindow)EditorWindow.GetWindow<ColorPickerWindow>(true);

			selfW.title = "Pick an Icon";
			selfW.minSize = new Vector2(410, 529);
			selfW.maxSize = new Vector2(410, 529);

			labelStyle = new GUIStyle();
			var fontGUID = AssetDatabase.FindAssets("Roboto-Light")[0];
			labelStyle.font = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontGUID));

			iconTextStyle = new GUIStyle();
			fontGUID = AssetDatabase.FindAssets("Roboto-Medium")[0];
			iconTextStyle.font = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontGUID));
			iconTextStyle.alignment = TextAnchor.LowerCenter;
			iconTextStyle.normal.textColor = MaterialDesignColorset.grey600;
			iconTextStyle.fontSize = 12;

			buttonStyle = new GUIStyle();

			selfW.onSelectionChanged = callback;
			List<Color> tempList = MaterialDesignColorset.colorSets;
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
			EditorGUI.DrawRect(titleRect, selected);
			EditorGUI.DrawRect(contentRect, MaterialDesignColorset.grey200);


			labelStyle.fontSize = 40;
			labelStyle.normal.textColor = MaterialDesignColorset.grey100;

			Rect titleLabelRect = titleRect;
			titleLabelRect.x = 15;
			titleLabelRect.y = (windowRect.height * 0.13f) / 2f - 20;
			if (!MaterialDesignColorset.IsDarkColor(selected)) labelStyle.normal.textColor = MaterialDesignColorset.grey700;
			EditorGUI.LabelField(titleLabelRect, "Colors :", labelStyle);

			Rect iconRect = new Rect(10, 10, 330, iconSize);
			Rect largeIconRect = new Rect(0, 0, 500, iconSize + 20);
			int lines = MaterialDesignColorset.colorSets.Count / itemPerLine;

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
						if (curLine - l < -8 || curLine - l > 1) continue;

						int index2Pick = l * itemPerLine + r;

						Color decoded = MaterialDesignColorset.colorSets[index2Pick];

						if (!MaterialDesignColorset.IsDarkColor(decoded)) 
							EditorGUI.DrawRect(largeIconRect, MaterialDesignColorset.grey500);
						EditorGUI.DrawRect(iconRect, decoded);
						
						if (decoded == selected)
						{
							selectedID = index2Pick;
						}
						if (GUI.Button(iconRect, "", buttonStyle))
						{
							selected = decoded;
							onSelectionChanged(selected);
						}



						Rect textRect = iconRect;
						textRect.yMax += 13;

						iconRect.x += iconSize + spacing;
					}
					iconRect.y += iconSize + spacing;
					largeIconRect.y += iconSize + spacing;
				}

			}
		}
	}
}
