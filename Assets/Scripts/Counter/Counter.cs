using UnityEngine;

public class Counter : MonoBehaviour
{
    private int _count = 0;

    public Counter(int count)
    {
        _count = count;
    }

    public int Count => _count;

    public void IncreaseCount() =>
                _count++;

    //public void DecreaseCount() =>
    //            _count--;
}