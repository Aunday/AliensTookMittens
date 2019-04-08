using UnityEngine;

public class PlayerShieldBar : MonoBehaviour
{
    public Transform shieldDamageTakenBar;
    public TextMesh healthShieldText;
    public TopPanel topPanel;
    private float playerHealthShieldMax;
    private float shieldBarLocationY;

    public void StartLevel()
    {
        playerHealthShieldMax = topPanel.gameManager.upgradeValues[7, PlayerStats.playerStats.upgradeStatus[7]];
        shieldBarLocationY = shieldDamageTakenBar.localPosition.y;
        MoveShieldBar(Mathf.Min(playerHealthShieldMax,topPanel.gameManager.upgradeValues[8, PlayerStats.playerStats.upgradeStatus[8]]));
    }

    public void MoveShieldBar(float playerCurrentShieldHealth)
    {
        shieldDamageTakenBar.localPosition = new Vector3((5.4f * (playerCurrentShieldHealth / playerHealthShieldMax)) - 2.7f, shieldBarLocationY, 0.0f);
        healthShieldText.text = Mathf.CeilToInt(playerCurrentShieldHealth) + "/" + playerHealthShieldMax;
    }
}