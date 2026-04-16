using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
   public enum SFX
    {
        GUNSHOT,
        SPLAT,
        TADAA, // plays when the player wins the game
        THROW,
        FOOTSTEPS,
        BULLETHIT
    }

    [SerializeField] private AudioClip[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;
    private bool HasAudioSource = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // audioSource = GetComponent<AudioSource>();

        if (!TryGetComponent(out audioSource)) Debug.Log("The Sound Manager did not have an AudioSource attached. It will not play sounds.");
        else HasAudioSource = true;
    }

    public static void PlaySound(SFX sound, float volume = 1)
    {
        if (instance.HasAudioSource) instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
    void Update()
    {
        
    }
}
