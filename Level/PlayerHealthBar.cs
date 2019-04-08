using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    public Transform damageTakenBar;
    public TextMesh healthText;
    private float playerHealthMax;
    public TopPanel topPanel;
    private float barLocationY;

    public void StartLevel()
    {
        playerHealthMax = topPanel.gameManager.upgradeValues[9, PlayerStats.playerStats.upgradeStatus[9]];
        barLocationY = damageTakenBar.localPosition.y;
        MoveHealthBar(playerHealthMax);
    }

    public void MoveHealthBar(float playerCurrentHealth)
    {
        damageTakenBar.localPosition = new Vector3((5.4f * (playerCurrentHealth / playerHealthMax)) - 2.7f, barLocationY, 0.0f);
        healthText.text = Mathf.CeilToInt(playerCurrentHealth) + "/" + playerHealthMax;
    }
}