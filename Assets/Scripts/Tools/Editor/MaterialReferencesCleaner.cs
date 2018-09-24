using UnityEngine;
using UnityEditor;

public class MaterialReferencesCleaner : EditorWindow
{
	private Material m_selectedMaterial;
	private SerializedObject m_serializedObject;

	[MenuItem("Tools/MaterialReferencesCleaner")]
	private static void Init()
	{
		GetWindow<MaterialReferencesCleaner>("Ref. Cleaner");
	}

	protected virtual void OnEnable()
	{
		GetSelectedMaterial();
	}

	protected virtual void OnSelectionChange()
	{
		GetSelectedMaterial();
	}

	protected virtual void OnProjectChange()
	{
		GetSelectedMaterial();
	}

	protected virtual void OnGUI()
	{
		EditorGUIUtility.labelWidth = 200f;

		if (m_selectedMaterial == null)
		{
			EditorGUILayout.LabelField("No material selected");
		}
		else
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Selected material:", m_selectedMaterial.name);
			EditorGUILayout.LabelField("Shader:", m_selectedMaterial.shader.name);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Properties");

			m_serializedObject.Update();

			EditorGUI.indentLevel++;

			EditorGUILayout.LabelField("Textures");
			EditorGUI.indentLevel++;
			ProcessProperties("m_SavedProperties.m_TexEnvs");
			EditorGUI.indentLevel--;

			EditorGUILayout.LabelField("Floats");
			EditorGUI.indentLevel++;
			ProcessProperties("m_SavedProperties.m_Floats");
			EditorGUI.indentLevel--;

			EditorGUILayout.LabelField("Colors");
			EditorGUI.indentLevel++;
			ProcessProperties("m_SavedProperties.m_Colors");
			EditorGUI.indentLevel--;

			EditorGUI.indentLevel--;
		}

		EditorGUIUtility.labelWidth = 0;
	}

	private void ProcessProperties(string path)
	{
		var properties = m_serializedObject.FindProperty(path);
		if (properties != null && properties.isArray)
		{
			for (int i = 0; i < properties.arraySize; i++)
			{
				string propName = properties.GetArrayElementAtIndex(i).displayName;
				bool exist = m_selectedMaterial.HasProperty(propName);

				if (exist)
				{
					EditorGUILayout.LabelField(propName, "Exist");
				}
				else
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField(propName, "Old reference", "CN StatusWarn");
						if (GUILayout.Button("Remove", GUILayout.Width(80f)))
						{
							properties.DeleteArrayElementAtIndex(i);
							m_serializedObject.ApplyModifiedProperties();
							GUIUtility.ExitGUI();
						}
					}

				}
			}
		}
	}

	private void GetSelectedMaterial()
	{
		m_selectedMaterial = Selection.activeObject as Material;
		if (m_selectedMaterial != null)
		{
			m_serializedObject = new SerializedObject(m_selectedMaterial);
		}

		Repaint();
	}
}
