using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] CameraMovement cameraMovement;
    [SerializeField] float cameraMaxY;
    [SerializeField] float cameraMinY;
    // Start is called before the first frame update
    void Start()
    {
        cameraMovement.maxY = cameraMaxY;
        cameraMovement.minY = cameraMinY;
    }
}
