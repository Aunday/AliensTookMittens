using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public const int GridColumns = 7;
    public const int GridRows = 7;
    public int numUniqueIcons;
    public GameManager gameManager;
    public GameObject iconPrefab;
    private Vector2 clickStart;
    enum SlideDirection { noDir, XDir, YDir };
    private SlideDirection slideLock;
    private bool saveChange = false;
    private Icon proxyIconX;
    private Icon proxyIconY;
    public Icon[,] boardIcons;
    public int[,][] tempBoardIcons;
    public Vector2 movedAmount;
    public Transform transformBoard;
    public int iconsInMotion;
    private float scaleRatioY;
    private float scaleRatioX;
    private int line = 0;
    private bool movingX = true;
    private PrefabPools prefabPools;
    public SpriteRenderer backLayer;
    public SpriteRenderer frontFade;
    public Animator backLayerAnimation;
    private BoxCollider boxCollider;
    private Ship ship;
    private TopPanel topPanel;
    private MissionTracker missionTracker;
    List<Icon> highlightIcons = new List<Icon>();
    private float noMatchCounter = 0.0f;
    private Icon[] noMatchIcons = new Icon[3];
    public Animator lowArmorWarning;
    private SpriteRenderer spriteLowArmorWarning;

    public void Start()
    {
        spriteLowArmorWarning = lowArmorWarning.GetComponent<SpriteRenderer>();
        iconsInMotion = 0;
    }

    public IEnumerator ActivateLowArmorWarning()
    {
        lowArmorWarning.enabled = true;
        yield return new WaitForSeconds(5.0f);
        DeactivateLowArmorWarning();
    }

    void DeactivateLowArmorWarning()
    {
        lowArmorWarning.enabled = false;
        if (spriteLowArmorWarning.enabled)
        {
            spriteLowArmorWarning.enabled = false;
        }
    }

    public void GenerateBoard()
    {
        boxCollider = GetComponent<BoxCollider>();
        Random.seed = (int)System.DateTime.Now.Ticks;
        boardIcons = new Icon[GridColumns, GridRows];
        tempBoardIcons = new int[GridColumns, GridRows][];
        for (int column = 0; column < GridColumns; column++)
        {
            for (int row = 0; row < GridRows; row++)
            {
                tempBoardIcons[column, row] = new int[] { column, row };
            }
        }
        numUniqueIcons = 7;
        scaleRatioY = gameManager.scaleRatioY;
        scaleRatioX = gameManager.scaleRatioX;
        prefabPools = gameManager.prefabPools;
        ship = gameManager.ship;
        missionTracker = gameManager.missionTracker;
        topPanel = gameManager.topPanel;

        transformBoard.gameObject.SetActive(false);
        for (int row = 0; row < GridRows; row++)
        {
            for (int column = 0; column < GridColumns; column++)
            {
                GameObject instantiatedIcon = gameManager.InstantiateObject(iconPrefab, transform, column * 0.71f, row * 0.71f, 0, 1.0f, 1.0f, 1.0f);
                boardIcons[column, row] = instantiatedIcon.GetComponent<Icon>();
                boardIcons[column, row].board = this;
                boardIcons[column, row].iconRow = row;
                boardIcons[column, row].iconColumn = column;
                boardIcons[column, row].StartSetup();
            }
        }

        GameObject instantiatedProxyXIcon = gameManager.InstantiateObject(iconPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        proxyIconX = instantiatedProxyXIcon.GetComponent<Icon>();
        proxyIconX.board = this;
        proxyIconX.StartSetup();
        GameObject instantiatedProxyYIcon = gameManager.InstantiateObject(iconPrefab, transform, 0.0f, 30.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        proxyIconY = instantiatedProxyYIcon.GetComponent<Icon>();
        proxyIconY.board = this;
        proxyIconY.StartSetup();
        transformBoard.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        boxCollider.enabled = false;
        if (iconsInMotion > 0)
        {
            print("Error: input in OnMouseDown while iconInMotion > 0");
        }
        clickStart = Input.mousePosition;
        slideLock = SlideDirection.noDir;
    }

    int CalculateRowOrColumn(float startPos, int numLines)
    {
        int line = Mathf.FloorToInt((startPos - 0.316f * scaleRatioY) / (scaleRatioY * 1.035f));
        line = Mathf.Min(numLines - 1, line);
        line = Mathf.Max(0, line);
        return line;
    }

    void OnMouseDrag()
    {
        SlideDirection slideDir = slideLock;
        Vector2 curPosition = Input.mousePosition;
        Vector2 delta = curPosition - clickStart;
        float mouseDeltaX = Mathf.Abs(delta.x);
        float mouseDeltaY = Mathf.Abs(delta.y);

        ResetTempIconLocationsArray();

        //set row/column lock if we have moved a certain distance
        if (slideLock == SlideDirection.noDir)
        {
            if (mouseDeltaX > mouseDeltaY)
            {
                slideDir = SlideDirection.XDir;
                if (mouseDeltaX > (scaleRatioX * 0.5))
                {
                    slideLock = SlideDirection.XDir;
                    int curRow = CalculateRowOrColumn(clickStart.y, GridRows);
                    for (int curCol = 0; curCol < GridColumns; curCol++)
                    {
                        if (boardIcons[curCol, curRow].spriteFire.activeSelf)
                        {
                            boardIcons[curCol, curRow].DeactivateFire();
                        }
                    }
                }
                int column = CalculateRowOrColumn(clickStart.x, GridColumns);
                float columnAdjusted = column * 0.71f;

                if (boardIcons[column, 0].transform.localPosition.y != 0)
                {
                    for (int row = 0; row < GridRows; row++)
                    {
                        boardIcons[column, row].transform.localPosition = new Vector2(columnAdjusted, row * 0.71f);
                    }
                    proxyIconY.transform.localPosition = new Vector2(0.0f, 30.0f);
                }
            }
            else
            {
                slideDir = SlideDirection.YDir;
                if (mouseDeltaY > (scaleRatioY * 0.5))
                {
                    slideLock = SlideDirection.YDir;
                    int curCol = CalculateRowOrColumn(clickStart.x, GridColumns);
                    for (int curRow = 0; curRow < GridRows; curRow++)
                    {
                        if (boardIcons[curCol, curRow].spriteFire.activeSelf)
                        {
                            boardIcons[curCol, curRow].DeactivateFire();
                        }
                    }
                }
                int row = CalculateRowOrColumn(clickStart.y, GridRows);
                float rowAdjusted = row * 0.71f;

                if (boardIcons[0, row].transform.localPosition.x != 0)
                {
                    for (int column = 0; column < GridColumns; column++)
                    {
                        boardIcons[column, row].transform.localPosition = new Vector2(column * 0.71f, rowAdjusted);
                    }
                    proxyIconX.transform.localPosition = new Vector2(0.0f, 30.0f);
                }
            }
        }

        if (slideDir == SlideDirection.XDir) //moving row x
        {
            int row = CalculateRowOrColumn(clickStart.y, GridRows);
            line = row;
            movingX = true;
            float gridWidth = GridColumns * scaleRatioX;
            float columnsTraversed = delta.x / scaleRatioX;
            float rowAdjusted = row * 0.71f;

            for (int column = 0; column < GridColumns; column++)
            {
                Transform iconTransform = boardIcons[column, row].transform;
                //if we are still within +/- full length of grid, move row
                if (mouseDeltaX < gridWidth)
                {
                    iconTransform.localPosition = new Vector2((column + columnsTraversed + GridColumns) % GridColumns * 0.71f, rowAdjusted);
                    int oldColumn = column - Mathf.RoundToInt(columnsTraversed);
                    int newColumn = (oldColumn + GridColumns) % GridColumns;
                    tempBoardIcons[column, row][0] = newColumn;
                    movedAmount.x = columnsTraversed;
                }
                //else stop moving row and set to initial icon locations
                else
                {
                    iconTransform.localPosition = new Vector2(column * 0.71f, rowAdjusted);
                    tempBoardIcons[column, row][0] = column;
                    movedAmount.x = 0;
                }
            }

            //move pseudo icon around edges if needed
            if (mouseDeltaX < gridWidth)
            {
                proxyIconX.transform.localPosition = new Vector2(boardIcons[0, row].transform.localPosition.x % 0.71f - 0.71f, rowAdjusted);
                proxyIconX.IconFace[proxyIconX.iconType].enabled = false;
                Icon mimicIcon = boardIcons[((GridColumns - Mathf.FloorToInt(columnsTraversed)) - 1) % GridColumns, row];

                int newType = mimicIcon.iconType % numUniqueIcons;

                proxyIconX.iconType = newType;
                if (mimicIcon.animator[newType].enabled)
                {
                    proxyIconX.animator[newType].enabled = true;
                }
                else
                {
                    proxyIconX.animator[newType].enabled = false;
                    proxyIconX.IconFace[newType].color = Color.white;
                }
                if (mimicIcon.spriteAcid.enabled)
                {
                    proxyIconX.spriteAcid.enabled = true;
                    proxyIconX.animatorAcid.enabled = true;
                }
                else
                {
                    proxyIconX.spriteAcid.enabled = false;
                    proxyIconX.animatorAcid.enabled = false;
                }
                if (mimicIcon.spriteArrow.enabled)
                {
                    proxyIconX.spriteArrow.enabled = true;
                    proxyIconX.animatorHalf.enabled = true;
                    proxyIconX.spriteArrow.transform.rotation = mimicIcon.spriteArrow.transform.rotation;
                }
                else
                {
                    proxyIconX.spriteArrow.enabled = false;
                    proxyIconX.animatorHalf.enabled = false;
                }
                if (mimicIcon.spriteLock.enabled)
                {
                    proxyIconX.spriteLock.enabled = true;
                    proxyIconX.spriteLock.color = mimicIcon.spriteLock.color;
                }
                else
                {
                    proxyIconX.spriteLock.enabled = false;
                }
                proxyIconX.IconFace[newType].enabled = true;
            }
            else
            {
                proxyIconX.transform.localPosition = new Vector2(0.0f, 30.0f);
            }
        }
        else //moving column y
        {
            int column = CalculateRowOrColumn(clickStart.x, GridColumns);
            line = column;
            movingX = false;
            float gridHeight = GridRows * scaleRatioY;
            float rowsTraversed = delta.y / scaleRatioY;
            float columnAdjusted = column * 0.71f;

            for (int row = 0; row < GridRows; row++)
            {
                Transform iconTransform = boardIcons[column, row].transform;
                //if we are still within +/- full length of grid, move column
                if (mouseDeltaY < gridHeight)
                {
                    iconTransform.localPosition = new Vector2(columnAdjusted, ((row + rowsTraversed + GridRows) % GridRows * 0.71f));
                    int oldRow = row - Mathf.RoundToInt(rowsTraversed);
                    int newRow = (oldRow + GridRows) % GridRows;
                    tempBoardIcons[column, row][1] = newRow;
                    movedAmount.y = rowsTraversed;
                }
                //else stop moving column and set to initial icon locations
                else
                {
                    iconTransform.localPosition = new Vector2(columnAdjusted, row * 0.71f);
                    tempBoardIcons[column, row][1] = row;
                    movedAmount.y = 0;
                }
            }
            //move pseudo icon around edges if needed
            if (mouseDeltaY < gridHeight)
            {
                proxyIconY.transform.localPosition = new Vector2(columnAdjusted, boardIcons[column, 0].transform.localPosition.y % 0.71f - 0.71f); //(rowsTraversed + GridRows)
                proxyIconY.IconFace[proxyIconY.iconType].enabled = false;
                Icon mimicIcon = boardIcons[column, ((GridRows - Mathf.FloorToInt(rowsTraversed)) - 1) % GridRows];

                int newType = mimicIcon.iconType % numUniqueIcons;
                proxyIconY.iconType = newType;
                if (mimicIcon.animator[newType].enabled)
                {
                    proxyIconY.animator[newType].enabled = true;
                }
                else
                {
                    proxyIconY.animator[newType].enabled = false;
                    proxyIconY.IconFace[newType].color = Color.white;
                }
                //if (mimicIcon.spriteFire.activeSelf)
                //{
                //    proxyIconY.spriteFire.SetActive(true);
                //}
                //else
                //{
                //    proxyIconY.spriteFire.SetActive(false);
                //}
                //if (mimicIcon.iconBomb != null)
                //{
                //    //proxyIconY.spriteBomb.enabled = true;
                //    //proxyIconY.spriteBomb.sprite = mimicIcon.iconBomb.spriteRenderer.sprite;
                //}
                //else
                //{
                //    //proxyIconY.spriteBomb.enabled = false;
                //}
                if (mimicIcon.spriteAcid.enabled)
                {
                    proxyIconY.spriteAcid.enabled = true;
                    proxyIconY.animatorAcid.enabled = true;
                }
                else
                {
                    proxyIconY.spriteAcid.enabled = false;
                    proxyIconY.animatorAcid.enabled = false;
                }
                if (mimicIcon.spriteArrow.enabled)
                {
                    proxyIconY.spriteArrow.enabled = true;
                    proxyIconY.animatorHalf.enabled = true;
                    proxyIconY.spriteArrow.transform.rotation = mimicIcon.spriteArrow.transform.rotation;
                }
                else
                {
                    proxyIconY.spriteArrow.enabled = false;
                    proxyIconY.animatorHalf.enabled = false;
                }
                if (mimicIcon.spriteLock.enabled)
                {
                    proxyIconY.spriteLock.enabled = true;
                    proxyIconY.spriteLock.color = mimicIcon.spriteLock.color;
                }
                else
                {
                    proxyIconY.spriteLock.enabled = false;
                }
                proxyIconY.IconFace[newType].enabled = true;
            }
            else
            {
                proxyIconY.transform.localPosition = new Vector2(0.0f, 30.0f);
            }
        }
        CheckMatches(false, 0);
    }

    public bool CheckMatches(bool completeMatches, int chainMatch)
    {
        boxCollider.enabled = false;
        List<Icon> allMatches = new List<Icon>();
        Stack<int> matchIntensity = new Stack<int>();

        for (int row = 0; row < GridRows; row++)
        {
            List<Icon> matchIcons = new List<Icon>();
            int matchType = -1;
            for (int column = 0; column < GridColumns; column++)
            {
                int[] tmpIcon = tempBoardIcons[column, row];

                Icon tempIcon = boardIcons[tmpIcon[0], tmpIcon[1]];

                if (tempIcon.iconType != matchType)
                {
                    if (matchIcons.Count > 2)
                    {
                        MatchFound(matchIcons, allMatches, matchIcons.Count, matchIntensity);
                    }
                    matchIcons.Clear();
                }
                matchIcons.Add(tempIcon);
                matchType = tempIcon.iconType;
            }
            if (matchIcons.Count > 2)
            {
                MatchFound(matchIcons, allMatches, matchIcons.Count, matchIntensity);
            }
        }
        for (int column = 0; column < GridColumns; column++)
        {
            List<Icon> matchIcons = new List<Icon>();
            int matchType = -1;
            for (int row = 0; row < GridRows; row++)
            {
                int[] tmpIcon = tempBoardIcons[column, row];

                Icon tempIcon = boardIcons[tmpIcon[0], tmpIcon[1]];

                if (tempIcon.iconType != matchType)
                {
                    if (matchIcons.Count > 2)
                    {
                        MatchFound(matchIcons, allMatches, matchIcons.Count, matchIntensity);
                    }
                    matchIcons.Clear();
                }
                matchIcons.Add(tempIcon);
                matchType = tempIcon.iconType;
            }
            if (matchIcons.Count > 2)
            {
                MatchFound(matchIcons, allMatches, matchIcons.Count, matchIntensity);
            }
        }
        if (allMatches.Count > 0)
        {
            saveChange = true;
            if (completeMatches)
            {
                //score the match
                boxCollider.enabled = false;
                if (chainMatch == 1)
                {
                    ConfirmGridChanges();
                }
                ResetGridPositions();
                StartCoroutine(ScoreMatch(allMatches, matchIntensity, chainMatch));
                return false;
            }
            else
            {
                HighlightRemove(allMatches);
                HighlightAdd(allMatches);
                return false;
            }
        }
        else
        {
            saveChange = false;
            HighlightRemove(allMatches);
            if (iconsInMotion == 0)
            {
                boxCollider.enabled = true;
            }
            else
            {
                print("Caution: icons in motion at end of save change method.");
            }
            if (completeMatches)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void CheckCloseMatches()
    {
        boxCollider.enabled = false;
        List<Icon> allMatches = new List<Icon>();
        Stack<int> matchIntensity = new Stack<int>();

        Vector2 curPosition = Input.mousePosition;
        Vector2 delta = (curPosition - clickStart) / scaleRatioX;
        float deltaX = Mathf.Abs(delta.x) % 1.0f;
        float deltaY = Mathf.Abs(delta.y) % 1.0f;
        int startIconColumn;
        int startIconRow;

        if ((deltaX <= 0.8f && deltaX >= 0.2f) || (deltaY <= 0.8f && deltaY >= 0.2f))
        {
            startIconColumn = CalculateRowOrColumn(clickStart.x, GridColumns);
            startIconRow = CalculateRowOrColumn(clickStart.y, GridRows);

            //check on inner portion of line
            if ((deltaX <= 0.5f && deltaX >= 0.2f) || (deltaY <= 0.5f && deltaY >= 0.2f))
            {
                if (movingX) //if moving x
                {
                    //move row over a slot
                    int direction;
                    if (delta.x > 0)
                    {
                        direction = Mathf.CeilToInt(delta.x);
                    }
                    else
                    {
                        direction = Mathf.FloorToInt(delta.x);
                    }

                    for (int column = 0; column < GridColumns; column++)
                    {
                        tempBoardIcons[column, startIconRow][0] = (column - direction + GridColumns) % GridColumns;
                    }

                }
                else //else moving y
                {
                    //move column over a slot
                    int direction;
                    if (delta.y > 0)
                    {
                        direction = Mathf.CeilToInt(delta.y);
                    }
                    else
                    {
                        direction = Mathf.FloorToInt(delta.y);
                    }

                    for (int row = 0; row < GridRows; row++)
                    {
                        tempBoardIcons[startIconColumn, row][1] = (row - direction + GridRows) % GridRows;
                    }
                }
            }
            else if ((deltaX <= 0.8f && deltaX > 0.5f) || (deltaY <= 0.8f && deltaY > 0.5f))
            //check on outer portion of line
            {
                if (movingX) //if moving x
                {
                    //move row over a slot
                    int direction;
                    if (delta.x > 0)
                    {
                        direction = Mathf.FloorToInt(delta.x);
                    }
                    else
                    {
                        direction = Mathf.CeilToInt(delta.x);
                    }

                    for (int column = 0; column < GridColumns; column++)
                    {
                        tempBoardIcons[column, startIconRow][0] = (column - direction + GridColumns) % GridColumns;
                    }

                }
                else //else moving y
                {
                    //move column over a slot
                    int direction;
                    if (delta.y > 0)
                    {
                        direction = Mathf.FloorToInt(delta.y);
                    }
                    else
                    {
                        direction = Mathf.CeilToInt(delta.y);
                    }

                    for (int row = 0; row < GridRows; row++)
                    {
                        tempBoardIcons[startIconColumn, row][1] = (row - direction + GridRows) % GridRows;
                    }
                }
            }

            for (int row = 0; row < GridRows; row++)
            {
                List<Icon> matchIcons = new List<Icon>();
                int matchType = -1;
                for (int column = 0; column < GridColumns; column++)
                {
                    int[] tmpIcon = tempBoardIcons[column, row];

                    Icon tempIcon = boardIcons[tmpIcon[0], tmpIcon[1]];

                    if (tempIcon.iconType != matchType)
                    {
                        if (matchIcons.Count > 2)
                        {
                            MatchFound(matchIcons, allMatches, matchIcons.Count, matchIntensity);
                        }
                        matchIcons.Clear();
                    }
                    matchIcons.Add(tempIcon);
                    matchType = tempIcon.iconType;
                }
                if (matchIcons.Count > 2)
                {
                    MatchFound(matchIcons, allMatches, matchIcons.Count, matchIntensity);
                }
            }
            for (int column = 0; column < GridColumns; column++)
            {
                List<Icon> matchIcons = new List<Icon>();
                int matchType = -1;
                for (int row = 0; row < GridRows; row++)
                {
                    int[] tmpIcon = tempBoardIcons[column, row];

                    Icon tempIcon = boardIcons[tmpIcon[0], tmpIcon[1]];

                    if (tempIcon.iconType != matchType)
                    {
                        if (matchIcons.Count > 2)
                        {
                            MatchFound(matchIcons, allMatches, matchIcons.Count, matchIntensity);
                        }
                        matchIcons.Clear();
                    }
                    matchIcons.Add(tempIcon);
                    matchType = tempIcon.iconType;
                }
                if (matchIcons.Count > 2)
                {
                    MatchFound(matchIcons, allMatches, matchIcons.Count, matchIntensity);
                }
            }
            if (allMatches.Count > 0)
            {
                //score the match
                boxCollider.enabled = false;
                ConfirmGridChanges();
                ResetGridPositions();
                StartCoroutine(ScoreMatch(allMatches, matchIntensity, 1));
            }
            else
            {
                if (iconsInMotion == 0)
                {
                    boxCollider.enabled = true;
                }
                else
                {
                    print("Caution: icons in motion at end of save change method.");
                }
            }
        }
    }

    void CheckPossibleMatches()
    {
        int columnOuter = 0;
        int rowOuter = 0;
        int columnInner = 0;
        int rowInner = 0;
        noMatchIcons[0] = null;
        noMatchIcons[1] = null;
        noMatchIcons[2] = null;

        while (columnOuter < GridColumns)
        {
            rowOuter = 0;
            while (rowOuter < GridRows)
            {
                columnInner = 0;
                while (columnInner < GridColumns)
                {
                    int currentIconType = boardIcons[(columnOuter + columnInner) % GridColumns, rowOuter].iconType;
                    if (rowOuter < GridRows - 2 || columnInner == 0)
                    {
                        if (currentIconType == boardIcons[columnOuter, (rowOuter + 1) % GridRows].iconType)
                        {
                            if (currentIconType == boardIcons[columnOuter, (rowOuter + 2) % GridRows].iconType)
                            {
                                PossiblMatchFound((columnOuter + columnInner) % GridColumns, rowOuter, columnOuter, (rowOuter + 1) % GridRows, columnOuter, (rowOuter + 2) % GridRows);
                                columnOuter = GridColumns;
                                rowOuter = GridRows;
                                columnInner = GridColumns;
                            }
                            else if ((rowOuter > 0 && rowOuter < GridRows - 1) || columnInner == 0)
                            {
                                if (currentIconType == boardIcons[columnOuter, (rowOuter - 1 + GridRows) % GridRows].iconType)
                                {
                                    PossiblMatchFound((columnOuter + columnInner) % GridColumns, rowOuter, columnOuter, (rowOuter + 1) % GridRows, columnOuter, (rowOuter - 1 + GridRows) % GridRows);
                                    columnOuter = GridColumns;
                                    rowOuter = GridRows;
                                    columnInner = GridColumns;
                                }
                            }
                        }
                    }
                    else if (rowOuter > 1 || columnInner == 0)
                    {
                        if (currentIconType == boardIcons[columnOuter, (rowOuter - 1 + GridRows) % GridRows].iconType)
                        {
                            if (currentIconType == boardIcons[columnOuter, (rowOuter - 2 + GridRows) % GridRows].iconType)
                            {
                                PossiblMatchFound((columnOuter + columnInner) % GridColumns, rowOuter, columnOuter, (rowOuter - 1 + GridRows) % GridRows, columnOuter, (rowOuter - 2 + GridRows) % GridRows);
                                columnOuter = GridColumns;
                                rowOuter = GridRows;
                                columnInner = GridColumns;
                            }
                        }
                    }
                    columnInner++;
                }

                if (noMatchIcons[2] == null)
                {
                    rowInner = 0;
                    while (rowInner < GridColumns)
                    {
                        int currentIconType = boardIcons[columnOuter, (rowOuter + rowInner) % GridRows].iconType;
                        if (columnOuter < GridColumns - 2 || rowInner == 0)
                        {
                            if (currentIconType == boardIcons[(columnOuter + 1) % GridColumns, rowOuter].iconType)
                            {
                                if (currentIconType == boardIcons[(columnOuter + 2) % GridColumns, rowOuter].iconType)
                                {
                                    PossiblMatchFound(columnOuter, (rowOuter + rowInner) % GridRows, (columnOuter + 1) % GridColumns, rowOuter, (columnOuter + 2) % GridColumns, rowOuter);
                                    columnOuter = GridColumns;
                                    rowOuter = GridRows;
                                    rowInner = GridColumns;
                                }
                                else if ((columnOuter > 0 && columnOuter < GridColumns - 1) || rowInner == 0)
                                {
                                    if (currentIconType == boardIcons[(columnOuter - 1 + GridColumns) % GridColumns, rowOuter].iconType)
                                    {
                                        PossiblMatchFound(columnOuter, (rowOuter + rowInner) % GridRows, (columnOuter + 1) % GridColumns, rowOuter, (columnOuter - 1 + GridColumns) % GridColumns, rowOuter);
                                        columnOuter = GridColumns;
                                        rowOuter = GridRows;
                                        rowInner = GridColumns;
                                    }
                                }
                            }
                        }
                        else if (columnOuter > 1 || rowInner == 0)
                        {
                            if (currentIconType == boardIcons[(columnOuter - 1 + GridColumns) % GridColumns, rowOuter].iconType)
                            {
                                if (currentIconType == boardIcons[(columnOuter - 2 + GridColumns) % GridColumns, rowOuter].iconType)
                                {
                                    PossiblMatchFound(columnOuter, (rowOuter + rowInner) % GridRows, (columnOuter - 1 + GridColumns) % GridColumns, rowOuter, (columnOuter - 2 + GridColumns) % GridColumns, rowOuter);
                                    columnOuter = GridColumns;
                                    rowOuter = GridRows;
                                    rowInner = GridColumns;
                                }
                            }
                        }
                        rowInner++;
                    }
                }
                rowOuter++;
            }
            columnOuter++;
        }
        if (noMatchIcons[2] == null && gameManager.tutorial == null)
        {
            Randomize(true);
        }
    }

    void PossiblMatchFound(int columnFirst, int rowFirst, int columnSecond, int rowSecond, int columnThird, int rowThird)
    {
        noMatchIcons[0] = boardIcons[columnFirst, rowFirst];
        noMatchIcons[1] = boardIcons[columnSecond, rowSecond];
        noMatchIcons[2] = boardIcons[columnThird, rowThird];
        //print("[" + Time.frameCount + "] found match");
    }

    void MatchFound(List<Icon> matchIcons, List<Icon> allMatches, int matchSize, Stack<int> matchIntensity)
    {
        allMatches.AddRange(matchIcons);
        matchIntensity.Push(matchSize);
        matchIcons.Clear();
    }
    
    public void HighlightAdd(List<Icon> allMatches)
    {
        foreach(Icon icon in allMatches)
        {
            if (!highlightIcons.Contains(icon))
            {
                highlightIcons.Add(icon);
            }
            icon.HighlightAdd();
        }
    }

    public void HighlightRemove(List<Icon> allMatches)
    {
        for (int icon = 0; icon < highlightIcons.Count; icon++)
        {
            Icon curicon = highlightIcons[icon];
            if (!allMatches.Contains(curicon))
            {
                curicon.HighlightRemove();
                highlightIcons.Remove(curicon);
            }
        }
    }

    public void StartScoreMatch(List<Icon> allMatches, Stack<int> matchIntensity, int chainMatch)
    {
        StartCoroutine(ScoreMatch(allMatches, matchIntensity, chainMatch));
    }

    //remove matches and 'replace' with new icons
    IEnumerator ScoreMatch(List<Icon> allMatches, Stack<int> matchIntensity, int chainMatch)
    {
        boxCollider.enabled = false;
        int currentIntensity = 0;
        bool multiMatch = false;
        int[] matchesInColumns = new int[GridColumns];

        //pause to ensure user can see their chain/matches
        yield return new WaitForSeconds(0.2f);
        boxCollider.enabled = false;

        //start counting down to showing potential match
        noMatchCounter = 0.0f;

        //throw up text if chain or multi match
        if (chainMatch > 1)
        {
            MatchText poppedMatchText = prefabPools.PopMatchText();
            if (poppedMatchText != null)
            {
                poppedMatchText.enabled = true;
                poppedMatchText.PopOffStack(chainMatch.ToString(), "", -2.031f, ((chainMatch - 1) * 0.71f) - 4.94f, 2, 0);
            }
            if (chainMatch > 3)
            {
                gameManager.missionTracker.IncrementMissionProgress(20, 1);
            }
        }

        if (matchIntensity.Count > 1 && matchIntensity.Peek() != 1)
        {
            MatchText poppedMatchText = prefabPools.PopMatchText();
            if (poppedMatchText != null)
            {
                poppedMatchText.enabled = true;
                poppedMatchText.PopOffStack("", "", Random.Range(-0.35f, 0.35f), Random.Range(-3.8f, -0.1f), 3, 0);
            }
            gameManager.missionTracker.IncrementMissionProgress(14, 1);
            multiMatch = true;
        }

        float effectMultiplier = 1.0f;
        currentIntensity = 0;
        int numberEmpoweredIcons = 0;
        for (int whichIcon = allMatches.Count - 1; whichIcon >= 0; whichIcon--)
        //foreach (Icon icon in allMatches)
        {
            Icon icon = allMatches[whichIcon];
            //play destruction animation on all icons being matched
            IconPhaseOut iconPhaseOut = gameManager.prefabPools.PopIconPhaseOut();
            iconPhaseOut.PopOffStack(icon.iconType, icon.transform.position.x, icon.transform.position.y);
            float currentMultiplier = effectMultiplier;
            effectMultiplier = icon.IconMatched(effectMultiplier);
            currentIntensity++;

            //check if match has empower icons
            if (effectMultiplier >= currentMultiplier * 2.0f)
            {
                if (icon.iconType < 2)
                {
                    numberEmpoweredIcons++;
                    if (numberEmpoweredIcons == 2)
                    {
                        gameManager.missionTracker.IncrementMissionProgress(16, 1);
                    }
                }
            }

            if (currentIntensity == matchIntensity.Peek())
            {
                numberEmpoweredIcons = 0;
                //apply module boost if this is a scrap columnn match
                if (currentIntensity == 1)
                {
                    effectMultiplier *= gameManager.upgradeValues[28, PlayerStats.playerStats.upgradeStatus[28]];
                }
                //int matchValue = 1;

                if (currentIntensity >= 3)
                {
                    missionTracker.IncrementMissionProgress(7, 1);
                    if (currentIntensity >= 4)
                    {
                        MatchText poppedMatchText = prefabPools.PopMatchText();
                        if (poppedMatchText != null)
                        {
                            poppedMatchText.enabled = true;
                            poppedMatchText.PopOffStack(currentIntensity + " IN A ROW!", (currentIntensity - 2) + "x EFFECT", Random.Range(-0.35f, 0.35f), Random.Range(-3.8f, -0.1f), 1, 0);
                        }
                        missionTracker.IncrementMissionProgress(23, 1);
                        if (currentIntensity >= 5)
                        {
                            missionTracker.IncrementMissionProgress(26, 1);
                        }
                    }
                }

                effectMultiplier *= 1.0f + (chainMatch - 1.0f) * gameManager.upgradeValues[16, PlayerStats.playerStats.upgradeStatus[16]];
                if (multiMatch)
                {
                    effectMultiplier *= gameManager.upgradeValues[15, PlayerStats.playerStats.upgradeStatus[15]];
                }
                int matchValue = ProduceMatchResult(icon, currentIntensity, effectMultiplier);
                
                effectMultiplier = 1.0f;

                CreateMatchText(icon.iconType, matchValue);

                matchIntensity.Pop();
                currentIntensity = 0;
            }

            MoveIconToTop(icon, matchesInColumns);
        }

        //wait until all icons have stopped moving
        yield return StartCoroutine(WaitForBoardSettle());

        //may need, still trying to determine
        ResetGridPositions();

        //check for newly formed matches
        ResetTempIconLocationsArray();
        chainMatch++;
        if (CheckMatches(true, chainMatch))
        {
            CheckPossibleMatches();
        }
    }

    int ProduceMatchResult(Icon icon, int currentIntensity, float effectMultiplier)
    {
        int matchValue = 1;
        int iconType = icon.iconType;
        float longMatchMultiplier = (Mathf.Max(currentIntensity - 3, 0.0f) * 2.0f + currentIntensity);
        if (iconType == 0) //concussion
        {
            matchValue = Mathf.CeilToInt(longMatchMultiplier * gameManager.upgradeValues[3, PlayerStats.playerStats.upgradeStatus[3]] * effectMultiplier);
            ship.ShootArmamentSecondary(matchValue);
        }
        else if (iconType == 1) //laser
        {
            matchValue = Mathf.CeilToInt(longMatchMultiplier * gameManager.upgradeValues[0, PlayerStats.playerStats.upgradeStatus[0]] * effectMultiplier);
            ship.ShootArmamentPrimary(matchValue, currentIntensity, 0);
            if (PlayerStats.playerStats.upgradeStatus[2] > 0)
            {
                ship.ShootArmamentMatch((int)(gameManager.upgradeValues[2, PlayerStats.playerStats.upgradeStatus[2]] * matchValue), currentIntensity, icon.transform);
            }
            missionTracker.IncrementMissionProgress(6, 1);
        }
        else if (iconType == 2) //Secondary Energy (green)
        {
            matchValue = Mathf.CeilToInt(longMatchMultiplier * gameManager.upgradeValues[12 + 1, PlayerStats.playerStats.upgradeStatus[12 + 1]] * effectMultiplier);
            topPanel.IncreaseEnergy(matchValue, 1);
            AudioManager.audioManager.PlaySound(12);
        }
        else if (iconType == 3) //Primary Energy (purple)
        {
            matchValue = Mathf.CeilToInt(longMatchMultiplier * gameManager.upgradeValues[12 + 0, PlayerStats.playerStats.upgradeStatus[12 + 0]] * effectMultiplier);
            topPanel.IncreaseEnergy(matchValue, 0);
            gameManager.audioManager.PlaySound(10);
        }
        else if (iconType == 4) //Resource
        {
            matchValue = Mathf.CeilToInt(longMatchMultiplier * gameManager.upgradeValues[17, PlayerStats.playerStats.upgradeStatus[17]] * effectMultiplier);
            gameManager.ChangeFunds(matchValue);
            gameManager.missionTracker.IncrementMissionProgress(13, matchValue);
            AudioManager.audioManager.PlaySound(13);
        }
        else if (iconType == 5) //shield
        {
            matchValue = Mathf.CeilToInt(longMatchMultiplier * gameManager.upgradeValues[6, PlayerStats.playerStats.upgradeStatus[6]] * effectMultiplier);
            ship.IncreasePlayerShield(matchValue);
            AudioManager.audioManager.PlaySound(16);
        }
        else if (iconType == 6) //Tertiary Energy (Yellow)
        {
            matchValue = Mathf.CeilToInt(longMatchMultiplier * gameManager.upgradeValues[12 + 2, PlayerStats.playerStats.upgradeStatus[12 + 2]] * effectMultiplier);
            topPanel.IncreaseEnergy(matchValue, 2);
            AudioManager.audioManager.PlaySound(11);
        }
        else if (iconType == 7) //Special module Scrap Column
        {
            //gameManager.IncreaseFunds();
        }
        return matchValue;
    }

    IEnumerator WaitForBoardSettle()
    {
        while (iconsInMotion > 0)
        {
            yield return null;
        }
        boxCollider.enabled = false;
        backLayerAnimation.enabled = false;
        backLayer.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    }

    void MoveIconToTop(Icon icon, int[] matchesInColumns)
    {
        //'create' new icon
        icon.IconFace[icon.iconType].enabled = false;

        int column = icon.iconColumn;

        for (int row = icon.iconRow; row < GridRows - 1; row++)
        {
            boardIcons[column, row] = boardIcons[column, row + 1];
            boardIcons[column, row].iconRow = row;
            if (!boardIcons[column, row].iconMobile)
            {
                //boardIcons[column, row].iconMobile = true;
                iconsInMotion++;
                StartCoroutine(boardIcons[column, row].IconDrop(row));
            }
        }

        //flash background
        backLayerAnimation.enabled = true;
        backLayerAnimation.Play(0);

        //move matched icon above to fall back down as new icon
        icon.iconType = Random.Range(0, numUniqueIcons);
        icon.IconFace[icon.iconType].enabled = true;
        icon.iconRow = GridRows - 1;
        icon.transform.localPosition = new Vector2(icon.iconColumn * 0.71f, (GridRows + matchesInColumns[column]) * 0.71f);
        boardIcons[column, GridRows - 1] = icon;
        if (!icon.iconMobile)
        {
            //boardIcons[column, GridRows - 1].iconMobile = true;
            iconsInMotion++;
            StartCoroutine(icon.IconDrop(GridRows - 1));
        }

        //fill in matchesInColumns array
        matchesInColumns[column]++;
    }

    //throw up text for each match
    public void CreateMatchText(int iconType, int matchValue)
    {
        float[,] textLocation = { { 0.2f, 0.62f }, { 0.2f, 0.62f }, { 0.0f, 3.58f }, { -1.75f, 3.58f }, { 0.0f, 3.25f }, { -1.6f, 2.59f }, { 1.65f, 3.58f } };

        MatchText poppedMatchText = prefabPools.PopMatchText();
        if (poppedMatchText != null)
        {
            poppedMatchText.enabled = true;
            poppedMatchText.PopOffStack(matchValue.ToString(), "", textLocation[iconType,0], textLocation[iconType, 1], 0, iconType);
        }
    }

    public void ResetGridPositions()
    {
        for (int column = 0; column < GridColumns; column++)
        {
            for (int row = 0; row < GridRows; row++)
            {
                Icon icon = boardIcons[column, row];
                icon.transform.localPosition = new Vector2(column * 0.71f, row * 0.71f);
                icon.HighlightRemove();
            }
        }
    }
    
    void ConfirmGridChanges()
    {
        Icon[] tempIcons;
        
        if (movingX)
        {
            tempIcons = new Icon[GridColumns];
            for (int column = 0; column < GridColumns; column++)
            {
                tempIcons[column] = boardIcons[column, line];
            }
            for (int column = 0; column < GridColumns; column++)
            {
                boardIcons[column, line] = tempIcons[tempBoardIcons[column,line][0]];
                //boardIcons[column, line] = tempIcons[(column - Mathf.RoundToInt(movedAmount.x) + GridColumns) % GridColumns];
                boardIcons[column, line].iconColumn = column;
            }
        }
        else
        {
            tempIcons = new Icon[GridRows];
            for (int row = 0; row < GridRows; row++)
            {
                tempIcons[row] = boardIcons[line, row];
            }
            for (int row = 0; row < GridRows; row++)
            {
                boardIcons[line, row] = tempIcons[tempBoardIcons[line, row][1]];
                //boardIcons[line, row] = tempIcons[(row - Mathf.RoundToInt(movedAmount.y) + GridRows) % GridRows];
                boardIcons[line, row].iconRow = row;
            }
        }
    }

    public void ResetTempIconLocationsArray()
    {
        for (int column = 0; column < GridColumns; column++)
        {
            for (int row = 0; row < GridRows; row++)
            {
                tempBoardIcons[column, row][0] = column;
                tempBoardIcons[column, row][1] = row;
            }
        }
    }

    void OnMouseUp()
    {
        proxyIconX.transform.localPosition = new Vector2(-2.0f, 0);
        proxyIconY.transform.localPosition = new Vector2(-2.0f, 0);
        //if matches
        if (saveChange)
        {
            CheckMatches(true, 1);
        }
        else //else reset
        {
            CheckCloseMatches();
            ResetGridPositions();
            boxCollider.enabled = true;
        }
    }

    public void UnlockBoard()
    {
        iconsInMotion = 0;
        for (int column = 0; column < GridColumns; column++)
        {
            for (int row = 0; row < GridRows; row++)
            {
                boardIcons[column, row].iconMobile = false;
            }
        }
        boxCollider.enabled = true;
    }

    public IEnumerator Shuffle()
    {
        //int randInt = Random.Range(0, GridRows);
        //randInt = 2;
        boxCollider.enabled = false;

        for (int column = 0; column < GridColumns; column++)
        {
            for (int row = 0; row < GridRows; row++)
            {
                StartCoroutine(boardIcons[(column + row) % numUniqueIcons, row].Shuffle());
            }
            //randInt++;
        }
        if (PlayerStats.playerStats.upgradeStatus[44] > 1)
        {
            if (Random.Range(0, (int)gameManager.upgradeValues[44, PlayerStats.playerStats.upgradeStatus[44]]) == 0)
            {
                int randomType = Random.Range(0, numUniqueIcons);
                //if we are doing a row
                if (Random.Range(0, 2) == 0)
                {
                    int randomRow = Random.Range(0, GridRows);
                    int randomColumn = Random.Range(0, GridColumns - 2);
                    for (int curIcon = 0; curIcon < 3; curIcon++)
                    {
                        boardIcons[randomColumn + curIcon, randomRow].SetType(randomType);
                    }
                }
                else
                {
                    int randomRow = Random.Range(0, GridRows - 2);
                    int randomColumn = Random.Range(0, GridColumns);
                    for (int curIcon = 0; curIcon < 3; curIcon++)
                    {
                        boardIcons[randomColumn, randomRow + curIcon].SetType(randomType);
                    }
                }
            }
        }
        yield return new WaitForSeconds(1.6f);
        boxCollider.enabled = true;
        CheckMatches(true, 1);
    }

    public void StartLevel(bool firstTime)
    {
        DeactivateLowArmorWarning();
        Randomize(firstTime);
        iconsInMotion = 0;
    }

    public void LockDisabled()
    {
        if (iconsInMotion <= 0)
        {
            CheckMatches(true, 1);
        }
    }

    public void MouseUpOnOther()
    {
        if (iconsInMotion <= 2)
        {
            boxCollider.enabled = true;
        }
    }

    public void Randomize(bool firstTime)
    {
        noMatchCounter = 0.0f;
        if (firstTime)
        {
            for (int column = 0; column < GridColumns; column++)
            {
                for (int row = 0; row < GridRows; row++)
                {
                    Icon curIcon = boardIcons[column, row];
                    int iconType = GenerateType(column, row);
                    if (!curIcon.spriteLock.enabled)
                    {
                        curIcon.DisableCurrentFace();
                        curIcon.SetStartIcon(column, row, iconType);
                    }
                }
            }
        }
        else
        {
            for (int column = 0; column < GridColumns; column++)
            {
                for (int row = 0; row < GridRows; row++)
                {
                    Icon curIcon = boardIcons[column, row];
                    int iconType = GenerateType(column, row);
                    curIcon.ResetPosition(column, row, iconType);
                    curIcon.HighlightRemove();
                }
            }
            UnlockBoard();
            ToggleFrontFade(false);
        }
        CheckPossibleMatches();
    }

    public void CleanBoard()
    {
        for (int column = 0; column < GridColumns; column++)
        {
            for (int row = 0; row < GridRows; row++)
            {
                Icon curIcon = boardIcons[column, row];
                curIcon.RemoveEffects();
                curIcon.HighlightRemove();
            }
        }

        for (int numChild = transform.childCount - 1; numChild > 50; numChild--)
        {
            Transform curChild = transform.GetChild(numChild);
            if (curChild != null)
            {
                switch (curChild.tag)
                {
                    case "EnemyProjectileAcid":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileDrain":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileWeaken":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileRocket":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileFire":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileLaser":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileLock":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                    case "EnemyProjectileBomb":
                        curChild.GetComponent<EnemyProjectile>().PushOnStack();
                        break;
                }
            }
        }
    }

    public void ToggleFrontFade(bool whichToggle)
    {
        frontFade.enabled = whichToggle;
    }

    public void SuppressBoard()
    {
        StartCoroutine(ContinuallySuppressBoard());
    }

    IEnumerator ContinuallySuppressBoard()
    {
        GameObject spriteMessageBox = gameManager.messageScreen.spriteMessageBox.gameObject;
        yield return new WaitForSeconds(0.5f);
        while (spriteMessageBox.activeSelf)
        {
            ToggleBoardSelectable(false);
            yield return null;
        }
        ToggleBoardSelectable(true);
    }

    int GenerateType(int column, int row)
    {
        int iconType;
        bool initialMatchError;
        do
        {
            initialMatchError = false;
            iconType = Random.Range(0, numUniqueIcons);
            if (column > 1)
            {
                if (iconType == boardIcons[column - 1, row].iconType)
                {
                    if (iconType == boardIcons[column - 2, row].iconType)
                    {
                        initialMatchError = true;
                    }
                }
            }
            if (row > 1)
            {
                if (iconType == boardIcons[column, row - 1].iconType)
                {
                    if (iconType == boardIcons[column, row - 2].iconType)
                    {
                        initialMatchError = true;
                    }
                }
            }
        } while (initialMatchError);
        return iconType;
    }

    public void ModuleDestroyIcon(int row, int column, int destroySource)
    {
        StartCoroutine(ModuleDestroyingIcon(row, column, destroySource));
    }

    public IEnumerator ModuleDestroyingIcon(int row, int column, int destroySource)
    {
        int[] matchesInColumns = new int[GridColumns];
        Icon icon = boardIcons[row, column];

        float effectMultiplier = 1.0f;
        if (destroySource == 1)
        {
            effectMultiplier = gameManager.upgradeValues[42, PlayerStats.playerStats.upgradeStatus[42]];
            if (icon.iconType < 2)
            {
                gameManager.missionTracker.IncrementMissionProgress(24, 1);
            }
        }
        effectMultiplier = icon.IconMatched(effectMultiplier);

        prefabPools.CreateExplosion(icon.transform, 0.33f);
        int matchValue = ProduceMatchResult(icon, 1, effectMultiplier);
        CreateMatchText(icon.iconType, matchValue);

        MoveIconToTop(icon, matchesInColumns);
        yield return StartCoroutine(WaitForBoardSettle());
        yield return null;

        //set icons to their correct locations
        //ResetGridPositions();

        //check for newly formed matches
        //ResetTempIconLocationsArray();
        CheckMatches(true, 1);
    }

    public IEnumerator ActivateModuleShatter()
    {
        boxCollider.enabled = false;
        int[] matchesInColumns = new int[GridColumns];
        int crystalsFound = 0;

        for (int column = 0; column < GridColumns; column++)
        {
            for (int row = GridRows - 1; row >= 0; row--)
            {
                if (boardIcons[column, row].iconType == 4)
                {
                    crystalsFound++;

                    Icon icon = boardIcons[column, row];
                    ModuleShatter poppedModuleShatter = prefabPools.PopModuleShatter();
                    if (poppedModuleShatter != null)
                    {
                        poppedModuleShatter.enabled = true;
                        poppedModuleShatter.PopOffStack(icon.transform.position.x, icon.transform.position.y);
                    }
                    MoveIconToTop(icon, matchesInColumns);
                }
            }
        }

        int matchValue = (int)(crystalsFound * gameManager.upgradeValues[17, PlayerStats.playerStats.upgradeStatus[17]] * gameManager.upgradeValues[47, PlayerStats.playerStats.upgradeStatus[47]]);
        CreateMatchText(4, matchValue);
        gameManager.ChangeFunds(matchValue);
        yield return StartCoroutine(WaitForBoardSettle());

        //may need, still trying to determine
        ResetGridPositions();

        //check for newly formed matches
        ResetTempIconLocationsArray();
        CheckMatches(true, 1);
    }
    
    public void ToggleBoardSelectable(bool whichToggle)
    {
        boxCollider.enabled = whichToggle;
    }

    void Update()
    {
        noMatchCounter += Time.deltaTime;
        if (noMatchCounter > 8.0f && gameManager.tutorial == null)
        {
            //if (noMatchIcons[0].iconType != noMatchIcons[1].iconType || noMatchIcons[0].iconType != noMatchIcons[2].iconType)
            //{
            CheckPossibleMatches();
            //}
            for (int currentIcon = 0; currentIcon < 3; currentIcon++)
            {
                noMatchIcons[currentIcon].HighlightAdd();
            }
        }
    }
}