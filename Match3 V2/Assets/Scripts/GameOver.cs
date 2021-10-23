using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameOver : MonoBehaviour
{
    public GameObject Stars;
    public Button button;
    public Text win_lose;
    public Text buttonText;

    public void Win(int star)
    {  
        win_lose.text = "YOU WIN!";
        buttonText.text = "NEXT LEVEL";

        if(PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "Star"))
        {
            if(PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Star") < star)
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Star", star);
            }
        }
        else
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Star", star);
        }

        for(int i = 0; i < Stars.transform.childCount; i++)
        {
            if(star != 0)
            {
                Stars.transform.GetChild(i).GetComponent<Image>().DOFade(1,0.25f).SetDelay(1f);
                star--;
            }
        }

        button.onClick.AddListener(delegate() { winButton(); });
    }

    public void Lose()
    {
        win_lose.text = "YOU LOSE!";
        buttonText.text = "RETRY";
        Destroy(Stars);

        Energy.Instance.UseEnergy();

        button.onClick.AddListener(delegate() { loseButton(); });
    }

    public void loseButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void winButton()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int level = (int)sceneName[sceneName.Length - 1] - 47;

        if(Energy.Instance.currentEnergy > 0)
        {
            if (Application.CanStreamedLevelBeLoaded(SceneManager.GetActiveScene().name.Substring(0, 5) + " " + level))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name.Substring(0, 5) + " " + level);
            }
            else
            {
                SceneManager.LoadScene("Random Level");
            }

            Energy.Instance.UseEnergy();
        }
    }
}
