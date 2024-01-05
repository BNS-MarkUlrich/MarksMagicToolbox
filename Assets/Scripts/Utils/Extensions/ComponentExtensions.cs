using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static bool TryGetCachedComponent<T, J>(
        this T targetComponent,
        ref Dictionary<T, J> cachedSet,
        out J cachedComponent
    ) where T : Component where J : Component
    {
        if (cachedSet.ContainsKey(targetComponent))
        {
            cachedComponent = cachedSet[targetComponent];
            return true;
        }

        if (targetComponent.TryGetComponent(out J component))
        {
            cachedSet.Add(targetComponent, component);
            cachedComponent = component;
            return true;
        }

        cachedComponent = default;
        Debug.LogWarning($"Collider {targetComponent} does not have component {typeof(Component)}");
        return false;
    }
}
