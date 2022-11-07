using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    [SerializeField] private InputEventSO inputEvent;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            inputEvent.RaiseTileTapped(SanatizeInput(position));
        }
    }

    private Vector2Int SanatizeInput(Vector3 postition)
    {
        var boardSize = TapMatchConfigSO.Instance.GetBoardSize();
        postition += new Vector3((int)(boardSize.x * .5f), (int)(boardSize.y * 0.5f));

        return new Vector2Int(
            Mathf.RoundToInt(postition.x),
            Mathf.RoundToInt(postition.y));
    }
}
