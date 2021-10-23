using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public Gems gem2;
    Board board;

    [HideInInspector] public Vector2 pos;
    Vector2 previousPos;
    Vector2 FirstTouchPosition;
    Vector2 FinalTouchPosition;

    [HideInInspector] public bool isMatched;
    bool Pressed;

    float swipeAngle;

    Gem otherGem;

    public enum GemType {blue,green,purple,red,yellow,bomb}
    public Gems.GemType type;

    public Color color;

    public GameObject destroyEffect,bombEffect;

    public int blastSize;

    public void Setup()
    {
        GetComponent<SpriteRenderer>().sprite = gem2.gemArt;
        color = gem2.color;
        type = gem2.type;
        destroyEffect = gem2.destroyEffect;
        bombEffect = gem2.bombEffect;
        blastSize = gem2.blastSize;
    }

    private void Update() 
    {
        if(Vector2.Distance(transform.position , pos) > 0.01f)
        {   
            transform.position = Vector2.Lerp(transform.position,pos,board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = pos;
            board.allGems[(int)pos.x,(int)pos.y] = this;
        }

        if(Pressed && Input.GetMouseButtonUp(0))
        {
            Pressed = false;

            FinalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    public void SetupGem(Vector2 _pos,Board _board)
    {
        pos = _pos;
        board = _board;
    }

    private void OnMouseDown() 
    {
        if(board.currentState == Board.boardState.waiting)
        {
            FirstTouchPosition = pos;
            Pressed = true;
        }
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(FinalTouchPosition.y - FirstTouchPosition.y , FinalTouchPosition.x - FirstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;

        if(Vector2.Distance(FirstTouchPosition , FinalTouchPosition) > 0.45f)
        {
            MovePieces();
        }
        else
        {
            CheckBomb();
        }
    }

    void CheckBomb()
    {
        if(type == Gems.GemType.bomb)
        { 
            board.boosterActiveted(new Vector2(pos.x,pos.y) , this);
            board.currentState = Board.boardState.moving;
            board.gemMoved();
        }
    }

    void MovePieces()
    {
        previousPos = pos;

        int x = Mathf.CeilToInt(pos.x);
        int y = Mathf.CeilToInt(pos.y);
        if(swipeAngle < 45 && swipeAngle > -45 && pos.x < board.Width - 1)
        {
            otherGem = board.allGems[x+1,y]; 
            otherGem.pos.x -= 1f;
            pos.x += 1f;
        } 
        else if(swipeAngle > 45 && swipeAngle <= 135 && pos.y < board.Height - 1)
        {
            otherGem = board.allGems[x,y+1]; 
            otherGem.pos.y -= 1f;
            pos.y += 1f;
        }
        else if(swipeAngle < -45 && swipeAngle >= -135 && pos.y > 0)
        {
            otherGem = board.allGems[x,y-1]; 
            otherGem.pos.y += 1f;
            pos.y -= 1f;
        }
        else if(swipeAngle > 135 || swipeAngle < -135  && pos.x > 0)
        {
            otherGem = board.allGems[x-1,y]; 
            otherGem.pos.x += 1f;
            pos.x -= 1f;
        } 

        int otherX = Mathf.CeilToInt(otherGem.pos.x);
        int otherY = Mathf.CeilToInt(otherGem.pos.y);
        board.allGems[x,y] = this;
        board.allGems[otherX,otherY] = otherGem;

        board.currentState = Board.boardState.moving;
        board.gemMoved();

        StartCoroutine(checkMove());
    }

    IEnumerator checkMove()
    {
        yield return new WaitForSeconds(0.5f);
        board.matchFinder.FindAllMatches();
        if(!isMatched && !otherGem.isMatched)
        {
            otherGem.pos = pos;
            pos = previousPos;

            board.allGems[(int)pos.x,(int)pos.y] = this;
            board.allGems[(int)otherGem.pos.x,(int)otherGem.pos.y] = otherGem;

            yield return new WaitForSeconds(0.25f);
            board.currentState = Board.boardState.waiting;
        }
        else
        {
            board.DestroyMatches();
        }

    }
}