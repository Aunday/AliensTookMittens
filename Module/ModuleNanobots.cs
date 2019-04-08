using UnityEngine;

public class ModuleNanobots : MonoBehaviour
{
    public SpriteRenderer spriteBeam1;
    public SpriteRenderer spriteBeam2;
    public SpriteRenderer spriteBeam3;
    public SpriteRenderer spriteNanobot1;
    public SpriteRenderer spriteNanobot2;
    public SpriteRenderer spriteNanobot3;
    public Animator animator;
    private float timer;
    public GameManager gameManager;
    private TopPanel topPanel;
    private Ship ship;
    private float healAmount;
    private float costPerTick;
    private Ability ability;

    public void StartSetup()
    {
        topPanel = gameManager.topPanel;
        ability = topPanel.abilities[1];
        ship = gameManager.ship;
    }

    public void ActivateNanobots()
    {
        spriteBeam1.enabled = true;
        spriteBeam2.enabled = true;
        spriteBeam3.enabled = true;
        spriteNanobot1.enabled = true;
        healAmount = gameManager.upgradeValues[41, PlayerStats.playerStats.upgradeStatus[41]];
        costPerTick = gameManager.upgradeValues[26, PlayerStats.playerStats.upgradeStatus[26]];
        spriteNanobot2.enabled = true;
        spriteNanobot3.enabled = true;
        animator.enabled = true;
        timer = 0.0f;
        ability.spriteModuleHighlight.enabled = true;
        ability.animatorModuleHighlight.enabled = true;
    }

    public void DeactivateNanobotes()
    {
        ability.ToggleCollider(true);
        spriteBeam1.enabled = false;
        spriteBeam2.enabled = false;
        spriteBeam3.enabled = false;
        spriteNanobot1.enabled = false;
        spriteNanobot2.enabled = false;
        spriteNanobot3.enabled = false;
        animator.enabled = false;
        ability.spriteModuleHighlight.enabled = false;
        ability.animatorModuleHighlight.enabled = false;
        enabled = false;
    }

    void SetBeamUnderShip()
    {
        spriteBeam1.sortingOrder = 58;
        spriteNanobot1.sortingOrder = 59;
    }

    void SetBeamOverShip()
    {
        spriteBeam1.sortingOrder = 72;
        spriteNanobot1.sortingOrder = 73;
    }

    void Update ()
    {
        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            timer = 0.0f;
            topPanel.DecreaseEnergy(costPerTick, 1);

            //heal here
            ship.PlayerHeal(healAmount);
            gameManager.missionTracker.IncrementMissionProgress(18, (int)healAmount);

            if (topPanel.moduleEnergyCurrent[1] < costPerTick)
            {
                DeactivateNanobotes();
            }
        }
    }
}
