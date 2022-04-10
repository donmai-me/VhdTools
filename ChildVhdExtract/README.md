# ChildVhdExtract

Do you only have a child VHD with no parent VHD? This tool is for you.

This tool will extract the raw bytes that has changed in a child VHD and save it with its location on disk as its filename.

## Usage

Just give it the path to the child VHD.

`ChildVhdExtract PathToChild.vhd`

## TODO
* For NTFS VHDs, can we recover more information about changed bytes by looking at changed entries in the MFT?
