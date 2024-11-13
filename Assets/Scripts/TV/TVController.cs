using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TVController : MonoBehaviour
{
    [SerializeField] private GameObject[] blockPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int spawnLimit = 25;
    private int blockSpawnCount = 0;

    [SerializeField] private GameManager gameManager;

    // Массивы текстовых элементов для каждой страницы
    public TMP_Text[] mainMenuItems;
    public TMP_Text[] levelMenuItems;
    public TMP_Text[] cameraMenuItems;
    public TMP_Text[] spawnMenuItems;
    public TMP_Text[] loadMenuItems;
    public TMP_Text[] saveMenuItems;

    // GameObjects для каждой страницы
    public GameObject mainMenuPage;
    public GameObject levelMenuPage;
    public GameObject cameraMenuPage;
    public GameObject spawnMenuPage;
    public GameObject saveMenuPage;
    public GameObject loadMenuPage;

    private TMP_Text[] currentMenuItems;
    private int selectedIndex = 0;
    private int currentPage = 0;

    private void Start()
    {
        SetCurrentMenu(mainMenuItems, mainMenuPage);
        UpdateMenu();
    }

    public void LeftArrow()
    {
        selectedIndex = Mathf.Max(0, selectedIndex - 1);
        UpdateMenu();
    }
    public void RightArrow()
    {
        selectedIndex = Mathf.Min(currentMenuItems.Length - 1, selectedIndex + 1);
        UpdateMenu();
    }
    public void Enter()
    {
        SelectItem();
    }
    private void SetCurrentMenu(TMP_Text[] menuItems, GameObject menuPage)
    {
        // Выключить все страницы
        mainMenuPage.SetActive(false);
        levelMenuPage.SetActive(false);
        cameraMenuPage.SetActive(false);
        spawnMenuPage.SetActive(false);
        saveMenuPage.SetActive(false);
        loadMenuPage.SetActive(false);

        // Включить текущую страницу
        menuPage.SetActive(true);
        currentMenuItems = menuItems;
        selectedIndex = 0;
        UpdateMenu();
    }

    private void UpdateMenu()
    {
        for (int i = 0; i < currentMenuItems.Length; i++)
        {
            currentMenuItems[i].color = (i == selectedIndex) ? Color.red : Color.white;
        }
    }

    private void SelectItem()
    {
        switch (currentPage)
        {
            case 0:
                HandleMainMenuSelection();
                break;
            case 1:
                HandleLevelMenuSelection();
                break;
            case 2:
                HandleCameraMenuSelection();
                break;
            case 3:
                HandleSpawnMenuSelection();
                break;
        }
    }

    private void HandleMainMenuSelection()
    {
        switch (selectedIndex)
        {
            case 0:
                SceneManager.LoadScene("GameScene"); 
                break;
            case 1:
                currentPage = 1; 
                SetCurrentMenu(levelMenuItems, levelMenuPage);
                break;
            case 2:
                currentPage = 2; 
                SetCurrentMenu(cameraMenuItems, cameraMenuPage);
                break;
            case 3:
                currentPage = 3; 
                SetCurrentMenu(spawnMenuItems, spawnMenuPage);
                break;
            //case 4: 
            //    currentPage = 4;
            //    SetCurrentMenu()
            //    break;
            case 4:
                Application.Quit(); 
                break;
        }
    }

    private void HandleLevelMenuSelection()
    {
        switch (selectedIndex)
        {
            case 0:
                // Запуск скриптов
                break;
            case 1:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезапуск сцены
                break;
            case 2:
                currentPage = 4;
                SetCurrentMenu(saveMenuItems, saveMenuPage);
                break;
            case 3:
                currentPage = 5;
                SetCurrentMenu(loadMenuItems, loadMenuPage);
                break;
            case 4:
                currentPage = 2; // Переход к выбору видеокамер
                SetCurrentMenu(cameraMenuItems, cameraMenuPage);
                break;
            case 5:
                currentPage = 0; // Возврат в главное меню
                SetCurrentMenu(mainMenuItems, mainMenuPage);
                break;
        }
    }

    private void HandleCameraMenuSelection()
    {
        if (selectedIndex == currentMenuItems.Length - 1)
        {
            // Переход на предыдущую страницу
            currentPage = 0; // Или другой номер страницы
            SetCurrentMenu(mainMenuItems, mainMenuPage);
        }
        else
        {
            // Выбор камеры
        }
    }


    private void HandleSpawnMenuSelection()
    {
        // Логика выбора спавна блоков
        switch (selectedIndex)
        {
            case 0:
                SpawnBlock(0);
                break;
            case 1:
                SpawnBlock(1);
                break;
            case 2:
                SpawnBlock(2);
                break;
            case 3:
                SpawnBlock(3);
                break;
            case 4:
                currentPage = 0; // Возврат в главное меню
                SetCurrentMenu(mainMenuItems, mainMenuPage);
                break;
        }
    }


    private void SpawnBlock(int blockType)
    {
        if (blockSpawnCount <= spawnLimit)
        {
            Instantiate(blockPrefabs[blockType], spawnPoint.position, Quaternion.identity);
            blockSpawnCount++;
        }
    }
}