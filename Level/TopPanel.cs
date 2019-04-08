using UnityEngine;

public class TopPanel : MonoBehaviour
{
    public GameObject abilityPrefab;
    public GameManager gameManager;
    public PlayerHealthBar playerHealthBar;
    public PlayerShieldBar playerShieldBar;
    public float currentThreat;
    public TextMesh textMoney;
    public TextMesh textThreat;
    public float[] moduleEnergyCurrent = new float[3];
    public Ability[] abilities;
    public Ability ability0;
    public Ability ability1;
    public Ability ability2;
    private int numAbilities = 3;
    public Animator animatorThreatShrink;

    public void StartSetup()
    {
        abilities = new Ability[] { ability0, ability1, ability2 };
        for (int i = 0; i < 3; i++)
        {
            abilities[i].StartSetup();
        }
        gameObject.SetActive(false);
    }

    public void SetFunds(int currentFunds)
    {
        textMoney.text = currentFunds.ToString();
    }

    public void IncreaseThreat(float incrementAmount)
    {
        currentThreat += incrementAmount;
        if (currentThreat >= 200.0f)
        {
            gameManager.missionTracker.IncrementMissionProgress(19, 1);
        }
        textThreat.text = Mathf.CeilToInt(currentThreat).ToString();
        animatorThreatShrink.Play(0);
    }

    //Places the abilities that player has chosen into the three slots
    public void SetupTopPanelForLevel(int whichLevel)
    {
        SetFunds(PlayerStats.playerStats.playerFunds);
        currentThreat = Mathf.Min(100.0f,PlayerStats.playerStats.levelThreat[whichLevel]);
        textThreat.text = Mathf.CeilToInt(currentThreat).ToString();
        playerHealthBar.StartLevel();
        playerShieldBar.StartLevel();

        for (int currentAbility = 0; currentAbility < numAbilities; currentAbility++)
        {
            abilities[currentAbility].StartLevel(PlayerStats.playerStats.moduleSelection[currentAbility] + currentAbility * 5);
        }
        moduleEnergyCurrent[0] = Mathf.Min(gameManager.upgradeValues[10, PlayerStats.playerStats.upgradeStatus[10]], gameManager.upgradeValues[11, PlayerStats.playerStats.upgradeStatus[11]]);
        moduleEnergyCurrent[1] = moduleEnergyCurrent[0];
        moduleEnergyCurrent[2] = moduleEnergyCurrent[0];
    }

    public void EndLevel()
    {
        for (int currentAbility = 0; currentAbility < numAbilities; currentAbility++)
        {
            abilities[currentAbility].EndLevel();
        }
    }

    public void IncreaseEnergy(int matchValue, int whichModule)
    {
        float maxEnergy = gameManager.upgradeValues[11, PlayerStats.playerStats.upgradeStatus[11]];
        moduleEnergyCurrent[whichModule] = Mathf.Min(maxEnergy, moduleEnergyCurrent[whichModule] + matchValue);
        if (moduleEnergyCurrent[whichModule] >= abilities[whichModule].energyCost)
        {
            abilities[whichModule].ImageBright();
        }
        abilities[whichModule].SetEnergyBar(moduleEnergyCurrent[whichModule]);

        if (moduleEnergyCurrent[0] == maxEnergy && moduleEnergyCurrent[1] == maxEnergy && moduleEnergyCurrent[2] == maxEnergy)
        {
            gameManager.missionTracker.IncrementMissionProgress(15, 1);
        }
    }

    public void DecreaseEnergy(float decreaseAmount, int whichModule)
    {
        moduleEnergyCurrent[whichModule] = Mathf.Max(0, moduleEnergyCurrent[whichModule] - decreaseAmount);
        if (moduleEnergyCurrent[whichModule] < abilities[whichModule].energyCost)
        {
            abilities[whichModule].ImageDim();
        }
        abilities[whichModule].SetEnergyBar(moduleEnergyCurrent[whichModule]);
    }

    public void ToggleModulesSelectable(bool whichToggle)
    {
        for (int whichModule = 0; whichModule < 3; whichModule++)
        {
            abilities[whichModule].ToggleCollider(whichToggle);
        }
    }
}