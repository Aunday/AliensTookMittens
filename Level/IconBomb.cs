using UnityEngine;

public class IconBomb : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Ship ship;
    public Board board;
    private float damage;
    public PrefabPools prefabPools;
    private int attachedRow;
    private int attachedColumn;

    public void PushOnStack()
    {
        //animator.SetBool("stopAnimation", true);
        spriteRenderer.enabled = false;
        animator.enabled = false;
        transform.parent = prefabPools.transform;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        prefabPools.stackIconBomb.Push(this);
        enabled = false;
    }

    public void PopOffStack(float newDamage, Transform iconTransform, int row, int column)
    {
        //animator.SetBool("stopAnimation", false);
        animator.enabled = true;
        animator.Play(0);
        spriteRenderer.enabled = true;
        transform.parent = iconTransform;
        transform.localPosition = new Vector2(0.0f, 0.0f);
        damage = newDamage;
        attachedRow = row;
        attachedColumn = column;
    }

    public void Explode()
    {
        ship.PlayerTakeDamage(damage);
        //prefabPools.CreateExplosion(transform, 0.35f);
        Icon parentIcon = GetComponentInParent<Icon>();
        if (parentIcon != null)
        {
            parentIcon.DeactivateBomb();
        }
        board.ModuleDestroyIcon(attachedColumn, attachedRow, 0);
        prefabPools.CreateExplosion(ship.transform, 0.25f);
    }
}
