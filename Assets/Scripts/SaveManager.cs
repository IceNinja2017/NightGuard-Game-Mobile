using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public int CurrentNight;
    public bool isShiftCompleted;

    string path;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            path = Application.persistentDataPath + "/save.json";
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            CurrentNight = CurrentNight,
            isShiftCompleted = isShiftCompleted
        };
            

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public void LoadGame()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            CurrentNight = data.CurrentNight;
            isShiftCompleted= data.isShiftCompleted;
            Debug.Log("Loaded SAVE! Night: " + CurrentNight);
        }
        else
        {
            CurrentNight = 1;
            isShiftCompleted = false;
        }
    }

    public void OnApplicationQuit()
    {
        SaveGame();
    }
}
