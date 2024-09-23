using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private float _cameraSpeed;
    [SerializeField] private float _edgeSize;

    private float _screenWidth;
    private float _screenHeight;

    private void Awake()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
    }

    private void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.mousePosition.x <= _edgeSize)
            movement.x = -_cameraSpeed * Time.deltaTime;
        if (Input.mousePosition.x >= _screenWidth - _edgeSize)
            movement.x = _cameraSpeed * Time.deltaTime;
        if (Input.mousePosition.y >= _screenHeight - _edgeSize)
            movement.z = _cameraSpeed * Time.deltaTime;
        if (Input.mousePosition.y <= _edgeSize)
            movement.z = -_cameraSpeed * Time.deltaTime;

        transform.Translate(movement, Space.World);
    }
}