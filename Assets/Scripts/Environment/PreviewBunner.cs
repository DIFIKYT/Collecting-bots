using UnityEngine;

public class PreviewBunner : MonoBehaviour
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
}