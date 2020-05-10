using UnityEngine;
using Opsive.UltimateCharacterController.Events;

public class FMOD_AudioManager : MonoBehaviour
{
    private static FMOD_AudioManager s_Instance;
    public static FMOD_AudioManager Instance
    {
        get
        {
            if (!s_Initialized)
            {
                s_Instance = new GameObject("FMOD Audio Manager").AddComponent<FMOD_AudioManager>();
                s_Initialized = true;
            }
            return s_Instance;
        }
    }
    private static bool s_Initialized;

    private void Awake()
    {
        EventHandler.RegisterEvent<Transform, string>("PlayFMODSound", Play);
    }
    public void Play(Transform location, string eventName)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventName, location.position);
    }

    public void OnDestroy()
    {
        EventHandler.UnregisterEvent<Transform, string>("PlayFMODSound", Play);
    }
}