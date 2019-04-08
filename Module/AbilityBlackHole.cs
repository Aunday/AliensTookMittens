using UnityEngine;
using System.Collections;

public class AbilityBlackHole : MonoBehaviour
{
    public Ability ability;
    private GameManager gameManager;
    private float timeRemaining = 0.0f;
    private float pullPower;
    private Space space;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private Transform transformSpace;
    private Transform transformPool;

    public void StartSetup ()
    {
        gameManager = ability.gameManager;
        space = gameManager.space;
        transformSpace = space.transform;
        transformPool = gameManager.prefabPools.transform;
	}

    void Update ()
    {
        timeRemaining -= Time.deltaTime;
        //Enemy enemy = space.enemy;
        //pull enemy back until they are on the black hole
        //if (deltaTime > 0.013f)
        //{
        //    if (enemy != null)
        //    {
        //        if (enemy.transform.position.x < transform.position.x)
        //        {
        //            enemy.transform.localPosition += new Vector3(pullPower, 0, 0);
        //        }
        //    }
        //    else if (gameManager.prefabPools.stackAsteroid.Count < 5)
        //    {
        //        //want to figure out how to pull the asteroids...but how to reference them...
        //    }
        //    deltaTime = 0.0f;
        //}

        if (timeRemaining <= 0.0f)
        {
            space.ChangeBlackHolePull(0.0f);
            DeactivateModule();
        }
    }

    public void ActivateModule()
    {
        //pullPower = gameManager.upgradeValues[38, PlayerStats.playerStats.upgradeStatus[38]];
        timeRemaining = gameManager.upgradeValues[23, PlayerStats.playerStats.upgradeStatus[23]];
        space.ChangeBlackHolePull(gameManager.upgradeValues[38, PlayerStats.playerStats.upgradeStatus[38]]);
        //gameManager.missionTracker.IncrementMissionProgress(19, 1);
        transform.parent = transformSpace;
        transform.localPosition = new Vector2(1.9f, 0.0f);
        spriteRenderer.enabled = true;
        animator.enabled = true;
        //animator.Play(0);
        //yield return new WaitForSeconds(gameManager.upgradeValues[23, PlayerStats.playerStats.upgradeStatus[23]]);
    }

    public void DeactivateModule()
    {
        transform.parent = transformPool;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        animator.enabled = false;
        spriteRenderer.enabled = false;
        ability.ToggleCollider(true);
        enabled = false;
    }
}