using UnityEngine;
using System.Collections;

public class InterLevelScreen : MonoBehaviour
{
    public GameManager gameManager;
    public int whichLevel;
    public TextMesh textUnlocked;
    public TextMesh textTitle;
    public SpriteRenderer rewardImage;
    public GameObject completedMission;
    public TextMesh textCompletedMissionTitle;
    public TextMesh textCompletedMissionDescription;
    public Animator rewardAnimator;
    public ParticleSystem particles;
    private Transform rewardTransform;
    public bool buttonContinueClicked = false;
    public GameObject buttonContinue;
    private MissionTracker missionTracker;
    private string[,] moduleTitles = new string[,]
    {
        { "Rocket Barrage", "Mega Laser", "Corrosion", "Ram", "Mines" },
        { "Black Hole", "Reflective Shield", "Side Drones", "Repair Nanobots", "Redirect" },
        { "Destabilize", "Shuffle", "Empower", "Conversion", "Shatter" }
    };
    private string[] levelNames = new string[] { "Nyan", "Fabris", "Eunomia", "Atlantis", "Kiyora", "Jagrit", "Phaedra", "Ægir", "Ceti Prime", "Klemmins", "Zi'Rik", "Karu", "Foros", "Destiny" }; //Adamastor Arae
    string[,] moduleDescriptions = new string[,]
    {
        {
            "Launches a barrage of rockets\nat the closest enemy",
            "Fires a constant laser blast at\nthe closest enemy",
            "Corrosive blast that covers\nenemy, damaging them over time",
            "Rams enemy with ship, dealing\nhigh damage to enemy and self",
            "Deploys mines, which explode on\napproaching enemies"
        },
        {
            "Pulls enemies away, preventing\nthem from attacking",
            "Reflects enemy projectiles back\nat them with increased strength",
            "Deploys side drones to destroy\nprojectiles and attack enemies",
            "Repairs ship's armor while\nactive",
            "Redirects incoming projectiles,\ndestroying contol icons instead"
        },
        {
            "Matches every icon in\nselected column",
            "Shuffles all ship control icons",
            "Empowers icons, increasing\ntheir match effect",
            "All damage taken also provides\nThermal or Nuclear energy",
            "Shatters all crystal icons,\nproviding reduced resources"
        }
    };
    private int[] moduleUnlocks = new int[] { 28, 19, 24, 29, 20, 25, 30, 21, 26, 31, 22, 27, 32 };

    public void StartSetup ()
    {
        rewardTransform = rewardImage.transform;
        missionTracker = gameManager.missionTracker;
        particles.startSize = particles.startSize * gameManager.scaleRatioX;
        particles.startSpeed = particles.startSpeed * gameManager.scaleRatioX;
        particles.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public bool StartBossCheck()
    {
        for (int checkLevel = 0; checkLevel < 13; checkLevel++)
        {
            if (PlayerStats.playerStats.levelThreat[checkLevel] >= 100.0f && PlayerStats.playerStats.upgradeStatus[moduleUnlocks[checkLevel]] <= 0)
            {
                whichLevel = checkLevel;
                gameManager.topPanel.currentThreat = PlayerStats.playerStats.levelThreat[checkLevel];
                gameManager.space.NewLevel(checkLevel);
                return true;
            }
        }
        return false;
    }

    IEnumerator checkBossDefeated()
    {
        if (whichLevel < 13)
        {
            textUnlocked.text = "";
            textTitle.text = "";
            buttonContinueClicked = false;
            int numLevelsUnlocked = 0;
            int abilityType = ((whichLevel + 2) % 3);
            int abilityNumber = Mathf.FloorToInt((whichLevel + 2) / 3);
            completedMission.SetActive(false);

            SetUpgradeStatus();

            rewardTransform.localPosition = new Vector2(0.12f, 0.12f);
            textUnlocked.text = "\n\n\n\n\nNEW MODULE\nUNLOCKED!\n\n\n\n\n" + moduleTitles[abilityType, abilityNumber];
            textTitle.text = "\n\n" + moduleDescriptions[abilityType, abilityNumber];
            rewardTransform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
            rewardImage.sprite = Resources.Load<Sprite>("AbilityFaces/Ability" + abilityType + abilityNumber);
            rewardImage.color = Color.white;

            PlayerStats.playerStats.previousLevel++;
            if(whichLevel == 2 || whichLevel == 5 || whichLevel == 8)
            {
                numLevelsUnlocked = 2;
                PlayerStats.playerStats.currentLevelProgress += 2;
            }
            else if (whichLevel != 3 && whichLevel != 6 && whichLevel != 9)
            {
                numLevelsUnlocked = 1;
                PlayerStats.playerStats.currentLevelProgress++;
            }
            yield return (StartCoroutine(GainReward())); //give module reward at this point to ensure level progress is saved

            rewardTransform.localScale = new Vector3(0.677f, 0.677f, 0.677f);
            for (int levelUnlocked = 0; levelUnlocked < numLevelsUnlocked; levelUnlocked++)
            {
                textUnlocked.text = "\n\n\n\n\nNEW SYSTEM\nDISCOVERED!\n\n\n\n\n" + levelNames[whichLevel + 1 + levelUnlocked];
                textTitle.text = "";
                rewardImage.sprite = Resources.Load<Sprite>("LevelSelectionImages/Level" + (whichLevel + 1 + levelUnlocked));
                yield return (StartCoroutine(GainReward())); //give level progress reward
            }
        }
        else
        {
            buttonContinueClicked = false;
            completedMission.SetActive(false);

            rewardTransform.localPosition = new Vector2(0.12f, 0.12f);
            textUnlocked.text = "\n\n\n\n\nCONGRATULATIONS!\nYOU SAVED MITTENS!\n\n\n\n\n";
            textTitle.text = "\n\nWorth it!";
            rewardTransform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
            rewardImage.sprite = Resources.Load<Sprite>("Mittens");
            rewardImage.color = Color.white;
            textTitle.color = new Color(1.0f, 0.71f, 0.02f);
            yield return (StartCoroutine(GainReward()));
            //play credits
            textUnlocked.text = "\n\n\n\n\nThank you for playing!\nHope you enjoyed it!\n\n\n\n\n";
            textTitle.text = "Programming, Art, Game\nDesign, Story:\nAndy Hertel\n\nSpecial Thanks to:\nGary Hertel, Tony Hertel\nFrankie Lam\n\nPlay Testers:\nGary Hertel, Tony Hertel,\nFrankie Lam, Kyle Clemmins\nFrancine Lam\n\n";
            rewardImage.color = Color.clear;
            yield return (StartCoroutine(GainReward()));
        }
        StartCoroutine(MissionHandler(true));
    }

    void SetUpgradeStatus()
    {
        int whichUpgrade = 18 + Mathf.FloorToInt((whichLevel + 2) / 3) + ((whichLevel + 2) % 3) * 5;
        PlayerStats.playerStats.upgradeStatus[whichUpgrade] = 1;
        PlayerStats.playerStats.upgradeStatus[whichUpgrade + 15] = 1;
        PlayerStats.playerStats.upgradeNew[2] = true;
        PlayerStats.playerStats.upgradeNew[((whichLevel + 2) % 3) + 9] = true;
        PlayerStats.playerStats.upgradeNew[whichUpgrade + 12] = true;
    }

    IEnumerator GainReward()
    {
        //rewardImage.transform.parent.localScale = new Vector3()
        rewardAnimator.gameObject.SetActive(true);
        rewardAnimator.enabled = true;
        rewardAnimator.Play(0);
        PlayerStats.playerStats.Save();
        yield return new WaitForSeconds(0.5f);
        particles.Play();
        buttonContinue.SetActive(true);
        rewardAnimator.Stop();

        while (!buttonContinueClicked)
        {
            yield return null;
        }
        buttonContinueClicked = false;
        buttonContinue.SetActive(false);

        rewardAnimator.enabled = false;
        rewardAnimator.gameObject.SetActive(false);
    }

    IEnumerator MissionHandler(bool alive)
    {
        bool alreadySaved = false;
        textUnlocked.text = "";
        textTitle.text = "";
        missionTracker.CheckMissionsComplete();
        for (int checkMission = 0; checkMission < 2; checkMission++)
        {
            Mission currentMission = missionTracker.currentMissions[checkMission];
            char rewardType = currentMission.ReturnMissionRewardType();
            int missionID = currentMission.missionID;
            if (PlayerStats.playerStats.allMissionStatus[missionID] == 2)
            {
                rewardTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                textUnlocked.text = "\n\n\n\n\nBONUS OBJECTIVE\nCOMPLETED!\n\n\n\n\n";
                completedMission.SetActive(true);
                textCompletedMissionTitle.text = missionTracker.RetrieveMissionTitle(missionID);
                textCompletedMissionDescription.text = missionTracker.RetrieveMissionDescription(missionID);

                if (rewardType == 'f')
                {
                    int rewardValue = currentMission.ReturnMissionRewardValue();
                    gameManager.ChangeFunds(rewardValue);
                    rewardTransform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                    //rewardImageCrystal.enabled = true;
                    rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeResource");
                    rewardImage.color = new Color(1.0f, 0.55f, 1.0f, 1.0f);
                    textTitle.color = new Color(1.0f, 0.55f, 1.0f, 1.0f);
                    textTitle.text = "x " + rewardValue + "\nResources Obtained";
                }
                else if (rewardType == 's')
                {
                    Color colorGold = new Color(1.0f, 0.71f, 0.02f);

                    int whichUpgrade = 0;
                    rewardTransform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                    switch (missionID)
                    {
                        case 0: //armor upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeArmor");
                            whichUpgrade = 9;
                            textTitle.text = "Ship Max Armor +30\nShip Upgrade Unlocked";
                            break;
                        case 2: //thermal energy upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeEnergy1");
                            whichUpgrade = 12;
                            textTitle.text = "Match Thermal Energy +1\nEnergy Upgrade Unlocked";
                            break;
                        case 3: //blast upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeBlastDamage");
                            whichUpgrade = 3;
                            textTitle.text = "Blast Damage +3\nBlast Upgrade Unlocked";
                            break;
                        case 4: //shield upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeShieldMatch");
                            whichUpgrade = 6;
                            textTitle.text = "Shield Match Power +3\nShield Upgrade Unlocked";
                            break;
                        case 5: //start energy upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeEnergyStart");
                            whichUpgrade = 10;
                            textTitle.text = "Starting Energy +5\nEnergy Upgrade Unlocked";
                            break;
                        case 6: //laser upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeLaserDamage");
                            whichUpgrade = 0;
                            textTitle.text = "Laser Damage +3\nLaser Upgrade Unlocked";
                            break;
                        case 8: //double laser upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeTripleLaser");
                            whichUpgrade = 1;
                            textTitle.text = "Double Laser Enabled\nLaser Upgrade Unlocked";
                            break;
                        case 9: //nuclear energy upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeEnergy2");
                            whichUpgrade = 13;
                            textTitle.text = "Match Nuclear Energy +1\nEnergy Upgrade Unlocked";
                            break;
                        case 10: //knockback upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeArrowRight");
                            whichUpgrade = 4;
                            textTitle.text = "Blast Knockback +10%\nBlast Upgrade Unlocked";
                            break;
                        case 11: //max shield upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeShieldMax");
                            whichUpgrade = 7;
                            textTitle.text = "Maximum Shield +20\nShield Upgrade Unlocked";
                            break;
                        case 12: //electrical energy upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeEnergy3");
                            whichUpgrade = 14;
                            textTitle.text = "Match Electrical Energy +1\nEnergy Upgrade Unlocked";
                            break;
                        case 14: //multi match upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeMulti");
                            whichUpgrade = 15;
                            textTitle.text = "Multi Match Effect +5%\nMatch Upgrade Unlocked";
                            break;
                        case 15: //max energy upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeEnergyMax");
                            whichUpgrade = 11;
                            textTitle.text = "Maximum Energy +5\nShip Upgrade Unlocked";
                            break;
                        case 16: //match laser upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeMatchLaser");
                            whichUpgrade = 2;
                            textTitle.text = "Match Laser Enabled\nLaser Upgrade Unlocked";
                            break;
                        case 17: //starting shield upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeShieldStart");
                            whichUpgrade = 8;
                            textTitle.text = "Starting Shield +20\nShield Upgrade Unlocked";
                            break;
                        case 18: //aftershock upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeAftershock");
                            whichUpgrade = 5;
                            textTitle.text = "Blast Aftershock Enabled\nBlast Upgrade Unlocked";
                            break;
                        case 20: //chain match upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeChain");
                            whichUpgrade = 16;
                            textTitle.text = "Chain Match Effect +2%\nMatch Upgrade Unlocked";
                            break;
                        case 21: //resource match upgrade
                            rewardImage.sprite = Resources.Load<Sprite>("UI/ButtonUpgradeResource");
                            whichUpgrade = 17;
                            textTitle.text = "Match Resources +1\nMatch Upgrade Unlocked";
                            break;
                    }
                    textTitle.color = colorGold;
                    rewardImage.color = colorGold;
                    PlayerStats.playerStats.upgradeStatus[whichUpgrade] = 1;
                    if (whichUpgrade < 9)
                    {
                        PlayerStats.playerStats.upgradeNew[0] = true;
                    }
                    else
                    {
                        PlayerStats.playerStats.upgradeNew[1] = true;
                    }
                    PlayerStats.playerStats.upgradeNew[Mathf.FloorToInt(whichUpgrade / 3) + 3] = true;
                    PlayerStats.playerStats.upgradeNew[whichUpgrade + 12] = true;
                }

                PlayerStats.playerStats.allMissionStatus[missionID] = 3;

                yield return (StartCoroutine(GainReward()));
            }
            completedMission.SetActive(false);
        }

        if (!alreadySaved)
        {
            PlayerStats.playerStats.Save();
            alreadySaved = true;
        }

        yield return new WaitForSeconds(0.2f);

        gameManager.EnterHanger();
        gameManager.transformBoard.gameObject.SetActive(false);
        gameManager.topPanel.gameObject.SetActive(false);
        gameManager.ship.LandInHanger();
        
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public void LevelStretchComplete()
    {
        PlayerStats.playerStats.levelThreat[whichLevel] = gameManager.topPanel.currentThreat;
        missionTracker.CheckMissionsComplete();
        StartCoroutine(checkBossDefeated());
    }
    
    public void PlayerDead()
    {
        float savedThreat = PlayerStats.playerStats.levelThreat[whichLevel];
        if (savedThreat >= 100.0f && savedThreat < gameManager.topPanel.currentThreat)
        {
            PlayerStats.playerStats.levelThreat[whichLevel] = gameManager.topPanel.currentThreat;
        }
        StartCoroutine(MissionHandler(false));
    }
}