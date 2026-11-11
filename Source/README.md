# Canonical CosmoTweest source

This is the only source folder for future changes. The parent **CosmoTweest FINAL** folder is the only player delivery.

Run `./build.ps1` from this Source directory to update the two parent EXEs in place. Close the final game and setup first. Builds never overwrite Data/settings.txt. The original project and old release folders are archival only.

For QA, use `./tests/Verify.ps1`; it builds into Source/tests/build and stores reports there, leaving the player EXEs and Data untouched. Explicit `-OutputDirectory` is reserved for test builds inside this Source tree.

The portable profile is selected by Data/portable.mode beside the game. Its path is resolved relative to the executable on every machine. Installed copies without that marker use the existing per-user local profile. Source/src/PortableProfile.cs contains this selection and the relocation verification entry point.

See FLEET-COMMAND.md for game systems. Historical reports are in tests/Verification; the final-folder/relocation report is added there after verification. No development tools are required to play the parent game.
