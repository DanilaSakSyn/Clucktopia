using UnityEngine;

public class OrientationBasedActivator : MonoBehaviour
{
    [Header("Target Object")]
    [SerializeField] private GameObject targetObject;

    [Header("Orientation Settings")]
    [SerializeField] private bool disableInPortrait = true;
    [SerializeField] private bool disableInLandscape = false;

    private void Update()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target object is not assigned.");
            return;
        }

        switch (Screen.orientation)
        {
            // Check the current screen orientation
            case ScreenOrientation.LandscapeLeft or ScreenOrientation.LandscapeRight when disableInLandscape:
            {
                // Disable the object in landscape mode
                if (targetObject.activeSelf)
                {
                    targetObject.SetActive(false);
                }

                break;
            }
            case ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown when disableInPortrait:
            {
                // Disable the object in portrait mode
                if (targetObject.activeSelf)
                {
                    targetObject.SetActive(false);
                }

                break;
            }
            default:
            {
                // Enable the object in other cases
                if (!targetObject.activeSelf)
                {
                    targetObject.SetActive(true);
                }

                break;
            }
        }
    }
}
