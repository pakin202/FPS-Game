using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class SettingManager : MonoBehaviour
{
    [Header("UI Components")]
    public Slider soundSlider;
    public Slider musicSlider;
    public Slider mouseSensitivitySlider;

    [Header("Audio Sources")]
    public AudioSource soundAudioSource; // สำหรับเสียงทั่วไป
    public AudioSource musicAudioSource; // สำหรับเสียงเพลง

    private StarterAssetsInputs playerInputs;

    private void Start()
    {
        playerInputs = FindObjectOfType<StarterAssetsInputs>();

        // เชื่อมโยง Event Listener กับ Slider
        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);

        LoadSettings(); // โหลดค่าที่บันทึกไว้เมื่อเกมเริ่มต้น
    }

    private void OnEnable()
    {
        LoadSettings(); // โหลดค่าตั้งต้นเมื่อ GameObject ถูกเปิดใช้งาน
    }

    public void SetSoundVolume(float volume)
    {
        if (soundAudioSource != null)
        {
            soundAudioSource.volume = volume; // ปรับ volume ของ Audio Source
            PlayerPrefs.SetFloat("SoundVolume", volume);
            PlayerPrefs.Save();
            Debug.Log($"[SettingManager] Set Sound Volume: {volume}");
        }
        else
        {
            Debug.LogError("[SettingManager] Sound AudioSource not found!");
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume; // ปรับ volume ของ Audio Source
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
            Debug.Log($"[SettingManager] Set Music Volume: {volume}");
        }
        else
        {
            Debug.LogError("[SettingManager] Music AudioSource not found!");
        }
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        if (playerInputs != null)
        {
            playerInputs.mouseSensitivity = sensitivity; // ใช้ตัวแปรโดยตรง
            PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
            PlayerPrefs.Save();
            Debug.Log($"[SettingManager] Set Mouse Sensitivity: {sensitivity}");
        }
        else
        {
            Debug.LogError("[SettingManager] StarterAssetsInputs not found!");
        }
    }

    private void LoadSettings()
    {
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        SetSoundVolume(soundSlider.value);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.2f);
        SetMusicVolume(musicSlider.value);

        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        SetMouseSensitivity(mouseSensitivitySlider.value);

        Debug.Log("[SettingManager] Loaded settings successfully.");
    }
}