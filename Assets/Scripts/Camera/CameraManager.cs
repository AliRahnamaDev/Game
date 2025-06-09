using UnityEngine;

public enum CameraMode
{
    Single,
    VerticalSplit,
    HorizontalSplit
}

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private CameraMode _currentMode = CameraMode.Single;
    [SerializeField] private int _activePlayer = 1;
    [SerializeField] [Range(0.1f, 0.9f)] private float _splitPosition = 0.5f;

    [Header("Zoom Settings")]
    [SerializeField] [Range(1f, 20f)] private float _singleCamSize = 5f;
    [SerializeField] [Range(1f, 20f)] private float _player1CamSize = 5f;
    [SerializeField] [Range(1f, 20f)] private float _player2CamSize = 5f;

    [Header("References")]
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2; 
    [SerializeField] private Camera _singleCam;
    [SerializeField] private Camera _player1Cam;
    [SerializeField] private Camera _player2Cam;

    public CameraMode CurrentMode => _currentMode;
    public float SplitPosition => _splitPosition;

    private void Start() => UpdateCameras();

    private void OnValidate()
    {
        if (Application.isPlaying)
            UpdateCameras();
    }

    public void UpdateCameras()
    {
        if (!CheckReferences()) return;

        SetCameraSizes();
        UpdateCameraStates();
    }

    private bool CheckReferences()
    {
        if (_player1 == null || _player2 == null ||
            _singleCam == null || _player1Cam == null || _player2Cam == null)
        {
            Debug.LogError("Missing references in CameraManager!");
            return false;
        }
        return true;
    }

    private void SetCameraSizes()
    {
        _singleCam.orthographicSize = _singleCamSize;
        _player1Cam.orthographicSize = _player1CamSize;
        _player2Cam.orthographicSize = _player2CamSize;
    }

    private void UpdateCameraStates()
    {
        _singleCam.gameObject.SetActive(false);
        _player1Cam.gameObject.SetActive(false);
        _player2Cam.gameObject.SetActive(false);

        switch (_currentMode)
        {
            case CameraMode.Single:
                _singleCam.gameObject.SetActive(true);
                SetFollowTarget(_singleCam, _activePlayer == 1 ? _player1 : _player2);
                break;

            case CameraMode.VerticalSplit:
                _player1Cam.gameObject.SetActive(true);
                _player2Cam.gameObject.SetActive(true);
                _player1Cam.rect = new Rect(0, 0, _splitPosition, 1);
                _player2Cam.rect = new Rect(_splitPosition, 0, 1 - _splitPosition, 1);
                SetFollowTarget(_player1Cam, _player1);
                SetFollowTarget(_player2Cam, _player2);
                break;

            case CameraMode.HorizontalSplit:
                _player1Cam.gameObject.SetActive(true);
                _player2Cam.gameObject.SetActive(true);
                _player1Cam.rect = new Rect(0, _splitPosition, 1, 1 - _splitPosition);
                _player2Cam.rect = new Rect(0, 0, 1, _splitPosition);
                SetFollowTarget(_player1Cam, _player1);
                SetFollowTarget(_player2Cam, _player2);
                break;
        }
    }

    private void SetFollowTarget(Camera cam, Transform parent)
    {
        var follow = cam.GetComponent<CameraFollow>();
        if (follow != null)
            follow.SetTarget(parent);
    }
}
