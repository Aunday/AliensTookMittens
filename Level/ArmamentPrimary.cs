using UnityEngine;

public class ArmamentPrimary : MonoBehaviour
{
    public SpriteRenderer armamentPrimaryFace;
    public SpriteRenderer armamentCorrosionFace;
    public SpriteRenderer armamentPrimaryFaceCenter;
    private float damage;
    private float deltaTime = 0.0f;
    private int intensity;
    private float travelRateX;
    private float travelRateY;
    private float knockback;
    new Transform transform;
    public PrefabPools prefabPools;
    private GameManager gameManager;
    public BoxCollider boxCollider;
    private bool notYetHitEnemy;
    private int type;
    //private bool hasTarget;

    public void StartSetup()
    {
        transform = gameObject.transform;
        gameManager = prefabPools.gameManager;
        PushOnStack();
    }

    public void PushOnStack()
    {
        boxCollider.enabled = false;
        armamentPrimaryFace.enabled = false;
        armamentPrimaryFaceCenter.enabled = false;
        armamentCorrosionFace.enabled = false;
        prefabPools.stackArmamentPrimary.Push(this);
        transform.localPosition = new Vector2(0.0f, 30.0f);
        transform.rotation = Quaternion.identity;
        if (type == 2)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        enabled = false;
    }

    public void PopOffStack(int matchValue, int matchIntensity, int projectileType, float positionX, float positionY, float newKnockback) //projectileType == 0 -> laser, == 1 -> corrosion, == 2 -> side drone laser
    {
        type = projectileType;
        boxCollider.enabled = true;
        notYetHitEnemy = true;
        travelRateX = 10.0f;
        travelRateY = 0.0f;
        knockback = newKnockback;
        if (type != 1)
        {
            matchIntensity = Mathf.Max(matchIntensity - 3, 0);
            armamentPrimaryFace.enabled = true;
            armamentPrimaryFaceCenter.enabled = true;
            if (matchIntensity == 0)
            {
                armamentPrimaryFace.color = Color.green;
            }
            else if (matchIntensity == 1)
            {
                armamentPrimaryFace.color = Color.blue;
            }
            else
            {
                armamentPrimaryFace.color = Color.red;
            }
            gameManager.audioManager.PlaySound(1);
            if (type == 2)
            {
                GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        else
        {
            armamentCorrosionFace.enabled = true;
            intensity = matchIntensity;
        }
        transform.position = new Vector2(positionX, positionY);
        damage = matchValue;
    }

    void Update()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime > 0.015f)
        {
            transform.localPosition += new Vector3(travelRateX * deltaTime, travelRateY * deltaTime, 0.0f);
            deltaTime = 0.0f;
            //DESTROY if go off screen
            if (transform.localPosition.x > 3.5f || transform.localPosition.y > 5.0f)
            {
                PushOnStack();
            }
        }
    }

    public void AimAtTarget(Transform target, Transform origin)
    {
        if (target != null)
        {
            float xRate = (target.position.x - origin.position.x - 0.1f) / prefabPools.gameManager.scaleRatioX;
            float yRate = (target.position.y - origin.position.y) / prefabPools.gameManager.scaleRatioY;
            travelRateX = (xRate / (Mathf.Abs(xRate) + Mathf.Abs(yRate))) * 18.0f;
            travelRateY = (yRate / (Mathf.Abs(xRate) + Mathf.Abs(yRate))) * 18.0f;
            //travelRateY = 10.0f - travelRateX;
            float rotateAngle = Mathf.Atan(yRate / xRate);
            rotateAngle = Mathf.Rad2Deg * rotateAngle;
            transform.Rotate(0.0f, 0.0f, rotateAngle);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        string colTag = col.gameObject.tag;
        if (colTag == "Enemy")
        {
            if (notYetHitEnemy)
            {
                notYetHitEnemy = false;
                if (type != 1)
                {
                    prefabPools.CreateExplosion(transform, 0.22f);
                    col.gameObject.GetComponent<Enemy>().EnemyTakeDamage(damage, knockback, false);
                }
                else
                {
                    col.gameObject.GetComponent<Enemy>().ActivateDamageOverTime(damage, intensity + 4, 0);
                }
                PushOnStack();
            }
        }
        else if (colTag == "Asteroid")
        {
            if (notYetHitEnemy)
            {
                notYetHitEnemy = false;
                if (type != 1)
                {
                    prefabPools.CreateExplosion(transform, 0.22f);
                    col.gameObject.GetComponent<Asteroid>().EnemyTakeDamage(damage, 1.0f);
                }
                else
                {
                    col.gameObject.GetComponent<Asteroid>().ActivateDamageOverTime(damage, intensity + 4, 0);
                }
                PushOnStack();
            }
        }
        else if ((colTag == "EnemyProjectileRocket" || colTag == "EnemyProjectileFire" || colTag == "EnemyProjectileLaser" || colTag == "EnemyProjectileAcid" || colTag == "EnemyProjectileDrain" || colTag == "EnemyProjectileWeaken") && type == 2)
        {
            if (notYetHitEnemy)
            {
                notYetHitEnemy = false;
                prefabPools.CreateExplosion(transform, 0.1f);
                col.gameObject.GetComponent<EnemyProjectile>().PushOnStack();
                PushOnStack();
            }
        }
    }
}