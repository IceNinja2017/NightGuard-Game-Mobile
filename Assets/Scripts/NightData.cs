using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightData : MonoBehaviour
{
    [SerializeField] List<NightAIData> nights;

    [SerializeField] public bool haspoweroutage;

    private int CurrentNight; //make sure to load from saved data in future
    public static NightData Instance { get; private set; } // Singleton instance
    public Animatronic JumpscaringAnimatronic { get; private set; } //name of the animatronic that caused the game over

    private Dictionary<int, NightAIData> nightLookup;

    public bool isShiftCompleted; //check for is the player has finished all 5 nights


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        nightLookup = new Dictionary<int, NightAIData>();

        foreach (var night in nights)
            nightLookup.Add(night.night, night);

    }

    private void Start()
    {
        CurrentNight = SaveManager.instance.CurrentNight;
        isShiftCompleted = SaveManager.instance.isShiftCompleted;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    string currentScene = SceneManager.GetActiveScene().name;

        //    Debug.Log("Loaded SAVE! Night: " + SaveManager.instance.CurrentNight);

        //    SaveManager.instance.SaveGame(); //SaveGame when pressing ESC

        //    // Exit application on Escape key press (for testing purposes)
        //    if (currentScene == "MainMenu")
        //    {
        //        Application.Quit();
        //        // Optional: Works in the editor
        //        #if UNITY_EDITOR
        //                    UnityEditor.EditorApplication.isPlaying = false;
        //        #endif
        //    }
        //    else
        //    {
        //        SceneManager.LoadScene("MainMenu");
        //    }

            
        //}
    }

    public int getCurrentNight()
    {
        return CurrentNight;
    }

    public void setCurrentNight(int night)
    {
        CurrentNight = night;
    }

    public void nextNight()
    {
        if (CurrentNight >= 5)
        {
            //handele win
            return;
        }

        CurrentNight++;
    }

    public int GetAnimatronicAIOnNight(int nightNumber, Animatronic animatronic)
    {
        if (!nightLookup.TryGetValue(nightNumber, out var nightData))
        {
            Debug.LogWarning($"No night data found for night {nightNumber}");
            return 0;
        }

        foreach (var animAI in nightData.animatronics)
        {
            if (animAI.animatronic == animatronic)
                return animAI.aiLevel;
        }

        Debug.LogWarning($"No AI data found for {animatronic} on night {nightNumber}");
        return 0;
    }

    public int getAnimatronicAIOnCurrentNight(Animatronic animatronic)
    {
        if (!nightLookup.TryGetValue(CurrentNight, out var nightData))
        {
            Debug.LogWarning($"No night data found for night {CurrentNight}");
            return 0;
        }

        foreach (var animAI in nightData.animatronics)
        {
            return animAI.aiLevel;
        }

        Debug.LogWarning($"No AI data found for {animatronic} on night {CurrentNight}");
        return 0;
    }

    public float getPowerdrainOnCurrentNight()
    {
        return nightLookup[CurrentNight].powerDrain;
    }

    //used for Jumpscare on the GameOver Scene to set which animatronic jumped
    public void SetJumpscaringAnimatronic(Animatronic animatronic)
    {
        JumpscaringAnimatronic = animatronic;
    }
}
