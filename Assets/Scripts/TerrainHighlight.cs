using UnityEngine;

/// <summary>
/// TerrainHighlight class is used to highlight the terrain pointed by the mouse
/// </summary>
public class TerrainHighlight : MonoBehaviour
{
    /// <summary>
    /// On Start, subscribe to PlayManager events
    /// </summary>
    private void Start()
    {
        PlayManager.OnLoadSquadsOnNewDay += delegate { Activate(true); };
        PlayManager.OnEndDay += delegate { Activate(false); };
    }

    /// <summary>
    /// OnDestroy, unsubscribe events
    /// </summary>
    private void OnDestroy()
    {
        // Remove the Highlight from PlayUpdate
        GameManager.PlayUpdate -= GridHighlightUpdate;
    }

    /// <summary>
    /// Activate method activates and deactivates the TerrainHighlight by subscribing/unsubscribing to the PlayUpdate
    /// </summary>
    /// <param name="_active">Active or deactive boolean</param>
    public void Activate(bool _active)
    {
        if (_active)
        {
            gameObject.SetActive(true);
            GameManager.PlayUpdate += GridHighlightUpdate;
        }
        else
        {
            GameManager.PlayUpdate -= GridHighlightUpdate;
            gameObject.SetActive(false);
        }
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
