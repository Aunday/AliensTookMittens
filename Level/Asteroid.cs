using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Asteroid : MonoBehaviour
{
    public Space space;
    private PrefabPools prefabPools;
    public Transform enemyHPBar;
    private float enemyHP;
    private float moneyReward;
    private float enemyMaxHP;
    private float enemySpeed;
    private float deltaTime = 0.0f;
    new Transform transform;
    public Transform asteroidAnimation;
    public Animator animator;
    private GameManager gameManager;
    public SpriteRenderer spriteRenderer1;
    public SpriteRenderer spriteRenderer2;
    public BoxCollider boxCollider;
    private bool pause = false;
    private Transform transformSpace;
    private Transform transformPool;
    private List<int> armamentIds = new List<int>();

    public float GetCurrentHP()
    {
        return enemyHP;
    }

    public void StartSetup()
    {
        transform = gameObject.transform;
        gameManager = space.gameManager;
        prefabPools = gameManager.prefabPools;
        transformSpace = space.transform;
        transformPool = prefabPools.transform;
        PushOnStack();
    }

    public void PushOnStack()
    {
        boxCollider.enabled = false;
        spriteRenderer1.enabled = false;
        spriteRenderer2.enabled = false;
        animator.enabled = false;
        armamentIds.Clear();
        prefabPools.stackAsteroid.Push(this);
        transform.parent = transformPool;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        enabled = false;
    }

    public void PopOffStack(float threat, int whichLevel, float randLocX, float randLocY)
    {
        SetStats(threat, whichLevel);
        boxCollider.enabled = true;
        asteroidAnimation.Rotate(0.0f, 180.0f * Random.Range(0, 2), 0.0f);
        spriteRenderer1.enabled = true;
        spriteRenderer2.enabled = true;
        animator.enabled = true;
        animator.speed = Random.Range(0.1f, 1.0f);
        animator.Play(0);
        transform.parent = transformSpace;
        transform.localPosition = new Vector2(randLocX, randLocY);
    }

    public void SetStats(float threat, int whichLevel)
    {
        if (threat < 100.0f)
        {
            threat = 0.5f + (threat / 100.0f) * 0.5f;
        }
        else
        {
            threat /= 100.0f;
        }

        enemyHP = whichLevel * 10.0f * threat + 35.0f;
        enemyMaxHP = enemyHP;
        moneyReward = whichLevel * 6.0f * threat + 10.0f;
        enemySpeed = (0.65f + (threat / 13.5f)) * Random.Range(0.006f, 0.009f);
        AdjustHPBar();
    }

    public void TogglePause()
    {
        pause = !pause;
    }

    void Update()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime > 0.013f)
        {
            if (!pause)
            {
                float locationX = transform.localPosition.x;
                float blackHolePull = space.blackHolePull;
                if (transform.localPosition.x > -3.2f)
                {
                    if (blackHolePull == 0.0f)
                    {
                        transform.localPosition -= new Vector3(enemySpeed * (deltaTime / 0.013f), 0.0f, 0.0f);
                    }
                    else
                    {
                        if (locationX < 1.9f)
                        {
                            transform.localPosition -= new Vector3((enemySpeed - (blackHolePull * 0.35f)) * (deltaTime / 0.013f), 0.0f, 0.0f);
                        }
                        else
                        {
                            transform.localPosition -= new Vector3(enemySpeed * (deltaTime / 0.013f), 0.0f, 0.0f);
                        }
                    }
                }
                else
                {
                    space.ChangeNumberOfEnemies(-10);
                    PushOnStack();
                }
            }
            deltaTime = 0.0f;
        }
    }

    public bool AlreadyHit(int armamentId)
    {
        if (!armamentIds.Contains(armamentId))
        {
            armamentIds.Add(armamentId);
            return false;
        }
        return true;
    }

    public void ActivateDamageOverTime(float damage, int intensity, int dotType)
    {
        StartCoroutine(DamageOverTime(damage, intensity, dotType));
    }

    IEnumerator DamageOverTime(float damage, int intensity, int dotType)
    {
        SpriteRenderer spriteEnemyHPBar = enemyHPBar.GetComponent<SpriteRenderer>();
        if (dotType == 0)
        {
            spriteEnemyHPBar.color = Color.green;
        }
        else
        {
            spriteEnemyHPBar.color = new Color(1.0f, 0.47f, 0.0f);
        }
        float waitPeriod = 5.0f / intensity;
        while (intensity > 0)
        {
            intensity--;
            EnemyTakeDamage(damage, 0.0f);
            yield return new WaitForSeconds(waitPeriod);
        }
        spriteEnemyHPBar.color = new Color(0.6f, 0.0f, 0.0f);
    }

    public void EnemyTakeDamage(float damageValue, float pushBack)
    {
        if (enemyHP > 0)
        {
            enemyHP -= damageValue;
            space.CreateTextDamage(damageValue, transform.localPosition.x, transform.localPosition.y, true);

            if (enemyHP <= 0)
            {
                int moneyRewardInt = Mathf.CeilToInt(moneyReward);
                prefabPools.CreateExplosion(transform, 0.33f);
                space.ChangeNumberOfEnemies(-10);
                gameManager.board.CreateMatchText(4, moneyRewardInt);
                gameManager.ChangeFunds(moneyRewardInt);
                space.AsteroidDestroyed();
                PushOnStack();
            }
            else
            {
                AdjustHPBar();
            }
        }
    }

    void AdjustHPBar()
    {
        enemyHPBar.localScale = new Vector3((enemyHP / enemyMaxHP), 1.0f, 1.0f);
    }
}
