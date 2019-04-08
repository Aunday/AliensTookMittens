using UnityEngine;

public class IconPhaseOut : MonoBehaviour
{
    public SpriteRenderer spriteSlash;
    public SpriteRenderer spriteTop;
    public SpriteRenderer spriteBottom;
    public Animator animator;
    public PrefabPools prefabPools;

    public void StartSetup()
    {
        PushOnStack();
    }

    public void PushOnStack()
    {
        animator.enabled = false;
        spriteSlash.enabled = false;
        spriteTop.enabled = false;
        spriteBottom.enabled = false;
        prefabPools.stackIconPhaseOut.Push(this);
        transform.localPosition = new Vector2(0.0f, 30.0f);
        enabled = false;
    }

    public void PopOffStack(int iconType, float positionX, float positionY)
    {
        spriteSlash.enabled = true;
        spriteTop.enabled = true;
        spriteBottom.enabled = true;
        animator.enabled = true;
        animator.SetInteger("type", iconType);
        animator.Play(0);
        transform.position = new Vector2(positionX, positionY);
    }
}
