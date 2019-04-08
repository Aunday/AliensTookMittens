using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public Space space;
    public Transform enemyHPBar;
    private Transform enemyHPShieldBar;
    private SpriteRenderer spriteShield;
    public float enemyDamage;
    public float moneyReward;
    public float enemySpeed;
    public float enemyHP;
    private float baseDamage;
    private float baseReward;
    private float baseSpeed;
    private float baseHP;
    private float enemyHPShield;
    private float enemyMaxHP;
    public float stopDistance;
    public float enemyAttackRate;
    public int enemyID;
    private float deltaTime;
    private float nextAttack;
    private bool microEnemy = false;
    new Transform transform;
    private Ship ship;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer spriteRenderer2;
    public Animator animator;
    private Animator animatorGnat;
    private bool dontInterrupt;
    private PrefabPools prefabPools;
    private bool pause = false;
    private List<int> armamentIds = new List<int>();
    private float movementY;
    private BoxCollider boxCollider;
    private int randomNum = 0;
    private int whichLevel;
    public float blackHoleMultiplier;
    public Transform transformProjectileOrigin;
    private bool enemyHurtable;
    //private int dotDuration;
    private int currentDots;

    public void StartSetup()
    {
        transform = gameObject.transform;
        gameManager = space.gameManager;
        prefabPools = gameManager.prefabPools;
        ship = gameManager.ship;

        movementY = 0.0f;
        if (enemyID == 9 || enemyID == 106)
        {
            spriteRenderer = transformProjectileOrigin.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenderer2 = transformProjectileOrigin.GetChild(0).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
        }
        else if (enemyID == 13 || enemyID == 103 || enemyID == 113)
        {
            enemyHPShieldBar = transformProjectileOrigin.GetChild(2).GetChild(0).transform;
            spriteShield = transformProjectileOrigin.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        }
        else if (enemyID == 14 || enemyID == 112)
        {
            boxCollider = GetComponent<BoxCollider>();
            spriteRenderer = transformProjectileOrigin.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        }
        else if (enemyID == 15)
        {
            movementY = Random.Range(-0.005f, 0.005f);
            stopDistance -= Random.Range(0.0f, 0.5f);
            SetMicro();
            animatorGnat = GetComponent<Animator>();
        }
        else if (enemyID == 50)
        {
            SetMicro();
        }

        baseDamage = enemyDamage;
        baseReward = moneyReward;
        baseSpeed = enemySpeed;
        baseHP = enemyHP;
    }

    public void SpawnEnemy (float threat, int setLevel)
    {
        deltaTime = 0.0f;
        //dotDuration = 0;
        pause = false;
        currentDots = 0;
        nextAttack = enemyAttackRate;
        enemyHurtable = true;
        if (threat < 100.0f)
        {
            threat = 0.7f + (threat / 100.0f) * 0.3f;
        }
        else
        {
            threat = 1.0f + ((threat - 100.0f) / 100.0f) * 0.5f;
        }
        whichLevel = setLevel;

        float hpMultiplier = threat * (0.75f + whichLevel * 0.18f);
        float difficultyMultiplier = threat * (0.75f + whichLevel * 0.16f);
        float damageMultiplier = threat * (0.8f + whichLevel * 0.11f);

        enemyDamage = baseDamage * damageMultiplier;
        moneyReward = baseReward * 0.8f * (difficultyMultiplier * Random.Range(1.0f, 1.25f));
        enemySpeed = baseSpeed * (0.9f + (difficultyMultiplier / 10.0f)) * 0.0079f;
        enemyHP = baseHP * hpMultiplier;
        enemyMaxHP = enemyHP;

        enemyHPBar.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.0f, 0.0f);

        if (enemyID != 15 && enemyID != 50)
        {
            transform.localPosition = new Vector2(3.8f, 0.0f);
        }

        if (enemyID == 4 || enemyID == 111)
        {
            animator.enabled = true;
            animator.speed = 0.0f;
            dontInterrupt = false;
        }
        else if (enemyID == 13 || enemyID == 103)
        {
            enemyHPShield = enemyHP * 1.2f;
            AdjustHPShieldBar();
            enemyHPShieldBar.gameObject.SetActive(true);
            spriteShield.enabled = true;
        }
        else if (enemyID == 14 || enemyID == 112)
        {
            animator.enabled = false;
            spriteRenderer.color = Color.white;
        }
        else if (enemyID == 9 || enemyID == 106)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer2.color = Color.white;
        }
        else if (enemyID == 15)
        {
            animatorGnat.enabled = true;
            if (threat > 0.99f && threat < 1.0f)
            {
                animatorGnat.SetBool("bossGnat", true);
            }
            else
            {
                animatorGnat.SetBool("bossGnat", false);
            }
            StartCoroutine(FlyFlat());
        }
        else if (enemyID == 113)
        {
            animator.enabled = true;
            animator.speed = 0.0f;
            dontInterrupt = true;
            enemyHPShield = enemyHP * 1.2f;
            AdjustHPShieldBar();
            enemyHPShieldBar.gameObject.SetActive(true);
            spriteShield.enabled = true;
        }
        AdjustHPBar();
        gameObject.SetActive(true);

        if (enemyID < 15)
        {
            if (PlayerStats.playerStats.threatMessageStatus[enemyID] == true)
            {
                StartCoroutine(PlayDescriptionMessage(enemyID));
            }
        }
    }

    IEnumerator PlayDescriptionMessage(int whichMessage)
    {
        enemyHurtable = false;
        while (transform.localPosition.x > 2.1f && enemyHP > 0.0f)
        {
            yield return null;
        }
        gameManager.board.ToggleFrontFade(true);
        gameManager.topPanel.ToggleModulesSelectable(false);
        TogglePause();
        gameManager.board.SuppressBoard();
        yield return StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(whichMessage, ""));
        TogglePause();
        enemyHurtable = true;
        gameManager.board.ToggleFrontFade(false);
        gameManager.topPanel.ToggleModulesSelectable(true);
    }

    private IEnumerator FlyFlat()
    {
        yield return new WaitForSeconds(2.0f);
        movementY = 0.0f;
    }

    public void SetMicro()
    {
        microEnemy = true;
    }

    public void TogglePause()
    {
        pause = !pause;
    }

    //void OnMouseDown()
    //{
    //    EnemyTakeDamage(900.0f, 1.0f, false);
    //}

    void Update ()
    {
        deltaTime += Time.deltaTime;
        nextAttack += Time.deltaTime;

        //move enemy forward until they 'hit' player
        if (deltaTime > 0.013f)
        {
            if (!pause)
            {
                float locationX = transform.localPosition.x;
                float blackHolePull = space.blackHolePull * blackHoleMultiplier;
                if (locationX > stopDistance)
                {
                    if (blackHolePull == 0.0f)
                    {
                        transform.localPosition -= new Vector3(enemySpeed * (deltaTime / 0.013f), movementY, 0.0f);
                    }
                    else
                    {
                        if (locationX < 1.9f)
                        {
                            transform.localPosition -= new Vector3((enemySpeed - blackHolePull) * (deltaTime / 0.013f), movementY, 0.0f);
                        }
                        else
                        {
                            transform.localPosition -= new Vector3(enemySpeed * (deltaTime / 0.013f), movementY, 0.0f);
                        }
                    }
                    if (enemyID == 4 || enemyID == 111 || enemyID == 113)
                    {
                        if (!dontInterrupt)
                        {
                            animator.speed = 0.0f;
                        }
                    }
                }
                else 
                {
                    if (blackHolePull > 0.0f)
                    {
                        transform.localPosition += new Vector3(blackHolePull, 0.0f, 0.0f);
                    }
                    if (nextAttack > enemyAttackRate)
                    {
                        EnemyAttack();
                        nextAttack = 0.0f;
                    }
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

    void OnCollisionEnter(Collision col)
    {
        if ((enemyID == 11 || enemyID == 50) && col.gameObject.tag == "Ship")
        {
            prefabPools.CreateExplosion(col.transform, 0.5f);
            ship.PlayerTakeDamage(enemyDamage);
            gameManager.ChangeFunds(Mathf.CeilToInt(moneyReward));
            space.EnemyDefeated(microEnemy);
            space.DestroyEnemy(this);
        }
    }

    public void ActivateDamageOverTime(float damage, int intensity, int dotType)
    {
        StartCoroutine(DamageOverTime(damage, intensity, dotType));
    }

    IEnumerator DamageOverTime(float damage, int intensity, int dotType)
    {
        currentDots++;
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
            if (damage >= enemyHP && enemyID == 8)
            {
                gameManager.missionTracker.IncrementMissionProgress(9, 1);
            }
            yield return new WaitForSeconds(waitPeriod);
            EnemyTakeDamage(damage, 0.0f, false);
        }
        currentDots--;
        if (currentDots <= 0)
        {
            spriteEnemyHPBar.color = new Color(0.6f, 0.0f, 0.0f);
        }
    }

    public void EnemyTakeDamage(float damageValue, float pushBack, bool secondary)
    {
        if (enemyHP > 0 && transform != null && enemyHurtable)
        {
            space.CreateTextDamage(damageValue, transform.localPosition.x, transform.localPosition.y, true);
            if (enemyID == 13 || enemyID == 103 || enemyID == 113)
            {
                if (enemyHPShield > 0.0f)
                {
                    damageValue = DecreaseEnemyShield(damageValue, secondary);
                }
            }
            enemyHP -= damageValue;

            if (enemyHP <= 0)
            {
                //give money reward
                int moneyRewardInt = Mathf.CeilToInt(moneyReward);
                gameManager.board.CreateMatchText(4, moneyRewardInt);
                gameManager.ChangeFunds(moneyRewardInt);
                gameManager.missionTracker.IncrementMissionProgress(21, moneyRewardInt);

                if (enemyID > 90)
                {
                    StartCoroutine(TriggerBossExplosion());
                }
                else
                {
                    prefabPools.CreateExplosion(transform, 0.33f);
                    if (enemyID == 11 || enemyID == 50)
                    {
                        gameManager.missionTracker.IncrementMissionProgress(10, 1);
                    }
                    space.EnemyDefeated(microEnemy);
                    space.DestroyEnemy(this);
                }
            }
            else
            {
                transform.localPosition += new Vector3((0.25f * pushBack), 0, 0);
                AdjustHPBar();
            }
        }
    }

    IEnumerator TriggerBossExplosion()
    {
        ship.ToggleKillable(false);
        TogglePause();
        gameManager.board.ToggleBoardSelectable(false);
        gameManager.board.ToggleFrontFade(false);

        prefabPools.CreateExplosion(transform, 0.33f);
        yield return new WaitForSeconds(0.3f);
        prefabPools.CreateExplosion(transform, 0.67f);
        yield return new WaitForSeconds(0.3f);
        prefabPools.CreateExplosion(transform, 0.9f);
        yield return new WaitForSeconds(0.45f);
        prefabPools.CreateExplosion(transform, 1.0f);
        yield return new WaitForSeconds(0.5f);

        if (enemyID == 113)
        {
            //trigger ending
            gameManager.missionTracker.IncrementMissionProgress(27, 1);
            transformProjectileOrigin.gameObject.SetActive(false);
            yield return StartCoroutine(space.TriggerEnding(transform.localPosition.x));
            transformProjectileOrigin.gameObject.SetActive(true);
        }
        space.EnemyDefeated(microEnemy);
        space.DestroyEnemy(this);
    }

    void AdjustHPBar()
    {
        enemyHPBar.localScale = new Vector3((enemyHP / enemyMaxHP), 1.0f, 1.0f);
    }

    void AdjustHPShieldBar()
    {
        enemyHPShieldBar.localScale = new Vector3((enemyHPShield / (enemyMaxHP * 1.2f)), 1.0f, 1.0f);
    }

    float DecreaseEnemyShield(float damageValue, bool secondary)
    {
        enemyHPShield -= damageValue;
        if (!secondary)
        {
            damageValue = Mathf.Max(0.0f, damageValue - (enemyHPShield + damageValue));
        }
        enemyHPShield = Mathf.Max(0.0f, enemyHPShield);
        AdjustHPShieldBar();

        if (enemyHPShield <= 0.0f)
        {
            enemyHPShieldBar.gameObject.SetActive(false);
            spriteShield.enabled = false;
        }

        return damageValue;
    }

    void EnemyAttack()
    {
        Enemy enemy;

        switch (enemyID)
        {
            case 0: //melee
                ship.PlayerTakeDamage(enemyDamage);
                break;
            case 1: //laser
                PopProjectileOffStack(0.08f, "EnemyProjectileLaser", transform.localPosition.x - 0.2f, transformProjectileOrigin.localPosition.y);
                break;
            case 2: //double rocket
                //randomNum = Random.Range(0, 2);
                //for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                //{
                PopProjectileOffStack(0.017f, "EnemyProjectileRocket", transform.localPosition.x - 0.25f, transformProjectileOrigin.localPosition.y - 0.36f + Random.Range(0, 2) * 0.72f);
                //}
                break;
            case 3: //fire
                PopProjectileOffStack(0.08f, "EnemyProjectileFire", transform.localPosition.x - 0.15f, transformProjectileOrigin.localPosition.y);
                break;
            case 4: //charge attack
                animator.speed = 1.0f;
                if (!gameManager.audioManager.audioSource.isPlaying)
                {
                    //gameManager.audioManager.PlaySound(7);
                }
                break;
            case 5: //bomb
                PopProjectileOffStack(0.12f, "EnemyProjectileBomb", transform.localPosition.x, transformProjectileOrigin.localPosition.y);
                break;
            case 6: //energy drain
                PopProjectileOffStack(0.12f, "EnemyProjectileDrain", transform.localPosition.x, transformProjectileOrigin.localPosition.y);
                break;
            case 7: //swarm
                //enemyPrefab = Resources.Load("Prefabs/Enemy/Enemy15") as GameObject;
                float posY = Random.Range(-0.15f, 0.15f);
                //instantiatedObject = gameManager.InstantiateObject(enemyPrefab, space.transform, transform.localPosition.x - 0.2f, transformProjectileOrigin.localPosition.y + posY, 0.0f, 1.0f, 1.0f, 1.0f);
                //enemy = instantiatedObject.GetComponent<Enemy>();
                //enemy.space = space;
                enemy = prefabPools.PopEnemyGnat();
                if (enemy != null)
                {
                    enemy.transform.parent = space.transform;
                    enemy.transform.localPosition = new Vector2(transform.localPosition.x - 0.2f, transformProjectileOrigin.localPosition.y + posY);
                    enemy.gameObject.SetActive(true);
                    enemy.SpawnEnemy(space.topPanel.currentThreat, whichLevel);
                    space.enemy.Add(enemy);
                }
                break;
            case 8: //acid attack
                PopProjectileOffStack(0.04f, "EnemyProjectileAcid", transform.localPosition.x - 0.15f, transformProjectileOrigin.localPosition.y);
                break;
            case 9: //weaken icons
                //PopProjectileOffStack(0.06f, "EnemyProjectileLaser", transform.localPosition.x - 0.4f, transformProjectileOrigin.localPosition.y);
                //ActivateHalfEffect(1);
                PopProjectileOffStack(0.12f, "EnemyProjectileWeaken", transform.localPosition.x, transformProjectileOrigin.localPosition.y);
                break;
            case 10: //single rocket
                PopProjectileOffStack(0.02f, "EnemyProjectileRocket", transform.localPosition.x - 0.25f, transformProjectileOrigin.localPosition.y);
                break;
            case 11: //Exploder
                break;
            case 12: //lock
                randomNum = (randomNum + 1) % 4;
                if (randomNum < 3)
                {
                    PopProjectileOffStack(0.12f, "EnemyProjectileLock", transform.localPosition.x, transformProjectileOrigin.localPosition.y);
                }
                PopProjectileOffStack(0.05f, "EnemyProjectileLaser", transform.localPosition.x - 0.15f, transformProjectileOrigin.localPosition.y);
                break;
            case 13: //shield
                PopProjectileOffStack(0.14f, "EnemyProjectileLaser", transform.localPosition.x - 0.3f, transformProjectileOrigin.localPosition.y);
                break;
            case 14: //ghost
                randomNum = (randomNum + 1) % 4;
                if (randomNum == 1)
                {
                    if (!animator.enabled)
                    {
                        animator.enabled = true;
                    }
                    animator.Play(0);
                }
                //for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                //{
                PopProjectileOffStack(0.025f, "EnemyProjectileRocket", transform.localPosition.x + 0.15f, transformProjectileOrigin.localPosition.y - 0.1f + Random.Range(0, 2) * 0.2f);
                //}
                break;
            case 15: //gnat
                ship.PlayerTakeDamage(enemyDamage);
                break;
            case 100: //laser (rainbow laser)
                PopProjectileOffStack(0.059f, "EnemyProjectileLaser", transform.localPosition.x - 0.55f, transformProjectileOrigin.localPosition.y + 0.25f);
                break;
            case 101: //rockets
                randomNum = Random.Range(0, 2);
                for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                {
                    PopProjectileOffStack(0.015f, "EnemyProjectileRocket", transform.localPosition.x - (0.27f * randomNum) - 0.04f, transformProjectileOrigin.localPosition.y - (0.55f - 0.21f * randomNum) + projectileNum * (1.11f - 0.42f * randomNum));
                }
                break;
            case 102: //bomb
                randomNum = Random.Range(0, 2);
                for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                {
                    PopProjectileOffStack(0.0085f, "EnemyProjectileBomb", transform.localPosition.x + 0.5f * randomNum, transformProjectileOrigin.localPosition.y - 0.38f + projectileNum * 0.76f);
                }
                break;
            case 103: //shield
                PopProjectileOffStack(0.13f, "EnemyProjectileLaser", transform.localPosition.x - 0.15f, transformProjectileOrigin.localPosition.y + 0.39f);
                break;
            case 104: //acid
                PopProjectileOffStack(0.04f, "EnemyProjectileAcid", transform.localPosition.x - 0.15f, transformProjectileOrigin.localPosition.y);
                break;
            case 105: //creates exploders
                enemy = prefabPools.PopEnemyExploder();
                if (enemy != null)
                {
                    enemy.transform.parent = space.transform;
                    enemy.transform.localPosition = new Vector2(transform.localPosition.x + 0.3f, 0.0f);
                    enemy.gameObject.SetActive(true);
                    enemy.SpawnEnemy(100.0f, whichLevel);
                    space.enemy.Add(enemy);
                }
                //enemyPrefab = Resources.Load("Prefabs/Enemy/Boss5Projectile") as GameObject;
                //instantiatedObject = gameManager.InstantiateObject(enemyPrefab, space.transform, transform.localPosition.x + 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
                //enemy = instantiatedObject.GetComponent<Enemy>();
                //enemy.SetMicro();
                //enemy.space = space;
                break;
            case 106: //weaken icons
                randomNum = (randomNum + 1) % 4;
                if (randomNum == 1)
                {
                    ActivateHalfEffect(10);
                }
                for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                {
                    PopProjectileOffStack(0.039f, "EnemyProjectileLaser", transform.localPosition.x + 0.5f, transformProjectileOrigin.localPosition.y - 0.33f + projectileNum * 0.66f);
                }
                break;
            case 107: //swarm
                posY = Random.Range(-0.15f, 0.15f);
                enemy = prefabPools.PopEnemyGnat();
                if (enemy != null)
                {
                    enemy.transform.parent = space.transform;
                    enemy.transform.localPosition = new Vector2(transform.localPosition.x - 0.2f, transformProjectileOrigin.localPosition.y + posY);
                    enemy.gameObject.SetActive(true);
                    enemy.SpawnEnemy(space.topPanel.currentThreat, whichLevel);
                    space.enemy.Add(enemy);
                }
                break;
            case 108: //lock
                randomNum = (randomNum + 1) % 2;
                if (randomNum == 1)
                {
                    for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                    {
                        PopProjectileOffStack(0.05f, "EnemyProjectileLaser", transform.localPosition.x - 0.15f, transformProjectileOrigin.localPosition.y - 0.3f + projectileNum * 0.6f);
                    }
                }
                PopProjectileOffStack(0.12f, "EnemyProjectileLock", transform.localPosition.x, transformProjectileOrigin.localPosition.y);
                break;
            case 109: //fire
                PopProjectileOffStack(0.085f, "EnemyProjectileFire", transform.localPosition.x - 0.48f, transformProjectileOrigin.localPosition.y + 0.36f);
                break;
            case 110: //energy drain
                PopProjectileOffStack(0.12f, "EnemyProjectileDrain", transform.localPosition.x, transformProjectileOrigin.localPosition.y);
                break;
            case 111: //charge attack
                animator.speed = 1.0f;
                break;
            case 112: //ghost 
                randomNum = (randomNum + 1) % 4;
                if (randomNum == 1)
                {
                    if (!animator.enabled)
                    {
                        animator.enabled = true;
                    }
                    animator.Play(0);
                }
                for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                {
                    if (randomNum % 2 == 1)
                    {
                        PopProjectileOffStack(0.05f, "EnemyProjectileLaser", transform.localPosition.x - 0.65f, transformProjectileOrigin.localPosition.y - 0.1f + projectileNum * 0.2f);
                    }
                    else
                    {
                        PopProjectileOffStack(0.05f, "EnemyProjectileLaser", transform.localPosition.x + 0.35f, transformProjectileOrigin.localPosition.y - 0.821f + projectileNum * 1.642f);
                    }
                }
                break;
            case 113: //all
                //give shield, charge attack, bomb, fire, rockets, exploders, lock, maybe acid
                randomNum = Random.Range(0, 7);


                if (dontInterrupt == false)
                {
                    animator.speed = 1.0f;
                }

                if (animator.speed == 0.0f)
                {
                    switch (randomNum)
                    {
                        case 0: //charge attack
                            animator.Play(0);
                            animator.speed = 1.0f;
                            dontInterrupt = false;
                            break;
                        case 1: //bomb
                            for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                            {
                                PopProjectileOffStack(0.0085f, "EnemyProjectileBomb", transform.localPosition.x + 0.2f, transformProjectileOrigin.localPosition.y - 0.78f + projectileNum * 1.56f);
                            }
                            break;
                        case 2: //fire
                            for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                            {
                                PopProjectileOffStack(0.085f, "EnemyProjectileFire", transform.localPosition.x - 0.48f, transformProjectileOrigin.localPosition.y - 0.36f + projectileNum * 0.72f);
                            }
                            break;
                        case 3: //rockets
                            for (int projectileNum = 0; projectileNum < 2; projectileNum++)
                            {
                                PopProjectileOffStack(0.013f, "EnemyProjectileRocket", transform.localPosition.x - 0.13f, transformProjectileOrigin.localPosition.y - 0.495f + projectileNum * 0.99f);
                            }
                            break;
                        case 4: //exploder
                            enemy = prefabPools.PopEnemyExploder();
                            if (enemy != null)
                            {
                                enemy.transform.parent = space.transform;
                                enemy.transform.localPosition = new Vector2(transform.localPosition.x + 0.3f, 0.0f);
                                enemy.gameObject.SetActive(true);
                                enemy.SpawnEnemy(100.0f, whichLevel);
                                space.enemy.Add(enemy);
                            }
                            break;
                        case 5: //lock
                            PopProjectileOffStack(0.059f, "EnemyProjectileLaser", transform.localPosition.x - 0.15f, transformProjectileOrigin.localPosition.y);
                            for (int projectileNum = 0; projectileNum < 3; projectileNum++)
                            {
                                PopProjectileOffStack(0.12f, "EnemyProjectileLock", transform.localPosition.x, transformProjectileOrigin.localPosition.y);
                            }
                            break;
                        case 6: //drain energy
                            PopProjectileOffStack(0.12f, "EnemyProjectileDrain", transform.localPosition.x, transformProjectileOrigin.localPosition.y);
                            break;
                            //case 7: //acid

                            //    break;
                    }
                }
                    break;
            default:
                PopProjectileOffStack(0.12f, "EnemyProjectileRocket", transform.localPosition.x - 0.15f, transformProjectileOrigin.localPosition.y);
                break;
        }
    }

    void LateUpdate()
    {
        if (enemyID == 14 || enemyID == 112)
        {
            if (enabled)
            {
                if (transform.rotation.z != 0.0f)
                {
                    transform.rotation = Quaternion.identity;
                }
            }
        }
    }

    private void ToggleCollider()
    {
        boxCollider.enabled = !boxCollider.enabled;
        //transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void ChargeAttack()
    {
        gameManager.audioManager.PlaySound(8);
        if (enemyID != 113)
        {
            ship.PlayerTakeDamage(enemyDamage);
        }
        else
        {
            ship.PlayerTakeDamage(enemyDamage * 3);
        }
        dontInterrupt = true;
    }

    private void StartCharging()
    {
        if (enemyID != 113)
        {
            animator.Play(0);
            dontInterrupt = false;
        }
        else
        {
            animator.speed = 0.0f;
        }
    }

    void PopProjectileOffStack(float speed, string type, float positionX, float positionY)
    {
        EnemyProjectile poppedEnemyProjectile = prefabPools.PopEnemyProjectile();
        if (poppedEnemyProjectile != null)
        {
            poppedEnemyProjectile.enabled = true;
            if (enemyID != 113)
            {
                poppedEnemyProjectile.PopOffStack(enemyDamage, speed, type, positionX, positionY);
            }
            else
            {
                switch (type)
                { 
                    case "EnemyProjectileRocket":
                        poppedEnemyProjectile.PopOffStack(enemyDamage * 0.5f, speed, type, positionX, positionY);
                        break;
                    case "EnemyProjectileFire":
                        poppedEnemyProjectile.PopOffStack(enemyDamage * 0.045f, speed, type, positionX, positionY);
                        break;
                    case "EnemyProjectileBomb":
                        poppedEnemyProjectile.PopOffStack(enemyDamage * 0.5f, speed, type, positionX, positionY);
                        break;
                    default:
                        poppedEnemyProjectile.PopOffStack(enemyDamage, speed, type, positionX, positionY);
                        break;
                }
            }
        }
    }

    void ActivateHalfEffect(int quantity)
    {
        int randIconType = Random.Range(0, 7);
        Board board = space.gameManager.board;
        Color[] textColor = { new Color(1.0f, 0.72f, 0), Color.red, Color.green, new Color(0.91f, 0, 1.0f), new Color(1.0f, 0.56f, 1.0f), new Color(0.0f, 0.75f, 1.0f), Color.yellow };

        if (quantity < 10)
        {
            int attempts = 20;
            int randRow;
            int randColumn;
            Icon icon;
            do
            {
                randRow = Random.Range(0, Board.GridRows);
                randColumn = Random.Range(0, Board.GridColumns);
                icon = board.boardIcons[randColumn, randRow];
                attempts--;
            } while (icon.currentEffectMultiplier != 1.0f && attempts > 0);
            icon.ActivateHalfEffect();
            randIconType = icon.iconType;
        }
        else
        {
            for (int column = 0; column < Board.GridColumns; column++)
            {
                for (int row = 0; row < Board.GridRows; row++)
                {
                    if (board.boardIcons[column, row].iconType == randIconType)
                    {
                        board.boardIcons[column, row].ActivateHalfEffect();
                    }
                }
            }
        }

        spriteRenderer.color = textColor[randIconType];
        spriteRenderer2.color = textColor[randIconType];
    }

    public void MicroPushOnStack()
    {
        transform.parent = prefabPools.transform;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        if (enemyID == 15)
        {
            animatorGnat.enabled = false;
            prefabPools.stackEnemyGnat.Push(this);
        }
        else
        {
            prefabPools.stackEnemyExploder.Push(this);
        }
        gameObject.SetActive(false);
    }

    public void SetColor(Color projectileColor)
    {
        spriteRenderer.color = projectileColor;
        spriteRenderer2.color = projectileColor;
    }
}