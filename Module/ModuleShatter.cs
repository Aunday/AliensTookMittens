using UnityEngine;

public class ModuleShatter : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spritePart1;
    public SpriteRenderer spritePart2;
    public SpriteRenderer spritePart3;
    public SpriteRenderer spritePart4;
    public PrefabPools prefabPools;

    public void StartSetup()
    {
        //transform = gameObject.transform;
        //space = gameManager.space;
        //transformPool = prefabPools.transform;
        //transformSpace = space.transform;
        PushOnStack();
    }

    public void PushOnStack()
    {
        animator.enabled = false;
        spritePart1.enabled = false;
        spritePart2.enabled = false;
        spritePart3.enabled = false;
        spritePart4.enabled = false;
        prefabPools.stackModuleShatter.Push(this);
        transform.localPosition = new Vector2(0.0f, 30.0f);
        //transform.parent = transformPool;
        enabled = false;
    }

    public void PopOffStack(float locationX, float locationY)
    {
        animator.enabled = true;
        spritePart1.enabled = true;
        spritePart2.enabled = true;
        spritePart3.enabled = true;
        spritePart4.enabled = true;
        //transform.parent = transformSpace;
        transform.position = new Vector2(locationX, locationY);
        //damage = rocketDamage;
    }
}