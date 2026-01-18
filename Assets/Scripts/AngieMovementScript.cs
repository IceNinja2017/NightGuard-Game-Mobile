using System.Collections.Generic;
using UnityEngine;
public class AngieMovementScript : AnimatronicBase
{
    [Header("Angie - Specific Settings")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject door; //rightdoor
    [SerializeField] private AvailableRooms selected; //this is for debugging

    private AudioSource[] sfx;
    private bool isSfxPlayed;
    private CameraLook camlook;
    private Door doorscript;

    public enum AvailableRooms
    {
        Stage, PartsAndService, DinningArea, Bathroom, RightHall, RightDoor, Office
    }

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
    }
    protected override void DefineRoomTransitions()
    {
        roomTransitions = new Dictionary<string, Dictionary<string, int>>()
        {
            {"Stage", new Dictionary<string, int>()
                {
                    { "PartsAndService", 100}
                }
            },
            {"PartsAndService", new Dictionary<string, int>()
                {
                    { "DinningArea", 90},
                }
            },
            {"DinningArea", new Dictionary<string, int>()
                {
                    { "Bathroom", 90},
                    { "PartsAndService", 10}
                }
            },
            {"Bathroom", new Dictionary<string, int>()
                {
                    { "RightHall", 70},
                    { "DinningArea", 30}
                }
            },
            { "RightHall", new Dictionary<string, int>()
                {
                    { "RightDoor", 70 },
                    { "Bathroom", 30 }
                }
            },
            { "RightDoor", new Dictionary<string, int>()
                {
                    { "Office", 100 }
                }
            }
        };
    }

    protected override void HandleDoorSFX()
    {
        if (current_room == "RightDoor")
        {
            if (camlook.getFlashlightState() && !isSfxPlayed && doorscript.getIsOpen() && camlook.getDoorSide() == 1)
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

    protected override void HandleRoomChange()
    {
        movementTimer += Time.deltaTime;
        if (movementTimer >= MovementInterval)
        {
            int roll = UnityEngine.Random.Range(0, 20);

            if (roll < AI_level)
            {
                string next = GetNextRoom(current_room);

                if (current_room == "RightDoor" && next == "Office")
                {
                    if (doorscript.getIsOpen() && camSystem.getIsCameraOpen() && current_room != "Office")
                    {
                        current_room = next;
                        jumpscareTimer = 0f;
                    }
                    else if (!doorscript.getIsOpen())
                    {
                        current_room = "DinningArea";
                        door.GetComponent<Door>().playDoorBonk();
                    }
                    else
                    {
                        //Debug.Log("LeftDoor to Office failed — camera not open.");
                    }
                }
                else
                {
                    current_room = next;
                    //Debug.Log("I've Moved to " + current_room);
                }
            }
            else
            {
                //Debug.Log("I Failed to Move ");}
            }
            movementTimer = 0;
        }
        ChangeRoom(current_room);
    }
}
