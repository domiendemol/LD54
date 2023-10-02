using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    [SerializeField] private BoxCollider2D mainFloorCollider;
    
    void Start()
    {
        // Get the height of the world.
        float worldHeight = mainFloorCollider.size.y;

        // Set the camera's orthographic size to half the height of the world.
        Camera.main.orthographicSize = worldHeight / 2f;
    }
}