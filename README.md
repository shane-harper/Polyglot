Polyglot2 - Localization Plugin for Unity3D
===============================

A simple localization plugin for Unity3D. 

This is a rewrite based on my findings from using the previous version on 2 projects. Some features have been removed, such as sprite localization and font switching. These features may be re-added in the future.

Features
--------

- Easily expandible string replacement
- GameObject toggling
- CSV import (tested with Google Sheets)
- Default localization selection
- Preview localization in editor
- Saving of last setting between sessions (PlayerPrefs)
- Basic Addressables Support

Known Issues
--------

- CSV import/export needs re-writing
    - Does not support double quotes
    - Does not support new line characters (use '\n')

Installation
------------

Copy the Polyglot folder to your project

Usage
------------

1. Create spreadsheet with localizations [Example](https://docs.google.com/spreadsheets/d/11xmSz3hNe-OQJxhZhs9ZoPOMK4_uK8ZMT6geqyczJtA/edit?usp=sharing)
2. Export localization CSV
3. Import CSV to project in Unity (Edit > Polyglot > Import Data...)
4. Set default localizations in Polyglot settings (Edit > Polyglot > Show Settings)

For Asset Bundles:
5. Rebuild streaming assets (Edit > Polyglot > Build Streaming Assets)

For Addressables:
5. Add 'POLYGLOT_ADDRESSABLES' to Scripting Define Symbols in Player Settings
6. Convert legacy asset bundles by accepting popup when opening Addressables window
7. Simplify entry names for language assets

Finally. Initialize Polyglot on app start. `StartCoroutine(Polyglot.LocManager.Initialize());`

Contributors
------------

I hope to keep improving this plugin. I have a few features I still wish to include. Suggestions for improvements/optimizations are very welcome!

- [Shane Harper](http://shaneharper.uk/) - Creator

License
-------

Licensed under the MIT. See [LICENSE] file for full license information.  

[LICENSE]: LICENSE