using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject background, mainMenu, settings, results, gameplay, pause, lose, worldSelector; // все вкладки меню
    private GameObject worldSelected; // выбранная доска
    public GameObject world_3x3, world_3x4, world_4x4; // все возможные доски
    [HideInInspector] public int lives; // жизни игрока
    [HideInInspector] public int pointsScrore; // набранные очки
    [HideInInspector] public float catScore; // очки к разным бонусам
    [HideInInspector] public float sheepScore;
    [HideInInspector] public float duckScore;
    [SerializeField] private Slider catSlider; // слайдеры бонусов
    [SerializeField] private Slider sheepSlider;
    [SerializeField] private Slider duckSlider;
    [SerializeField] private Text result_1_PlaceB, result_2_PlaceB, result_3_PlaceB; // результате в вкладке Результаты
    [SerializeField] private Text result_1_PlaceF, result_2_PlaceF, result_3_PlaceF;
    [SerializeField] private Slider musicSliderMM, soundSliderMM; // слайдеры в настройках меню
    [SerializeField] private Slider musicSliderP, soundSliderP; // слайдеры в паузе
    private int restart = 0;
    public Text scoreTextBack, scoreTextForw; // текст отображаюший результат
    public Text livesTextBack, livesTextForw; // текст отображающий кол-во жизней
    public Text scoreNumberForw, scoreNumberBack; // текст отображающий итоговый результат
    private bool lost = false;

    static public UIManager instance;
    private void Awake()
    {
        if(UIManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        UIManager.instance = this;
    }
    public void Start()
    {
        if (PlayerPrefs.HasKey("Restart"))
            restart = PlayerPrefs.GetInt("Restart");
        lives = 5;
        worldSelected = world_3x3;
        InizializationUI(ref restart);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            PlayerPrefs.DeleteAll();
        ScoreLiveUpdate(ref scoreTextBack, ref scoreTextForw, ref livesTextBack, ref livesTextForw);
        if (lives < 1 && !lost)
            Lose();
        catSlider.value = catScore;
        sheepSlider.value = sheepScore;
        duckSlider.value = duckScore;
        ScoreBonusCheck();
    }
    #region MainMenu
    public void StartMainMenu()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        Switcher(mainMenu, worldSelector);
    }
    public void Settings()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        Switcher(mainMenu, settings);
       //Первый запуск игры
        if (!PlayerPrefs.HasKey("musicVolume"))
            musicSliderMM.value = 0.5f;
        if (!PlayerPrefs.HasKey("soundVolume"))
            soundSliderMM.value = 0.5f;
        // Настройки уже имеются
        if (PlayerPrefs.HasKey("musicVolume"))
            musicSliderMM.value = PlayerPrefs.GetFloat("musicVolume");
        if (PlayerPrefs.HasKey("soundVolume"))
            soundSliderMM.value = PlayerPrefs.GetFloat("soundVolume");
    }
    public void Results()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        Switcher(mainMenu, results);
        ShowSavedResults(PlayerPrefs.GetInt("Save_1").ToString(), PlayerPrefs.GetInt("Save_2").ToString(), PlayerPrefs.GetInt("Save_3").ToString());
    }
    public void ExitMainMenu()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        Application.Quit();
    }
    #endregion

    #region WorldSelector
    public void Button_3x3()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        worldSelected = world_3x3;
        Switcher(worldSelector, gameplay);
        worldSelected.SetActive(true);
        Time.timeScale = 1f;
    }
    public void Button_3x4()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        worldSelected = world_3x4;
        Switcher(worldSelector, gameplay);
        worldSelected.SetActive(true);
        Time.timeScale = 1f;
    }
    public void Button_4x4()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        worldSelected = world_4x4;
        Switcher(worldSelector, gameplay);
        worldSelected.SetActive(true);
        Time.timeScale = 1f;
    }
    public void ExitToMainMenuWorldSelector()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        Switcher(worldSelector, mainMenu);
    }
    #endregion

    #region Settings
    public void MusicSlider(float volume)
    {
        SoundManager.instance.ChangeMusicVolume(volume);
    }
    public void SoundSlider(float volume)
    {
        SoundManager.instance.ChangeSoundVolume(volume);
    }
    public void ExitToMainMenuSettings()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        Switcher(settings, mainMenu);
    }
    #endregion

    #region Result
    public void ExitToMainMenuResult()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        Switcher(results, mainMenu);
    }
    #endregion

    #region Pause
    public void ContinuePause()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        Switcher(pause, gameplay);
        Time.timeScale = 1f;
        worldSelected.SetActive(true);
    }
    public void ExitPause()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region Gameplay
    public void PauseGP()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        worldSelected.SetActive(false);
        Switcher(gameplay, pause);

        if (!PlayerPrefs.HasKey("musicVolume"))
            musicSliderP.value = 0.5f;
        if (!PlayerPrefs.HasKey("soundVolume"))
            soundSliderP.value = 0.5f;

        if (PlayerPrefs.HasKey("musicVolume"))
            musicSliderP.value = PlayerPrefs.GetFloat("musicVolume");
        if (PlayerPrefs.HasKey("soundVolume"))
            soundSliderP.value = PlayerPrefs.GetFloat("soundVolume");
    }
    #endregion

    #region Lose
    public void Restart()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        if(worldSelected == world_3x3)
            PlayerPrefs.SetInt("Restart", 1);
        if((worldSelected == world_3x4))
            PlayerPrefs.SetInt("Restart", 2);
        if ((worldSelected == world_4x4))
            PlayerPrefs.SetInt("Restart", 3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitToMainMenuLose()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.click);
        lose.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region UIMethods
    /// <summary>
    /// Первоначальная настройка UI при включении игры
    /// </summary>
    /// <param name="restart"></param>
    public void InizializationUI(ref int restart)
    {
        background.SetActive(true);
        mainMenu.SetActive(true);
        settings.SetActive(false);
        results.SetActive(false);
        gameplay.SetActive(false);
        pause.SetActive(false);
        lose.SetActive(false);
        worldSelector.SetActive(false);
        world_3x3.SetActive(false);
        world_3x4.SetActive(false);
        world_4x4.SetActive(false);
        Time.timeScale = 0f;
        // В случае рестарта :
        if(restart > 0)
        {
            Switcher(mainMenu, gameplay);
            Time.timeScale = 1f;
            if (restart == 1)
                worldSelected = world_3x3;
            if (restart == 2)
                worldSelected = world_3x4;
            if (restart == 3)
                worldSelected = world_4x4;
            restart = 0;
            PlayerPrefs.SetInt("Restart", 0);
            worldSelected.SetActive(true);
        }
    }
    /// <summary>
    /// Переключает вкладки UI между собой(переходы в меню)
    /// </summary>
    /// <param name="switchOff">Отключаемая вкладка</param>
    /// <param name="switchOn">Включаемая вкладка</param>
    public void Switcher(GameObject switchOff, GameObject switchOn)
    {
        switchOff.SetActive(false);
        switchOn.SetActive(true);
    }
    /// <summary>
    /// Выводит обновлящийся результат
    /// </summary>
    /// <param name="scoreBack">Счет очков(фронтальный)</param>
    /// <param name="scoreForw">Счет очков(задний)</param>
    /// <param name="livesBack">Счет жизней(фронтальный)</param>
    /// <param name="livesForw">Счет жизней(задний)</param>
    public void ScoreLiveUpdate(ref Text scoreBack, ref Text scoreForw, ref Text livesBack, ref Text livesForw)
    {
        scoreBack.text = pointsScrore.ToString();
        scoreForw.text = pointsScrore.ToString();
        livesBack.text = lives.ToString();
        livesForw.text = lives.ToString();
        scoreNumberForw.text = pointsScrore.ToString();
        scoreNumberBack.text = pointsScrore.ToString();
    }
    /// <summary>
    /// Логика при проигрыше
    /// </summary>
    public void Lose()
    {
        lost = true;
        SoundManager.instance.musicSource.Stop();
        SoundManager.instance.PlaySound(SoundManager.instance.lose);
        worldSelected.SetActive(false);
        Switcher(gameplay, lose);
        SaveResult();
        Time.timeScale = 0f;
    }
    /// <summary>
    /// Добавляет жизни при достаточном количетсве набранных бонусов
    /// </summary>
    private void ScoreBonusCheck()
    {
        if (catScore == 5)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.addLive);
            lives++;
            catScore = 0;
        }
        if (sheepScore == 5)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.addLive);
            lives++;
            sheepScore = 0;
        }
        if (duckScore == 5)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.addLive);
            lives++;
            duckScore = 0;
        }
    }
    /// <summary>
    /// Сортирует и сохраняет лучшие результаты
    /// </summary>
    private void SaveResult()
    {
        if (!PlayerPrefs.HasKey("Save_1"))
        {
            PlayerPrefs.SetInt("Save_1", 0);
            PlayerPrefs.SetInt("Save_2", 0);
            PlayerPrefs.SetInt("Save_3", 0);
        }

        int[] saveSlots = new int[4];
        saveSlots[0] = PlayerPrefs.GetInt("Save_1");
        saveSlots[1] = PlayerPrefs.GetInt("Save_2");
        saveSlots[2] = PlayerPrefs.GetInt("Save_3");
        saveSlots[3] = 0;

        int[] dataSlots = new int[4];
        dataSlots[0] = PlayerPrefs.GetInt("Data_1");
        dataSlots[1] = PlayerPrefs.GetInt("Data_2");
        dataSlots[2] = PlayerPrefs.GetInt("Data_3");
        dataSlots[3] = 0;

        if (saveSlots[2] > pointsScrore)
            return;
        saveSlots[3] = pointsScrore;
        int tempScore;
        for (int i = saveSlots.Length - 1; i > -1; i--)
        {
            if (i == 0) break;
            if(saveSlots[i] > saveSlots[i - 1])
            {
                tempScore = saveSlots[i - 1];
                saveSlots[i - 1] = saveSlots[i];
                saveSlots[i] = tempScore;
            }
        }
        PlayerPrefs.SetInt("Save_1", saveSlots[0]);
        PlayerPrefs.SetInt("Save_2", saveSlots[1]);
        PlayerPrefs.SetInt("Save_3", saveSlots[2]);
    }
    /// <summary>
    /// Выводить сохраненые результаты во вкладку Результаты
    /// </summary>
    /// <param name="result_1Place">Первое место</param>
    /// <param name="result_2Place">Второе место</param>
    /// <param name="result_3Place">Третье место</param>
    private void ShowSavedResults(string result_1Place, string result_2Place, string result_3Place)
    {
        result_1_PlaceB.text = result_1Place;
        result_2_PlaceB.text = result_2Place;
        result_3_PlaceB.text = result_3Place;

        result_1_PlaceF.text = result_1Place;
        result_2_PlaceF.text = result_2Place;
        result_3_PlaceF.text = result_3Place;
    }
    #endregion
}
