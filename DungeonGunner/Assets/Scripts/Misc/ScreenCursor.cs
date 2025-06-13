using UnityEngine;

public class ScreenCursor : MonoBehaviour
{
    private void Awake()
    {
        // Hide default cursor
        Cursor.visible = false;
    }

    private void Update()
    {
        //This script will be on a gameObject in the UI, the transform is a rectTransform (pixel units in the screen space)
        //Input.mousePosition is in screen space, so we can directly set the position of the cursor
        transform.position = Input.mousePosition;
    }
}
