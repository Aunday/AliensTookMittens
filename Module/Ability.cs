using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour
{
    public TopPanel topPanel;
    public GameManager gameManager;
    public SpriteRenderer[] spriteAbilityFace;
    public SpriteRenderer spriteAbilityFace0;
    public SpriteRenderer spriteAbilityFace1;
    public SpriteRenderer spriteAbilityFace2;
    public SpriteRenderer spriteAbilityFace3;
    public SpriteRenderer spriteAbilityFace4;
    public SpriteRenderer spriteModuleHighlight;
    public Animator animatorModuleHighlight;
    public Transform abilityFaceTransform;
    public Animator animator;
    public int whichModule;
    private int moduleNumber;
    private int thisModule;
    public GameObject abilityBlackHolePrefab;
    public GameObject ability01Prefab;
    public GameObject abilityScrapColumnPrefab;
    private AbilityBlackHole abilityBlackHole;
    public Ability01 ability01 { get; private set; }
    public AbilityScrapColumn abilityScrapColumn { get; private set; }
    public Transform energyBarEmpty;
    public Transform energyBar;
    public Transform energyBarRequired;
    private SpriteRenderer spriteEnergyBar;
    private BoxCollider boxCollider;
    private Ship ship;
    private Board board;
    //private int maxEnergy;
    public int energyCost;
    private int[] energyCosts = new int[] { 10, 40, 35, 45, 20,
                                            30, 12, 7, 11, 50,
                                            40, 35, 45, 10, 50};
    private float[] alphaValues = new float[] { 0.33f, 0.5f, 0.45f };

    public void StartSetup()
    {
        //gameManager = topPanel.gameManager;
        //abilityFaceTransform = spriteAbilityFace.transform;
        thisModule = whichModule;
        spriteAbilityFace = new SpriteRenderer[5];
        spriteAbilityFace[0] = spriteAbilityFace0;
        spriteAbilityFace[1] = spriteAbilityFace1;
        spriteAbilityFace[2] = spriteAbilityFace2;
        spriteAbilityFace[3] = spriteAbilityFace3;
        spriteAbilityFace[4] = spriteAbilityFace4;
        ship = gameManager.ship;
        board = gameManager.board;
        boxCollider = GetComponent<BoxCollider>();
        spriteEnergyBar = energyBar.GetComponent<SpriteRenderer>();

        //set up Energy levels
        //SetEnergyMax();
    }

    public void ToggleCollider(bool whichToggle)
    {
        boxCollider.enabled = whichToggle;
    }

    public void SetEnergyMax()
    {
        energyBarEmpty.localScale = new Vector3(1.0f, gameManager.upgradeValues[11, PlayerStats.playerStats.upgradeStatus[11]] / PlayerStats.gameEnergyMax, 1.0f);
    }

    public void StartLevel(int chosenModule)
    {
        whichModule = chosenModule;
        boxCollider.enabled = true;
        //moduleNumber = ;
        SetSprite(whichModule % 5);
        SetEnergyStartLevel();
    }

    public void EndLevel()
    {
        boxCollider.enabled = false;
    }

    private void SetEnergyStartLevel()
    {
        if (whichModule == 0)
        {
            energyCost = (int)gameManager.upgradeValues[18, PlayerStats.playerStats.upgradeStatus[18]];
        }
        else if (whichModule == 7)
        {
            energyCost = (int)(12.0f * (1.0f / gameManager.upgradeValues[25, PlayerStats.playerStats.upgradeStatus[25]]));
        }
        else if (whichModule == 8)
        {
            energyCost = (int)gameManager.upgradeValues[26, PlayerStats.playerStats.upgradeStatus[26]] * 2;
        }
        else if (whichModule == 11)
        {
            energyCost = (int)gameManager.upgradeValues[29, PlayerStats.playerStats.upgradeStatus[29]];
        }
        else if (whichModule == 14)
        {
            energyCost = (int)gameManager.upgradeValues[32, PlayerStats.playerStats.upgradeStatus[32]];
        }
        else
        {
            energyCost = energyCosts[whichModule];
        }
        float energyStart = gameManager.upgradeValues[10, PlayerStats.playerStats.upgradeStatus[10]];
        SetEnergyBar(energyStart);
        SetEnergyRequiredBar(energyCost);
        if (energyStart < energyCost)
        {
            ImageDim();
        }
        else
        {
            spriteAbilityFace[moduleNumber].color = Color.white;
        }
    }

    public void SetSprite(int newmoduleNumber)
    {
        //Sprite abilitySprite;
        spriteAbilityFace[moduleNumber].enabled = false;
        moduleNumber = newmoduleNumber;
        spriteAbilityFace[moduleNumber].enabled = true;

        if (whichModule >= 10)
        {
            if (PlayerStats.playerStats.upgradeStatus[28] <= 0)
            {
                gameObject.SetActive(false);
            }
            else if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
    }

    public void ImageDim()
    {
        spriteAbilityFace[moduleNumber].color = new Color(0.2f, 0.2f, 0.2f);
    }

    public void ImageBright()
    {
        if (spriteAbilityFace[moduleNumber].color.r < 1.0f)
        {
            spriteAbilityFace[moduleNumber].color = Color.white;
            //animator.enabled = true;
        }
        animator.Play(0);
    }

    public void SetEnergyBar(float scale)
    {
        energyBar.localScale = new Vector3(1.0f, scale / PlayerStats.gameEnergyMax, 1.0f);
        if(scale < energyCost)
        {
            if (spriteEnergyBar.color.a == 1.0f)
            {
                spriteEnergyBar.color -= new Color(0.0f, 0.0f, 0.0f, alphaValues[thisModule]);
            }
        }
        else
        {
            if (spriteEnergyBar.color.a < 1.0f)
            {
                spriteEnergyBar.color += new Color(0.0f, 0.0f, 0.0f, alphaValues[thisModule]);
            }
        }
    }

    void SetEnergyRequiredBar(int energyCost)
    {
        energyBarRequired.transform.localPosition = new Vector3(energyBarRequired.transform.localPosition.x, -0.41f + energyCost * 0.0094f, 0.0f);
    }

    IEnumerator OnMouseDown()
    {
        int moduleType = Mathf.FloorToInt(whichModule / 5);
        if (topPanel.moduleEnergyCurrent[moduleType] >= energyCost)
        {
            ToggleCollider(false);
            switch (whichModule)
            {
                case 0: //Rocket Barrage
                    float numberRocketsToFire = Mathf.Floor(topPanel.moduleEnergyCurrent[0] / energyCost);
                    for (int rocketNumber = 0; rocketNumber < numberRocketsToFire; rocketNumber++)
                    {
                        AbilityRocketBarrage abilityRocketBarrage = gameManager.prefabPools.PopAbilityRocketBarrage();
                        if (abilityRocketBarrage != null)
                        {
                            abilityRocketBarrage.enabled = true;
                            abilityRocketBarrage.PopOffStack(ship.transform);
                        }

                        topPanel.DecreaseEnergy(energyCost, 0);
                        yield return new WaitForSeconds(0.08f);
                    }
                    ToggleCollider(true);
                    break;
                case 1:  //Super Laser
                    //if (gameManager.space.enemy != null && topPanel.moduleEnergyCurrent[0] >= energyCost)
                    if (gameManager.space.enemy.Count > 0.0f || gameManager.prefabPools.stackAsteroid.Count < 5.0f)
                    {
                        if (ability01 == null)
                        {
                            GameObject instantiatedAbility = gameManager.InstantiateObject(ability01Prefab, ship.shipAnimation.transform, -0.05f, 0, 0, 0.9f, 0.8f, 1.0f);
                            ability01 = instantiatedAbility.GetComponent<Ability01>();
                            ability01.enabled = true;
                            ability01.ability = this;
                            ability01.space = gameManager.space;
                            ability01.StartSetup();
                        }
                        else
                        {
                            ability01.enabled = true;
                        }

                        ability01.ActivateModule();

                        topPanel.DecreaseEnergy(energyCost, 0);
                    }
                    else
                    {
                        ToggleCollider(true);
                    }
                    break;
                case 2:  //Corrosion
                    ship.ShootArmamentPrimary(Mathf.CeilToInt(gameManager.upgradeValues[20, PlayerStats.playerStats.upgradeStatus[20]]), PlayerStats.playerStats.upgradeStatus[35], 1);
                    topPanel.DecreaseEnergy(energyCost, 0);
                    ToggleCollider(true);
                    break;
                case 3:  //Ram
                    ship.ActivateRam();
                    topPanel.DecreaseEnergy(energyCost, 0);
                    break;
                case 4:  //Mines
                    float numberMinesToLaunch = Mathf.Floor(topPanel.moduleEnergyCurrent[0] / energyCost);
                    for (int mineNumber = 0; mineNumber < numberMinesToLaunch; mineNumber++)
                    {
                        ModuleMines moduleMines = gameManager.prefabPools.PopModuleMines();
                        if (moduleMines != null)
                        {
                            moduleMines.enabled = true;
                            moduleMines.PopOffStack(ship.transform);
                        }

                        topPanel.DecreaseEnergy(energyCost, 0);
                        yield return new WaitForSeconds(0.08f);
                    }
                    ToggleCollider(true);
                    break;
                case 5:  //Black Hole
                    if (abilityBlackHole == null)
                    {
                        GameObject instantiatedAbility = gameManager.InstantiateObject(abilityBlackHolePrefab, gameManager.space.transform, 1.9f, 0, 0, 1.0f, 1.0f, 1.0f);
                        abilityBlackHole = instantiatedAbility.GetComponent<AbilityBlackHole>();
                        abilityBlackHole.ability = this;
                        abilityBlackHole.enabled = true;
                        abilityBlackHole.StartSetup();
                    }
                    else
                    {
                        abilityBlackHole.enabled = true;
                    }
                    abilityBlackHole.ActivateModule();
                    topPanel.DecreaseEnergy(energyCost, 1);
                    break;
                case 6:  //Reflective Shield
                    ship.ActivateReflectiveShield();
                    break;
                case 7:  //Side Drones
                    ship.ActivateSideDrones();
                    break;
                case 8:  //Repair Bots
                    ship.ActivateNanobots();
                    break;
                case 9:  //Redirect
                    ship.ActivateModuleRedirect();

                    topPanel.DecreaseEnergy(energyCost, 1);
                    break;
                case 10:  //Destablize
                    if (abilityScrapColumn == null)
                    {
                        GameObject instantiatedAbility = gameManager.InstantiateObject(abilityScrapColumnPrefab, gameManager.transformBoard.transform, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
                        abilityScrapColumn = instantiatedAbility.GetComponent<AbilityScrapColumn>();
                        abilityScrapColumn.enabled = true;
                        abilityScrapColumn.ability = this;
                        abilityScrapColumn.StartSetup();
                    }
                    else
                    {
                        abilityScrapColumn.enabled = true;
                    }
                    abilityScrapColumn.ActivateModule();

                    topPanel.DecreaseEnergy(energyCost, 2);
                    break;
                case 11:  //Shuffle
                    topPanel.DecreaseEnergy(energyCost, 2);
                    StartCoroutine(gameManager.board.Shuffle());
                    yield return new WaitForSeconds(1.6f);
                    ToggleCollider(true);
                    break;
                case 12:  //Empower
                    topPanel.DecreaseEnergy(energyCost, 2);
                    int iconsToEmpower = (int)gameManager.upgradeValues[30, PlayerStats.playerStats.upgradeStatus[30]];
                    for (int empowerNumber = 0; empowerNumber < iconsToEmpower; empowerNumber++)
                    {
                        int attempts = 49;
                        int randRow;
                        int randColumn;
                        Icon icon;
                        do
                        {
                            randRow = Random.Range(0, Board.GridRows);
                            randColumn = Random.Range(0, Board.GridColumns);
                            icon = board.boardIcons[randColumn, randRow];
                            attempts--;
                        } while (icon.currentEffectMultiplier > 1.0f && attempts > 0);
                        icon.ActivateEmpower();
                    }
                    ToggleCollider(true);
                    break;
                case 13:  //Energy Conversion
                    ship.ActivateEnergyConversion();
                    break;
                case 14:  //Shatter
                    yield return StartCoroutine(board.ActivateModuleShatter());
                    topPanel.DecreaseEnergy(energyCost, 2);
                    ToggleCollider(true);
                    break;
            }
        }
    }

    void OnMouseUp()
    {
        board.MouseUpOnOther();
    }
}