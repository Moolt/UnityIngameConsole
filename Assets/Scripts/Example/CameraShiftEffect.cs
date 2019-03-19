using IngameConsole;
using UnityEngine;

public class CameraShiftEffect : MonoBehaviour
{
    public float _offset = 5f;

    private Camera _camera;
    private Vector3 _defaultRotation;
    private ConsoleIO _consoleIO;
    private bool _paused = false;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _defaultRotation = _camera.transform.rotation.eulerAngles;
        _consoleIO = FindObjectOfType<ConsoleIO>();

        if(_consoleIO != null)
        {
            _consoleIO.VisibilityChanged += OnVisibilityChanged;
        }
    }

    void Update()
    {
        if(_paused)
        {
            return;
        }

        Vector2 screen = new Vector2(Screen.width, Screen.height);
        Vector2 mousePos = Input.mousePosition;

        Vector2 normalizedPosition = mousePos / screen;

        var _cameraOffset = new Vector3(
            _offset * 0.5f - _offset * normalizedPosition.y,
            -_offset * 0.5f + _offset * normalizedPosition.x,
            0f);

        var rotation = _camera.transform.rotation;
        _camera.transform.rotation = Quaternion.Euler(_defaultRotation + _cameraOffset);
    }

    private void OnDestroy()
    {
        if (_consoleIO != null)
        {
            _consoleIO.VisibilityChanged -= OnVisibilityChanged;
        }
    }

    private void OnVisibilityChanged(object sender, VisibilityChangedArgs args)
    {
        _paused = args.Visibility;
    }
}
