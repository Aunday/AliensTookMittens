using UnityEngine;

public class TextStatic : MonoBehaviour
{
    void Start()
    {
        GetComponent<Renderer>().sortingOrder = transform.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
}