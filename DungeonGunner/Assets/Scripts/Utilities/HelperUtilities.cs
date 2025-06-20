using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering.Universal;

public static class HelperUtilities
{
    public static Camera mainCamera;

    /// <summary>
    /// Returns the world position of the mouse cursor in 2D space.
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Clamp mouse position to screen bounds
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0, Screen.height);

        Vector3 worldPosition =  Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0; // Ensure z is zero for 2D games

        return worldPosition;
    }

    /// <summary>
    /// Returns angle in degrees from a Vector3
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float angleInRadians = Mathf.Atan2(vector.y, vector.x);
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
        return angleInDegrees;
    }

    public static AimDirection GetAimDirection(float angleInDegrees)
    {
        AimDirection aimDirection;

        if (angleInDegrees >= 22f && angleInDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        else if (angleInDegrees > 67f && angleInDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        else if (angleInDegrees > 112f && angleInDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        else if ((angleInDegrees > 158f && angleInDegrees <= 180f) || (angleInDegrees > -180f && angleInDegrees <= -135f))
        {
            aimDirection = AimDirection.Left;
        }
        else if (angleInDegrees > -135f && angleInDegrees <= -45f)
        {
            aimDirection = AimDirection.Down;
        }
        else if ((angleInDegrees > -45f && angleInDegrees <= 0) || (angleInDegrees > 0 && angleInDegrees < 22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right; // Default case if angle is not in the expected range
        }

       return aimDirection;
    }

    /// <summary>
    /// Empty String Debug Check
    /// <returns></returns>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    /// <summary>
    /// Null Object Debug Check
    /// <returns></returns>
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldName + " is null and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    /// <summary>
    /// list empty or contains null value check - returns true if there is an error
    /// <returns></returns>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + " is null in object " + thisObject.name.ToString());
            return true;
        }

        foreach(var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    /// <summary>
    /// Positive value debug check if zero is allowed, set isZeroAllowed to true. Returns true if there is an error.
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="valueToCheck"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if(valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in the object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a non-zero positive value " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    /// <summary>
    /// Positive range debug check. Set isZeroAllowed to true if zero is allowed in the range check. Returns true if there is an error.
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldNameMinimum"></param>
    /// <param name="valueToCheckMinimum"></param>
    /// <param name="fieldNameMaximum"></param>
    /// <param name="valueToCheckMaximum"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum,
        float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if(valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in the object " + thisObject.name.ToString());
            error = true;
        }

        if(ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed))
        {
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed))
        {
            error = true;
        }

        return error;
    }

    /// <summary>
    /// Positive value debug check if zero is allowed, set isZeroAllowed to true. Returns true if there is an error.
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="valueToCheck"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in the object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a non-zero positive value " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    public static Vector3 GetSpawnPointNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid currentRoomGrid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(100000000f, 100000000f, 0);

        //Loop through all the spawn positions in the room
        foreach(Vector2Int spawnPositionGridPos in currentRoom.spawnPositionArray)
        {
            // convert the spawn grid position to world position
            Vector3 spawnPointWorldPos = currentRoomGrid.CellToWorld((Vector3Int)spawnPositionGridPos);

            if(Vector3.Distance(spawnPointWorldPos, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPointWorldPos;
            }
        }

        return nearestSpawnPosition;
    }
}
