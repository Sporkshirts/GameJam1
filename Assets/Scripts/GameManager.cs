
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 2f;

    [SerializeField] private CinemachineInputAxisController axisControllers;

    List<Enemy> enemies;
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
        Application.targetFrameRate = 60;
        
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

    public IEnumerator EndGame()
    {
        yield return Fade(0f, 1f);
    }

    private IEnumerator Fade(float start, float end)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        yield return new WaitForSeconds(2f);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);

            float tempVolume = Mathf.Lerp(storedVolume, 0, elapsed / fadeDuration);
            AudioListener.volume = tempVolume;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, end);

        SceneManager.LoadScene(2);

    }


    private void SetDefaultMenu()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("End Scene"))
        {
            return;
        }

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
        GameObject volumeGameobject = GameObject.FindWithTag("Volume Slider");
        GameObject cameraGameobject = GameObject.FindWithTag("Camera Slider");

        if (volumeGameobject)
        {
            volumeSlider = volumeGameobject.GetComponent<Slider>();
        }

        if (cameraGameobject)
        {
            cameraSlider = cameraGameobject.GetComponent<Slider>();
        }

        GameObject fadeGameObject = GameObject.FindWithTag("Fade");

        if (fadeGameObject)
        {
            fadeImage = fadeGameObject.GetComponent<Image>();
        }
        var enemyFound = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        enemies = new List<Enemy>(enemyFound);

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
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("End Scene"))
        {
            return;
        }

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

        Cursor.lockState = CursorLockMode.None;

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

        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (axisControllers == true && axisControllers.enabled == false)
        {
            axisControllers.enabled = true;
        }
    }


}
