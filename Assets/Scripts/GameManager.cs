using System;
using NUnit.Framework.Interfaces;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsGameobject;
    [SerializeField] private GameObject menuGameobject;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider cameraSlider;

    [SerializeField] private CinemachineInputAxisController axisControllers;

    public static GameManager Instance;

    private Button settingsButton;
    private Button startButton;
    private Button exitButton;
    private Button returnButton;
    private Button resumeButton;

    private float storedSensitivity;
    private float storedVolume;

    private bool initialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void SetDefaultMenu()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))
        {
            menuGameobject.SetActive(true);
            settingsGameobject.SetActive(false);
        }
        else
        {
            menuGameobject.SetActive(false);
            settingsGameobject.SetActive(false);
        }
    }

    private void GetInitialValues()
    {
        if (volumeSlider != null)
        {
            storedVolume = volumeSlider.value;
        }

        if (cameraSlider != null)
        {
            storedSensitivity = cameraSlider.value;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Settings()
    {
        settingsGameobject.SetActive(true);
        menuGameobject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AdjustVolume()
    {
        float volume = volumeSlider.value;

        AudioListener.volume = volume;
        storedVolume = volume;
    }

    public void AdjustCamera()
    {
        storedSensitivity = cameraSlider.value;
        ApplySensitivityIfCameraExists();
    }

    public void ApplySensitivityIfCameraExists()
    {
        if (axisControllers == null)
        {
            axisControllers = FindFirstObjectByType<CinemachineInputAxisController>();
        }

        if (axisControllers == null)
        {
            return;
        }

        foreach (var c in axisControllers.Controllers)
        {
            if (c.Name == "Look X (Pan)")
            {
                c.Input.Gain = storedSensitivity;
            }

            if (c.Name == "Look Y (Tilt)")
            {
                c.Input.Gain = -storedSensitivity;
            }
        }
    }

    private void ApplyVolume()
    {
        AudioListener.volume = storedVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        volumeSlider = GameObject.FindWithTag("Volume Slider").GetComponent<Slider>();
        cameraSlider = GameObject.FindWithTag("Camera Slider").GetComponent<Slider>();
        AssignGameobjects();
        AssignButtons();
        AssignSliders();

        if (initialized)
        {
            SetSliderValues();
        }
        else
        {
            GetInitialValues();
            initialized = true;
        }

        ApplySensitivityIfCameraExists();
        ApplyVolume();

        SetDefaultMenu();

    }

    private void AssignSliders()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(); });
        }

        if (cameraSlider != null)
        {
            cameraSlider.onValueChanged.AddListener(delegate { AdjustCamera(); });
        }
    }

    private void SetSliderValues()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = storedVolume;
        }

        if (cameraSlider != null)
        {
            cameraSlider.value = storedSensitivity;
        }
    }
    public void Return()
    {
        settingsGameobject.SetActive(false);
        menuGameobject.SetActive(true);
    }

    private void AssignGameobjects()
    {
        menuGameobject = GameObject.FindGameObjectWithTag("Menu Gameobject");
        settingsGameobject = GameObject.FindGameObjectWithTag("Settings Gameobject");
    }

    private void AssignButtons()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))
        {
            startButton = GameObject.FindGameObjectWithTag("Start Button").GetComponent<Button>();
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            resumeButton = GameObject.FindGameObjectWithTag("Resume Button").GetComponent<Button>();
            resumeButton.onClick.AddListener(Resume);
        }

        settingsButton = GameObject.FindGameObjectWithTag("Settings Button").GetComponent<Button>();
        returnButton = GameObject.FindGameObjectWithTag("Return Button").GetComponent<Button>();
        exitButton = GameObject.FindGameObjectWithTag("Exit Button").GetComponent<Button>();

        settingsButton.onClick.AddListener(Settings);
        returnButton.onClick.AddListener(Return);
        exitButton.onClick.AddListener(ExitGame);

    }

    public void PauseMenu()
    {
        settingsGameobject.SetActive(false);
        menuGameobject.SetActive(true);

        if (axisControllers == true && axisControllers.enabled == true)
        {
            axisControllers.enabled = false;
        }

        Time.timeScale = 0f;
        AudioListener.pause = true;

    }

    public void Resume()
    {
        settingsGameobject.SetActive(false);
        menuGameobject.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (axisControllers == true && axisControllers.enabled == false)
        {
            axisControllers.enabled = true;
        }
    }


}
