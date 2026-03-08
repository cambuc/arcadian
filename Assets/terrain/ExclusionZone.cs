using UnityEngine;

public class ExclusionZone : MonoBehaviour
{
    public TerrainPlacementTool tpt;

    public Vector2 boundingBox;

    public bool WithinBounds(float x, float y)
    {
        if(x >= transform.position.x - boundingBox.x / 2f && x <= transform.position.x + boundingBox.x / 2f &&
            y >= transform.position.z - boundingBox.y / 2f && y <= transform.position.z + boundingBox.y / 2f)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(boundingBox.x, 2, boundingBox.y));
    }
}
