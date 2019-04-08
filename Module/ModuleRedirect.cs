using UnityEngine;
using System.Collections;

public class ModuleRedirect : MonoBehaviour
{
    private Ship ship;
    public GameManager gameManager;
    private TopPanel topPanel;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private float projectileQuantity;
    private Ability ability;

    public void StartSetup()
    {
        topPanel = gameManager.topPanel;
        ability = topPanel.abilities[1];
        ship = gameManager.ship;
    }

    public void ActivateModule()
    {
        projectileQuantity = gameManager.upgradeValues[27, PlayerStats.playerStats.upgradeStatus[27]];
        StartCoroutine(Redirect());
        ability.spriteModuleHighlight.enabled = true;
        ability.animatorModuleHighlight.enabled = true;
    }

    public IEnumerator Redirect() //
    {
        animator.enabled = true;
        spriteRenderer.enabled = true;
        if (!ship.CheckRedirection())
        {
            ship.ToggleRedirection(true);
        }
        while (projectileQuantity > 0.0f && gameManager.currentGameScreen == GameManager.CurrentGameScreen.level)
        {
            yield return new WaitForSeconds(0.1f);
        }
        DeactivateModule();
    }

    public void ProjectileRedirected()
    {
        projectileQuantity--;
    }

    public void DeactivateModule()
    {
        ship.ToggleRedirection(false);
        animator.enabled = false;
        spriteRenderer.enabled = false;
        ability.ToggleCollider(true);
        ability.spriteModuleHighlight.enabled = false;
        ability.animatorModuleHighlight.enabled = false;
    }

}
