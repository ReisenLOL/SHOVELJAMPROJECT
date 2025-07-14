using UnityEngine;

public class AttachInfoPanelToCursor : MonoBehaviour
{
    public Vector3 offset;
    public RectTransform infoPanel;
    
    private void Update()
    { 
        infoPanel.position = Input.mousePosition + offset;
    }
}
