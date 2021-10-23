using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Board")]
    public int height;
    public int width;
    public GameObject gemPrefab;
    public GameObject bgTilePrefab;
    public int gemSpeed;
    public int maxMoveCount;

    [Header("Available Gems")]
    public Gems[] gems;

    [Header("Collects(Sort in the order above)")]
    public int[] collects;

    [Header("Stars")]
    public int star_1;
    public int star_2;
    public int star_3;

    [Header("Boosters")]
    public Gems bombGem;
    public Gems dynamiteGem;
    public Gems atomicBombGem;

    [Header("Boosters(Number of colored tiles required for boosters)")]
    public int bomb;
    public int dynamite;
    public int atomicBomb;

    [Header("Boosters Power(How many neighbors they can collect)")]
    public int bombPower;
    public int dynamitePower;
    public int atomicBombPower;

    [Header("Random Generate")]
    public bool allRandom;
    public bool r_BoardSize;
    public bool r_MaxMoveCount;
    public bool r_Gems;
    public bool r_Collects;
    public bool r_Stars;

    [Header("Other")]
    public GameObject gameOverPrefab;
    Text levelText;
    MoveCount mCount;

    void Awake() 
    {
        if(collects.Length != gems.Length)
        {
            int[] old = collects;
            collects = new int [gems.Length];
            for(int i = 0; i < old.Length; i++)
            {
                collects[i] = old[i];
            }
        }

        mCount = FindObjectOfType<MoveCount>();
        randomGenerate();
    }

    void Start() 
    {
        if(width > 5 || height > 8)
        { 
            if(width - 5 > height - 8)
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize + (width - 5) * 0.85f; 
            }
            else
            {
                Camera.main.orthographicSize = Camera.main.orthographicSize + (height - 8) * 0.75f; 
            }
        }
        Camera.main.transform.position = new Vector3((width - 1) / 2f,(height - 1) / 2f,-10);

        levelText = GameObject.FindWithTag("LevelText").GetComponent<Text>();
        levelText.text = SceneManager.GetActiveScene().name;
    }

    public void gameOver(bool win)
    {
        GameObject g = Instantiate(gameOverPrefab);
        if(win)
        {
            if(mCount.moveCount > star_3)
            {
                g.GetComponent<GameOver>().Win(3);
            }
            else if(mCount.moveCount > star_2)
            {
                g.GetComponent<GameOver>().Win(2);
            }
            else if(mCount.moveCount > star_1)
            {
                g.GetComponent<GameOver>().Win(1);
            }
            else
            {
                g.GetComponent<GameOver>().Win(0);
            }

        }
        else
        {
            g.GetComponent<GameOver>().Lose();
        }
    }

    void randomGenerate()
    {
        if(r_BoardSize || allRandom)
        {
            height = Random.Range(5,10 + 1);
            width = Random.Range(5,10 + 1);
        }

        if(r_MaxMoveCount || allRandom)
        {
            maxMoveCount = Random.Range(10,25 + 1);
        }

        if(r_Stars || allRandom)
        {
            star_1 = maxMoveCount / 4;
            star_2 = maxMoveCount / 3;
            star_3 = maxMoveCount / 2;
        }

        if(r_Collects || allRandom)
        {
            int total = 0;
            for(int i = 0; i < collects.Length; i++)
            {
                if(total > maxMoveCount * 1.85f){ break; }
                collects[i] = Random.Range(5,20 + 1);
                total += collects[i];
            }
        }
    }
}
