using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Space : MonoBehaviour
{
    public GameManager gameManager;
    public SpriteRenderer spriteSpace;
    public List<Enemy> enemy = new List<Enemy>();
    public Enemy[] enemyArray { get; private set; }
    public GameObject bossWarningMessage;
    private TextMesh bossWarningMessageText;
    private Animator bossWarningMessageAnimator;
    public TopPanel topPanel;
    public GameObject mittensPrefab;
    private float enemySpawnTimer = 0;
    private int enemySpawnTime = 2;
    private int currentNumEnemies = 50;
    private int enemiesDefeated = 0;
    private int asteroidsDestroyed;
    public float blackHolePull { get; private set; }
    public int whichLevel { get; private set; }
    private int[] enemiesInLevel = new int[] { 4, 10, 10, 15, 15, 15, 20, 20, 20, 25, 25, 25, 25, 30 };
    private int[,] enemyIDRange = new int[,] { { 0, 1, 10 }, { 0, 1, 2 }, { 0, 2, 5 }, { 1, 5, 13 }, { 0, 13, 8 }, { 10, 8, 11 }, { 13, 11, 9 }, { 5, 9, 7 }, { 8, 7, 12 }, { 2, 12, 3 }, { 11, 3, 6 }, { 7, 6, 4 }, { 3, 4, 14 }, { 6, 4, 14 } };
    private Transform transformPrefabPools;

    public void StartSetup()
    {
        enemyArray = new Enemy[15];
        transformPrefabPools = gameManager.prefabPools.transform;
        bossWarningMessageText = bossWarningMessage.transform.GetChild(0).GetComponent<TextMesh>();
        bossWarningMessageAnimator = bossWarningMessage.GetComponent<Animator>();
    }

    public void NewLevel(int setLevel)
    {
        whichLevel = setLevel;
    }

    public void EnemyDefeated(bool microEnemy)
    {
        if (!microEnemy)
        {
            ChangeNumberOfEnemies(-1);

            //set threat to new point
            if (topPanel.currentThreat < 99.0f)
            {
                enemiesDefeated++;
                topPanel.IncreaseThreat(99.5f / enemiesInLevel[whichLevel]);
            }
            else if (topPanel.currentThreat >= 100.0f)
            {
                enemiesDefeated++;
                topPanel.IncreaseThreat(10.0f);
            }
            else
            {
                topPanel.IncreaseThreat(100.0f - topPanel.currentThreat);
                gameManager.EndLevel(true);
            }

            //start level break if appropriate
            if (enemiesDefeated % 5 == 0)
            {
                PlayerStats.playerStats.levelThreat[whichLevel] = topPanel.currentThreat;
                PlayerStats.playerStats.Save();
                if (Mathf.CeilToInt(topPanel.currentThreat) != 100.0f)
                {
                    asteroidsDestroyed = 0;
                    gameManager.ship.ResetDamageTaken();
                    LevelBreak(5);
                }
            }
        }
    }

    public void AsteroidDestroyed()
    {
        asteroidsDestroyed++;
        if (asteroidsDestroyed == 5)
        {
            gameManager.missionTracker.IncrementMissionProgress(17, 1);
        }

        if (bossWarningMessage.activeSelf)
        {
            ToggleSpaceMessage(true, false);
        }
    }

    public void ChangeBlackHolePull(float pullAmount)
    {
        blackHolePull = pullAmount;
    }

    public void DestroyEnemy(Enemy dyingEnemy)
    {
        int enemyID = dyingEnemy.enemyID;
        if (enemyID < 15)
        {
            enemyArray[enemyID].transform.parent = transformPrefabPools;
            enemyArray[enemyID].transform.localPosition = new Vector2(0.0f, 30.0f);
            enemyArray[enemyID].gameObject.SetActive(false);
        }
        else if (enemyID == 15 || enemyID == 50)
        {
            dyingEnemy.MicroPushOnStack();
        }
        else
        {
            Destroy(dyingEnemy.gameObject);
            Resources.UnloadUnusedAssets();
        }
        enemy.Remove(dyingEnemy);
    }

    public IEnumerator TriggerEnding(float positionX)
    {
        gameManager.board.ToggleBoardSelectable(false);
        topPanel.ToggleModulesSelectable(false);
        gameManager.optionsMenu.ToggleOptionMenuButton(false);
        GameObject mittens = gameManager.InstantiateObject(mittensPrefab, gameManager.ship.shipAnimation.transform, 3.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSeconds(0.4f);
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(99, "Meeeoooowwww")));
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "Mittens! You are alive!\nHmmm, it's a good thing\ncats can breathe in\nspace.\nGet over here!")));
        while (mittens.transform.localPosition.x > 2.0f)
        {
            mittens.transform.localPosition -= new Vector3(0.02f, 0.0f, 0.0f);
            gameManager.board.ToggleBoardSelectable(false);
            topPanel.ToggleModulesSelectable(false);
            yield return new WaitForSeconds(0.02f);
        }
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(99, "Meeeoooowwww")));
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "For how much effort I...\nwe... went through to find\nyou Mittens, you sure\ntake your sweet time.")));
        while (mittens.transform.localPosition.x > 0.0f)
        {
            mittens.transform.localPosition -= new Vector3(0.02f, 0.0f, 0.0f);
            yield return new WaitForSeconds(0.02f);
        }

        //activate scratch whirlwind
        mittens.GetComponent<Animator>().enabled = true;

        yield return new WaitForSeconds(2.0f);
        mittens.SetActive(false);
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(100, "Why did we want to\nsave you again?              \nJust kidding...\nWelcome back Mittens.\nWe missed you.\nI'll tell you about this\nperson that keeps trying\nto control our ship on\nout trip home.")));
        gameManager.optionsMenu.ToggleOptionMenuButton(true);
    }

    public IEnumerator CreateBoss()
    {
        ToggleEnemyCreation(false);
        ToggleSpaceMessage(true, true);

        yield return new WaitForSeconds(5.0f);
        if (PlayerStats.playerStats.bossMessageStatus[whichLevel])
        {
            yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(whichLevel, null)));
        }
        ToggleSpaceMessage(true, false);
        GameObject enemyPrefab = Resources.Load("Prefabs/Enemy/Boss" + whichLevel) as GameObject;
        GameObject instantiatedObject = gameManager.InstantiateObject(enemyPrefab, transform, 5.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        Enemy newEnemy = instantiatedObject.GetComponent<Enemy>();
        newEnemy.space = this;
        newEnemy.StartSetup();
        newEnemy.SpawnEnemy(topPanel.currentThreat, whichLevel);
        enemy.Add(newEnemy);
    }

    void ToggleSpaceMessage(bool whichMessage, bool whichToggle)
    {
        if (gameManager.ship.GetPlayerHealth() > 0.0f)
        {
            bossWarningMessage.SetActive(whichToggle);
            if (whichToggle)
            {
                if (whichMessage)
                {
                    bossWarningMessageText.text = "WARNING!     LARGE\nENTITY DETECTED";
                    bossWarningMessageText.color = new Color(1.0f, 0.0f, 0.0f, 0.1f);
                    bossWarningMessageAnimator.SetBool("whichMessage", true);
                }
                else
                {
                    bossWarningMessageText.text = "CHECKPOINT\nREACHED!";
                    bossWarningMessageText.color = new Color(1.0f, 0.67f, 0.0f, 0.1f);
                    bossWarningMessageAnimator.SetBool("whichMessage", false);
                }
            }
        }
    }

    public void StartLevel()
    {
        blackHolePull = 0.0f;
        currentNumEnemies = 0;
        enemiesDefeated = 0;
    }

    public void CleanSpaceImmediate()
    {
        gameManager.backgroundPlanet.transform.localPosition = new Vector2(3.5f, 0.0f);
        //backgroundPlanet.ResetPlanet();
        gameManager.backgroundPlanet.ToggleAnimation(false);
        spriteSpace.enabled = false;
        ToggleEnemyCreation(false);
        if (bossWarningMessage.activeSelf)
        {
            ToggleSpaceMessage(true, false);
        }
        for (int numChild = transform.childCount - 1; numChild > 1; numChild--)
        {
            Transform curChild = transform.GetChild(numChild);
            if (curChild != null)
            {
                switch (curChild.tag)
                {
                    case "Enemy":
                        //Destroy(curChild.gameObject);
                        DestroyEnemy(curChild.GetComponent<Enemy>());
                        break;
                    case "AbilityBlackHole":
                        curChild.GetComponent<AbilityBlackHole>().DeactivateModule();
                        break;
                    case "Asteroid":
                        curChild.GetComponent<Asteroid>().PushOnStack();
                        break;
                    case "EnemyProjectileRocket":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileLaser":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileAcid":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileBomb":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileFire":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileWeaken":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileDrain":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                }
            }
        }
    }

    public void CleanSpaceDelayed()
    {
        for (int numChild = transform.childCount - 1; numChild > 1; numChild--)
        {
            Transform curChild = transform.GetChild(numChild);
            if (curChild != null)
            {
                switch (curChild.tag)
                {
                    case "ArmamentPrimary":
                        curChild.GetComponent<ArmamentPrimary>().PushOnStack();
                        break;
                    case "ArmamentSecondary":
                        curChild.GetComponent<ArmamentSecondary>().PushOnStack();
                        break;
                    case "AbilityRocketBarrageRocket":
                        curChild.GetComponent<AbilityRocketBarrage>().PushOnStack();
                        break;
                    case "DamageText":
                        curChild.GetComponent<TextDamage>().PushOnStack();
                        break;
                    case "Explosion":
                        curChild.GetComponent<Explosion>().PushOnStack();
                        break;
                    case "ModuleMines":
                        curChild.GetComponent<ModuleMines>().PushOnStack();
                        break;
                }
            }
        }
        gameManager.backgroundPlanet.ToggleAnimation(false);
        gameObject.SetActive(false);
    }

    public void SpawnEnemy()
    {
        int enemyIndex;
        Enemy newEnemy;
        if (whichLevel < 13)
        {
            if (enemiesDefeated > 0)
            {
                int randInt = Random.Range(0, 100);
                if (randInt < 29)
                {
                    enemyIndex = enemyIDRange[whichLevel, 0];
                }
                else if (randInt < 58)
                {
                    enemyIndex = enemyIDRange[whichLevel, 1];
                }
                else
                {
                    enemyIndex = enemyIDRange[whichLevel, 2];
                }
            }
            else
            {
                enemyIndex = enemyIDRange[whichLevel, 2];
                //if (enemiesDefeated == 0)
                //{
                //    if (PlayerStats.playerStats.threatMessageStatus[whichLevel] == true)
                //    {
                //        //StartCoroutine(PlayDescriptionMessage(whichLevel));
                //    }
                //}
                //else
                //{
                //    if (PlayerStats.playerStats.threatMessageStatus[Mathf.Max(whichLevel - 1, 0)] == true)
                //    {
                //        //StartCoroutine(PlayDescriptionMessage(whichLevel - 1));
                //    }
                //}
            }
        }
        else
        {
            enemyIndex = Random.Range(0, 15);
        }
        if (enemyArray[enemyIndex] != null)
        {
            newEnemy = enemyArray[enemyIndex];
            newEnemy.transform.parent = transform;
        }
        else
        {
            newEnemy = CreateEnemy(enemyIndex);
        }
        //GameObject enemyPrefab = Resources.Load("Prefabs/Enemy/Enemy" + enemyIndex) as GameObject;
        //GameObject instantiatedObject = gameManager.InstantiateObject(enemyPrefab, transform, 3.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        //Enemy newEnemy = instantiatedObject.GetComponent<Enemy>();
        //newEnemy.space = this;

        newEnemy.SpawnEnemy(topPanel.currentThreat, whichLevel);
        enemy.Add(newEnemy);
        ChangeNumberOfEnemies(1);
    }

    public Enemy CreateEnemy(int enemyIndex)
    {
        GameObject enemyPrefab = Resources.Load("Prefabs/Enemy/Enemy" + enemyIndex) as GameObject;
        GameObject instantiatedObject = gameManager.InstantiateObject(enemyPrefab, transform, 3.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        Enemy newEnemy = instantiatedObject.GetComponent<Enemy>();
        newEnemy.space = this;
        newEnemy.StartSetup();
        enemyArray[enemyIndex] = newEnemy;

        return newEnemy;
    }

    //IEnumerator PlayDescriptionMessage(int whichMessage)
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    gameManager.board.ToggleFrontFade(true);
    //    yield return new WaitForSeconds(1.5f);
    //    enemy[0].TogglePause();
    //    yield return StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(whichMessage, ""));
    //    enemy[0].TogglePause();
    //    gameManager.board.ToggleFrontFade(false);
    //}

    private void LevelBreak(int qtyToCreate)
    {
        float currentThreat = topPanel.currentThreat;
        ChangeNumberOfEnemies(qtyToCreate * 10);
        //StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Checkpoint Reached"));

        ToggleSpaceMessage(false, true);

        for (int asteroidNumber = 0; asteroidNumber < qtyToCreate; asteroidNumber++)
        {
            float randLocX = Random.Range(3.8f, 6.5f);
            float randLocY = Random.Range(-0.8f, 0.8f);

            Asteroid poppedAsteroid = gameManager.prefabPools.PopAsteroid();
            if (poppedAsteroid != null)
            {
                poppedAsteroid.enabled = true;
                poppedAsteroid.PopOffStack(currentThreat, whichLevel, randLocX, randLocY);
            }
        }
    }

    public void ToggleEnemyCreation(bool whichToggle)
    {
        if (whichToggle)
        {
            ChangeNumberOfEnemies(-50);
        }
        else
        {
            ChangeNumberOfEnemies(50);
        }
    }

    public void CreateTextDamage(float damageAmount, float locationX, float locationY, bool takingDamage)
    {
        float randLocX = Random.Range(-0.3f, 0.3f);
        float randLocY = Random.Range(-0.3f, 0.3f);

        TextDamage textDamage = gameManager.prefabPools.PopTextDamage();
        if (textDamage != null)
        {
            textDamage.enabled = true;
            textDamage.PopOffStack(damageAmount, locationX + randLocX, locationY + randLocY, takingDamage);
        }
    }

    public int GetCurrentNumberOfEnemies()
    {
        return currentNumEnemies;
    }

    public void ChangeNumberOfEnemies(int value)
    {
        currentNumEnemies = Mathf.Max(0, currentNumEnemies + value);

        //currentNumEnemies += value;
    }

	void Update () 
    {
        if (currentNumEnemies == 0)
        {
            enemySpawnTimer += Time.deltaTime * 2.0f;
        }
        else
        {
            enemySpawnTimer += Time.deltaTime;
        }

        //if enough random time has passed, summon next enemy
        if (enemySpawnTimer > enemySpawnTime)
        {
            if (Mathf.FloorToInt(topPanel.currentThreat) == 99.0f && currentNumEnemies == 0)
            {
                StartCoroutine(CreateBoss());
            }
            else
            {
                if (currentNumEnemies == 0)
                {
                    enemySpawnTime = Random.Range(10, 16);
                    enemySpawnTimer = 0.0f;
                    SpawnEnemy();
                }
                else
                {
                    enemySpawnTimer -= 5.0f;
                }
            }
        }
	}
}