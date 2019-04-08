using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float projectileSpeed;
    private float projectileDamage;
    private string projectileType;
    private bool pause;
    private Board board;
    private GameManager gameManager;
    private Ship ship;
    private int randRow = 0;
    private int randColumn = 0;
    private Vector3 randIconLoc = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 projectileMoveSpeed = new Vector3(0.0f, 0.0f, 0.0f);
    private bool reflected; 
    private bool redirected; 
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer spriteRenderer2;
    public PrefabPools prefabPools;
    public BoxCollider boxCollider;
    private Transform transformSpace;
    private Transform transformPool;
    private Transform transformBoard;
    public Animator animator;
    public bool normalProjectile{ get; private set; }

    public void StartSetup()
    {
        gameManager = prefabPools.gameManager;
        board = gameManager.board;
        ship = gameManager.ship;
        transformSpace = gameManager.space.transform;
        transformPool = prefabPools.transform;
        transformBoard = board.transform;
        //PushOnStack();
    }

    public void PushOnStack()
    {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        spriteRenderer2.enabled = false;
        animator.enabled = false;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        transform.parent = transformPool;
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        spriteRenderer.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        prefabPools.stackEnemyProjectile.Push(this);
        enabled = false;
    }

    public void PopOffStack(float enemyProjectileDamage, float enemyProjectileSpeed, string enemyProjectileType, float enemyLocationX, float enemyLocationY)
    {
        deltaTime = 0.0f;
        projectileType = enemyProjectileType;
        reflected = false;
        redirected = false;
        pause = false;
        //if (projectileType != "EnemyProjectileWeakenBoss")
        //{
        gameObject.tag = projectileType;
        //}
        //else
        //{
        //    gameObject.tag = "EnemyProjectileWeaken";
        //}
        projectileDamage = enemyProjectileDamage;
        projectileSpeed = enemyProjectileSpeed;
        transform.parent = transformSpace;

        switch (projectileType)
        {
            case "EnemyProjectileLaser":
                gameManager.audioManager.PlaySound(1);
                break;
            case "EnemyProjectileFire":
                gameManager.audioManager.PlaySound(15);
                break;
            default:
                break;
        }

        if (projectileType != "EnemyProjectileBomb" && projectileType != "EnemyProjectileLock" && projectileType != "EnemyProjectileDrain" && projectileType != "EnemyProjectileWeaken")
        {
            transform.localPosition = new Vector2(enemyLocationX, enemyLocationY);
            spriteRenderer.color = Color.white;
            if (projectileSpeed != 0.059f && projectileSpeed != 0.039f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("ArmamentFaces/" + projectileType);
            }
            else
            {
                //if (projectileType == "EnemyProjectileWeakenBoss")
                //{
                //    spriteRenderer.sprite = Resources.Load<Sprite>("ArmamentFaces/EnemyProjectileWhite");
                //    spriteRenderer.color = Color.red;
                //    projectileType = "EnemyProjectileWeaken";
                //}
                //else
                //{
                    spriteRenderer.sprite = Resources.Load<Sprite>("ArmamentFaces/EnemyProjectileLaserRainbow");
                //}
            }
            normalProjectile = true; 
            projectileSpeed /= 0.014f;
            spriteRenderer.sortingOrder = 38;

            if (projectileType == "EnemyProjectileRocket")
            {
                spriteRenderer2.enabled = true;
                animator.enabled = true;
                animator.SetBool("isRocket", true);
            }
            else if (projectileType == "EnemyProjectileFire")
            {
                animator.enabled = true;
                animator.SetBool("isRocket", false);
            }
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("ArmamentFaces/EnemyProjectileWhite");
            projectileSpeed /= 0.014f;
            if (projectileType == "EnemyProjectileBomb")
            {
                transform.localPosition = new Vector2(enemyLocationX, enemyLocationY);
                spriteRenderer.color = Color.red;
                transform.parent = transformBoard;

                bool iconFound = false;
                while (!iconFound)
                {
                    randRow = Random.Range(0, Board.GridRows);
                    randColumn = Random.Range(0, Board.GridColumns);
                    if (board.boardIcons[randRow, randColumn].iconBomb == null)
                    {
                        iconFound = true;
                    }
                }

                randIconLoc = new Vector3(randRow * 0.71f, randColumn * 0.71f, 0.0f);
            }
            else if (projectileType == "EnemyProjectileLock")
            {
                transform.localPosition = new Vector2(enemyLocationX, enemyLocationY);
                spriteRenderer.color = Color.black;
                transform.parent = transformBoard;
                randRow = Random.Range(0, Board.GridRows);
                randColumn = Random.Range(0, Board.GridColumns);
                randIconLoc = new Vector3(randRow * 0.71f, randColumn * 0.71f, 0.0f);
            }
            else if (projectileType == "EnemyProjectileDrain")
            {
                int randomNum = Random.Range(0, 3);
                float drainAmount = Mathf.Ceil(projectileDamage / 10.0f);
                if (randomNum == 0)
                {
                    transform.localPosition = new Vector2(-2.47f, 1.85f);
                    spriteRenderer.color = Color.magenta;
                    board.CreateMatchText(3, -(int)drainAmount);
                }
                else if (randomNum == 1)
                {
                    transform.localPosition = new Vector2(-0.81f, 1.85f);
                    spriteRenderer.color = Color.green;
                    board.CreateMatchText(2, -(int)drainAmount);
                }
                else
                {
                    transform.localPosition = new Vector2(0.87f, 1.85f);
                    spriteRenderer.color = Color.yellow;
                    board.CreateMatchText(6, -(int)drainAmount);
                }
                gameManager.topPanel.DecreaseEnergy(drainAmount, randomNum);

                randIconLoc = new Vector3(enemyLocationX, enemyLocationY, 0.0f);
            }
            else
            {
                int randIconType = Random.Range(0, 7);
                Color[] projectileColor = { new Color(1.0f, 0.72f, 0), Color.red, Color.green, new Color(0.91f, 0, 1.0f), new Color(1.0f, 0.56f, 1.0f), new Color(0.0f, 0.75f, 1.0f), Color.yellow };
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
                spriteRenderer.color = projectileColor[randIconType];

                //transform.parent = transformBoard;
                transform.position = icon.transform.position;

                randIconLoc = new Vector3(enemyLocationX, enemyLocationY, 0.0f);
            }
            normalProjectile = false;
            projectileMoveSpeed = new Vector3((transform.localPosition.x - randIconLoc.x) / 120.0f, (transform.localPosition.y - randIconLoc.y) / 120.0f, 0.0f);
            spriteRenderer.sortingOrder = 84;
        }
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
    }

    public void SetReflected()
    {
        if (!reflected)
        {
            transform.Rotate(0.0f, 180.0f, 0.0f);
            transform.localPosition += new Vector3(0.28f, 0.0f, 0.0f);
            projectileSpeed = -Mathf.Abs(projectileSpeed);
            reflected = true;
            projectileDamage *= gameManager.upgradeValues[39, PlayerStats.playerStats.upgradeStatus[39]];
        }
    }

    public void SetRedirected()
    {
        if (!redirected)
        {
            ship.moduleRedirect.ProjectileRedirected();
            spriteRenderer.sortingOrder = 84;
            transform.parent = transformBoard;
            randRow = Random.Range(0, Board.GridRows);
            randColumn = Random.Range(0, Board.GridColumns);
            randIconLoc = new Vector3(randRow * 0.71f, randColumn * 0.71f, 0.0f);

            //rotate to aim at icon
            float xRate = (transform.localPosition.x - randIconLoc.x);
            float yRate = (transform.localPosition.y - randIconLoc.y);
            float travelRateX = (xRate / (Mathf.Abs(xRate) + Mathf.Abs(yRate))) * 0.25f;
            float travelRateY = (yRate / (Mathf.Abs(xRate) + Mathf.Abs(yRate))) * 0.25f;
            float rotateAngle = Mathf.Atan(yRate / xRate);
            rotateAngle = Mathf.Rad2Deg * rotateAngle;
            if (transform.localPosition.x < randIconLoc.x)
            {
                rotateAngle += 180.0f;
            }
            transform.Rotate(0.0f, 0.0f, rotateAngle);
            //transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotateAngle);

            redirected = true;
            normalProjectile = false;
            projectileMoveSpeed = new Vector3(travelRateX * projectileSpeed, travelRateY * projectileSpeed, 0.0f);
        }
    }

    public void TogglePause()
    {
        pause = !pause;
    }

    void Update ()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime > 0.013f)
        {
            if (!pause)
            {
                if (normalProjectile)
                {
                    transform.localPosition -= new Vector3(projectileSpeed * deltaTime, 0.0f, 0.0f);
                    deltaTime = 0.0f;
                    //DESTROY if go off screen //may be able to move this to effect both sides of if
                    if (transform.localPosition.x < -3.5f || transform.localPosition.x > 3.5f)
                    {
                        PushOnStack();
                    }
                }
                else
                {
                    transform.localPosition -= new Vector3(projectileMoveSpeed.x * deltaTime, projectileMoveSpeed.y * deltaTime, 0.0f);

                    if (projectileType == "EnemyProjectileWeaken")
                    {
                        if (transform.localPosition.y >= randIconLoc.y && transform.parent == transformSpace)
                        {
                            gameManager.audioManager.PlaySound(2);
                            normalProjectile = true;
                            projectileSpeed *= 0.65f;
                            gameManager.space.enemyArray[9].SetColor(spriteRenderer.color);
                            deltaTime = 0.0f;
                        }
                    }
                    else if (transform.localPosition.y <= randIconLoc.y)
                    {

                        if (board.boardIcons[randRow, randColumn].iconBomb == null && projectileType == "EnemyProjectileBomb") //only activate if it is not already
                        {
                            board.boardIcons[randRow, randColumn].ActivateBomb(projectileDamage, randRow, randColumn);
                        }
                        else if (!board.boardIcons[randRow, randColumn].spriteLock.enabled && projectileType == "EnemyProjectileLock")
                        {
                            board.boardIcons[randRow, randColumn].ActivateLock(15, randRow, randColumn);
                        }
                        else if (transform.parent == transformSpace && projectileType == "EnemyProjectileDrain")
                        {
                            gameManager.audioManager.PlaySound(2);
                            normalProjectile = true;
                            projectileSpeed *= 0.65f;
                            deltaTime = 0.0f;
                        }
                        else
                        {
                            board.ModuleDestroyIcon(randRow, randColumn, 1);
                        }
                        if (!normalProjectile)
                        {
                            PushOnStack();
                        }
                    }
                }
            }
        }
	}

    void OnCollisionEnter(Collision col)
    {
        GameObject objectCollision = col.gameObject;
        string collisionTag = objectCollision.tag;

        if (collisionTag == "Ship")
        {
            if (!ship.CheckReflection() && !ship.CheckRedirection())
            {
                if (projectileType == "EnemyProjectileLaser")
                {
                    prefabPools.CreateExplosion(transform, 0.16f);
                    ship.PlayerTakeDamage(projectileDamage);
                }
                else if (projectileType == "EnemyProjectileRocket")
                {
                    prefabPools.CreateExplosion(transform, 0.27f);
                    ship.PlayerTakeDamage(projectileDamage);
                    gameManager.audioManager.PlaySound(6);
                }
                else if (projectileType == "EnemyProjectileAcid")
                {
                    ship.ActivateAcid(5);
                    //for (int curIcon = 0; curIcon < 2; curIcon++)
                    //{
                    randRow = Random.Range(0, Board.GridRows);
                    randColumn = Random.Range(0, Board.GridColumns);
                    board.boardIcons[(randRow), (randColumn)].ActivateAcid(projectileDamage, 5);
                    //}
                }
                else if (projectileType == "EnemyProjectileDrain")
                {
                    if (normalProjectile)
                    {
                        prefabPools.CreateExplosion(transform, 0.22f);
                        ship.PlayerTakeDamage(projectileDamage);
                    }
                }
                else if (projectileType == "EnemyProjectileWeaken")
                {
                    if (normalProjectile)
                    {
                        prefabPools.CreateExplosion(transform, 0.22f);
                        ship.PlayerTakeDamage(projectileDamage);
                    }
                }
                else if (projectileType == "EnemyProjectileFire")
                {
                    randRow = Random.Range(0, Board.GridRows);
                    randColumn = Random.Range(0, Board.GridColumns);
                    for (int curRow = 0; curRow < 3; curRow++)
                    {
                        for (int curCol = 0; curCol < 3; curCol++)
                        {
                            int randomNumber = Random.Range(0, 3);

                            if (randomNumber > 0)
                            {
                                board.boardIcons[(randRow + curRow) % Board.GridRows, (randColumn + curCol) % Board.GridColumns].ActivateFire(projectileDamage);
                            }
                        }
                    }
                }
                if (normalProjectile)
                {
                    PushOnStack();
                }
            }
            else if (ship.CheckReflection() && normalProjectile)
            {
                if (projectileType == "EnemyProjectileRocket" || projectileType == "EnemyProjectileLaser" || projectileType == "EnemyProjectileFire" || projectileType == "EnemyProjectileAcid" || projectileType == "EnemyProjectileDrain" || projectileType == "EnemyProjectileWeaken")
                {
                    SetReflected();
                    gameManager.missionTracker.IncrementMissionProgress(11, 1);
                }
            }
            else if (normalProjectile)
            {
                if ((projectileType == "EnemyProjectileRocket" || projectileType == "EnemyProjectileLaser" || projectileType == "EnemyProjectileFire" || projectileType == "EnemyProjectileAcid" || projectileType == "EnemyProjectileDrain" || projectileType == "EnemyProjectileWeaken") && normalProjectile)
                {
                    SetRedirected();
                }
            }
        }
        else if (collisionTag == "Enemy")
        {
            if (reflected)
            {
                if (projectileType == "EnemyProjectileLaser")
                {
                    prefabPools.CreateExplosion(transform, 0.13f);
                    objectCollision.GetComponent<Enemy>().EnemyTakeDamage(projectileDamage, 1.0f, false);
                }
                else if (projectileType == "EnemyProjectileRocket")
                {
                    prefabPools.CreateExplosion(transform, 0.13f);
                    objectCollision.GetComponent<Enemy>().EnemyTakeDamage(projectileDamage, 1.2f, false);
                }
                else if (projectileType == "EnemyProjectileAcid")
                {
                    objectCollision.GetComponent<Enemy>().ActivateDamageOverTime(projectileDamage, 5, 0);
                }
                else if (projectileType == "EnemyProjectileFire")
                {
                    objectCollision.GetComponent<Enemy>().ActivateDamageOverTime(projectileDamage, 5, 1);
                }
                else if (projectileType == "EnemyProjectileDrain")
                {
                    prefabPools.CreateExplosion(transform, 0.2f);
                    objectCollision.GetComponent<Enemy>().EnemyTakeDamage(projectileDamage, 1.2f, false);
                }
                else
                {
                    prefabPools.CreateExplosion(transform, 0.2f);
                    objectCollision.GetComponent<Enemy>().EnemyTakeDamage(projectileDamage, 1.2f, false);
                }
               PushOnStack();
            }
        }
        else if (collisionTag == "ArmamentSecondary" && normalProjectile)
        {
            gameManager.missionTracker.IncrementMissionProgress(3, 1);
            gameManager.prefabPools.CreateExplosion(transform, 0.16f);
            PushOnStack();
        }
        else if (collisionTag == "Asteroid")
        {
            if (projectileType == "EnemyProjectileLaser")
            {
                prefabPools.CreateExplosion(transform, 0.13f);
                objectCollision.GetComponent<Asteroid>().EnemyTakeDamage(projectileDamage, 0.0f);
            }
            else if (projectileType == "EnemyProjectileRocket")
            {
                prefabPools.CreateExplosion(transform, 0.13f);
                objectCollision.GetComponent<Asteroid>().EnemyTakeDamage(projectileDamage, 0.0f);
            }
            else if (projectileType == "EnemyProjectileAcid")
            {
                objectCollision.GetComponent<Asteroid>().ActivateDamageOverTime(projectileDamage, 5, 0);
            }
            else if (projectileType == "EnemyProjectileFire")
            {
                objectCollision.GetComponent<Asteroid>().ActivateDamageOverTime(projectileDamage, 5, 1);
            }
            else if (projectileType == "EnemyProjectileDrain")
            {
                prefabPools.CreateExplosion(transform, 0.2f);
                objectCollision.GetComponent<Asteroid>().EnemyTakeDamage(projectileDamage, 0.0f);
            }
            else
            {
                prefabPools.CreateExplosion(transform, 0.2f);
                objectCollision.GetComponent<Asteroid>().EnemyTakeDamage(projectileDamage, 0.0f);
            }
            PushOnStack();
        }
    }
}