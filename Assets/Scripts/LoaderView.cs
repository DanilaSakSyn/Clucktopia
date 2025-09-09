using System.Collections;
using UnityEngine;

public class LoaderView : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 360f; // градусов в секунду
    [SerializeField] private bool rotateOnEnable = true;
    
    private Coroutine _rotationCoroutine;
    private bool _isRotating;

    private void OnEnable()
    {
        if (rotateOnEnable)
        {
            StartRotation();
        }
    }

    private void OnDisable()
    {
        StopRotation();
    }

    public void StartRotation()
    {
        if (!_isRotating)
        {
            _isRotating = true;
            _rotationCoroutine = StartCoroutine(RotateObject());
        }
    }

    public void StopRotation()
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
            _rotationCoroutine = null;
        }
        _isRotating = false;
    }

    private IEnumerator RotateObject()
    {
        while (_isRotating)
        {
            // Плавное вращение вокруг Z-оси
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            yield return null; // Ждем следующий кадр
        }
    }

    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }
}
