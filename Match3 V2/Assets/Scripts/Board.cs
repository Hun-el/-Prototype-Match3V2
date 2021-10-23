using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Board : MonoBehaviour
{
    List<int> gemsCountList = new List<int>();
    [HideInInspector] public int Height;
    [HideInInspector] public int Width;

    GameObject bgTilePrefab;

    public Gem[,] allGems;

    Gems[] gems;
    GameObject gemPrefab;

    [HideInInspector] public float gemSpeed;

    [HideInInspector] public MatchFinder matchFinder;
    GameManager gameManager;
    Collect collect;
    MoveCount moveCount;

    public enum boardState {moving, waiting, end}
    public boardState currentState = boardState.waiting;

    private void Awake() 
    {
        matchFinder = FindObjectOfType<MatchFinder>();
        gameManager = FindObjectOfType<GameManager>();
        collect = FindObjectOfType<Collect>();
        moveCount = FindObjectOfType<MoveCount>();
    }

    void Start() 
    {
        Height = gameManager.height;
        Width = gameManager.width;
        gems = gameManager.gems;
        gemSpeed = gameManager.gemSpeed;
        gemPrefab = gameManager.gemPrefab;
        bgTilePrefab = gameManager.bgTilePrefab;

        allGems = new Gem[Width,Height];
        Setup();

        matchFinder.FindAllMatches();
    }

    void Setup()
    {
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                Vector2 pos = new Vector2(x,y);
                GameObject bgTile = Instantiate(bgTilePrefab,pos,Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = "BG Tile - " + x + " , " + y;

                for(int i = 0; i < gems.Length; i++){ gemsCountList.Add(i); }

                int randomGem = Random.Range(gemsCountList[0],gemsCountList.Count);
                while(MatchesAt(new Vector2Int(x,y),gems[randomGem].type, randomGem) && gemsCountList != null)
                {
                    randomGem = Random.Range(gemsCountList[0],gemsCountList.Count);
                }

                SpawnGem(new Vector2(x,y) , gems[randomGem] , 1);
                gemsCountList.Clear();
            }
        }

        if(!matchFinder.checkBoardHorizontal() && !matchFinder.checkBoardVertical())
        {
            Shuffle();
        }
    }

    void SpawnGem(Vector2 pos , Gems gem , int sliding)
    {
        GameObject g = Instantiate(gemPrefab , new Vector2(pos.x ,pos.y + Height * sliding), Quaternion.identity);

        g.GetComponent<Gem>().gem2 = gem;
        g.GetComponent<Gem>().Setup();

        if(g.GetComponent<Gem>().type == Gems.GemType.bomb)
        {
            if(g.tag == "Bomb"){ g.GetComponent<Gem>().blastSize = gameManager.bombPower; }
            if(g.tag == "Dynamite"){ g.GetComponent<Gem>().blastSize = gameManager.dynamitePower; }
            if(g.tag == "Atomic Bomb"){ g.GetComponent<Gem>().blastSize = gameManager.atomicBombPower; }
        }

        g.transform.parent = transform;
        g.name = "Gem - "+ pos.x + " , " + pos.y;
        int x = Mathf.CeilToInt(pos.x);
        int y = Mathf.CeilToInt(pos.y);
        allGems[x , y] = g.GetComponent<Gem>();
        g.GetComponent<Gem>().SetupGem(new Vector2(pos.x ,pos.y) , this);
    }

    bool MatchesAt(Vector2Int posToCheck , Gems.GemType gemToCheck , int _randomGem)
    {
        if(posToCheck.x > 1)
        {
            if(allGems[posToCheck.x - 1 , posToCheck.y].type == gemToCheck && allGems[posToCheck.x - 2 , posToCheck.y].type == gemToCheck)
            {
                gemsCountList.Remove(_randomGem);
                return true;
            }
        }
        
        if(posToCheck.y > 1)
        {
            if(allGems[posToCheck.x , posToCheck.y - 1].type == gemToCheck && allGems[posToCheck.x , posToCheck.y - 2].type == gemToCheck)
            {
                gemsCountList.Remove(_randomGem);
                return true;
            }
        }
        return false;
    }

    void DestroyMatchedGemAt(Vector2Int pos)
    {
        if(allGems[pos.x , pos.y] != null)
        {
            if(allGems[pos.x , pos.y].isMatched)
            {
                Instantiate(allGems[pos.x , pos.y].destroyEffect , new Vector2(pos.x , pos.y) , Quaternion.identity);

                collect.decrease(allGems[pos.x , pos.y].type.ToString());

                Destroy(allGems[pos.x , pos.y].gameObject);
                allGems[pos.x , pos.y] = null;
            }
        }
        Camera.main.GetComponent<DOTweenAnimation>().DORestart();
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < matchFinder.currentMatches.Count; i++)
        {
            if(matchFinder.currentMatches[i] != null)
            {
                if(matchFinder.currentMatches[i].isMatched)
                {
                    DestroyMatchedGemAt(new Vector2Int((int)matchFinder.currentMatches[i].pos.x,(int)matchFinder.currentMatches[i].pos.y));
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(0.2f);

        int nullCounter = 0;
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                if(allGems[x,y] == null)
                {
                    nullCounter++;
                }
                else if(nullCounter > 0)
                {
                    allGems[x,y].pos.y -= nullCounter;
                    allGems[x,y-nullCounter] = allGems[x,y];
                    allGems[x,y] = null;
                }
            }
            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }
    IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(0.5f);
        RefillBoard(matchFinder.justMatches.Count);

        yield return new WaitForSeconds(0.5f);
        matchFinder.FindAllMatches();
        if(matchFinder.currentMatches.Count > 0){
            yield return new WaitForSeconds(0.75f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
            currentState = boardState.waiting;
            if(!matchFinder.checkBoardHorizontal() && !matchFinder.checkBoardVertical())
            {
                Shuffle();
            }
        }
    }

    void RefillBoard(int matches)
    {
        bool boosters = false;
        if(matches >= gameManager.bomb){boosters = true;}
        if(boosters)
        {
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    if(allGems[x,y] == null && boosters)
                    {
                        if(matches >= gameManager.atomicBomb)
                        { 
                            SpawnGem(new Vector2(x,y) , gameManager.atomicBombGem , 1); 
                            boosters = false;
                            break;
                        }
                        if(matches >= gameManager.dynamite)
                        { 
                            SpawnGem(new Vector2(x,y) , gameManager.dynamiteGem , 1); 
                            boosters = false;
                            break;
                        }
                        if(matches >= gameManager.bomb)
                        { 
                            SpawnGem(new Vector2(x,y) , gameManager.bombGem , 1); 
                            boosters = false;
                            break;
                        }
                    }
                }
            }
        }

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                if(allGems[x,y] == null)
                {
                    int randomGem = Random.Range(0,gems.Length);
                    SpawnGem(new Vector2(x,y) , gems[randomGem] , 1);
                }
            }
        }
    }

    public void gemMoved()
    {
        moveCount.decreaseMove();
    }

    public void boosterActiveted(Vector2 bombPos , Gem bomb)
    {
        matchFinder.MarkBombArea(bombPos,bomb);
    }

    public void Shuffle()
    {
        if(currentState == boardState.waiting)
        {
            currentState = boardState.moving;
            List<Gem> gemsList = new List<Gem>();

            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    gemsList.Add(allGems[x,y]);
                    allGems[x,y] = null;
                }
            }

            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    int gemToUse = Random.Range(0,gemsList.Count);

                    int iterations = 0;
                    while(MatchesAt(new Vector2Int(x,y),gemsList[gemToUse].type, gemToUse) && iterations < 100 && gemsList.Count > 1)
                    {
                        gemToUse = Random.Range(0,gemsList.Count);
                        iterations++;
                    }

                    gemsList[gemToUse].SetupGem(new Vector2Int(x,y),this);
                    allGems[x,y] = gemsList[gemToUse];
                    gemsList.RemoveAt(gemToUse);
                }
            }
            StartCoroutine(FillBoardCo());
        }
    }
}
