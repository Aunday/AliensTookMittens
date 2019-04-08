using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer explosion1;
    public SpriteRenderer explosion2;
    public SpriteRenderer explosion3;
    //public SpriteRenderer implosion1;
    //public SpriteRenderer implosion2;
    //public SpriteRenderer implosion3;
    public PrefabPools prefabPools;

    public void PopOffStack(float locationX, float locationY)
    {
        //animator.enabled = true;
        transform.position = new Vector2(locationX, locationY);
    }

    public void PushOnStack()
    {
        animator.enabled = false;
        explosion1.enabled = false;
        explosion2.enabled = false;
        explosion3.enabled = false;
        //implosion1.enabled = false;
        //implosion2.enabled = false;
        //implosion3.enabled = false;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        prefabPools.stackExplosion.Push(this);
        //enabled = false;
    }

    public void Explode(Transform explodingObject, float explosionSizeMultiplier)
    {
        float explosionScale = explodingObject.transform.localScale.x * explosionSizeMultiplier;
        PopOffStack(explodingObject.position.x, explodingObject.position.y);
        transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);
        explosion1.enabled = true;
        explosion2.enabled = true;
        explosion3.enabled = true;
        animator.enabled = true;
        animator.SetBool("explosionType", false);
        animator.Play(0);
    }

    //public void Implode(Transform implodingObject, int type)
    //{
    //    //Color[] iconColor = { new Color(1.0f, 0.72f, 0), Color.red, Color.green, new Color(0.91f, 0, 1.0f), new Color(1.0f, 0.56f, 1.0f), new Color(0.0f, 0.75f, 1.0f), Color.yellow };
    //    PopOffStack(implodingObject.position.x, implodingObject.position.y);
    //    transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
    //    //implosion1.enabled = true;
    //    //implosion1.color = iconColor[type];
    //    //implosion2.enabled = true;
    //    //implosion2.color = iconColor[type];
    //    //implosion3.enabled = true;
    //    //implosion3.color = iconColor[type];
    //    animator.enabled = true;
    //    animator.SetBool("explosionType", true);

    //    animator.Play(0);
    //}
}