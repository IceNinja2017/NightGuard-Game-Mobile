using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovementScript : AnimatronicBase
{
    [Header("Cat - Specific Progression Settings")]
    [SerializeField, Tooltip("Current numerical progress value (0 to 600)")]
    private int progress = 0;

    private float flashTimer = 0f;
    private float requiredFlashDuration = -1f;
    private bool isBeingFlashed = false;

    // Track Cat's specific stage configuration locally to handle static triggers cleanly
    private string lastLoggedStage;

    protected override void Start()
    {
        startingRoom = "PartsAndService";
        base.Start();

        // Initialize our stage tracker so it doesn't trigger static on the very first frame
        lastLoggedStage = EvaluateRoomFromProgress(progress);
        UpdateCatVisuals();
    }

    protected override void Update()
    {
        HandleJumpscare();
        HandleRoomChange();

        if (current_room != "Office")
        {
            HandleFlashlightDefense();
        }

        // Custom static handler called every frame to catch desyncs instantly
        HandleCatStaticUI();
    }

    protected override void HandleRoomChange()
    {
        if (current_room == "Office") return;

        movementTimer += Time.deltaTime;

        if (movementTimer >= MovementInterval)
        {
            int roll = UnityEngine.Random.Range(0, 20);

            if (roll < AI_level)
            {
                int addedProgress = UnityEngine.Random.Range(25, 60);
                progress += addedProgress;
                if (progress > 600) progress = 600;

                Debug.Log($"Cat rolled successfully! Added +{addedProgress} progress. Total Progress: {progress}");

                string evaluatedRoom = EvaluateRoomFromProgress(progress);

                if (evaluatedRoom != current_room)
                {
                    current_room = evaluatedRoom;
                    Debug.Log($"Cat crossed threshold! Moved physically to room: {current_room}");
                    ChangeRoom(current_room);
                }
            }
            else
            {
                Debug.Log("Cat failed his movement roll.");
            }

            movementTimer = 0f;
        }
    }

    private string EvaluateRoomFromProgress(int currentProgress)
    {
        if (currentProgress >= 500) return "Office";
        if (currentProgress >= 400) return "Stage4";
        if (currentProgress >= 300) return "Stage3";
        if (currentProgress >= 200) return "Stage2";
        if (currentProgress >= 100) return "Stage1";
        return "PartsAndService";
    }

    private void HandleFlashlightDefense()
    {
        if (camSystem.getIsCameraOpen() &&
            camSystem.getCurrentActiveCam() == "PartsAndService" &&
            camSystem.getCamLightOn())
        {
            if (requiredFlashDuration < 0)
            {
                requiredFlashDuration = UnityEngine.Random.Range(1f, 2f);
                flashTimer = 0f;
                isBeingFlashed = true;
                Debug.Log($"Flashing Cat! Must hold for: {requiredFlashDuration:F2}s");
            }

            flashTimer += Time.deltaTime;

            if (flashTimer >= requiredFlashDuration)
            {
                if (progress > 0)
                {
                    int subtractedProgress = UnityEngine.Random.Range(50, 101);
                    progress -= subtractedProgress;
                    if (progress < 0) progress = 0;

                    Debug.Log($"Flash hit! Cat lost {subtractedProgress} progress. Current Progress: {progress}");
                    UpdateCatVisuals();
                }

                requiredFlashDuration = UnityEngine.Random.Range(1f, 2f);
                flashTimer = 0f;
            }
        }
        else
        {
            if (isBeingFlashed)
            {
                requiredFlashDuration = -1f;
                flashTimer = 0f;
                isBeingFlashed = false;
                Debug.Log("Player let go of flash or switched cams early.");
            }
        }
    }

    private void UpdateCatVisuals()
    {
        if (current_room == "Office") return;

        string targetStateKey = EvaluateRoomFromProgress(progress);
        current_room = targetStateKey;
        ChangeRoom(targetStateKey);
    }

    /// <summary>
    /// Custom static overlay rules designed specifically for Cat's progressive state changes
    /// </summary>
    private void HandleCatStaticUI()
    {
        // If his stage group layout hasn't changed at all, don't execute any heavy rendering calculations
        if (current_room == lastLoggedStage) return;

        // Trigger monitor interference ONLY if the player has their camera system actively pulled up 
        // AND they are physically inspecting Cat's location ("PartsAndService")
        if (camSystem.getIsCameraOpen() && camSystem.getCurrentActiveCam() == "PartsAndService")
        {
            Debug.Log($"Cat changed stage from {lastLoggedStage} to {current_room}! Triggering visual static.");
            camSystem.animatronicMoveStatic();
        }

        // Keep our state-machine baseline synchronized
        lastLoggedStage = current_room;
    }

    protected override void DefineRoomTransitions()
    {
        roomTransitions = new Dictionary<string, Dictionary<string, int>>()
        {
            {"PartsAndService", new Dictionary<string, int>() { { "Stage1", 100 } } },
            {"Stage1", new Dictionary<string, int>() { { "Stage2", 100 } } },
            {"Stage2", new Dictionary<string, int>() { { "Stage3", 90 }, { "Stage1", 10 } } },
            {"Stage3", new Dictionary<string, int>() { { "Stage4", 70 }, { "Stage2", 30 } } },
            {"Stage4", new Dictionary<string, int>() { { "Office", 100 } } }
        };
    }

    protected override void HandleDoorSFX() { }
}