using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public InputActionReference m_RotateAction;

    private void OnEnable()
    {
        m_RotateAction.action.Enable();
        m_RotateAction.action.performed += OnRotatePerformed;
    }

    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        // if the button is pressed, rotate the object 45 degrees around the Y axis
        transform.Rotate(0f, 0, 45f);
    }

    private void OnDisable()
    {
        m_RotateAction.action.performed -= OnRotatePerformed;
    }
}
