using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains a variety of extension methods that affect the transform properties of the target.
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// Rotates the target to face the direction parameter.
    /// </summary>
    /// <param name="target">The gameobject this function is called in.</param>
    /// <param name="direction">The direction the target object should face.</param>
    public static void LookAt(this GameObject target, Vector2 direction)
    {
        var localScale = target.transform.localScale;
        var isInvalidDirection = direction == Vector2.zero;
        
        if (isInvalidDirection) return;

        var horizontalDirection = Mathf.Sign(direction.x);

        localScale.x = horizontalDirection;
        target.transform.localScale = localScale;
        
        var angleRad = Mathf.Atan2(direction.y, direction.x);
        var angleDeg = angleRad * Mathf.Rad2Deg;
        angleDeg = horizontalDirection > 0 ? angleDeg : angleDeg - 180;
        target.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
    }

    /// <summary>
    /// A shortcut to setting the X position of a transform.
    /// </summary>
    /// <param name="target">The transform this function is called in.</param>
    /// <param name="xPosition">The X position you want to set for the target object.</param>
    public static void SetPositionX(this Transform target, float xPosition)
    {
        var targetPosition = target.position;
        targetPosition.x = xPosition;

        target.position = targetPosition;
    }
    
    /// <summary>
    /// A shortcut to setting the Y position of a transform.
    /// </summary>
    /// <param name="target">The transform this function is called in.</param>
    /// <param name="yPosition">The Y position you want to set for the target object.</param>
    public static void SetPositionY(this Transform target, float yPosition)
    {
        var targetPosition = target.position;
        targetPosition.y = yPosition;

        target.position = targetPosition;
    }

    /// <summary>
    /// A shortcut to setting the Z position of a transform.
    /// </summary>
    /// <param name="target">The transform this function is called in.</param>
    /// <param name="zPosition">The Z position you want to set for the target object.</param>
    public static void SetPositionZ(this Transform target, float zPosition)
    {
        var targetPosition = target.position;
        targetPosition.z = zPosition;

        target.position = targetPosition;
    }

    /// <summary>
    /// If true: hides target object by setting its scale to zero. <br></br> If false: reveals target object by setting its scale to one.
    /// </summary>
    /// <param name="target">The gameobject this function is called in.</param>
    /// <param name="isHidden">Checks whether the target object should be hidden. <br></br> (Default: true)</param>
    public static void Hide(this GameObject target, bool isHidden = true)
    {
        target.transform.localScale = isHidden ? Vector3.zero : Vector3.one;
    }
}