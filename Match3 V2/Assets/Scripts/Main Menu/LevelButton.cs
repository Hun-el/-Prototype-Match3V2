using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform instantiateLoc;

    public void spawnButton()
    {
        int level = 0;
        if(instantiateLoc.childCount > 0)
        {
            level = instantiateLoc.GetChild(instantiateLoc.childCount - 1).GetComponent<LevelSelectButton>().level + 1;
        }
        else
        {
            level = 1;
        }

        GameObject buttonClone = Instantiate(buttonPrefab,instantiateLoc);
        buttonClone.GetComponent<LevelSelectButton>().levelText.text = level.ToString();
        buttonClone.GetComponent<LevelSelectButton>().level = level;
        buttonClone.name = "Level " + level + " Button";
    }
}
