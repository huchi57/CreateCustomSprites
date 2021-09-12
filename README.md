# Create Custom Sprites Window
A simple custom sprite creator, currently able to create these sprites:
1. An outlined circle.
2. A rounded solid rectangle, with sprite border being set automatically based on the corner radius.
## How To Use
1. Simply download the script and put it under an `Editor` folder.
2. `Custom` > `Create Custom Sprites`.
3. Modify the parameters.
![The window in use in Unity](/CreateCustomSpritesWindow_Readme.png)
4. Click `Generate` to create a sprite. It will be created under `Assets` root folder or currently selected folder.
## TODO (Not Implemented Yet)
1. Create more shapes, e.g. hollow/solid/other polygons.
2. Generate sprite asset appropriately, i.e. generate it in the currently selected folder in the Editor window. Sometimes this may not work, you might have to find in other asset folders to find the created sprite. An Editor log will show where the file is.
3. Enhance Editor UI.
