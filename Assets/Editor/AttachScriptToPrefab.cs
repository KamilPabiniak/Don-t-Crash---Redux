using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class AttachScriptToPrefab : EditorWindow
{
    public GameObject prefabToSearch;
    public MonoScript scriptToAttachFromAssets;
    public SearchScope searchScope;

    public enum SearchScope
    {
        ActiveScene,
        AllScenesInBuild
    }

    [MenuItem("Czarny's_Tools/Attach Script to Prefab")]
    static void Init()
    {
        AttachScriptToPrefab window = (AttachScriptToPrefab)EditorWindow.GetWindow(typeof(AttachScriptToPrefab));
        window.Show();
    }

    void OnEnable()
    {
        string prefabToSearchPath = EditorPrefs.GetString("AttachScriptToPrefab_PrefabToSearch", string.Empty);
        if (!string.IsNullOrEmpty(prefabToSearchPath))
        {
            prefabToSearch = AssetDatabase.LoadAssetAtPath<GameObject>(prefabToSearchPath);
        }

        string scriptToAttachFromAssetsPath = EditorPrefs.GetString("AttachScriptToPrefab_ScriptToAttachFromAssets", string.Empty);
        if (!string.IsNullOrEmpty(scriptToAttachFromAssetsPath))
        {
            scriptToAttachFromAssets = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptToAttachFromAssetsPath);
        }

        searchScope = (SearchScope)EditorPrefs.GetInt("AttachScriptToPrefab_SearchScope", 0);
    }

    void OnGUI()
    {
        GUILayout.Label("Attach Script to Prefab", EditorStyles.boldLabel);
        prefabToSearch = EditorGUILayout.ObjectField("Prefab to Search", prefabToSearch, typeof(GameObject), true) as GameObject;

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Script to Attach from Assets", GUILayout.Width(180));
        scriptToAttachFromAssets = EditorGUILayout.ObjectField(scriptToAttachFromAssets, typeof(MonoScript), true) as MonoScript;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Search Scope", GUILayout.Width(150));
        searchScope = (SearchScope)EditorGUILayout.EnumPopup(searchScope);

        EditorGUILayout.Space();

        if (GUILayout.Button("Attach Script"))
        {
            SavePreferences();
            AttachScriptToPrefabsInScenes();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Instrukcja Obsługi", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("**Przed użyciem narzędzia zaleca się zapisanie wszystkich zmian w projekcie.**", MessageType.Warning);
        EditorGUILayout.HelpBox(
            "1. Prefab do przeszukania:\n" +
            "   - Wybierz prefab, który chcesz przeszukać.\n\n" +
            "2. Skrypt do przypisania z zasobów:\n" +
            "   - Wybierz skrypt (MonoBehavior) z zasobów projektu, który chcesz przypisać do prefabu.\n\n" +
            "3. Zakres przeszukiwania:\n" +
            "   - Aktywna scena: Przeszukaj tylko aktualnie aktywną scenę.\n" +
            "   - Wszystkie sceny w budowie: Przeszukaj wszystkie sceny dodane do budowy projektu.\n\n" +
            "4. Przycisk 'Attach Script':\n" +
            "   - Po wybraniu prefabu, skryptu oraz zakresu przeszukiwania, kliknij przycisk 'Attach Script' aby rozpocząć proces przypisania skryptu.\n\n" +
            "- Przypisywanie skryptu do prefabu w scenach:\n" +
            "   - Na podstawie wybranego zakresu przeszukiwania, narzędzie przeszukuje odpowiednie sceny i przypisuje wybrany skrypt do prefabu.\n" +
            "   - Skrypt zostanie przypisany do prefabu tylko jeśli prefab istnieje w scenie jako instancja.\n\n" +
            "Przykład:\n" +
            "Załóżmy, że chcesz przypisać skrypt ExampleScript do wszystkich instancji prefabu ExamplePrefab w aktywnej scenie:\n" +
            "1. Wybierz ExamplePrefab w polu 'Prefab to Search'.\n" +
            "2. Wybierz ExampleScript w polu 'Script to Attach from Assets'.\n" +
            "3. Wybierz 'Active Scene' w sekcji 'Search Scope'.\n" +
            "4. Kliknij przycisk 'Attach Script'.\n\n" +
            "Uwagi:\n" +
            "- Upewnij się, że skrypt, który chcesz przypisać, jest odpowiednio skonfigurowany i kompatybilny z prefabem.\n" +
            "**Przed użyciem narzędzia zaleca się zapisanie wszystkich zmian w projekcie.**\n\n" +
            "Autorski skrypt i narzędzie zostały opracowane dla wygody pracy z prefabrykatami w projektach Unity. Jeśli masz pytania lub problemy, skontaktuj się z działem wsparcia technicznego Kamil.\n\n" +
            "Oby się wam przydało",
            MessageType.None
        );
    }

    void SavePreferences()
    {
        if (prefabToSearch != null)
        {
            string prefabToSearchPath = AssetDatabase.GetAssetPath(prefabToSearch);
            EditorPrefs.SetString("AttachScriptToPrefab_PrefabToSearch", prefabToSearchPath);
        }

        if (scriptToAttachFromAssets != null)
        {
            string scriptToAttachFromAssetsPath = AssetDatabase.GetAssetPath(scriptToAttachFromAssets);
            EditorPrefs.SetString("AttachScriptToPrefab_ScriptToAttachFromAssets", scriptToAttachFromAssetsPath);
        }

        EditorPrefs.SetInt("AttachScriptToPrefab_SearchScope", (int)searchScope);
    }

    void AttachScriptToPrefabsInScenes()
    {
        var scenesToProcess = new System.Collections.Generic.List<Scene>();

        if (searchScope == SearchScope.ActiveScene)
        {
            scenesToProcess.Add(SceneManager.GetActiveScene());
        }
        else if (searchScope == SearchScope.AllScenesInBuild)
        {
            var scenesInBuild = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToList();
            foreach (var scenePath in scenesInBuild)
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                scenesToProcess.Add(scene);
            }
        }

        foreach (var scene in scenesToProcess)
        {
            if (scene.isLoaded)
            {
                AttachScriptToPrefabInScene(scene);
                if (searchScope == SearchScope.AllScenesInBuild)
                {
                    EditorSceneManager.SaveScene(scene);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }
    }

    void AttachScriptToPrefabInScene(Scene scene)
    {
        var rootObjects = scene.GetRootGameObjects();
        var scriptTypeToAttach = scriptToAttachFromAssets.GetClass();
        var scriptToAttachFromScene = Object.FindObjectOfType(scriptTypeToAttach) as MonoBehaviour;

        foreach (var rootObject in rootObjects)
        {
            var prefabInstances = rootObject.GetComponentsInChildren<Transform>(true);
            foreach (var instance in prefabInstances)
            {
                if (PrefabUtility.GetPrefabInstanceStatus(instance.gameObject) == PrefabInstanceStatus.Connected)
                {
                    var prefabAsset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(instance.gameObject);
                    if (prefabAsset == prefabToSearch)
                    {
                        var scriptInstances = instance.GetComponentsInChildren<MonoBehaviour>(true);
                        foreach (var scriptInstance in scriptInstances)
                        {
                            var fields = scriptInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            foreach (var field in fields)
                            {
                                if (field.FieldType.IsAssignableFrom(scriptTypeToAttach))
                                {
                                    if (scriptToAttachFromScene != null)
                                    {
                                        field.SetValue(scriptInstance, scriptToAttachFromScene);
                                        EditorUtility.SetDirty(instance.gameObject);
                                        PrefabUtility.RecordPrefabInstancePropertyModifications(scriptInstance);
                                        Debug.Log("Attached script from scene to field " + field.Name + " in script " + scriptInstance.GetType().Name + " on prefab instance: " + instance.name);
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Script to attach not found in the scene: " + scriptTypeToAttach.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
