using UnityEngine;
using UnityEditor;

namespace SpadesPlus.Card {
    [CanEditMultipleObjects]
	[CustomEditor(typeof(PeakCard))]
	public class PeakCardInspector : Editor {

		private PeakCard peakCard;

		void OnEnable() {
			peakCard = target as PeakCard;
		}

		public override void OnInspectorGUI () {
			DrawDefaultInspector();

			GUILayout.Space(15);

			serializedObject.Update();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Initialize")) {
				RankType rank = (RankType) serializedObject.FindProperty("rankType").enumValueIndex;
				SuitType suit = (SuitType) serializedObject.FindProperty("suitType").enumValueIndex;
				Undo.RecordObject(peakCard, "Initialize");
				peakCard.Initialize(suit, rank);
				EditorUtility.SetDirty(peakCard);
			}
			if (GUILayout.Button("Set Color")) {
				Color tint = serializedObject.FindProperty("tint").colorValue;
				Undo.RecordObject(peakCard, "Set Color");
				peakCard.SetColor(tint);
				EditorUtility.SetDirty(peakCard);
			}
			if (GUILayout.Button("Evaluate Atlas")) {
				string atlas = serializedObject.FindProperty("textureAtlas").stringValue;
				Undo.RecordObject(peakCard, "Evaluate Atlas");
				peakCard.EvaluateAtlas(atlas);
				EditorUtility.SetDirty(peakCard);
			}
			EditorGUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();


		}
	}
}