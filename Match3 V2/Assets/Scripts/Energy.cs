using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using System;

public class Energy : MonoBehaviour
{
    [SerializeField] Text energyText;
    [SerializeField] Text timerText;
    [SerializeField] int maxEnergy = 5;
    [HideInInspector] public int currentEnergy;
    [SerializeField] int restoreDuration = 1;

    DateTime nextEnergyTime;
    DateTime lastEnergyTime;
    bool isRestoring = false;

    public static Energy Instance;

    void Start() 
    {
        if(!PlayerPrefs.HasKey("currentEnergy"))
        {
            PlayerPrefs.SetInt("currentEnergy", maxEnergy);
            Load();
            StartCoroutine(RestoreEnergy());

            UpdateEnergy();
        }
        else
        {
            Load();
            StartCoroutine(RestoreEnergy());

            UpdateEnergy();
        }

        DontDestroyOnLoad (this);
        if (Instance == null) { Instance = this; } 
        else{ Destroy(this.gameObject); }
    }

    public void UseEnergy()
    {
        if(currentEnergy >= 1)
        {
            currentEnergy--;
            UpdateEnergy();

            if(!isRestoring)
            {
                if(currentEnergy + 1 == maxEnergy)
                {
                    nextEnergyTime = AddDuration(DateTime.Now,restoreDuration);
                }

                StartCoroutine(RestoreEnergy());
            }
        }
        else
        {
            
        }
    }

    IEnumerator RestoreEnergy()
    {
        UpdateEnergyTimer();
        isRestoring = true;
        while(currentEnergy < maxEnergy)
        {
            DateTime currentDateTime = DateTime.Now;
            DateTime nextDateTime = nextEnergyTime;
            bool isEnergyAdding = false;

            while(currentDateTime > nextDateTime)
            {
                if(currentDateTime > nextDateTime && currentEnergy < maxEnergy)
                {
                    isEnergyAdding = true;
                    currentEnergy++;
                    UpdateEnergy();
                    DateTime timeToAdd = lastEnergyTime > nextDateTime ? lastEnergyTime : nextDateTime;
                    nextDateTime = AddDuration(timeToAdd,restoreDuration);
                }
                else
                {
                    break;
                }
            }

            if(isEnergyAdding)
            {
                lastEnergyTime = DateTime.Now;
                nextEnergyTime = nextDateTime;
            }

            UpdateEnergyTimer();
            UpdateEnergy();
            Save();
            yield return null;
        }
        isRestoring = false;
    }

    DateTime AddDuration(DateTime dateTime,int duration)
    {
        return dateTime.AddMinutes(duration);
    }

    void UpdateEnergy()
    {
        if(SceneManager.GetActiveScene().name != "Main Menu"){ return; }
        energyText.text = currentEnergy.ToString() + "/" + maxEnergy.ToString();
    }

    void UpdateEnergyTimer()
    {
        if(SceneManager.GetActiveScene().name != "Main Menu"){ return; }
        if(currentEnergy >= maxEnergy)
        {
            timerText.text = "FULL";
            return;
        }

        TimeSpan time = nextEnergyTime - DateTime.Now;
        string timeValue = String.Format("{0:D2}:{1:D1}" , time.Minutes, time.Seconds);
        timerText.text = timeValue;
    }

    DateTime StringToDate(string dateTime)
    {
        if(String.IsNullOrEmpty(dateTime))
        {
            return DateTime.Now;
        }
        else
        {
            return DateTime.Parse(dateTime);
        }
    }

    void Load()
    {
        currentEnergy = PlayerPrefs.GetInt("currentEnergy");

        if(currentEnergy > maxEnergy){ currentEnergy = maxEnergy; PlayerPrefs.SetInt("currentEnergy",currentEnergy); }

        nextEnergyTime = StringToDate(PlayerPrefs.GetString("nextEnergyTime"));
        lastEnergyTime = StringToDate(PlayerPrefs.GetString("lastEnergyTime"));
    }

    void Save()
    {
        PlayerPrefs.SetInt("currentEnergy",currentEnergy);
        PlayerPrefs.SetString("nextEnergyTime",nextEnergyTime.ToString());
        PlayerPrefs.SetString("lastEnergyTime",lastEnergyTime.ToString());
    }
}
