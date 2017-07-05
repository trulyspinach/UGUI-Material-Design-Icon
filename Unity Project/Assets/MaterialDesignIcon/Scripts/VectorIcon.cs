using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialDesignIcon
{
	public class VectorIcon : Text
	{

		[SerializeField]
		private float m_Size = 10;
		public float size
		{
			get { return m_Size; }
			set
			{
				m_Size = value;
				UpdateScale();
			}
		}


		protected override void Start()
		{
			base.Start();
		}

		public void UpdateScale()
		{

			rectTransform.sizeDelta = new Vector2(m_Size, m_Size);
			fontSize = (int)m_Size;
		}

		readonly UIVertex[] m_TempVerts = new UIVertex[4];
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (font == null)
				return;

			// We don't care if we the font Texture changes while we are doing our Update.
			// The end result of cachedTextGenerator will be valid for this instance.
			// Otherwise we can get issues like Case 619238.
			m_DisableFontTextureRebuiltCallback = true;

			Vector2 extents = rectTransform.rect.size;

			var settings = GetGenerationSettings(extents);
			cachedTextGenerator.Populate(DecodeUnicodeString(text), settings);

			Rect inputRect = rectTransform.rect;

			Vector2 refPoint = new Vector2(inputRect.xMin, inputRect.yMax);

			// Determine fraction of pixel to offset text mesh.
			Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

			// Apply the offset to the vertices
			IList<UIVertex> verts = cachedTextGenerator.verts;
			float unitsPerPixel = 1 / pixelsPerUnit;
			//Last 4 verts are always a new line...
			int vertCount = verts.Count - 4;

			toFill.Clear();
			if (roundingOffset != Vector2.zero)
			{
				for (int i = 0; i < vertCount; ++i)
				{
					int tempVertsIndex = i & 3;
					m_TempVerts[tempVertsIndex] = verts[i];
					m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
					m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
					m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
					if (tempVertsIndex == 3)
						toFill.AddUIVertexQuad(m_TempVerts);
				}
			}
			else
			{
				for (int i = 0; i < vertCount; ++i)
				{
					int tempVertsIndex = i & 3;
					m_TempVerts[tempVertsIndex] = verts[i];
					m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
					if (tempVertsIndex == 3)
						toFill.AddUIVertexQuad(m_TempVerts);
				}
			}
			m_DisableFontTextureRebuiltCallback = false;
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			SetLayoutDirty();
		}
#endif

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			UpdateScale();
		}

		private string DecodeUnicodeString(string s)
		{
			return System.Text.RegularExpressions.Regex.Unescape(s);
		}
	}
}