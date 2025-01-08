# LiveSplit.HollowKnight
Hollow Knight Autosplitter / Load Remover

## Setting up the autosplitter in LiveSplit
- Open LiveSplit
- Edit your splits
- Click the 'Activate' autosplitter button
- Add any splits to your main splits file
- Hit the 'Settings' button to then set up the same autosplitter splits

## Building the autosplitter from source
If you would like to modify and build the autosplitter yourself, download the source and change the file path to your livesplit folder in `LiveSplit.HollowKnight.csproj`.
```xml
<!-- This is what it is by default, change the path here, please do not commit this change in any pull requests-->
<PropertyGroup>
  <LiveSplitRefs>..\..\LiveSplit</LiveSplitRefs>
</PropertyGroup>
```
For example, I set it to `<LiveSplitRefs>D:\Documents\LiveSplit</LiveSplitRefs>` as that is where my `LiveSplit.exe` is. With this you should be set all to build it.

## Make your own autosplit
If you would like to add or modify an autosplit, there are two things you must do:
Each split is a value in an enum called `SplitName`, which is declared in `HollowKnightSplitSettings.cs`. You must add a value for your splits, and give it the proper attributes that will be the split's name and mouse-over description inside LiveSplit. To add logic for your split, you need to add a case for it in the giant switch in `HollowKnightComponent`, and have it appropriately set the value of `shouldSplit`, `shouldSkip` and `shouldReset` to true for the conditions under which you wish to split, skip or reset respectively.

**Important note** : Modifications to the autosplitter are allowed but may be subject to retiming at moderators' discretion. Modifications to the load removal or the start/end conditions may result in your run time being adjusted or retimed RTA.


## Contributing to the official autosplitter
Autosplitter maintenance is currently led by slaurent22. If you would like to contribute, send pull requests to [his branch](https://github.com/slaurent22/LiveSplit.HollowKnight) and he will manage including it in the next official update.

***
If you have any specific questions about the autosplitter's code, you can message @slaurent, @mayonnaisical or @cerpintext on discord
