using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TVController : MonoBehaviour
{
    // Prefabs and spawn configuration
    [SerializeField] private GameObject[] blockPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int spawnLimit = 25;
    private int _blockSpawnCount = 0;
    private bool _startedRide;

    // UI Pages and Items
    [SerializeField] private TMP_Text[] lobbyItems;
    [SerializeField] private TMP_Text[] spawnMenuItems;

    [SerializeField] private GameObject lobbyPage;
    [SerializeField] private GameObject spawnMenuPage;
    [SerializeField] private GameObject playPongPage;
    [SerializeField] private GameObject sleepScreen;

    private TMP_Text[] currentMenuItems;
    private int selectedIndex;
    private int currentPage; // Tracks the current menu page
    
    private float inactivityTimer = 0f;
    [SerializeField] private float sleepDelay = 10f;
    [SerializeField] private Animator vehicle;
    [SerializeField] private string boolParameterName = "Start";
    [SerializeField] private GameObject leverForKronk;

    private void Start()
    {
        sleepScreen.SetActive(true);
        SetCurrentMenu(lobbyItems, lobbyPage);
    }
    
    private void Update()
    {
        inactivityTimer += Time.deltaTime;

        if (!(inactivityTimer >= sleepDelay)) return;
        if (playPongPage.activeSelf) return;
        sleepScreen.SetActive(true);
    }

    
    public void NavigateUp()
    {
        if (HandleSleepScreen()) return;
        selectedIndex = Mathf.Max(0, selectedIndex - 1);
        UpdateMenuHighlight();
        ResetInactivityTimer();
    }
    public void NavigateDown()
    {
        if (HandleSleepScreen()) return; 
        selectedIndex = Mathf.Min(currentMenuItems.Length - 1, selectedIndex + 1);
        UpdateMenuHighlight();
        ResetInactivityTimer();
    }

    public void ConfirmSelection()
    {
        if (HandleSleepScreen()) return;
        HandlePageSelection();
        ResetInactivityTimer();
    }

    public void NavigateLobby()
    {
        if (HandleSleepScreen()) return;
        currentPage = 0;
        SetCurrentMenu(lobbyItems, lobbyPage);
        ResetInactivityTimer();
    }
    
    private bool HandleSleepScreen()
    {
        if (!sleepScreen.activeSelf) return false;
        sleepScreen.SetActive(false);
        inactivityTimer = 0f;
        return true;
    }

    // Update the current menu and highlight the selected item
    private void SetCurrentMenu(TMP_Text[] pageItems, GameObject page)
    {
        // Disable all menu pages
        lobbyPage.SetActive(false);
        spawnMenuPage.SetActive(false);
        playPongPage.SetActive(false);

        // Activate the selected page and set menu items
        page.SetActive(true);
        currentMenuItems = pageItems;
        selectedIndex = 0;
        UpdateMenuHighlight();
    }
    
    private void UpdateMenuHighlight()
    {
        for (int i = 0; i < currentMenuItems.Length; i++)
        {
            currentMenuItems[i].color = (i == selectedIndex) ? Color.red : Color.white;
        }
    }

    // Handle selection based on the current page
    private void HandlePageSelection()
    {
        switch (currentPage)
        {
            case 0:
                HandleLobbySelection();
                break;
            case 1:
                HandleSpawnMenuSelection();
                break;
            case 2:
                HandleCameraMenuSelection();
                break;
        }
    }

    // Logic for the lobby selection
    private void HandleLobbySelection()
    {
        switch (selectedIndex)
        {
            case 0:
                SetCurrentMenu(spawnMenuItems, spawnMenuPage);
                currentPage = 1;
                break;
            case 1:
                if (_startedRide) return;
                _startedRide = true;
                leverForKronk.SetActive(true);
                vehicle.SetBool(boolParameterName, true);
                currentPage = 2;
                break;
            case 2:
                SetCurrentMenu(null, playPongPage);
                currentPage = 3;
                break;
            case 3:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case 4:
                Application.Quit();
                break;
        }
    }

    private void HandleCameraMenuSelection()
    {
        if (selectedIndex == currentMenuItems.Length - 1)
        {
            NavigateLobby();
        }
    }

    private void HandleSpawnMenuSelection()
    {
        if (selectedIndex < blockPrefabs.Length)
        {
            SpawnBlock(selectedIndex);
        }
    }
    
    private void SpawnBlock(int blockType)
    {
        if (_blockSpawnCount >= spawnLimit) return; 

        Instantiate(blockPrefabs[blockType], spawnPoint.position, Quaternion.identity);
        _blockSpawnCount++;
    }
    
    private void ResetInactivityTimer()
    {
        inactivityTimer = 0f;
        if (sleepScreen.activeSelf) sleepScreen.SetActive(false);
    }
}
