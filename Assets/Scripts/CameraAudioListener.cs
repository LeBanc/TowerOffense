using UnityEngine;

public class CameraAudioListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.SetActiveDefaultAudioListener(false);
    }

    private void OnDestroy()
    {
        AudioManager.SetActiveDefaultAudioListener(true);
    }
}
