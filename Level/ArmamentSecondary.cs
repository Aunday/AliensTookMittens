using UnityEngine;

public class ArmamentSecondary : MonoBehaviour
{
    //public float armamentSecondarySpeed;
    public SpriteRenderer ArmamentSecondaryFace;
    private float damage;
    //float deltaTime = 0.0f;
    //public int matchIntensity;
    //public Transform transformBlast;
    public PrefabPools prefabPools;
    //private GameManager gameManager;
    public BoxCollider boxCollider;
    public float knockback { get; private set; }
    public Animator animator;
    public int armamentId;
    private int armamentIdRandom;

    public void StartSetup(int armamentSecondaryTotal)
    {
        //gameManager = prefabPools.gameManager;
        armamentId = armamentSecondaryTotal;
        PushOnStack();
    }

    public void PushOnStack()
    {
        animator.enabled = false;
        boxCollider.enabled = false;
        ArmamentSecondaryFace.enabled = false;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        prefabPools.stackArmamentSecondary.Push(this);
        enabled = false;
    }

    public void PopOffStack(int matchValue, Transform shipLocation, float knockbackValue)
    {
        armamentIdRandom = armamentId + Time.frameCount;
        knockback = knockbackValue;
        boxCollider.enabled = true;
        ArmamentSecondaryFace.enabled = true;
        transform.position = new Vector2(shipLocation.position.x, shipLocation.position.y);
        animator.enabled = true;
        animator.Play(0);
        damage = matchValue;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            if (!enemy.AlreadyHit(armamentIdRandom))
            {
                prefabPools.CreateExplosion(enemy.transform, 0.16f);
                enemy.EnemyTakeDamage(damage, knockback, true);
            }
        }
        else if (col.gameObject.tag == "Asteroid")
        {
            Asteroid asteroid = col.gameObject.GetComponent<Asteroid>();
            if (!asteroid.AlreadyHit(armamentIdRandom))
            {
                prefabPools.CreateExplosion(asteroid.transform, 0.16f);
                asteroid.EnemyTakeDamage(damage * 0.6f, knockback);
            }
        }
    }
}