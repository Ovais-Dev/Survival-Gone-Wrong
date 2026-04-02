using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake _instance;
    public static CameraShake Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CameraShake>();
            }
            return _instance;
        }
    }
    public CinemachineImpulseSource cinemachineImpulseSource;
    
    public void Shake()
    {
        cinemachineImpulseSource.GenerateImpulse();
    }
    public void Shake(Vector3 velocity)
    {
        cinemachineImpulseSource.GenerateImpulse(velocity);
    }
}