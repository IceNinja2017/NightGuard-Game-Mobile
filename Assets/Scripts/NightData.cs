using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NightData : MonoBehaviour
{
    [SerializeField] List<NightAIData> nights;

    [SerializeField] bool haspoweroutage;

    public static NightData Instance { get; private set; } // Singleton instance
    public Animatronic JumpscaringAnimatronic { get; private set; } //name of the animatronic that caused the game over

    private int CurrentNight = 1; //make sure to load from saved data in future

    private Dictionary<int, NightAIData> nightLookup;


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
    void Update()
    {
        // Exit application on Escape key press (for testing purposes)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            // Optional: Works in the editor
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    public int getCurrentNight()
    {
        return CurrentNight;
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
