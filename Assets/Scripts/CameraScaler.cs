using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    void Start()
    {
        // Get the height of the world.
        float worldHeight = GetComponent<BoxCollider2D>().size.y;

        // Set the camera's orthographic size to half the height of the world.
        Camera.main.orthographicSize = worldHeight / 2f;
    }
}