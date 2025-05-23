using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DarkenScene : MonoBehaviour
{
    public Volume volume;
    public float amount = -50f;

    public void DarkIt()
    {
        if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.postExposure.value = amount; // Negative values darken the scene
        }
    }
}
