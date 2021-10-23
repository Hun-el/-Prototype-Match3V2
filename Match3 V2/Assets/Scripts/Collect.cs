using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collect : MonoBehaviour
{
    public GameObject collectPrefab;
    List<GameObject> collectCloneList = new List<GameObject>();
    Transform spawnPoint;

    int totalCount;

    int[] collects;
    Gems[] gems;

    GameManager gameManager;

    void Awake() 
    {
        gameManager = FindObjectOfType<GameManager>();
        spawnPoint = GameObject.FindWithTag("Collects").transform;
    }

    void Start() 
    {
        gems = gameManager.gems;
        collects = gameManager.collects;
        for(int i = 0; i < gems.Length; i++)
        {
            if(collects[i] > 0)
            {
                totalCount += collects[i];

                GameObject g = Instantiate(collectPrefab,spawnPoint);
                g.name = gems[i].type.ToString();
                collectCloneList.Add(g);

                setupClone(g,collects[i],gems[i].color);
            }
        }
    }

    void setupClone(GameObject clone , int count, Color color)
    {
        clone.GetComponent<Outline>().effectColor = color;
        clone.transform.GetChild(0).GetComponent<Text>().color = color;
        clone.transform.GetChild(0).GetComponent<Text>().text = count.ToString();
    }

    void updateClone(GameObject clone , int count)
    {
        clone.transform.GetChild(0).GetComponent<Text>().text = count.ToString();
    }

    public void decrease(string gemType)
    {
        for(int i = 0; i < gems.Length; i++)
        {
            if(gems[i].type.ToString() == gemType)
            {
                if(collects[i] > 0)
                {
                    collects[i]--;
                    totalCount--;

                    if(totalCount <= 0)
                    {
                        gameManager.gameOver(true);
                    }

                    foreach(var clone in collectCloneList)
                    {
                        if(clone.name == gemType)
                        {
                            updateClone(clone , collects[i]);
                        }
                    } 
                }
            }
        }
    }
}
