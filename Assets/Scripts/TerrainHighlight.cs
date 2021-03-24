using UnityEngine;

/// <summary>
/// TerrainHighlight class is used to highlight the terrain pointed by the mouse
/// </summary>
public class TerrainHighlight : MonoBehaviour
{
    private Camera currentCamera;

    /// <summary>
    /// On Start, subscribe to PlayManager events and fetches the main camera
    /// </summary>
    private void Start()
    {
        PlayManager.OnLoadSquadsOnNewDay += Activate;
        PlayManager.OnEndDay += Deactivate;

        currentCamera = Camera.main;
    }

    /// <summary>
    /// OnDestroy, unsubscribe events
    /// </summary>
    private void OnDestroy()
    {
        // Remove the Highlight from PlayUpdate
        GameManager.PlayUpdate -= GridHighlightUpdate;

        // Unsubscribe from other events
        PlayManager.OnLoadSquadsOnNewDay -= Activate;
        PlayManager.OnEndDay -= Deactivate;
    }

    /// <summary>
    /// Activate method activates the TerrainHighlight by subscribing to the PlayUpdate
    /// </summary>
    private void Activate()
    {
        gameObject.SetActive(true);
        GameManager.PlayUpdate += GridHighlightUpdate;
        
    }

    /// <summary>
    /// Deactivate method deactivates the TerrainHighlight by unsubscribing to the PlayUpdate
    /// </summary>
    private void Deactivate()
    {
        GameManager.PlayUpdate -= GridHighlightUpdate;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// GridHighlightUpdate catches the mouse position on terrain on get the nearest grid coordinates to place the green quad at them
    /// </summary>
    private void GridHighlightUpdate()
    {
        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                // Shows the quad and moves it to the nearest grid coordinates of the hit point
                GetComponent<MeshRenderer>().enabled = true;
                transform.position = GridAdjustment.GetGridCoordinates(hit.point) + Vector3.up * 0.01f;
            }
            else
            {
                if(Physics.Raycast(hit.transform.position + Vector3.up,-Vector3.up,out RaycastHit hitTerrain, Mathf.Infinity,LayerMask.GetMask("Terrain")))
                {
                    GetComponent<MeshRenderer>().enabled = true;
                    transform.position = GridAdjustment.GetGridCoordinates(hitTerrain.point) + Vector3.up * 0.01f;
                }
            }            
        }
        else
        {
            // If not on terrain, hides the quad
            GetComponent<MeshRenderer>().enabled = false;
        }
    }    
}
