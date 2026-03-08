using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    public static PlayerHud runtime;

    public int displayMessageLength = 2000;

    public GameObject crouchIndicator;

    public GameObject wheelRoot;

    public SoundPlayer wheelSelectSound;

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        PlayerMovement.LockMovement(gameObject, false);
        wheelRoot.SetActive(false);
    }

    public void SetCrouch(bool crouch)
    {
        crouchIndicator.SetActive(crouch);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PlayerMovement.LockMovement(gameObject, true);
            wheelRoot.SetActive(true);
        }

        PlayerWheelOption selection = null;
        if (wheelRoot.activeSelf)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponentInParent<PlayerWheelOption>())
                {
                    selection = result.gameObject.GetComponentInParent<PlayerWheelOption>();
                }
            }

            foreach(PlayerWheelOption option in FindObjectsByType<PlayerWheelOption>(FindObjectsSortMode.None))
            {
                if (option == selection)
                    option.Select();
                else
                    option.Deselect();
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (selection)
            {
                wheelSelectSound.PlaySound();
                selection.Open();
            }
            PlayerMovement.LockMovement(gameObject, false);
            wheelRoot.SetActive(false);
        }

        if (!Input.GetKey(KeyCode.Tab))
            return;
    }
}
