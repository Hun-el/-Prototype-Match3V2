using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    Board board;
    [HideInInspector] public List<Gem> currentMatches = new List<Gem>();
    [HideInInspector] public List<Gem> justMatches = new List<Gem>();
    [HideInInspector] List<Gem> otherBombs  = new List<Gem>();

    private void Awake() {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        currentMatches.Clear();
        justMatches.Clear();

        for(float x = 0; x < board.Width; x++)
        {
            for(float y = 0; y < board.Height; y++)
            {
                if(x > 0 && x < board.Width - 1)
                {
                    Gem currentGem = board.allGems[(int)x,(int)y];
                    if(currentGem != null)
                    {
                        Gem leftGem = board.allGems[(int)x-1,(int)y];
                        Gem rightGem = board.allGems[(int)x+1,(int)y];
                        if(leftGem != null && rightGem != null)
                        {
                            if(leftGem.type == currentGem.type && rightGem.type == currentGem.type)
                            {
                                currentGem.isMatched = true;
                                leftGem.isMatched = true;
                                rightGem.isMatched = true;

                                if(currentMatches.IndexOf(currentGem) == -1 ){ currentMatches.Add(currentGem); }
                                if(currentMatches.IndexOf(leftGem) == -1 ){ currentMatches.Add(leftGem); }
                                if(currentMatches.IndexOf(rightGem) == -1 ){ currentMatches.Add(rightGem); }
                            }
                        }
                    }
                }

                if(y > 0 && y < board.Height - 1)
                {
                    Gem currentGem = board.allGems[(int)x,(int)y];
                    if(currentGem != null)
                    {
                        Gem aboveGem = board.allGems[(int)x,(int)y+1];
                        Gem belowGem = board.allGems[(int)x,(int)y-1];
                        if(aboveGem != null && belowGem != null)
                        {
                            if(aboveGem.type == currentGem.type && belowGem.type == currentGem.type)
                            {
                                currentGem.isMatched = true;
                                aboveGem.isMatched = true;
                                belowGem.isMatched = true;

                                if(currentMatches.IndexOf(currentGem) == -1 ){ currentMatches.Add(currentGem); }
                                if(currentMatches.IndexOf(aboveGem) == -1 ){ currentMatches.Add(aboveGem); }
                                if(currentMatches.IndexOf(belowGem) == -1 ){ currentMatches.Add(belowGem); }
                            }
                        }
                    }
                }
            }
        }
        justMatches.AddRange(currentMatches);
        //CheckBomb();
    }

    void CheckBomb()
    {
        for(int x = 0; x < board.Width; x++)
        {
            for(int y = 0; y < board.Height; y++)
            {
                Gem gem = board.allGems[x,y];

                if(board.allGems[x,y].type == Gems.GemType.bomb)
                {
                    MarkBombArea(gem.pos , gem);
                }
            }
        }
    }

    public void MarkBombArea(Vector2 bombPos , Gem bomb)
    {
        if(otherBombs.Count > 0){ otherBombs.RemoveAt(otherBombs.Count - 1); }
        for(int x = (int)bombPos.x - bomb.blastSize; x <= bombPos.x + bomb.blastSize; x++)
        {
            for(int y = (int)bombPos.y - bomb.blastSize; y <= bombPos.y + bomb.blastSize; y++)
            {
                if(x >= 0 && x < board.Width && y >= 0 && y < board.Height)
                {
                    if(board.allGems[x,y] != null)
                    {
                        if(y == bombPos.y)
                        {
                            if(board.allGems[x,y].type == Gems.GemType.bomb && board.allGems[x,y] != bomb)
                            {
                                otherBombs.Add(board.allGems[x,y]);
                                //MarkBombArea(board.allGems[x,y].pos , board.allGems[x,y]);
                            }
                            else
                            {
                                board.allGems[x,y].isMatched = true;
                                board.allGems[x,y].destroyEffect = board.allGems[x,y].bombEffect;
                                currentMatches.Add(board.allGems[x,y]);
                            }
                        }
                        else if(x == bombPos.x)
                        {
                            if(board.allGems[x,y].type == Gems.GemType.bomb && board.allGems[x,y] != bomb)
                            {
                                otherBombs.Add(board.allGems[x,y]);
                                //MarkBombArea(board.allGems[x,y].pos , board.allGems[x,y]);
                            }
                            else
                            {
                                board.allGems[x,y].isMatched = true;
                                board.allGems[x,y].destroyEffect = board.allGems[x,y].bombEffect;
                                currentMatches.Add(board.allGems[x,y]);
                            }
                        }
                    }
                }
            }
        }
        board.DestroyMatches();
        
        if(otherBombs.Count > 0)
        {
            MarkBombArea(otherBombs[otherBombs.Count - 1].pos , otherBombs[otherBombs.Count - 1]);
        }
    }

    public bool checkBoardHorizontal()
    {
        for(float x = 1; x < board.Width; x++)
        {
            for(float y = 0; y < board.Height; y++)
            {
                Gem currentGem = board.allGems[(int)x,(int)y];
                Gem leftGem = board.allGems[(int)x-1,(int)y];
                if(currentGem != null && leftGem != null && leftGem.type == currentGem.type)
                {
                    if(x > 1)
                    {
                        if(y != board.Height - 1)
                        {
                            Gem gem1 = board.allGems[(int)x-2,(int)y+1];
                            if(leftGem.type == gem1.type)
                            {
                                return true;
                            }
                        }
                        if(y != 0)
                        {
                            Gem gem2 = board.allGems[(int)x-2,(int)y-1];
                            if(leftGem.type == gem2.type)
                            {
                                return true;
                            }
                        }
                    }
                    if(x > 0 && x != board.Width - 1)
                    {
                        if(y != board.Height - 1)
                        {
                            Gem gem1 = board.allGems[(int)x+1,(int)y+1];
                            if(currentGem.type == gem1.type)
                            {
                                return true;
                            }
                        }
                        if(y != 0)
                        {
                            Gem gem2 = board.allGems[(int)x+1,(int)y-1];
                            if(currentGem.type == gem2.type)
                            {
                                return true;
                            }
                        }
                    }

                    if(x > 0 && x < board.Width - 2)
                    {
                        Gem gem2 = board.allGems[(int)x+2,(int)y];
                        if(leftGem.type == gem2.type)
                        {
                            return true;
                        }
                    }
                    if(x > 2)
                    {
                        Gem gem2 = board.allGems[(int)x-3,(int)y];
                        if(leftGem.type == gem2.type)
                        {
                            return true;
                        }
                    }
                }

                if(x >= 2)
                {
                    Gem left2Gem = board.allGems[(int)x-2,(int)y];
                    if(left2Gem.type == currentGem.type)
                    {
                        if(y != board.Height - 1)
                        {
                            Gem gem2 = board.allGems[(int)x - 1,(int)y + 1];
                            if(left2Gem.type == gem2.type)
                            {
                                return true;
                            }
                        }
                        if(y != 0)
                        {
                            Gem gem2 = board.allGems[(int)x - 1,(int)y - 1];
                            if(left2Gem.type == gem2.type)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool checkBoardVertical()
    {
        for(float x = 0; x < board.Width; x++)
        {
            for(float y = 0; y < board.Height - 1; y++)
            {
                Gem currentGem = board.allGems[(int)x,(int)y];
                Gem upGem = board.allGems[(int)x,(int)y+1];
                if(currentGem != null && upGem != null && upGem.type == currentGem.type)
                {
                    if(y < board.Height - 3)
                    {
                        Gem gem = board.allGems[(int)x,(int)y + 3];
                        if(currentGem.type == gem.type)
                        {
                            return true;
                        }
                    }
                    if(y > 1)
                    {
                        Gem gem = board.allGems[(int)x,(int)y - 2];
                        if(currentGem.type == gem.type)
                        {
                            return true;
                        }
                    }

                    if(y < board.Height - 2 && x < board.Width - 1)
                    {
                        Gem gem = board.allGems[(int)x + 1,(int)y + 2];
                        if(currentGem.type == gem.type)
                        {
                            return true;
                        }
                    }
                    if(y < board.Height - 2 && x > 0)
                    {
                        Gem gem = board.allGems[(int)x - 1,(int)y + 2];
                        if(currentGem.type == gem.type)
                        {
                            return true;
                        }
                    }

                    if(y > 0 && x < board.Width - 1)
                    {
                        Gem gem = board.allGems[(int)x + 1,(int)y - 1];
                        if(currentGem.type == gem.type)
                        {
                            return true;
                        }
                    }
                    if(y > 0 && x > 0)
                    {
                        Gem gem = board.allGems[(int)x - 1,(int)y - 1];
                        if(currentGem.type == gem.type)
                        {
                            return true;
                        }
                    }
                }
                if(y < board.Height - 2)
                {
                    Gem up2Gem = board.allGems[(int)x,(int)y+2];
                    if(currentGem != null && up2Gem != null && up2Gem.type == currentGem.type)
                    {
                        if(x < board.Width - 1)
                        {
                            Gem gem = board.allGems[(int)x + 1,(int)y + 1];
                            if(currentGem.type == gem.type)
                            {
                                return true;
                            }
                        }
                        if(x > 0)
                        {
                            Gem gem = board.allGems[(int)x -1,(int)y + 1];
                            if(currentGem.type == gem.type)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
}

