using UnityEngine;
using System.Collections.Generic;

public class AbilityScrapColumn : MonoBehaviour
{
    public Ability ability;
    private Vector3 clickLocation;
    private Board board;
    public BoxCollider boxCollider;
    private BoxCollider boardBoxCollider;
    private int upgradeStatus;
    private GameManager gameManager;
    public SpriteRenderer spriteBar;

    public void StartSetup ()
    {
        gameManager = ability.topPanel.gameManager;
        board = gameManager.board;
        boardBoxCollider = board.GetComponent<BoxCollider>();
    }

    void OnMouseUp ()
    {
        List<Icon> allMatches = new List<Icon>();
        Stack<int> matchIntensity = new Stack<int>();
        //List<int> matchIntensity = new List<int>();

        //determine which column to remove
        clickLocation = Input.mousePosition;
        int column = Mathf.RoundToInt(clickLocation.x / gameManager.scaleRatioX) - 1;

        if (column >= 0 && column < Board.GridColumns)
        {
            for (int row = 0; row < Board.GridRows; row++)
            {
                Icon icon = board.boardIcons[column, row];
                allMatches.Add(icon);
                matchIntensity.Push(1);
                //matchIntensity.Add(1);
                if (icon.iconBomb != null)
                {
                    gameManager.missionTracker.IncrementMissionProgress(5, 1);
                }
            }
            
            //if upgraded, add additional icons
            while (upgradeStatus > 0)
            {
                bool iconFound = false;
                Icon icon = null;
                while (!iconFound)
                {
                    int randomRow = Random.Range(0, Board.GridRows - 1);
                    int randomColumn = Random.Range(0, Board.GridColumns - 1);
                    icon = board.boardIcons[randomColumn, randomRow];
                    if (!allMatches.Contains(icon))
                    {
                        iconFound = true;
                    }
                }
                allMatches.Add(icon);
                matchIntensity.Push(1);
                upgradeStatus--;
            }

            board.ResetTempIconLocationsArray();
            board.StartScoreMatch(allMatches, matchIntensity, 0);

            DeactivateModule();
        }
    }

    void Update()
    {
        if (boardBoxCollider.enabled)
        {
            boardBoxCollider.enabled = false;
        }
    }

    public void ActivateModule()
    {
        boardBoxCollider.enabled = false; //can be foiled if used right after a match
        spriteBar.enabled = true;
        //List<Icon> allMatches = new List<Icon>();
        //foreach (Icon icon in board.boardIcons)
        //{
        //    allMatches.Add(icon);
        //}
        //board.HighlightAdd(allMatches);
        boxCollider.enabled = true;
        //transform.parent = transformSpace;
        transform.localPosition = new Vector2(0.0f, 0.0f);
        upgradeStatus = (int)gameManager.upgradeValues[43, PlayerStats.playerStats.upgradeStatus[43]];
        //spriteRenderer.enabled = true;
    }

    public void DeactivateModule()
    {
        boxCollider.enabled = false;
        spriteBar.enabled = false;
        //transform.parent = transformPool;
        transform.localPosition = new Vector2(0.0f, 30.0f);
        ability.ToggleCollider(true);
        //enable gamestate
        boardBoxCollider.enabled = true;
        enabled = false;
    }
}