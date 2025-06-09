using UnityEngine;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown splitScreenDropdown;
    public TMP_Dropdown cameraControlDropdown;

    [Header("Audio Sliders")]
    public UnityEngine.UI.Slider backgroundVolumeSlider;
    public UnityEngine.UI.Slider sfxVolumeSlider;

    private void Start()
    {
        // اطمینان از وجود EventSystem در صحنه برای کارکرد UI
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.LogWarning("SettingsUI: EventSystem not found in scene. Created one automatically.");
        }

        SetupDropdownOptions();
        LoadInitialValues(); // بارگذاری مقادیر فعلی بازی یا مقادیر ذخیره شده
    }

    void SetupDropdownOptions()
    {
        
    }

    void LoadInitialValues()
    {
        // TODO: اینجا باید مقادیر فعلی صدا و تنظیمات دوربین را از سیستم های مدیریت مربوطه بخوانید
        // و آنها را در Slider ها و Dropdown ها نمایش دهید (مثلاً از AudioManager.Instance.GetBackgroundVolume())
        backgroundVolumeSlider.value = PlayerPrefs.GetFloat("BackgroundVolume", 0.8f); // مثال: بارگذاری از PlayerPrefs
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f); // مثال: بارگذاری از PlayerPrefs

        // TODO: مقادیر فعلی SplitScreen و CameraControl را از سیستم مدیریت دوربین بخوانید و Dropdown ها را تنظیم کنید
        
        // اطمینان حاصل کنید که متدهای OnValueChanged بعد از تنظیم مقادیر اولیه فراخوانی می شوند
        OnBackgroundVolumeChanged(backgroundVolumeSlider.value);
        OnSFXVolumeChanged(sfxVolumeSlider.value);
       
    }

    public void OnBackgroundVolumeChanged(float volume)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBackgroundVolume(volume);
    }

    public void OnSFXVolumeChanged(float volume)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(volume);
    }


    // public void OnSplitScreenModeChanged(int index)
    // {
    //     // index مربوط به گزینه انتخاب شده در Dropdown است (0: Single, 1: Horizontal, 2: Vertical)
    //     string modeName = splitScreenDropdown.options[index].text;
    //     Debug.Log("Split Screen Mode changed to: " + modeName + " (Index: " + index + ")");
    //
    //     // TODO: این index یا modeName را به سیستم مدیریت Split Screen دوربین ارسال کنید
    //     // مثال: اگر SplitScreenManager دارید
    //     // if (SplitScreenManager.Instance != null) SplitScreenManager.Instance.ApplyMode(index);
    //
    //      // مثال: ذخیره مقدار در PlayerPrefs
    //     PlayerPrefs.SetInt("SplitScreenMode", index);
    // }

    // public void OnCameraControlChanged(int index)
    // {
    //      // index مربوط به گزینه انتخاب شده در Dropdown است (0: Player1, 1: Player2)
    //     //string controllerName = cameraControlDropdown.options[index].text;
    //     Debug.Log("Camera Controller changed to: " + controllerName + " (Index: " + index + ")");
    //
    //     // TODO: این index یا controllerName را به سیستم مدیریت دوربین شما ارسال کنید تا کنترلر مربوطه فعال شود
    //     // مثال: اگر CameraManager دارید
    //     // if (CameraManager.Instance != null) CameraManager.Instance.SetController(index);
    //
    //     // مثال: ذخیره مقدار در PlayerPrefs
    //     //PlayerPrefs.SetInt("CameraControl", index);
    // }

    // این متد برای دکمه برگشت در منوی تنظیمات استفاده می شود
    public void OnBackButtonPressed()
    {
        // بستن منوی تنظیمات از طریق UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseSettingsMenu();
        }
    }
}
