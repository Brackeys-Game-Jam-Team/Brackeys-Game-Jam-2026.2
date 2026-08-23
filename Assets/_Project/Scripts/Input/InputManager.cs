using UnityEngine;

public class InputManager : MonoBehaviour
{
    public InputSystem_Actions InputActions { get; private set; }

    private void Awake()
    {
        InputActions = new();
    }

    public void EnablePlayer()
    {
        DisableAll();
        InputActions.Player.Enable();
    }

    public void EnableUI()
    {
        DisableAll();
        InputActions.UI.Enable();
    }

    private void DisableAll()
    {
        InputActions.Player.Disable();
        InputActions.UI.Enable();
    }
}