#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Whumpus.Editor;
/*

public class RoomsUpdater : EditorWindow
	{
		private LevelData m_selectedLevel; // TODO: Remplacer ça par un array de scènes (strings de leurs noms ou de leur path, je suppose)

		[MenuItem("Toross/Room/Rooms Updater")]
		public static void OpenWindow()
		{
			Open();
		}

		private static void Open()
		{
			var window = GetWindow<RoomsUpdater>();
			window.Show();
			window.titleContent = new GUIContent("Rooms Updater", DataEditorIcons.dungeonViewerIcon);
		}

		private void OnGUI()
		{
			GUILayout.Label("Room Updater", EditorStyles.boldLabel);
			m_selectedLevel = (LevelData)EditorGUILayout.ObjectField("Level to update", m_selectedLevel, typeof(LevelData), false);

			if (m_selectedLevel != null && GUILayout.Button("Update All Level Rooms"))
			{
				if (EditorUtility.DisplayDialog("Update all level rooms ?", "Be sure to know what this tool do before updating all rooms.", "Yes", "No"))
				{
					UpdateAllLevelRooms();
				}
			}
		}

		private void UpdateAllLevelRooms()
		{
			// TODO: Réécrire le début de cette fonction pour que ça itère directement à travers les scènes de la variable.
			
			if (m_selectedLevel == null)
			{
				return;
			}

			for (int i = 0; i < m_selectedLevel.rooms.Count; i++)
			{
				RoomData roomData = m_selectedLevel.rooms[i].editorData;
				if (roomData.scene == null || roomData.scene.IsEmpty())
				{
					continue;
				}

				// Le chargement de chaque scène a lieu ici. à part l'argument de GetAssetPath(), c'est ce qu'il ne faut PAS changer
				string scenePath = AssetDatabase.GetAssetPath(roomData.scene.editorAsset);
				Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
				UpdateScene(scene);
			}
		}

		private void UpdateScene(Scene scene)
		{
			// TODO: Ici tu mets les code que tu veux éxecuter dans chaque scène :)
			// throw new NotImplementedException();

			EditorSceneManager.SaveScene(scene);
		}
	}
*/

#endif