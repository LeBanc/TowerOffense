using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHighlight : MonoBehaviour
{
    private void Start()
    {
        
        GameManager.PlayUpdate += GridHighlightUpdate;
    }
    private void OnDestroy()
    {
        // Remove the Highlight from PlayUpdate
        GameManager.PlayUpdate -= GridHighlightUpdate;
    }

    /// <summary>
    /// GridHighlightUpdate catches the mouse position on terrain on get the nearest grid coordinates to place the green quad at them
    /// </summary>
    private void GridHighlightUpdate()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            // Shows the quad and moves it to the nearest grid coordinates of the hit point
            GetComponent<MeshRenderer>().enabled = true;
            transform.position = GridAdjustment.GetGridCoordinates(hit.point) + Vector3.up * 0.01f;
        }
        else
        {
            // If not on terrain, hides the quad
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    
}
