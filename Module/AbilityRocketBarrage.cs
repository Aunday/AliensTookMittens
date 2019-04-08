using UnityEngine;

public class AbilityRocketBarrage : MonoBehaviour
{
    public GameManager gameManager;
    private float deltaTime = 0.0f;
    private bool peaked;
    private float rocketSpeed;
    private float peak;
    private float yChangeRate;
    new Transform transform;
    private Space space;
    public BoxCollider boxCollider;
    public PrefabPools prefabPools;
    private Transform transformSpace;
    private Transform transformPool;
    public SpriteRenderer spriteRenderer;
    private bool notYetHitEnemy = true;

    public void StartSetup()
    {
        transform = gameObject.transform;
        space = gameManager.space;
        transformPool = prefabPools.transform;
        transformSpace = space.transform;
        PushOnStack();
    }

    public void PushOnStack()
    {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        prefabPools.stackAbilityRocketBarrage.Push(this);
        transform.localPosition = new Vector2(0.0f, 30.0f);
        transform.parent = transformPool;
        enabled = false;
    }

    public void PopOffStack(Transform shipLocation)
    {
        notYetHitEnemy = true;
        peaked = false;
        rocketSpeed = 0.04f / 0.013f;
        peak = Random.Range(-0.9f, 0.9f);
        yChangeRate = peak * 0.07f;
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
        transform.parent = transformSpace;
        transform.position = new Vector2(shipLocation.position.x + 0.1f * gameManager.scaleRatioX, shipLocation.position.y);
        //damage = rocketDamage;
    }

    void Update ()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime > 0.013f)
        {
            transform.localPosition += new Vector3(rocketSpeed * deltaTime, yChangeRate, 0.0f);
            float yPosition = transform.localPosition.y;
            if (((yPosition >= peak && yChangeRate > 0) || (yPosition < peak && yChangeRate <= 0)) && !peaked)
            {
                peaked = true;
                rocketSpeed = 0.1f / 0.013f;

                yChangeRate = yPosition * -0.025f;

                //aim toward enemy if there is one
                //Enemy enemy = ;
                if (space.enemy.Count > 0)
                {
                    yChangeRate *= (5.0f / (space.enemy[0].transform.localPosition.x + 2));
                }
            }

            deltaTime = 0.0f;
            //return to stack if go off screen
            if (transform.localPosition.x > 6.0f || yPosition < -1.4f || yPosition > 1.4f)
            {
                PushOnStack();
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (notYetHitEnemy)
        {
            if (col.gameObject.tag == "Enemy")
            {
                float damage = gameManager.upgradeValues[33, PlayerStats.playerStats.upgradeStatus[33]];
                notYetHitEnemy = false;
                Enemy enemy = col.gameObject.GetComponent<Enemy>();
                prefabPools.CreateExplosion(transform, 0.13f);
                if (enemy.enemyHP <= damage)
                {
                    gameManager.missionTracker.IncrementMissionProgress(2, 1);
                }
                enemy.EnemyTakeDamage(damage, 1.0f, false);
                PushOnStack();
            }
            else if (col.gameObject.tag == "Asteroid")
            {
                float damage = gameManager.upgradeValues[33, PlayerStats.playerStats.upgradeStatus[33]];
                notYetHitEnemy = false;
                prefabPools.CreateExplosion(transform, 0.13f);
                col.gameObject.GetComponent<Asteroid>().EnemyTakeDamage(damage, 1.0f);
                PushOnStack();
            }
        }
    }
}