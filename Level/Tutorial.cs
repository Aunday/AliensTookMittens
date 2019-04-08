using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour
{
    public GameManager gameManager;
    private Board board;
    private Enemy enemy;
    private BoxCollider gridCollider;
    public GameObject images;
    private SpriteRenderer fader;
    private SpriteRenderer boardFade;
    public GameObject arrow;
    public GameObject arrow2;
    public GameObject arrow3;
    public SpriteRenderer spriteBox;
    public Animator animatorbox;
    public TextMesh textMesh;
    public bool pauseTutorial = false;
    private Space space;
    private Transform transformSpace;
    List<Icon> allMatches = new List<Icon>();
    private int[,] iconArray = new int[,]
    {
        { 0,1,4,5,5,1,0 },
        { 2,2,5,3,1,3,2 },
        { 3,2,1,0,5,0,4 },
        { 5,0,0,3,0,0,2 },
        { 4,3,4,4,1,4,2 },
        { 5,2,0,2,1,3,4 },
        { 2,1,0,2,5,3,2 },

        { 3,5,4,5,0,5,5 },
        { 3,1,0,4,1,1,4 },
        { 3,1,0,4,1,1,4 },
        { 3,1,0,4,1,1,5 },
        { 3,5,1,3,4,1,5 },
        { 3,5,1,3,4,5,5 }
    };
    private int[,] iconArrayFinal = new int[,]
    {
        { 0,1,5,4,4,5,5 },
        { 2,2,1,5,1,1,5 },
        { 3,1,1,2,1,4,4 },
        { 3,5,0,1,0,2,3 },
        { 4,2,4,2,4,1,0 },
        { 5,2,4,4,5,5,2 },
        { 2,3,5,2,5,1,2 }
    };

    public IEnumerator FirstHalf()
    {
        gameManager.missionTracker.CheckBothMissions(0);
        yield return StartCoroutine(gameManager.StartLevel(0));
        yield return new WaitForSeconds(0.01f);
        space = gameManager.space;
        transformSpace = space.transform;
        board = gameManager.board;
        gridCollider = board.GetComponent<BoxCollider>();
        fader = images.transform.GetChild(0).GetComponent<SpriteRenderer>();
        boardFade = board.frontFade;
        space.ToggleEnemyCreation(false);
        board.numUniqueIcons = 6;

        boardFade.enabled = true;

        //spawn first enemy, to showcase how enemies attack, and blast matches
        enemy = space.CreateEnemy(1);
        enemy.SpawnEnemy(0.0f, 0);
        enemy.enemySpeed *= 1.9f;
        space.enemy.Add(enemy);
        gridCollider.enabled = false;

        //set predefined grid
        for (int row = 0; row < Board.GridRows; row++)
        {
            for (int column = 0; column < Board.GridColumns; column++)
            {
                Icon curIcon = board.boardIcons[column, Mathf.Abs(row - Board.GridRows + 1)];
                int curType = iconArray[row, column];
                curIcon.IconFace[curIcon.iconType].enabled = false;
                curIcon.iconType = curType;
                curIcon.IconFace[curType].enabled = true;
            }
        }
        ObstructMatches();
        while (enemy.transform.localPosition.x > 3.25f)
        {
            yield return null;
        }
        PlayerStats.playerStats.messageStatus[0] = 0;
        StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "What do we have here?"));
        while (enemy.transform.localPosition.x > 0.9f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);

        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Did that alien just shoot\nus?")));
        enemy.TogglePause();
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Oh that is it! Blast 'em!")));

        //highlight first match
        gridCollider.size = new Vector2(0.71f, 4.739f);
        gridCollider.center = new Vector2(2.031f, 2.031f);
        Highlight("FadeMatch0", "DRAG TO MATCH", 180.0f, 0.0f, -3.02f, true);
        images.SetActive(true);
        allMatches.Add(board.boardIcons[1, 3]);  
        allMatches.Add(board.boardIcons[2, 3]);  
        allMatches.Add(board.boardIcons[3, 4]);  
        allMatches.Add(board.boardIcons[4, 3]);  
        allMatches.Add(board.boardIcons[5, 3]);
        foreach (Icon icon in allMatches)
        {
            icon.iconType = 0;
        }
        while (!allMatches[0].iconMobile)
        {
            foreach (Icon icon in allMatches)
            {
                icon.HighlightAdd();
            }
            yield return null;
        }
        RemoveTip();
        for (int column = 1; column < 6; column++)
        {
            Icon curIcon = board.boardIcons[column, 6];
            int curType = iconArray[7, column];
            curIcon.IconFace[curIcon.iconType].enabled = false;
            curIcon.iconType = curType;
            curIcon.IconFace[curType].enabled = true;
        }

        board.boardIcons[1, 6].HighlightRemove();
        board.boardIcons[2, 6].HighlightRemove();
        board.boardIcons[3, 6].HighlightRemove();
        board.boardIcons[4, 6].HighlightRemove();
        board.boardIcons[5, 6].HighlightRemove();
        ObstructMatches();
        while (board.boardIcons[4, 6].iconMobile)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        gridCollider.enabled = false;
        enemy.TogglePause();

        //spawn second enemy, to showcase laser
        enemy = space.CreateEnemy(0);
        enemy.SpawnEnemy(20.0f, 0);
        enemy.enemySpeed *= 1.5f;
        enemy.enemyHP = 60.0f;
        space.enemy.Add(enemy);

        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Good.....Another one?\nTry the laser!")));

        while (enemy.transform.localPosition.x > 0.7f)
        {
            yield return null;
        }
        enemy.TogglePause();
        gridCollider.size = new Vector2(4.739f, 0.71f);
        gridCollider.center = new Vector2(2.031f, 2.031f);

        Highlight("FadeMatch1", "DRAGGING MOVES\nENTIRE ROW/COLUMN", 270.0f, 1.41f, -2.1f, true);
        images.SetActive(true);
        board.boardIcons[4, 1].iconType = 1;
        board.boardIcons[4, 2].iconType = 1;
        board.boardIcons[2, 3].iconType = 1;
        board.boardIcons[4, 4].iconType = 1;
        while (!board.boardIcons[4, 6].iconMobile)
        {
            board.boardIcons[4, 1].HighlightAdd();
            board.boardIcons[4, 2].HighlightAdd();
            board.boardIcons[2, 3].HighlightAdd();
            board.boardIcons[4, 4].HighlightAdd();
            yield return null;
        }
        RemoveTip();

        for (int row = 1; row < 5; row++)
        {
            Icon curIcon = board.boardIcons[4, row + 2];
            int curType = iconArray[8, row + 2];
            curIcon.IconFace[curIcon.iconType].enabled = false;
            curIcon.iconType = curType;
            curIcon.IconFace[curType].enabled = true;
        }
        board.boardIcons[4, 3].HighlightRemove();
        board.boardIcons[4, 4].HighlightRemove();
        board.boardIcons[4, 5].HighlightRemove();
        board.boardIcons[4, 6].HighlightRemove();
        ObstructMatches();
        while (board.boardIcons[4, 2].iconMobile)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        gridCollider.enabled = false;

        //start shield example
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "We should raise the\nshield.")));

        yield return new WaitForSeconds(0.3f);
        gridCollider.size = new Vector2(0.71f, 4.739f);
        gridCollider.center = new Vector2(0.71f, 2.031f);
        Highlight("FadeMatch2", "ARMOR                                      \n\n\n\nSHIELD                                      \n\n\n\nCAN DRAG IN\nEITHER DIRECTION\n\n\n\n\n\n\n\n", 0.0f, -1.42f, 1.7f, true);
        images.SetActive(true);
        textMesh.transform.localPosition = new Vector2(0.0f, 0.08f);
        arrow.GetComponent<Animator>().Play(0);
        arrow2.SetActive(true);
        board.boardIcons[1, 6].iconType = 5;
        board.boardIcons[2, 4].iconType = 5;
        board.boardIcons[3, 4].iconType = 5;
        while (!board.boardIcons[1, 6].iconMobile)
        {
            board.boardIcons[1, 6].HighlightAdd();
            board.boardIcons[2, 4].HighlightAdd();
            board.boardIcons[3, 4].HighlightAdd();
            yield return null;
        }
        textMesh.transform.localPosition = new Vector2(0.0f, 0.0f);

        RemoveTip();
        arrow2.SetActive(false);

        for (int column = 1; column < 4; column++)
        {
            Icon curIcon = board.boardIcons[column, 6];
            int curType = iconArray[9, column];
            curIcon.IconFace[curIcon.iconType].enabled = false;
            curIcon.iconType = curType;
            curIcon.IconFace[curType].enabled = true;
        }
        board.boardIcons[1, 6].HighlightRemove();
        board.boardIcons[2, 6].HighlightRemove();
        board.boardIcons[3, 6].HighlightRemove();
        ObstructMatches();
        while (board.boardIcons[1, 6].iconMobile)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        gridCollider.enabled = false;
        enemy.TogglePause();

        //spawn third enemy, to showcase projectile defense
        enemy = space.CreateEnemy(10);
        enemy.SpawnEnemy(0.0f, 0);
        enemy.enemySpeed *= 2.5f;
        enemy.enemyHP = 50.0f;

        space.enemy.Add(enemy);
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Things are getting\ninteresting.")));

        while (transformSpace.childCount < 4)
        {
            yield return null;
        }
        
        StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Defense time."));
        Transform missle1 = transformSpace.GetChild(3);
        while (missle1.localPosition.x > -0.55f)
        {
            yield return null;
        }
        enemy.TogglePause();
        missle1.GetComponent<EnemyProjectile>().TogglePause();

        gridCollider.size = new Vector2(4.739f, 0.71f);
        gridCollider.center = new Vector2(2.031f, 1.354f);
        Highlight("FadeMatch3", "BLASTS ALSO\nDESTROY ENEMY\nPROJECTILES", 90.0f, -1.4f, -2.8f, true);
        images.SetActive(true);
        board.boardIcons[2, 0].iconType = 0;
        board.boardIcons[2, 1].iconType = 0;
        board.boardIcons[4, 2].iconType = 0;
        while (!board.boardIcons[2, 6].iconMobile)
        {
            board.boardIcons[2, 0].HighlightAdd();
            board.boardIcons[2, 1].HighlightAdd();
            board.boardIcons[4, 2].HighlightAdd();
            yield return null;
        }
        RemoveTip();

        for (int row = 0; row < 3; row++)
        {
            Icon curIcon = board.boardIcons[2, row + 4];
            int curType = iconArray[10, row + 4];
            curIcon.IconFace[curIcon.iconType].enabled = false;
            curIcon.iconType = curType;
            curIcon.IconFace[curType].enabled = true;
        }
        board.boardIcons[2, 4].HighlightRemove();
        board.boardIcons[2, 5].HighlightRemove();
        board.boardIcons[2, 6].HighlightRemove();
        ObstructMatches();
        while (board.boardIcons[2, 6].iconMobile)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        gridCollider.enabled = false;
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "We have rockets too.\nJust need to get some\nenergy.")));

        gridCollider.size = new Vector2(4.739f, 0.71f);
        gridCollider.center = new Vector2(2.031f, 1.354f);
        arrow.transform.rotation = Quaternion.identity;
        Highlight("FadeMatch4", "PURPLE, GREEN, AND\nYELLOW ICONS\nPROVIDE ENERGY", 270.0f, 1.35f, -2.8f, true);
        images.SetActive(true);
        arrow3.SetActive(true);
        board.boardIcons[1, 2].iconType = 3;
        board.boardIcons[5, 0].iconType = 3;
        board.boardIcons[5, 1].iconType = 3;
        board.boardIcons[5, 3].iconType = 3;
        board.boardIcons[5, 4].iconType = 3;
        while (!board.boardIcons[5, 6].iconMobile)
        {
            board.boardIcons[5, 0].HighlightAdd();
            board.boardIcons[5, 1].HighlightAdd();
            board.boardIcons[1, 2].HighlightAdd();
            board.boardIcons[5, 3].HighlightAdd();
            board.boardIcons[5, 4].HighlightAdd();
            yield return null;
        }
        gameManager.topPanel.IncreaseEnergy(3, 0);
        RemoveTip();
        arrow3.SetActive(false);

        for (int row = 0; row < 5; row++)
        {
            Icon curIcon = board.boardIcons[5, row + 2];
            int curType = iconArray[11, row + 2];
            curIcon.IconFace[curIcon.iconType].enabled = false;
            curIcon.iconType = curType;
            curIcon.IconFace[curType].enabled = true;
        }
        board.boardIcons[5, 2].HighlightRemove();
        board.boardIcons[5, 3].HighlightRemove();
        board.boardIcons[5, 4].HighlightRemove();
        board.boardIcons[5, 5].HighlightRemove();
        board.boardIcons[5, 6].HighlightRemove();
        ObstructMatches();
        while (board.boardIcons[5, 6].iconMobile)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        gridCollider.enabled = false;
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Let 'em fly.")));
        gameManager.topPanel.abilities[0].GetComponent<BoxCollider>().enabled = true;
        Highlight("FadeMatch5", "TAP MODULES\nTO USE THEM", 0.0f, -1.8f, 4.0f, false);
        images.SetActive(true);

        while (gameManager.topPanel.moduleEnergyCurrent[0] > 9)
        {
            yield return null;
        }
        RemoveTip();

        yield return new WaitForSeconds(0.1f);
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Great work!\nGrab resources in the lull.")));
        enemy.TogglePause();

        gridCollider.size = new Vector2(4.739f, 0.71f);
        gridCollider.center = new Vector2(2.031f, 2.031f);
        Highlight("FadeMatch6", "CRYSTALS\nPROVIDE RESOURCES\n\n\n\n\n\n\n\n\n\n", 270.0f, 2.8f, -2.1f, true);
        images.SetActive(true);
        board.boardIcons[4, 3].iconType = 4;
        board.boardIcons[6, 4].iconType = 4;
        board.boardIcons[6, 2].iconType = 4;
        board.boardIcons[6, 1].iconType = 4;
        while (!board.boardIcons[6, 6].iconMobile)
        {
            board.boardIcons[4, 3].HighlightAdd();
            board.boardIcons[6, 4].HighlightAdd();
            board.boardIcons[6, 2].HighlightAdd();
            board.boardIcons[6, 1].HighlightAdd();
            yield return null;
        }
        RemoveTip();
        for (int row = 1; row < 5; row++)
        {
            Icon curIcon = board.boardIcons[6, row + 2];
            int curType = iconArray[12, row + 2];
            curIcon.IconFace[curIcon.iconType].enabled = false;
            curIcon.iconType = curType;
            curIcon.IconFace[curType].enabled = true;
        }

        space.SpawnEnemy();
        space.enemy[0].enemyHP = 75.0f;
        gridCollider.center = new Vector2(2.031f, 2.031f);
        gridCollider.size = new Vector2(4.739f, 4.739f);
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "I'll let you finish off the\nlast one.")));

        //set predefined grid
        for (int row = 0; row < Board.GridRows; row++)
        {
            for (int column = 0; column < Board.GridColumns; column++)
            {
                Icon curIcon = board.boardIcons[column, Mathf.Abs(row - Board.GridRows + 1)];
                int curType = iconArrayFinal[row, column];
                curIcon.iconType = curType;
            }
        }
        gridCollider.enabled = true;
        boardFade.enabled = false;

        while (space.enemy.Count != 0.0f)
        {
            yield return null;
            if (gameManager.ship.GetPlayerHealth() < 33.0f)
            {
                gameManager.ship.IncreasePlayerHealth(33.0f - gameManager.ship.GetPlayerHealth());
            }
        }
        
        PlayerStats.playerStats.messageStatus[0] = 99;

        yield return (StartCoroutine(space.CreateBoss()));

        while (space.enemy.Count != 0.0f)
        {
            yield return null;
            if (gameManager.ship.GetPlayerHealth() < 33.0f)
            {
                gameManager.ship.IncreasePlayerHealth(33.0f - gameManager.ship.GetPlayerHealth());
            }
        }
        PlayerStats.playerStats.levelThreat[0] = gameManager.topPanel.currentThreat;
        gameManager.missionTracker.IncrementMissionProgress(0, 1);

        board.numUniqueIcons = 7;

    }

    public IEnumerator SecondHalf()
    {
        gameManager.missionTracker.CheckBothMissions(0);
        board = gameManager.board;
        board.numUniqueIcons = 6;
        yield return StartCoroutine(gameManager.StartLevel(0));

        yield return new WaitForSeconds(0.01f);
        space = gameManager.space;
        gridCollider = board.GetComponent<BoxCollider>();
        fader = images.transform.GetChild(0).GetComponent<SpriteRenderer>();
        space.ToggleEnemyCreation(false);

        while (PlayerStats.playerStats.messageStatus[1] == 0)
        {
            yield return null;
        }
        PlayerStats.playerStats.messageStatus[1] = 0;
        RemoveTip();

        yield return (StartCoroutine(gameManager.messageScreen.PlayMessageNoPause(0, "Look out!")));

        yield return (StartCoroutine(space.CreateBoss()));

        yield return new WaitForSeconds(2.0f);
        images.SetActive(false);
        gridCollider.enabled = true;

        while (space.enemy.Count != 0.0f)
        {
            yield return null;
            if (gameManager.ship.GetPlayerHealth() < 33.0f)
            {
                gameManager.ship.IncreasePlayerHealth(33.0f - gameManager.ship.GetPlayerHealth());
            }
        }
        gameManager.missionTracker.IncrementMissionProgress(0, 1);
        PlayerStats.playerStats.messageStatus[1] = 99;
        board.numUniqueIcons = 7;
    }

    public IEnumerator HangerTutorial()
    {
        Hanger hanger = gameManager.hanger;
        fader = images.transform.GetChild(0).GetComponent<SpriteRenderer>();
        fader.sortingOrder = 114;
        gridCollider = hanger.buttonUpgradeShip.GetComponent<BoxCollider>();
        hanger.ToggleUpgradeButtons(false);
        gameManager.ToggleFade(false);
        boardFade = gameManager.board.frontFade;
        gameManager.optionsMenu.ToggleOptionMenuButton(false);

        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "Welcome to my humble\nabode. I know what you\nare thinking, \"This looks\nlike a heap of scrap\nmetal.\"\nWell it isn't...Okay, I guess\nit is. But it is OUR heap of\nscrap metal!\n\n\nBut enough of that, we\ngot some ship\nmodifications while we\nwere out! Let's check 'em\nout!")));

        textMesh.text = "";
        textMesh.transform.localPosition = new Vector2(0.0f, -2.02f);
        images.SetActive(true);
        Highlight("FadeMatch9", "SHIP CAN BE UPGRADED\nIN THE HANGER", 180.0f, 0.0f, -3.6f, true);
        gridCollider.GetComponent<SpriteRenderer>().sortingOrder = 115;

        GameObject upgradePanelObject = hanger.buttonUpgradeArmament.upgradePanelObject;
        UpgradePanelArmament upgradePanelArmament = upgradePanelObject.GetComponent<UpgradePanelArmament>();
        while (!upgradePanelObject.activeSelf)
        {
            yield return null;
        }

        gridCollider.GetComponent<SpriteRenderer>().sortingOrder = 46;
        GameObject objectButton = upgradePanelArmament.buttonUpgrade0;
        upgradePanelArmament.transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        upgradePanelArmament.subPanel1.enabled = false;
        upgradePanelArmament.subPanel2.enabled = false;
        objectButton.GetComponent<BoxCollider>().enabled = false;
        upgradePanelArmament.buttonUpgrade1.GetComponent<BoxCollider>().enabled = false;
        upgradePanelArmament.buttonUpgrade2.GetComponent<BoxCollider>().enabled = false;
        textMesh.text = "";
        arrow.SetActive(false);
        fader.sortingOrder = 114;
        gameManager.messageScreen.spriteGif.sortingOrder = 116;
        gameManager.messageScreen.spriteMessageBox.sortingOrder = 115;
        gameManager.messageScreen.textMessage.GetComponent<MeshRenderer>().sortingOrder = 116;
        gameManager.messageScreen.textMessage2.GetComponent<MeshRenderer>().sortingOrder = 117;
        gameManager.messageScreen.tapToContinue.GetComponent<MeshRenderer>().sortingOrder = 115;
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "Let's beef up our armor\nsome more. I would rather\nnot explode in space...\nor anywhere.")));
        SpriteRenderer spriteButton = objectButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
        //fader.sortingOrder = 114;
        arrow.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 115;
        spriteButton.sortingOrder = 115;
        objectButton.transform.GetChild(2).GetComponent<MeshRenderer>().sortingOrder = 115;
        objectButton.GetComponent<BoxCollider>().enabled = true;

        yield return new WaitForSeconds(0.2f);
        Highlight("FadeMatch9", "SELECT\nARMOR UPGRADE", 180.0f, -1.54f, -1.35f, false);
        textMesh.transform.localPosition = new Vector2(-0.81f, 0.18f);
        textMesh.GetComponent<MeshRenderer>().sortingOrder = 115;
        fader.sortingOrder = 114;
        arrow.SetActive(true);

        while (spriteButton.color.b < 1.0f)
        {
            yield return null;
        }
        objectButton.GetComponent<BoxCollider>().enabled = false;
        spriteButton.sortingOrder = 110;
        objectButton.transform.GetChild(2).GetComponent<MeshRenderer>().sortingOrder = 110;

        Highlight("FadeMatch9", "SELECT UPGRADE", 180.0f, 0.77f, -3.95f, false);
        upgradePanelArmament.buttonUpgradeConfirmSprite.sortingOrder = 115;
        upgradePanelArmament.buttonUpgradeConfirmText.GetComponent<MeshRenderer>().sortingOrder = 116;
        textMesh.transform.localPosition = new Vector2(0.76f, -2.58f);
        fader.sortingOrder = 114;

        while (PlayerStats.playerStats.upgradeStatus[9] < 2)
        {
            yield return null;
        }
        //spriteButton.sortingOrder = 110;
        upgradePanelArmament.buttonUpgradeConfirmSprite.sortingOrder = 110;
        upgradePanelArmament.buttonUpgradeConfirmText.GetComponent<MeshRenderer>().sortingOrder = 111;

        textMesh.text = "";
        arrow.SetActive(false);
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "Can never have too much\narmor. Close the panel\nwhen you are done.")));
        spriteBox.sortingOrder = 113;
        fader.enabled = false;
        spriteBox.enabled = true;
        animatorbox.enabled = true;
        spriteBox.transform.localPosition = new Vector2(1.96f, 3.78f);
        upgradePanelArmament.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
        upgradePanelArmament.subPanel1.enabled = true;
        upgradePanelArmament.subPanel2.enabled = true;
        objectButton.GetComponent<BoxCollider>().enabled = true;
        upgradePanelArmament.buttonUpgrade1.GetComponent<BoxCollider>().enabled = true;
        upgradePanelArmament.buttonUpgrade2.GetComponent<BoxCollider>().enabled = true;

        while (upgradePanelObject.activeSelf)
        {
            yield return null;
        }

        spriteBox.sortingOrder = 104;
        hanger.ToggleUpgradeButtons(false);
        //spriteBox.enabled = false;
        //animatorbox.enabled = false;

        fader.enabled = true;
        spriteBox.transform.localPosition = new Vector2(2.2f, 0.8f);
        spriteBox.transform.localScale = new Vector2(1.96f, 6.0f);
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "You ready to save\nMittens? Tap the\narrows in front of the\nship when you want to\nget back out there.")));
        fader.enabled = false;
        hanger.ToggleUpgradeButtons(true);
        //spriteBox.enabled = true;
        //animatorbox.enabled = true;
        textMesh.transform.localPosition = new Vector2(1.3f, 2.0f);
        textMesh.text = "TAP ARROWS\nTO TAKE OFF";
        textMesh.GetComponent<MeshRenderer>().sortingOrder = 104;

        Transform transformShip = gameManager.ship.transform;
        GameObject levelOne = hanger.buttonLevelSelect[0].animator;
        BoxCollider boxColliderEnterLevel = hanger.buttonEnterLevel.GetComponent<BoxCollider>();
        boxColliderEnterLevel.enabled = false;
        hanger.buttonLevelSelect[0].GetComponent<BoxCollider>().enabled = false;
        hanger.buttonLevelSelect[1].GetComponent<BoxCollider>().enabled = false;
        while (transformShip.localScale.y < 2.4f)
        {
            yield return null;
        }
        hanger.buttonHangerToLevelSelect.GetComponent<BoxCollider>().enabled = false;
        textMesh.text = "";
        spriteBox.enabled = false;
        animatorbox.enabled = false;
        
        while (transformShip.localPosition.x > -2.0f)
        {
            yield return null;
        }

        fader.enabled = true;
        spriteBox.transform.localPosition = new Vector2(-0.55f, -1.4f);
        spriteBox.transform.localScale = new Vector2(7.0f, 4.0f);
        spriteBox.enabled = true;
        animatorbox.enabled = true;
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "And now to decide which\nsystem to fly to. We can\nselect any discovered\nsystem.\n\nI have already targeted\nFabris, the newly\ndiscovered system. Just\ntap the orange box and\nwe will head out.")));
        gameManager.messageScreen.spriteGif.sortingOrder = 104;
        gameManager.messageScreen.spriteMessageBox.sortingOrder = 102;
        gameManager.messageScreen.textMessage.GetComponent<MeshRenderer>().sortingOrder = 103;
        gameManager.messageScreen.textMessage2.GetComponent<MeshRenderer>().sortingOrder = 104;
        gameManager.messageScreen.tapToContinue.GetComponent<MeshRenderer>().sortingOrder = 104;
        fader.enabled = false;
        hanger.buttonLevelSelect[0].GetComponent<BoxCollider>().enabled = true;
        hanger.buttonLevelSelect[1].GetComponent<BoxCollider>().enabled = true;

        while (transformShip.localPosition.y < 1.55f)
        {
            if (levelOne.activeSelf)
            {
                boxColliderEnterLevel.enabled = false;
                spriteBox.transform.localScale = new Vector3(2.0f, 1.6f, 2.0f);
                spriteBox.transform.localPosition = new Vector3(-2.08f, 2.75f, 0.0f);
            }
            else
            {
                boxColliderEnterLevel.enabled = true;
                spriteBox.transform.localPosition = new Vector2(-0.55f, -1.4f);
                spriteBox.transform.localScale = new Vector2(7.0f, 4.0f);
            }
            yield return null;
        }

        hanger.buttonLevelSelect[0].GetComponent<BoxCollider>().enabled = true;
        gameManager.optionsMenu.ToggleOptionMenuButton(true);

        Destroy(gameObject);
    }

    public IEnumerator HangerTutorialPartTwo()
    {
        Hanger hanger = gameManager.hanger;
        fader = images.transform.GetChild(0).GetComponent<SpriteRenderer>();
        fader.sortingOrder = 92;
        gridCollider = hanger.buttonUpgradeAbility.GetComponent<BoxCollider>();
        boardFade = gameManager.board.frontFade;

        hanger.ToggleUpgradeButtons(false);
        gameManager.ToggleFade(false);
        gameManager.optionsMenu.ToggleOptionMenuButton(false);

        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "We just unlocked our\nfourth module! As cool as\nthat is, I have a\nconfession to make.\n\nOur ship is only fancy\nenough to bring three\nmodules at any time. We\ncan choose our three in\nthe module panel.")));

        textMesh.text = "";
        textMesh.transform.localPosition = new Vector2(0.0f, -2.02f);
        images.SetActive(true);
        Highlight("FadeMatch9", "MODULES CAN BE\nSELECTED IN THE\nHANGER", 180.0f, 1.87f, -3.6f, true);
        gridCollider.GetComponent<SpriteRenderer>().sortingOrder = 115;

        GameObject upgradePanelObject = hanger.upgradePanelAbility.gameObject;
        UpgradePanelAbility upgradePanelModule = upgradePanelObject.GetComponent<UpgradePanelAbility>();
        while (!upgradePanelObject.activeSelf)
        {
            yield return null;
        }
        upgradePanelModule.ChangeTabDetails(0);
        gridCollider.GetComponent<SpriteRenderer>().sortingOrder = 46;
        BoxCollider subpanelButton0 = upgradePanelModule.transform.GetChild(7).GetComponent<BoxCollider>();
        BoxCollider subpanelButton1 = upgradePanelModule.transform.GetChild(8).GetComponent<BoxCollider>();
        BoxCollider upgradeButton0 = upgradePanelModule.boxColliderUpgradeAbility[0];
        BoxCollider upgradeButton1 = upgradePanelModule.boxColliderUpgradeAbility[1];
        upgradeButton0.enabled = false;
        upgradeButton1.enabled = false;
        subpanelButton0.enabled = false;
        subpanelButton1.enabled = false;
        upgradePanelModule.buttonUpgradeConfirm.enabled = false;
        textMesh.text = "";
        arrow.SetActive(false);
        fader.sortingOrder = 114;
        gameManager.messageScreen.spriteGif.sortingOrder = 116;
        gameManager.messageScreen.spriteMessageBox.sortingOrder = 115;
        gameManager.messageScreen.textMessage.GetComponent<MeshRenderer>().sortingOrder = 116;
        gameManager.messageScreen.textMessage2.GetComponent<MeshRenderer>().sortingOrder = 117;
        gameManager.messageScreen.tapToContinue.GetComponent<MeshRenderer>().sortingOrder = 116;
        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "The modules selected in\nthis panel will be the ones\nthat we take with us into\nbattle.\n\nLet's go ahead and equip\nthe new module, Mega\nLaser.")));
        SpriteRenderer spriteButton = upgradePanelModule.spriteRendererUpgradeAbility[1];
        arrow.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 115;
        spriteButton.sortingOrder = 115;
        upgradeButton1.enabled = true;

        Highlight("FadeMatch9", "SELECT MEGA LASER", 0.0f, -0.9f, 2.3f, false);
        fader.sortingOrder = 114;
        textMesh.transform.localPosition = new Vector2(0.0f, -1.1f);
        textMesh.GetComponent<MeshRenderer>().sortingOrder = 115;
        arrow.SetActive(true);

        SpriteRenderer laserNew = upgradePanelModule.spriteUpgradeNew[4];
        while (laserNew.enabled)
        {
            yield return null;
        }
        upgradePanelModule.buttonUpgradeConfirm.enabled = false;
        images.SetActive(false);

        yield return (StartCoroutine(gameManager.messageScreen.PlayMessagePause(0, "Feel free to choose any\nmodule that you would like\nto use.")));
        gameManager.messageScreen.spriteGif.sortingOrder = 104;
        gameManager.messageScreen.spriteMessageBox.sortingOrder = 102;
        gameManager.messageScreen.textMessage.GetComponent<MeshRenderer>().sortingOrder = 103;
        gameManager.messageScreen.textMessage2.GetComponent<MeshRenderer>().sortingOrder = 104;
        gameManager.messageScreen.tapToContinue.GetComponent<MeshRenderer>().sortingOrder = 104;
        upgradePanelModule.buttonUpgradeConfirm.enabled = true;
        spriteButton.sortingOrder = 110;
        upgradeButton0.enabled = true;
        upgradeButton1.enabled = true;
        subpanelButton0.enabled = true;
        subpanelButton1.enabled = true;
        gameManager.optionsMenu.ToggleOptionMenuButton(true);

        Destroy(gameObject);
    }

    void Highlight(string fade, string messageText, float arrowRotation, float posX, float posY, bool enable)
    {
        fader.sprite = Resources.Load<Sprite>("Tutorial/" + fade);
        textMesh.text = messageText;
        arrow.transform.localPosition = new Vector2(posX, posY);
        arrow.transform.localEulerAngles = new Vector3(0.0f, 0.0f, arrowRotation);
        fader.sortingOrder = 92;
        gridCollider.enabled = enable;
        boardFade.enabled = false;
    }

    void RemoveTip()
    {
        boardFade.enabled = true;
        images.SetActive(false);
    }

    void ObstructMatches()
    {
        for (int row = 0; row < Board.GridRows; row++)
        {
            for (int column = 0; column < Board.GridColumns; column++)
            {
                Icon curIcon = board.boardIcons[column, Mathf.Abs(row - Board.GridRows + 1)];
                curIcon.iconType += (6 + column + row * 6) * 6;
            }
        }
    }
}