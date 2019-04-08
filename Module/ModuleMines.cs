using UnityEngine;
using System.Collections;

public class ModuleMines : MonoBehaviour
{
    private GameManager gameManager;
    private float deltaTime = 0.0f;
    private bool peaked;
    //private float rotateAmount;
    private float mineSpeed;
    private float peak;
    private float yChangeRate;
    new Transform transform;
    //private Transform transformSprite;
    private Space space;
    public SphereCollider sphereCollider;
    public PrefabPools prefabPools;
    private Transform transformSpace;
    private Transform transformPool;
    public SpriteRenderer spriteRenderer;
    private Animator animatorsprite;
    //private bool notYetHitEnemy;
    private float damage;
    private float pullIntensity;
    private Enemy enemy;
    private Asteroid asteroid;
    private float duration;

    public void StartSetup()
    {
        gameManager = prefabPools.gameManager;
        transform = gameObject.transform;
        space = gameManager.space;
        transformPool = prefabPools.transform;
        transformSpace = space.transform;
        //transformSprite = spriteRenderer.transform;
        animatorsprite = spriteRenderer.GetComponent<Animator>();
        PushOnStack();
    }

    public void PushOnStack()
    {
        sphereCollider.enabled = false;
        spriteRenderer.enabled = false;
        animatorsprite.enabled = false;
        prefabPools.stackModuleMines.Push(this);
        transform.localPosition = new Vector2(0.0f, 30.0f);
        transform.parent = transformPool;
        enabled = false;
    }

    public void PopOffStack(Transform shipLocation)
    {
        damage = gameManager.upgradeValues[22, PlayerStats.playerStats.upgradeStatus[22]];
        duration = gameManager.upgradeValues[37, PlayerStats.playerStats.upgradeStatus[37]];
        //notYetHitEnemy = true;
        peaked = false;
        enemy = null;
        pullIntensity = 0.003f;
        mineSpeed = Random.Range(0.09f, 0.11f);
        peak = Random.Range(-0.9f, 0.9f);
        //rotateAmount = Random.Range(0.2f, 0.8f);
        animatorsprite.speed = Random.Range(0.1f, 0.5f);
        //transform.localEulerAngles = new Vector3(0.0f, 0.0f, Random.Range(20.0f, 170.0f));
        yChangeRate = peak * 0.07f;
        sphereCollider.enabled = true;
        spriteRenderer.enabled = true;
        animatorsprite.enabled = true;
        gameManager.audioManager.PlaySound(14);
        transform.parent = transformSpace;
        transform.localPosition = new Vector2(-2.0f, 0.0f);
    }

    IEnumerator MoveMineToPosition()
    {
        yield return new WaitForSeconds(0.026f);
    }

    void Update()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime > 0.013f)
        {
            duration -= deltaTime;
            transform.localPosition += new Vector3(mineSpeed, yChangeRate, 0.0f);
            //transformSprite.Rotate(0.0f, 0.0f, rotateAmount);
            //transform.localEulerAngles = new Vector3(0.0f, 0.0f, transform.localEulerAngles.z + rotateAmount);
            mineSpeed = Mathf.Max(0.0f, mineSpeed - (0.125f * deltaTime));
            float yPosition = transform.localPosition.y;
            if (((yPosition >= peak && yChangeRate > 0) || (yPosition < peak && yChangeRate <= 0)) && !peaked)
            {
                peaked = true;
                yChangeRate = 0.0f;
            }

            deltaTime = 0.0f;
        }
        if (duration <= 0.0f)
        {
            prefabPools.CreateExplosion(transform, 0.25f);
            PushOnStack();
        }
    }

    //void LateUpdate()
    //{
    //    if (enemy != null)
    //    {
    //        if (enemy.enemyID == 14 || enemy.enabled)
    //        {
    //            enemy.transform.rotation = Quaternion.identity;
    //        }
    //    }
    //}

    void OnCollisionExit(Collision col)
    {
        enemy = null;
    }

    void OnCollisionStay(Collision col)
    {
        if (enabled)
        {
            if (col.gameObject.tag == "Enemy")
            {
                if (enemy == null)
                {
                    enemy = col.gameObject.GetComponent<Enemy>();
                }
                else if (!enemy.enabled)
                {
                    enemy = col.gameObject.GetComponent<Enemy>();
                }

                //if (enemy.enemyID == 14)
                //{
                //    enemy.transform.rotation = Quaternion.identity;
                //}

                //if this is the same enemy we have been targetting
                //if (enemy == col.gameObject.GetComponent<Enemy>())
                //{
                Vector2 distance = enemy.transform.localPosition - transform.localPosition;
                //if enemy is close enough, explode
                if (Mathf.Abs(distance.x) < 0.1f && Mathf.Abs(distance.y) < 0.1f)
                {
                    //notYetHitEnemy = false;
                    prefabPools.CreateExplosion(transform, 0.13f);
                    enemy.EnemyTakeDamage(damage, 1.0f, false);
                    PushOnStack();
                }
                //else move toward enemy
                else
                {
                    //pullIntensity
                    transform.localPosition += new Vector3(pullIntensity * distance.x, pullIntensity * distance.y, 0.0f);
                    pullIntensity += 0.002f;
                }
                //}
            }
            else if (col.gameObject.tag == "Asteroid")
            {
                if (asteroid == null || !asteroid.enabled)
                {
                    asteroid = col.gameObject.GetComponent<Asteroid>();
                }

                //if this is the same enemy we have been targetting
                //if (asteroid == col.gameObject.GetComponent<Asteroid>())
                //{
                Vector2 distance = asteroid.transform.localPosition - transform.localPosition;
                //if enemy is close enough, explode
                if (Mathf.Abs(distance.x) < 0.1f && Mathf.Abs(distance.y) < 0.1f)
                {
                    //notYetHitEnemy = false;
                    prefabPools.CreateExplosion(transform, 0.25f);
                    asteroid.EnemyTakeDamage(damage, 1.0f);
                    PushOnStack();
                }
                //else move toward enemy
                else
                {
                    //pullIntensity
                    transform.localPosition += new Vector3(pullIntensity * distance.x, pullIntensity * distance.y, 0.0f);
                    pullIntensity += 0.002f;
                }
                //}
            }
        }
    }
}
