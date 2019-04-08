using UnityEngine;

public class ButtonCloseHangerPanel : MonoBehaviour
{
    public Hanger hanger;
    public GameObject upgradePanel;
    private GameManager gameManager;

    void Start()
    {
        gameManager = hanger.gameManager;
    }

    void OnMouseDown()
    {
        gameManager.audioManager.PlaySound(9);
        if (gameManager.currentGameScreen == GameManager.CurrentGameScreen.upgradeAbility || gameManager.currentGameScreen == GameManager.CurrentGameScreen.upgradeArmament || gameManager.currentGameScreen == GameManager.CurrentGameScreen.upgradeShip)
        {
            hanger.CheckSave();
        }

        gameManager.currentGameScreen = GameManager.CurrentGameScreen.hanger;
        hanger.ToggleUpgradeButtons(true);
        upgradePanel.SetActive(false);
    }
}
