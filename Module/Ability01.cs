using UnityEngine;
using System.Collections;

public class Ability01 : MonoBehaviour
{
    public Ability ability;
    private float deltaTime = 0.0f;
    private float dealDamage = 0.0f;
    private float damage;
    public Transform laserPulse;
    public Transform laserBeam;
    public SpriteRenderer spriteRenderer1;
    public SpriteRenderer spriteRenderer2;
    public SpriteRenderer spriteRenderer3;
    public SpriteRenderer spriteRenderer4;
    public SpriteRenderer spriteRenderer5;
    public SpriteRenderer spriteRenderer6;
    public Animator animator1;
    public Animator animator2;
    public Animator animator3;
    public Animator animator4;
    public Animator animator5;
    public Animator animator6;
    public Space space;
    private PrefabPools prefabPools;
    private Transform transformSpace;
    private MissionTracker missionTracker;
    private int ticksRemaining;

    public void StartSetup ()
    {
        laserPulse.localPosition = new Vector3(6.0f, 0.0f, 0.0f);
        missionTracker = space.gameManager.missionTracker;
        prefabPools = space.gameManager.prefabPools;
        transformSpace = space.transform;
    }
	
	void Update () {
        deltaTime += Time.deltaTime;
        dealDamage += Time.deltaTime;

        if (deltaTime > 0.013f)
        {
            deltaTime = 0.0f;

            int currentEnemyQuantity = space.enemy.Count;
            if (currentEnemyQuantity > 0.0f)
            {
                Vector3 beamScale = laserBeam.localScale;

                currentEnemyQuantity--;
                Enemy enemy = space.enemy[currentEnemyQuantity];
                float enemyPosX = enemy.transform.localPosition.x;
                while (currentEnemyQuantity > 0)
                {
                    currentEnemyQuantity--;
                    if (space.enemy[currentEnemyQuantity].transform.localPosition.x <= enemyPosX)
                    {
                        enemy = space.enemy[currentEnemyQuantity];
                        enemyPosX = enemy.transform.localPosition.x;
                    }
                }

                if (beamScale.x >= (enemyPosX + 1.9f) / 5.9f)
                {
                    //Vector3 pulsePosition = laserPulse.localPosition;
                    if (dealDamage > 0.2f)
                    {
                        enemy.EnemyTakeDamage(damage, 0.0f, false);
                        missionTracker.IncrementMissionProgress(8, (int)damage);
                    }
                    laserBeam.localScale = new Vector3((enemyPosX + 1.9f) / 5.76f, 1.0f, 1.0f);
                    laserPulse.localPosition = new Vector3((enemyPosX + 1.4f) / 0.9f, laserPulse.localPosition.y, 0.0f);
                }
                else
                {
                    laserBeam.localScale = new Vector3(beamScale.x + 0.25f, 1.0f, 1.0f);
                    laserPulse.localPosition = new Vector3(6.0f, 0.0f, 0.0f);
                }
            }
            else if (prefabPools.stackAsteroid.Count < 5.0f)
            {
                bool findChild = true;
                int whichChild = transformSpace.childCount - 1;
                while (findChild)
                {
                    while (transformSpace.GetChild(whichChild).tag != "Asteroid" && whichChild > 1)
                    {
                        whichChild--;
                    }
                    if (whichChild > 1)
                    {
                        if (transformSpace.GetChild(whichChild).localPosition.x > -1.2f)
                        {
                            findChild = false;
                        }
                        else
                        {
                            whichChild--;
                        }
                    }
                    else
                    {
                        findChild = false;
                    }
                }
                if (whichChild > 1)
                {
                    Asteroid asteroid = transformSpace.GetChild(whichChild).GetComponent<Asteroid>();
                    Vector3 beamScale = laserBeam.localScale;
                    float enemyPosX = asteroid.transform.localPosition.x;
                    if (beamScale.x >= (enemyPosX + 1.9f) / 5.9f)
                    {
                        //Vector3 pulsePosition = laserPulse.localPosition;
                        if (dealDamage > 0.2f)
                        {
                            asteroid.EnemyTakeDamage(damage, 0.0f);
                            missionTracker.IncrementMissionProgress(8, (int)damage);
                        }
                        laserBeam.localScale = new Vector3((enemyPosX + 1.9f) / 5.76f, 1.0f, 1.0f);
                        laserPulse.localPosition = new Vector3((enemyPosX + 1.4f) / 0.9f, laserPulse.localPosition.y, 0.0f);
                    }
                    else
                    {
                        laserBeam.localScale = new Vector3(beamScale.x + 0.25f, 1.0f, 1.0f);
                        laserPulse.localPosition = new Vector3(6.0f, 0.0f, 0.0f);
                    }
                }
                else
                {
                    laserBeam.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    laserPulse.localPosition = new Vector3(6.0f, 0.0f, 0.0f);
                }
            }
            else
            {
                laserBeam.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                laserPulse.localPosition = new Vector3(6.0f, 0.0f, 0.0f);
            }
            if (dealDamage > 0.2f)
            {
                ticksRemaining--;
                dealDamage = 0.0f;
                if (ticksRemaining <= 0)
                {
                    DeactivateModule();
                }
            }
        }
    }

    public void ActivateModule()
    {
        transform.localPosition = new Vector2(0.0f, 0.0f);
        spriteRenderer1.enabled = true;
        spriteRenderer2.enabled = true;
        spriteRenderer3.enabled = true;
        spriteRenderer4.enabled = true;
        spriteRenderer5.enabled = true;
        spriteRenderer6.enabled = true;
        animator1.enabled = true;
        animator2.enabled = true;
        animator3.enabled = true;
        animator4.enabled = true;
        animator5.enabled = true;
        animator6.enabled = true;
        animator1.Play(0);
        animator2.Play(0);
        animator3.Play(0);
        animator4.Play(0);
        animator5.Play(0);
        animator6.Play(0);
        damage = space.gameManager.upgradeValues[19, PlayerStats.playerStats.upgradeStatus[19]];
        ticksRemaining = (int)(space.gameManager.upgradeValues[34, PlayerStats.playerStats.upgradeStatus[34]] * 5);
        //yield return new WaitForSeconds(4.0f);
    }

    public void DeactivateModule()
    {
        transform.localPosition = new Vector2(0.0f, 30.0f);
        animator1.enabled = false;
        animator2.enabled = false;
        animator3.enabled = false;
        animator4.enabled = false;
        animator5.enabled = false;
        animator6.enabled = false;
        spriteRenderer1.enabled = false;
        spriteRenderer2.enabled = false;
        spriteRenderer3.enabled = false;
        spriteRenderer4.enabled = false;
        spriteRenderer5.enabled = false;
        spriteRenderer6.enabled = false;
        ability.ToggleCollider(true);
        enabled = false;
    }
}