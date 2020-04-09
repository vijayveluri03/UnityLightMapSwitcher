#define SHADER_OCCURENCE_ALTERNATE_APPROACH
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class FilePropertiesWindow : EditorWindow
{
	[MenuItem("MyMenu/File Properties")]
	public static void Open()
	{
		GetWindow<FilePropertiesWindow>();
	}

	Vector2 scrollPos = Vector2.zero;
	void OnGUI()
	{
		UnityEngine.Object[] objects =  Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

		
		if ( objects == null || objects.Length == 0 )
		{
			GUILayout.Label("Empty");
			return;
		}

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.Width(this.position.width), GUILayout.Height(this.position.height));
 		
		foreach ( UnityEngine.Object obj in objects )
		{

			System.String pathToSelectedObject = AssetDatabase.GetAssetPath ( obj.GetInstanceID() );
			string createdDate = File.GetCreationTime(pathToSelectedObject ).ToString("dd/MM/yyyy h:mm tt");
			string modifiedDate = File.GetLastWriteTime(pathToSelectedObject ).ToString("dd/MM/yyyy h:mm tt");
			string accessedDate = File.GetLastAccessTime(pathToSelectedObject ).ToString("dd/MM/yyyy h:mm tt");
			
			GUILayout.Label( string.Format( "File selected {0}", pathToSelectedObject ) );
			GUILayout.Label( string.Format( "Created Date \t {0}", createdDate ) );
			GUILayout.Label( string.Format( "Modified Date \t {0}", modifiedDate ) );
			GUILayout.Label( string.Format( "Accessed Date \t {0}", accessedDate ) );

			GUILayout.Space(10);
		}

		EditorGUILayout.EndScrollView();
	}
}