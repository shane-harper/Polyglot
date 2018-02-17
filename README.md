Polyglot - Localization Plugin for Unity3D
===============================

A simple localization plugin for Unity3D

Features
--------

- Easily expandible object replacement
    - Strings
    - Sprites
    - AudioClips
    - Fonts
- GameObject/Behaviour toggling
- CSV import/export
- Default localization selection
- Editor window with summary
    - Includes missing keys value warnings
- Preview localization in editor
- Saving last setting between sessions (PlayerPrefs)

Installation
------------

Copy the Polyglot folder to your project

Usage
------------

1. Add localizations using Localization window (Window/Localization)
2. Export CSV template and import into Excel
3. Add string localizations
4. Re-import CSV from Excel into Unity
5. Add localization components where required (Component/Localization/...)
6. Preview in editor through the Localization window or set localization in app by calling LocalizationManager.SetLocalization

Contributors
------------

I hope to keep improving this plugin. I have a few features I still wish to include. Suggestions for improvements/optimizations are very welcome!

- [Shane Harper](http://shaneharper.uk/) - Creator

License
-------

Licensed under the MIT. See [LICENSE] file for full license information.  

[LICENSE]: LICENSE