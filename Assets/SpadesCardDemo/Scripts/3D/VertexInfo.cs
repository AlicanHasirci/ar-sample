using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(VertexInfo))]
public class VertexInfoInspector : Editor {

    private VertexInfo component;
    private Mesh sharedMesh;
    private Vector3[] vertices;
    private Vector2[] uv;

    private const float handleSize = 0.1f;
    private const float pickSize = 0.06f;

    private int selectedIndex = -1;

    private void OnSceneGUI () {
        component = (VertexInfo)target;
        sharedMesh = component.GetComponent<MeshFilter>().sharedMesh;
        vertices = sharedMesh.vertices;
        uv = sharedMesh.uv;

        for(int i = 0; i < vertices.Length; i++) {
            ShowVertex(i);
        }
    }

    private void ShowVertex (int index) {
        Vector3 vertex = component.transform.TransformPoint(vertices[index]);
        Vector3 gui = HandleUtility.WorldToGUIPoint(vertex);

        float size = HandleUtility.GetHandleSize(vertex);
        if (Handles.Button(vertex, Quaternion.identity, size * handleSize, size * pickSize, Handles.SphereHandleCap)) {
            selectedIndex = index;
            Repaint();
        }
        Handles.color = Color.green;
        if (selectedIndex == index) {
            Handles.BeginGUI();
            GUILayout.BeginArea( new Rect(gui.x, gui.y, 200,500));
            GUILayout.BeginVertical("box");
            GUILayout.Label("Index: " + index);
            GUILayout.Label("UV: (" + uv[index].x + ", " + uv[index].y + ")");
            GUILayout.EndVertical();
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VertexInfo : MonoBehaviour {}
