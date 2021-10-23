using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public int level;
    public Text levelText;
    string levelToLoad;
    public GameObject star1,star2,bigStar;

    void Start() 
    {
        levelToLoad = "Level " + level;

        if(PlayerPrefs.HasKey(levelToLoad + "Star"))
        {
            if(PlayerPrefs.GetInt(levelToLoad + "Star") == 3)
            {
                star1.GetComponent<Image>().color = Color.white;
                star2.GetComponent<Image>().color = Color.white;
                bigStar.GetComponent<Image>().color = Color.white;
            }
            if(PlayerPrefs.GetInt(levelToLoad + "Star") == 2)
            {
                star1.GetComponent<Image>().color = Color.white;
                star2.GetComponent<Image>().color = Color.white;
            }
            if(PlayerPrefs.GetInt(levelToLoad + "Star") == 1)
            {
                star1.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void LoadLevel()
    {
        if(Energy.Instance.currentEnergy > 0)
        {
            SceneManager.LoadScene(levelToLoad);
            Energy.Instance.UseEnergy();
        }
    }
}
