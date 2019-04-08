using UnityEngine;

public class ModuleSideDrones : MonoBehaviour
{
    public GameManager gameManager;
    private TopPanel topPanel;
    private Transform transformSpace;
    private PrefabPools prefabPools;
    public SpriteRenderer spriteSideDrone1;
    public SpriteRenderer spriteSideDrone2;
    public Animator animatorSideDrones;
    private float timer;
    private float timeBetweenShot;
    private float energyPerShot;
    private int damagePerShot;
    private bool outOfEnergy;
    private bool topDroneShooting;
    private Ability ability;

    public void StartSetup()
    {
        topPanel = gameManager.topPanel;
        transformSpace = gameManager.space.transform;
        prefabPools = gameManager.prefabPools;
        ability = topPanel.abilities[1];
    }

    public void ActivateSideDrones()
    {
        spriteSideDrone1.enabled = true;
        spriteSideDrone2.enabled = true;
        animatorSideDrones.enabled = true;
        animatorSideDrones.SetBool("moveIn", false);
        animatorSideDrones.Play(0);
        //animatorSideDrones.speed = 1.0f;
        outOfEnergy = false;
        topDroneShooting = true;
        timeBetweenShot = 1.0f / gameManager.upgradeValues[25, PlayerStats.playerStats.upgradeStatus[25]];
        energyPerShot = 7.0f * timeBetweenShot;
        damagePerShot = (int)gameManager.upgradeValues[40, PlayerStats.playerStats.upgradeStatus[40]];
        timer = 0.0f;
        ability.spriteModuleHighlight.enabled = true;
        ability.animatorModuleHighlight.enabled = true;
    }

    public void DeactivateSideDrones()
    {
        ability.ToggleCollider(true);
        spriteSideDrone1.enabled = false;
        spriteSideDrone2.enabled = false;
        animatorSideDrones.enabled = false;
        ability.spriteModuleHighlight.enabled = false;
        ability.animatorModuleHighlight.enabled = false;
        enabled = false;
    }

    void LocateTarget()
    {
        Transform target = null;
        int currentTarget = transformSpace.childCount - 1;
        while (currentTarget > 1 && target == null)
        {
            Transform currentChild = transformSpace.GetChild(currentTarget);
            if (currentChild.tag == "EnemyProjectileRocket" || currentChild.tag == "Asteroid" || currentChild.tag == "EnemyProjectileFire" || currentChild.tag == "EnemyProjectileLaser" || currentChild.tag == "EnemyProjectileAcid" || currentChild.tag == "Enemy")
            {
                target = transformSpace.GetChild(currentTarget);
            }
            else if (currentChild.tag == "EnemyProjectileWeaken" || currentChild.tag == "EnemyProjectileDrain")
            {
                if (currentChild.GetComponent<EnemyProjectile>().normalProjectile)
                {
                    target = transformSpace.GetChild(currentTarget);
                }
                else
                {
                    currentTarget--;
                }
            }
            else
            {
                currentTarget--;
            }
        }
        ShootTarget(target);
    }

    void ShootTarget(Transform target)
    {
        if (topDroneShooting)
        {
            ArmamentPrimary shipArmamentPrimary = prefabPools.PopArmamentPrimary();
            if (shipArmamentPrimary != null)
            {
                shipArmamentPrimary.enabled = true;
                shipArmamentPrimary.PopOffStack(damagePerShot, 0, 2, spriteSideDrone1.transform.position.x, spriteSideDrone1.transform.position.y, 0.75f);
                shipArmamentPrimary.AimAtTarget(target, spriteSideDrone1.transform);
            }
        }
        else
        {
            ArmamentPrimary shipArmamentPrimary = prefabPools.PopArmamentPrimary();
            if (shipArmamentPrimary != null)
            {
                shipArmamentPrimary.enabled = true;
                shipArmamentPrimary.PopOffStack(damagePerShot, 0, 2, spriteSideDrone2.transform.position.x, spriteSideDrone2.transform.position.y, 0.75f);
                shipArmamentPrimary.AimAtTarget(target, spriteSideDrone2.transform);
            }
        }
        topDroneShooting = !topDroneShooting;
    }

    void Update ()
    {
        timer += Time.deltaTime;
        if (timer >= (timeBetweenShot) && !outOfEnergy)
        {
            timer = 0.0f;
            topPanel.DecreaseEnergy(energyPerShot, 1);
            gameManager.missionTracker.IncrementMissionProgress(12, 1);

            LocateTarget();

            if (topPanel.moduleEnergyCurrent[1] < energyPerShot)
            {
                outOfEnergy = true;
                animatorSideDrones.SetBool("moveIn", true);
                animatorSideDrones.Play(0);
            }
        }
    }
}