# VhdChangedFiles

If you have a child VHD and all successive parent VHDs upto the base VHD and want to only extract the modified files in the child VHD, then this tool is for you.

## Usage

Give the directory of VHDs and the program will ask which VHDs you want to use.

`VhdChangedFiles PathToVhdDirectory`

## TODO
* Give a list of added, modified, and deleted files in a text format.
* Accept a series of VHD files instead of a directory.
* Move current behaviour behind a `-i` or `--interactive` flag.
