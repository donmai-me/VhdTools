# FullVhdExtract

If you have a child VHD and all successive parent VHDs upto the very first one, you can use this tool to extract all files stored in the VHD. Including all the changes upto the child VHD.

## Usage

Give a path to the directory of VHDs and the program will ask for which VHDs you want to use.

`FullVhdExtract PathToVhdDirectory`

## TODO
* Accept a series of VHD files instead of a single directory.
* Move current behaviour behind a `-i` or `--interactive` flag.
