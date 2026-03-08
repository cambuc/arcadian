using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class ForceUpNormals : MonoBehaviour
{
    [Tooltip("Enable this to force all normals upward.")]
    public bool applyUpNormals = false;

    private void OnValidate()
    {
        if (!applyUpNormals)
            return;

        applyUpNormals = false; // Reset toggle (acts like button)

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("No MeshFilter or sharedMesh found.");
            return;
        }

        Mesh mesh = mf.sharedMesh; // IMPORTANT: modifies asset itself

        Vector3[] normals = new Vector3[mesh.vertexCount];

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
        }

        mesh.normals = normals;

#if UNITY_EDITOR
        EditorUtility.SetDirty(mesh);
        AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Normals permanently set upward on mesh asset: {mesh.name}");
    }
}
