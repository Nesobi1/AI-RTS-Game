using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour
{
    private Vector2 mouseFirstPos;
    private Vector2 mouseLastPos;
    [SerializeField] LayerMask groundLayer;

    private bool selection;
    private Rect selectionBox;
    [SerializeField] private Image selectionUI;
    [SerializeField] private Canvas canvas;
    private float scaleUI;

    private List<UnitScript> selectedUnits = new List<UnitScript>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scaleUI = canvas.scaleFactor;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            mouseFirstPos = Mouse.current.position.ReadValue();
        }
        if (Mouse.current.leftButton.isPressed && Mathf.Abs((mouseFirstPos - Mouse.current.position.ReadValue()).magnitude) > 20f)
        {
            Debug.Log("Selection started");
            mouseLastPos = Mouse.current.position.ReadValue();
            selection = true;
            float x = Mathf.Min(mouseFirstPos.x, mouseLastPos.x);
            float y = Mathf.Min(mouseFirstPos.y, mouseLastPos.y);
            float width = Mathf.Abs(mouseLastPos.x - mouseFirstPos.x);
            float height = Mathf.Abs(mouseLastPos.y - mouseFirstPos.y);
            selectionBox = new Rect(x, y, width, height);

            selectionUI.rectTransform.anchoredPosition = new Vector2(selectionBox.xMin, selectionBox.yMin) / scaleUI;
            selectionUI.rectTransform.sizeDelta = new Vector2(selectionBox.width, selectionBox.height) / scaleUI;
            selectionUI.gameObject.SetActive(true);

        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            selection = false;
            selectionUI.gameObject.SetActive(false);
            Debug.Log("Selection finished");
            deselectUnits();

            float x = Mathf.Min(mouseFirstPos.x, mouseLastPos.x);
            float y = Mathf.Min(mouseFirstPos.y, mouseLastPos.y);
            float width = Mathf.Abs(mouseLastPos.x - mouseFirstPos.x);
            float height = Mathf.Abs(mouseLastPos.y - mouseFirstPos.y);
            selectionBox = new Rect(x, y, width, height);
            selectUnits();
            
            Debug.Log("Selected Unit Count: " + selectedUnits.Count);
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Debug.Log("Move Command Issued");
            Debug.Log("Count: " + selectedUnits.Count);
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 10000f, groundLayer)) { 
                List<Vector3> positions = getFormationPositions(hit.point, selectedUnits.Count, 2f);
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    selectedUnits[i].MoveTo(positions[i]);
                }
            }
        }
    }

    void selectUnits()
    {
        foreach (UnitScript unit in UnitScript.all)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (screenPos.z > 0 && selectionBox.Contains(screenPos))
            {
                selectedUnits.Add(unit);
                unit.SetSelected(true);
            }
        }
    }

    void deselectUnits()
    {
        foreach(UnitScript unit in selectedUnits)
        {
            unit.SetSelected(false);
        }
        selectedUnits.Clear();
    }

    List<Vector3> getFormationPositions(Vector3 center, int count, float spacing)
    {
        List<Vector3> positions = new List<Vector3>();

        int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt(count / (float)columns);

        for(int i = 0; i<count; i++)
        {
            int row = i / columns;
            int col = i % columns;
            float x = (col - (columns - 1) / 2f) * spacing;
            float z = (row - (rows - 1) / 2f) * spacing;

            positions.Add(center + new Vector3(x, 0f, z));
        }

        return positions;
    }
}
