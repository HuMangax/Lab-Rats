using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public abstract class Unlockable : MonoBehaviour
{
    public List<Activator> activators;

    protected bool isUnlocked;

    private void OnEnable()
    {
        foreach (var activator in activators) activator.StateChanged += OnStateChanged;

        CheckUnlock();
    }

    private void OnDisable()
    {
        foreach (var activator in activators) activator.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged(bool active)
    {
        CheckUnlock();
    }

    private void CheckUnlock()
    {
        bool shouldUnlock = activators.All(a => a.IsActive);

        if (shouldUnlock == isUnlocked)
            return;

        isUnlocked = shouldUnlock;

        if (isUnlocked)
            Unlock();
        else
            Lock();
    }

    protected abstract void Unlock();

    protected abstract void Lock();
}
