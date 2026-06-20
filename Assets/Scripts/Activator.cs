using UnityEngine;
using System;

public abstract class Activator : MonoBehaviour
{
    public event Action<bool> StateChanged;

    protected bool isActive;
    public bool IsActive => isActive;

    protected void SetActive(bool active)
    {
        if (isActive == active) return;

        isActive = active;
        StateChanged?.Invoke(isActive);
    }
}