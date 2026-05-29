using System.Collections.Generic;
using UnityEngine;

public class WortoxMovementScript : AnimatronicBase
{
    [Header("Wortox - Specific Settings")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject door; //leftdoor

    private AudioSource[] sfx;
    private bool isSfxPlayed;
    private CameraLook camlook;
    private Door doorscript;

    //when power is out
    private float powerOutJumpscareTimer = 0;
    public float jumpsacreDelay = -1f;
    //public enum AvailableRooms
    //{
    //    Stage, PrizeCorner, StudyArea, StudyArea2, Kitchen, LeftHall, LeftDoor, Office
    //}

    protected override void Start()
    {
        base.Start();
        camlook = player.GetComponent<CameraLook>();
        doorscript = door.GetComponent<Door>();
        sfx = GetComponents<AudioSource>();
    }

    protected override void Update()
    {
        base.Update();

        if (NightData.Instance.haspoweroutage)
        {
            powerOutJumpscareTimer += Time.deltaTime;
            if (jumpsacreDelay < 0) jumpsacreDelay = UnityEngine.Random.Range(4, 10);

            if (powerOutJumpscareTimer >= jumpsacreDelay)
            {
                current_room = "Office";
            }
        }
    }

    protected override void HandleRoomChange()
    {
        movementTimer += Time.deltaTime;
        if (movementTimer >= MovementInterval)
        {
            int roll = UnityEngine.Random.Range(0, 20);

            if (roll < AI_level)
            {
                string next = GetNextRoom(current_room);

                if (current_room == "LeftDoor" && next == "Office")
                {
                    if (doorscript.getIsOpen() && camSystem.getIsCameraOpen() && current_room != "Office")
                    {
                        current_room = next;
                        jumpscareTimer = 0f;
                    }
                    else if (!doorscript.getIsOpen())
                    {
                        current_room = "StudyArea";
                        door.GetComponent<Door>().playDoorBonk();
                    }
                }
                else
                {
                    current_room = next;
                }
            }

            movementTimer = 0;
        }
        ChangeRoom(current_room);
    }

    protected override void HandleDoorSFX()
    {
        if (current_room == "LeftDoor")
        {
            if (camlook.getFlashlightState() && !isSfxPlayed && doorscript.getIsOpen() && camlook.getDoorSide() == -1)
            {
                sfx[0].Play();
                isSfxPlayed = true;
            }
        }
        else
        {
            isSfxPlayed = false;
        }
    }

    protected override void DefineRoomTransitions()
    {
        roomTransitions = new Dictionary<string, Dictionary<string, int>>()
        {
            {"Stage", new Dictionary<string, int>()
                {
                    { "PrizeCorner", 100}
                }
            },
            {"PrizeCorner", new Dictionary<string, int>()
                {
                    { "StudyArea", 100},
                }
            },
            {"StudyArea", CreateDinningAreaToKitchen()},
            {"StudyArea2", CreateDinningAreaToKitchen()},
            {"Kitchen", new Dictionary<string, int>()
                {
                    { "LeftHall", 90},
                    { "StudyArea2", 10}
                }
            },
            {"LeftHall", new Dictionary<string, int>()
                {
                    { "LeftDoor", 70},
                    { "Kitchen", 30}
                }
            },
            { "LeftDoor", new Dictionary<string, int>()
                {
                    { "Office", 100 }
                }
            }
        };
    }

    private Dictionary<string, int> CreateDinningAreaToKitchen()
    {
        return new Dictionary<string, int>()
        {
            {"Kitchen", 70},
            {"PrizeCorner", 30}
        };
    }
}