using UnityEngine;

public class MissionTracker : MonoBehaviour
{
    public GameManager gameManager;
    protected string[] titles = new string[]
        {
            "First Steps",
            "Blank",
            "Rocket What Now?",
            "Multipurpose",
            "Shield Your Eyes",
            "Minesweeper",
            "Shark's Best Friend",
            " Three = Magic Number",
            "Full Power",
            "Two Can Play",
            "Stay Back",
            "Reflections Can Kill",
            "Three is Company",
            "Raking it in",
            "Awesome Alliteration",
            "I Feel The Power",
            "Dial it Up to 11",
            "None Shall Pass",
            "Size Doesn't Matter",
            "It Goes Past 100?",
            "Chain Reaction",
            "Require More Minerals",
            "Come at Me",
            "Is That Possible?",
            "What Goes Around",
            "Can't touch this",
            "Lower Requirements",
            "For All the Marbles",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        };
    protected string[] descriptions = new string[]
        {
            "Defeat the furball\nguardian", //0
            "Secret? Nope",
            "Destroy an enemy using\nRocket Barrage module", //1
            "Destroy two projectiles\nwith blast matches",
            "Fill shield bar over\nhalf way", //2
            "Remove two bombs using\nDestabilize module",
            "Complete five\nlaser matches", //3
            "Complete ten matches of\nat least three length",
            "Deal 100 damage with\nMega Laser module", //4
            "Destroy Acid Spewer enemy\nusing corrosion module",
            "Destroy two bombs\nbefore they explode", //5
            "Reflect three projectiles\nback at enemies",
            "Use Side Drones\nfor 25 seconds", //6
            "Gather 100 resources\nfrom crystal matches",
            "Make Many Multi Matches\n...mmmmmm 3 is enough", //7
            "Fill all energy\nbars to max",
            "Create a match with\ntwo empowered icons", //8
            "Destroy all five asteroids\nwithin a checkpoint",
            "Recover 200 HP using\nNanobots module", //9
            "Reach 200% danger level",
            "Obtain a fourth level\nChain Match", //10
            "Gather 500 resources\nin one life",
            "Have four mines active\nat one time", //11
            "Complete five matches of\nat least four length",
            "Create five projectiles\nfrom redirected projectiles", //12
            "Receive 0 ship damage\nbetween two checkpoints",
            "Complete three matches of\nat least five length", //13
            "Save Mittens",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        };
    protected int[] missionGoals = new int[]
        {
            1,      //0
            1,      //1
            1,      //2
            2,      //3
            1,      //4
            2,      //5
            5,      //6
            10,     //7
            100,    //8
            1,      //9
            2,      //10
            3,      //11
            50,     //12
            100,    //13
            3,      //14
            1,      //15
            1,      //16
            1,      //17 (checking if 5, then incrementing when there)
            200,    //18
            1,      //19 (checking if 300, then incrementing when there)
            1,      //20 (checking if > 3, then incrementing when there)
            500,    //21
            1,      //22
            5,      //23
            5,      //24
            1,      //25
            3,      //26
            1,      //27
            1,      //28
            1,      //29
            1,      //30
            1,      //31
            1,      //32
            1,      //33
            1,      //34
            1,      //35
            1,      //36
            1,      //37
            1,      //38
            1,      //39
            1,      //40
            1,      //41
            1,      //42
            1,      //43
            1,      //44
            1,      //45
            1,      //46
            1,      //47
            1,      //48
            1       //49
        };
    protected char[] missionRewardType = new char[]
        {
            's',    //0
            'f',    //1
            's',    //2
            's',    //3
            's',    //4
            's',    //5
            's',    //6
            'f',    //7
            's',    //8
            's',    //9
            's',    //10
            's',    //11
            's',    //12
            'f',    //13
            's',    //14
            's',    //15
            's',    //16
            's',    //17
            's',    //18
            'f',    //19
            's',    //20
            's',    //21
            'f',    //22
            'f',    //23
            'f',    //24
            'f',    //25
            'f',    //26
            'f',    //27
            'f',    //28
            'f',    //29
            'f',    //30
            'f',    //31
            'f',    //32
            'f',    //33
            'f',    //34
            'f',    //35
            'f',    //36
            'f',    //37
            'f',    //38
            'f',    //39
            'f',    //40
            'f',    //41
            'f',    //42
            'f',    //43
            'f',    //44
            'f',    //45
            'f',    //46
            'f',    //47
            'f',    //48
            'f'     //49
        }; //b = boss, m = module, f = funds, s = special (extra health/damage/module energy etc)
    protected int[] missionRewardValue = new int[]
        {
            1,      //0
            100,    //1
            1,      //2
            1,      //3
            1,      //4
            1,      //5
            1,      //6
            250,    //7
            1,      //8
            1,      //9
            1,      //10
            1,      //11
            1,      //12
            400,    //13
            1,      //14
            1,      //15
            1,      //16
            1,      //17
            1,      //18
            600,    //19
            1,      //20
            1,      //21
            700,    //22
            800,    //23
            900,    //24
            1000,   //25
            1200,   //26
            1500,   //27
            1,      //28
            1,      //29
            1,      //30
            1,      //31
            1,      //32
            1,      //33
            1,      //34
            1,      //35
            1,      //36
            1,      //37
            1,      //38
            1,      //39
            1,      //40
            1,      //41
            1,      //42
            1,      //43
            1,      //44
            1,      //45
            1,      //46
            1,      //47
            1,      //48
            1       //49
        };
    public GameObject missionPrefab;
    public Mission[] currentMissions;

    public void ResetTracker()
    {
        for (int checkMission = 0; checkMission < 2; checkMission++)
        {
            currentMissions[checkMission].counter = 0;
        }
    }

    public string RetrieveMissionTitle(int whichMission)
    {
        return titles[whichMission];
    }

    public string RetrieveMissionDescription(int whichMission)
    {
        return descriptions[whichMission];
    }

    public int RetrieveMissionGoal(int whichMission)
    {
        return missionGoals[whichMission];
    }

    public char RetrieveMissionRewardType(int whichMission)
    {
        return missionRewardType[whichMission];
    }

    public int RetrieveMissionRewardValue(int whichMission)
    {
        return missionRewardValue[whichMission];
    }
    
    public void CreateMissions()
    {
        currentMissions = new Mission[2];
        for (int creatingMission = 0; creatingMission < 2; creatingMission++)
        {
            GameObject instantiatedMission = Instantiate(missionPrefab) as GameObject;
            instantiatedMission.transform.parent = transform;
            currentMissions[creatingMission] = instantiatedMission.GetComponent<Mission>();
            currentMissions[creatingMission].missionTracker = this;
            currentMissions[creatingMission].counter = 0;
        }
    }

    public void UnsetCurrentMissions()
    {
        int curMission = 0;
        while (curMission < PlayerStats.totalMissions)
        {
            if (PlayerStats.playerStats.allMissionStatus[curMission] == 1)
            {
                PlayerStats.playerStats.allMissionStatus[curMission] = 0;
            }
            curMission++;
        }
        for (int checkMission = 0; checkMission < 2; checkMission++)
        {
            currentMissions[checkMission].missionID = 0;
        }
    }

    public void IncrementMissionProgress(int missionID, int counterIncrement)
    {
        if (PlayerStats.playerStats.allMissionStatus[missionID] == 1)
        {
            for (int checkMission = 0; checkMission < 2; checkMission++)
            {   
                if (currentMissions[checkMission].missionID == missionID)
                {
                    currentMissions[checkMission].IncrementCounter(counterIncrement);
                    checkMission++;
                }
            }
        }
    }

    public void CheckMissionsComplete()
    {
        for (int checkMission = 0; checkMission < 2; checkMission++)
        {
            int missionID = currentMissions[checkMission].missionID;
            if (currentMissions[checkMission].CheckMissionComplete() && PlayerStats.playerStats.allMissionStatus[missionID] < 3)
            {
                PlayerStats.playerStats.allMissionStatus[missionID] = 2; //mark mission complete
            }
        }
    }

    public void CheckBothMissions(int whichLevel)
    {
        UnsetCurrentMissions();
        for (int whichMission = 0; whichMission < 2; whichMission++)
        {
            SwapNewMission(whichLevel, whichMission, false);
        }
    }

    public bool StartMissionCheck()
    {
        bool returnBool = false;
        int whichMission = 0;
        
        for (int getMission = 0; getMission < PlayerStats.playerStats.allMissionStatus.Length; getMission++)
        {
            if (PlayerStats.playerStats.allMissionStatus[getMission] == 2)
            {
                returnBool = true;
                currentMissions[whichMission].missionID = getMission;
                whichMission++;
            }
        }

        return returnBool;
    }
    
    public void SwapNewMission(int whichLevel, int whichMission, bool repeatLevel)
    {
        int nextMission = whichLevel * 2 + whichMission;
        if (PlayerStats.playerStats.allMissionStatus[nextMission] < 3)
        {
            PlayerStats.playerStats.allMissionStatus[nextMission] = 1;
            currentMissions[whichMission].counter = 0;
        }
        currentMissions[whichMission].missionID = nextMission;
    }
}