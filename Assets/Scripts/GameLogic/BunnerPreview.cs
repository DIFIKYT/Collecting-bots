using UnityEngine;

public class BunnerPreview : MonoBehaviour
{
    public bool IsActive => gameObject.activeSelf;

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}