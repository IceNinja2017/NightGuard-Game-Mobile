using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AnimatronicBase : MonoBehaviour
{
    [Header("Base - General Settings")]
    [SerializeField, Tooltip("Animatronic GameObject with Animator component")]
    protected GameObject animatronic;
    [SerializeField, Tooltip("Enter the animatronic's name")]
    protected Animatronic animatronicName;

    [SerializeField, Tooltip("List of rooms with their associated transforms")]
    protected List<RoomEntry> roomsList;

    [SerializeField, Tooltip("Starting room name")]
    protected string startingRoom = "Stage";

    [SerializeField, Tooltip("Reference to ShiftTimer for timing info")]
    protected ShiftTimer timerScript;

    [SerializeField, Tooltip("Reference to CameraSystem script for the Monitor")]
    protected CameraSystem camSystem;

    [SerializeField, Tooltip("Sets the interval between movement Opportunities")]
    protected int MovementInterval = 7;

    [SerializeField, Tooltip("Current AI level of animatronic")]
    protected int AI_level = 0;
    protected Dictionary<String, Dictionary<String, int>> roomTransitions;
    protected float movementTimer = 0;

    protected Animator anim;
    protected Dictionary<string, Roomvalue> roomDict = new Dictionary<string, Roomvalue>();
    protected string current_room;
    protected float jumpscareTimer;
    protected bool wasOnCam = false;
    protected string previous_room;

    protected virtual void Awake()
    {
        foreach (var entry in roomsList)
        {
            if (!roomDict.ContainsKey(entry.key))
            {
                roomDict.Add(entry.key, entry.value);
            }
        }
    }

    protected virtual void Start()
    {
        Debug.Log("Starting " + animatronicName);
        AI_level = NightData.Instance.getAnimatronicAIOnCurrentNight(animatronicName);
        anim = animatronic.GetComponent<Animator>();
        current_room = startingRoom;
        DefineRoomTransitions();
    }

    protected virtual void Update()
    {
        HandleStaticUI();
        HandleAILevelIncrease();
        HandleRoomChange();
        HandleDoorSFX();
        HandleJumpscare();
    }

    //Increases AI level as the night progress;
    protected virtual void HandleAILevelIncrease()
    {
        if (timerScript.IsnewHour())
        {
            int hour = timerScript.getCurrentHour();
            if (hour >= 2 && hour <= 5 && AI_level > 20)
            {
                AI_level += 1;
            }
        }
    }

    protected abstract void HandleRoomChange();
    protected abstract void HandleDoorSFX();
    protected abstract void DefineRoomTransitions();
    protected virtual void HandleJumpscare()
    {
        if (current_room != "Office") return;

        jumpscareTimer += Time.deltaTime;

        if (jumpscareTimer <= 4f && camSystem.getIsCameraOpen())
        {
            camSystem.toggleCamera();
            Debug.Log("Camera Forced down for jumpscare");
        }

        if (!camSystem.getIsCameraOpen())
        {
            Debug.Log("jumpscare!!!!");
            NightData.Instance.SetJumpscaringAnimatronic(animatronicName);
            SceneManager.LoadScene("JumpscareScene");
        }
    }

    //Get's transform values of the named room
    public virtual Roomvalue? GetKey(string roomName)
    {
        return roomDict.TryGetValue(roomName, out Roomvalue room) ? room : (Roomvalue?)null;
    }

    //handles the roomchange for the animatronic
    public virtual void ChangeRoom(string room)
    {
        Roomvalue? target = GetKey(room);
        if (target == null) return;

        if (animatronic.transform.position != target.Value.trans.position)
        {
            anim.Play(target.Value.animationName);
            animatronic.transform.SetPositionAndRotation(target.Value.trans.position, target.Value.trans.rotation);
        }
    }

    public virtual bool SucessfulMove()
    {
        int rand = UnityEngine.Random.Range(1, 20);
        if (AI_level <= rand)
        {
            return true;
        }
        return false;
    }

    public virtual string GetNextRoom(string currentRoom)
    {
        if (!roomTransitions.ContainsKey(currentRoom)) return currentRoom;

        var possibleRooms = roomTransitions[currentRoom];
        int roll = UnityEngine.Random.Range(0, 100);
        int sum = 0;

        foreach (var room in possibleRooms)
        {
            sum += room.Value;
            if (roll < sum) return room.Key;
        }
        return currentRoom;
    }

    private void HandleStaticUI()
    {
        string activeCam = camSystem.getCurrentActiveCam();

        if (current_room != previous_room)
        {
            bool wasVisible = previous_room == activeCam;
            bool nowVisible = current_room == activeCam;

            if (wasVisible || nowVisible)
            {
                camSystem.animatronicMoveStatic();
            }

            previous_room = current_room;
        }
    }
}