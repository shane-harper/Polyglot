using Polyglot;
using UnityEngine;

/// <summary>
///     Closes the app
/// </summary>
public class PolyglotExample : MonoBehaviour
{
    private void Awake()
    {
        // Initialize the LocManager at the start
        StartCoroutine(LocManager.LoadLocalizationAsync());
    }
    
    public void CloseApp()
    {
        #if UNITY_EDITOR // Stop playing if in the editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else // Quit app if not
        Application.Quit();
        #endif
    }
}