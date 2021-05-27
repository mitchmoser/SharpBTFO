# SharpBTFO
POC for deleting file artifacts from disk

- Clears all Windows Event Logs
- Deletes PowerShell History files for all users
- Deletes .log files from all disks

## Build Notes
This utility does not take any destructive actions in its current state.

Once the utility is configured to take the desired actions, _**build without running**_ unless you want the actions performed on your host upon compilation.
### Visual Studio
Build > Build Solution

`Ctrl+Shift+B`
