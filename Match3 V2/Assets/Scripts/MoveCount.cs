using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCount : MonoBehaviour
{
    public int moveCount;

    GameManager gameManager;
    Board board;

    Text moveCountText;

    void Awake() 
    {
        gameManager = FindObjectOfType<GameManager>();
        board = FindObjectOfType<Board>();
    }

    void Start() 
    {
        moveCountText = GameObject.FindWithTag("MoveCountText").GetComponent<Text>();
        moveCount = gameManager.maxMoveCount;
        moveCountText.text = moveCount.ToString();
    }

    public void decreaseMove()
    {
        if(moveCount > 0)
        {
            moveCount--;
            moveCountText.text = moveCount.ToString();
        }

        if(moveCount <= 0)
        {
            if(board.currentState == Board.boardState.moving){ Invoke("decreaseMove",1f); return; }
            else{ gameManager.gameOver(false); }
        }
    }
}
