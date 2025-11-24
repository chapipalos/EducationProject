using UnityEngine;
using UnityEngine.InputSystem;

public class FiguresController : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector2 currentPointerPos;

    private VisibleGrid grid;   // referencia al grid

    public MaterialSelector m_MaterialSelector;

    private void Awake()
    {
        cam = Camera.main;
        grid = FindObjectOfType<VisibleGrid>();

        if (grid == null)
            Debug.LogWarning("No se encontró VisibleGrid en la escena.");
    }

    private void Start()
    {
        if (grid != null)
            transform.position = SnapToGrid(transform.position);
    }

    public void OnPoint(InputAction.CallbackContext ctx)
    {
        currentPointerPos = ctx.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(currentPointerPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - (Vector3)worldPos;
            }
        }

        if (ctx.canceled)
        {
            isDragging = false;

            if (grid != null)
                transform.position = SnapToGrid(transform.position);
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(currentPointerPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                transform.Rotate(0f, 0, 45f);
            }
        }
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        float size = grid.cellSize;

        float x = Mathf.Round((pos.x - grid.transform.position.x) / size) * size + grid.transform.position.x;
        float y = Mathf.Round((pos.y - grid.transform.position.y) / size) * size + grid.transform.position.y;

        return new Vector3(x, y, pos.z);
    }

    private void Update()
    {
        if (!isDragging) return;

        Vector2 worldPos = cam.ScreenToWorldPoint(currentPointerPos);
        transform.position = new Vector3(worldPos.x + offset.x, worldPos.y + offset.y, 0);
    }
}

