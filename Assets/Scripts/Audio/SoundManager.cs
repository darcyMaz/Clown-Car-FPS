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
        FOOTSTEPS
    }

    [SerializeField] private AudioClip[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SFX sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
    void Update()
    {
        
    }
}
