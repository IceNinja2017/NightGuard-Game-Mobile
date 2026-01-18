using UnityEngine;

public class Door : MonoBehaviour, IPowerUser
{
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closePosition;
    [SerializeField] private float doorSpeed = 5;
    [SerializeField] private AudioSource DoorSfx;
    [SerializeField] private AudioSource DoorBonkSfx;
    private bool isOpen;
    public bool disabled = false;
    private Coroutine moveCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = openPosition;
        isOpen = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = isOpen ? openPosition : closePosition;

        if (Vector3.Distance(transform.position, targetPosition) <= 0.3f)
        {
            transform.position = targetPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, doorSpeed * Time.deltaTime);
        }
    }

    public void ToggleDoor()
    {
        if (!disabled)
        {
            if (getIsOpen()) DoorSfx.pitch = 1f;
            else DoorSfx.pitch = 0.9f;
            DoorSfx.Play();
            isOpen = !isOpen;
        }
    }

    public bool getIsOpen()
    {
        return isOpen;
    }

    //for interface
    public bool IsUsingPower()
    {
        return !getIsOpen();
    }
    public void setDisabled() //disabled the doors
    {
        if (!isOpen) ToggleDoor();
        disabled = true;

    }

    public void playDoorBonk()
    {
        DoorBonkSfx.Play();
    }
}
