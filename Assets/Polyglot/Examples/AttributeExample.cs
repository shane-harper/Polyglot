using Polyglot;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Class to demonstrate the usage of the LocalizationKey attribute
/// </summary>
/// <remarks>
///     Note that custom scripts using these attributes will not update when previewing in the editor outside of play
///     mode
/// </remarks>
[RequireComponent(typeof(Text))]
public class AttributeExample : MonoBehaviour
{
    #region Inspector

    /// <summary>
    ///     LocalizationKey attribute will replace the text field with a dropdown box
    /// </summary>
    [LocalizationKey(LocalizationData.Type.String)] 
    [SerializeField] private string _key;
    [SerializeField] private string _name = "Shane";

    [Header("UI Elements")] 
    [SerializeField] private Text _display;

    #endregion
    
    private void Start()
    {
        string format;

        // Try to get the string from the localization manager
        if (LocalizationManager.TryGetString(_key, out format))
            _display.text = string.Format(format, _name);
        else
            _display.text = string.Format("Could not find key {0}", _key);
    }

    private void OnValidate()
    {
        // Make sure _display Text component is linked
        _display = GetComponent<Text>();
    }
}