using System;
using UnityEngine;

public class WorldLoadingState : MonoBehaviour
{
    public bool IsLoading { get; private set; }
    public bool IsLoaded { get; private set; }
    public float Progress01 { get; private set; }

    public void BeginLoading(int totalChunks)
    {
        IsLoading = true;
        IsLoaded = false;
        Progress01 = 0f;

        SetProgress(0, totalChunks);
    }

    public void SetProgress(int loadedChunks, int totalChunks)
    {
        if (totalChunks <= 0)
        {
            Progress01 = 0f;
            return;
        }
        Progress01 = Mathf.Clamp01((float)loadedChunks / totalChunks);
    }

    public void FinishLoading()
    {
        IsLoading = false;
        IsLoaded = true;
        Progress01 = 1f;
    }
}
