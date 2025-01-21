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
    private int blockSpawnCount = 0;

    // Game manager reference
    [SerializeField] private GameManager gameManager;

    // Menu items and pages
    [SerializeField] private TMP_Text[] lobbyItems;
    [SerializeField] private TMP_Text[] cameraMenuItems;
    [SerializeField] private TMP_Text[] spawnMenuItems;

    [SerializeField] private GameObject lobbyPage;
    [SerializeField] private GameObject cameraMenuPage;
    [SerializeField] private GameObject spawnMenuPage;

    private TMP_Text[] currentMenuItems;
    private int selectedIndex = 0;
    private int currentPage = 0; // Tracks the current menu page

    private void Start()
    {
        SetCurrentMenu(lobbyItems, lobbyPage);
    }
    
    public void NavigateLeft()
    {
        selectedIndex = Mathf.Max(0, selectedIndex - 1);
        UpdateMenuHighlight();
    }
    public void NavigateRight()
    {
        selectedIndex = Mathf.Min(currentMenuItems.Length - 1, selectedIndex + 1);
        UpdateMenuHighlight();
    }

    public void ConfirmSelection() => HandlePageSelection();
    public void NavigateLobby()
    {
        currentPage = 0;
        SetCurrentMenu(lobbyItems, lobbyPage);
    }

    // Update the current menu and highlight the selected item
    private void SetCurrentMenu(TMP_Text[] pageItems, GameObject page)
    {
        // Disable all menu pages
        lobbyPage.SetActive(false);
        cameraMenuPage.SetActive(false);
        spawnMenuPage.SetActive(false);

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
                HandleCameraMenuSelection();
                break;
            case 2:
                HandleSpawnMenuSelection();
                break;
        }
    }

    // Logic for the lobby selection
    private void HandleLobbySelection()
    {
        switch (selectedIndex)
        {
            case 0:
                SetCurrentMenu(spawnMenuItems, spawnMenuPage); // Go to spawn menu
                break;
            case 1:
                currentPage = 1;
                //vehicle build and ride logic here
                SetCurrentMenu(cameraMenuItems, cameraMenuPage); // Go to camera menu
                break;
            case 2:
                currentPage = 2;
              //reset vehicle logic here
                break;
            case 3:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Restart game
                break;
            case 4:
                Application.Quit(); 
                break;
        }
    }

    // Logic for the camera menu selection
    private void HandleCameraMenuSelection()
    {
        if (selectedIndex == currentMenuItems.Length - 1) // Back to main menu
        {
            currentPage = 0;
            SetCurrentMenu(lobbyItems, lobbyPage);
        }
    }

    // Logic for the spawn menu selection
    private void HandleSpawnMenuSelection()
    {
        if (selectedIndex < blockPrefabs.Length) // Spawn block
        {
            SpawnBlock(selectedIndex);
        }
    }
    
    private void SpawnBlock(int blockType)
    {
        if (blockSpawnCount >= spawnLimit) return; 

        Instantiate(blockPrefabs[blockType], spawnPoint.position, Quaternion.identity);
        blockSpawnCount++;
    }
}
