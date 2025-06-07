using UnityEngine;

[ExecuteAlways]
public class SplitScreenDivider : MonoBehaviour
{
    [Header("Divider Settings")]
    [SerializeField] private Color lineColor = Color.black;
    [SerializeField] [Range(1, 20)] private int lineWidth = 2;

    private CameraManager camManager;
    private Texture2D colorTexture;

    private void Awake()
    {
        camManager = GetComponent<CameraManager>();
        CreateColorTexture();
    }

    private void OnValidate()
    {
        CreateColorTexture(); // در صورت تغییر رنگ در Inspector
    }

    private void CreateColorTexture()
    {
        if (colorTexture != null)
        {
            DestroyImmediate(colorTexture);
        }

        colorTexture = new Texture2D(1, 1);
        colorTexture.SetPixel(0, 0, lineColor);
        colorTexture.Apply();
    }

    private void OnGUI()
    {
        if (camManager == null || camManager.CurrentMode == CameraMode.Single)
            return;

        GUI.depth = -9999;

        GUIStyle style = new GUIStyle();
        style.normal.background = colorTexture;

        if (camManager.CurrentMode == CameraMode.VerticalSplit)
        {
            float xPos = Screen.width * camManager.SplitPosition;
            GUI.Box(new Rect(xPos - lineWidth / 2f, 0, lineWidth, Screen.height), GUIContent.none, style);
        }
        else if (camManager.CurrentMode == CameraMode.HorizontalSplit)
        {
            float yPos = Screen.height * camManager.SplitPosition;
            GUI.Box(new Rect(0, yPos - lineWidth / 2f, Screen.width, lineWidth), GUIContent.none, style);
        }
    }
}