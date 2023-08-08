# BG3HighlightEverything
 BG3HighlightEverything is a Baldur's Gate 3 mod that highlights all items when you left-alt, as many items are hidden/secret.

# Screenshot

 ![Imgur Image](https://i.imgur.com/WW4pGPz.jpg)
 
# Install
 The easier method to install is to use https://github.com/LaughingLeader/BG3ModManager - 
 
 1. Open BG3ModManager, (download @ https://github.com/LaughingLeader/BG3ModManager/releases/latest/download/BG3ModManager_Latest.zip)  
 2. Select 'File', 'Import Mod', then select the HighlightEverything.zip (download @ https://github.com/shalzuth/HighlightEverything/releases/latest/download/HighlightEverything.zip)
  
# How does it work?
 - Iterates over all entities/RootTemplates 
 - Checks if they are of type 'item' 
 - Sets the 'Tooltip' value to 2.
  
  