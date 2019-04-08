using UnityEngine;
using System.Collections;

public class ModuleReflectiveShield : MonoBehaviour
{
    private Ship ship;
    public GameManager gameManager;
    private TopPanel topPanel;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private float durationPerEnergy;
    private Ability ability;

    public void StartSetup()
    {
        topPanel = gameManager.topPanel;
        ability = topPanel.abilities[1];
        ship = gameManager.ship;
    }

    public void ActivateReflectiveShield()
    {
        durationPerEnergy = gameManager.upgradeValues[24, PlayerStats.playerStats.upgradeStatus[24]];
        StartCoroutine(ReflectiveShield());
        ability.spriteModuleHighlight.enabled = true;
        ability.animatorModuleHighlight.enabled = true;
    }

    public IEnumerator ReflectiveShield()
    {
        animator.enabled = true;
        spriteRenderer.enabled = true;
        if (!ship.CheckReflection())
        {
            ship.ToggleReflection(true);
        }
        while (topPanel.moduleEnergyCurrent[1] >= 11.0f && gameManager.currentGameScreen == GameManager.CurrentGameScreen.level)
        {
            topPanel.DecreaseEnergy(11.0f, 1);
            yield return new WaitForSeconds(durationPerEnergy);
        }
        DeactivateReflectiveShield();
    }

    public void DeactivateReflectiveShield()
    {
        ship.ToggleReflection(false);
        animator.enabled = false;
        spriteRenderer.enabled = false;
        ability.ToggleCollider(true);

        ability.spriteModuleHighlight.enabled = false;
        ability.animatorModuleHighlight.enabled = false;
    }
}
