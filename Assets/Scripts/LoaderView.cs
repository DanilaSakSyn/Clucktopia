using System.Collections;
using UnityEngine;

public class LoaderView : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f; // скорость движения
    [SerializeField] private float movementAmplitude = 1f; // амплитуда движения
    [SerializeField] private bool moveOnEnable = true;

    private Coroutine _movementCoroutine;
    private bool _isMoving;

    private void OnEnable()
    {
        if (moveOnEnable)
        {
            StartMovement();
        }
    }

    private void OnDisable()
    {
        StopMovement();
    }

    public void StartMovement()
    {
        if (!_isMoving)
        {
            _isMoving = true;
            _movementCoroutine = StartCoroutine(MoveObject());
        }
    }

    public void StopMovement()
    {
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        _isMoving = false;
    }

    private IEnumerator MoveObject()
    {
        Vector3 startPosition = transform.position;
        while (_isMoving)
        {
            // Движение вверх и вниз с использованием синусоиды
            float newY = startPosition.y + Mathf.Sin(Time.time * movementSpeed) * movementAmplitude;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
            yield return null; // Ждем следующий кадр
        }
    }

    public void SetMovementSpeed(float newSpeed)
    {
        movementSpeed = newSpeed;
    }

    public void SetMovementAmplitude(float newAmplitude)
    {
        movementAmplitude = newAmplitude;
    }
}
