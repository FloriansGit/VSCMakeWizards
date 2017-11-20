<p align="center">
<img src="Resources/VSCMakeWizards.png" height="20%" width="20%"/>
<br>
<b>VSCMakeWizards</b>
</p>

This package adds New Project/Item Wizards for a project with an initial CMake setup to start from.

This open source project was inspiered by a question on StackOverflow: 

### [Creating a cmake project with visual studio](https://stackoverflow.com/questions/46741850/creating-a-cmake-project-with-visual-studio) ###

After building and installing this extension you will currently have two new C++ project wizards:

- CMake Executable Template
- CMake Library Template

![New Project Wizards](https://i.stack.imgur.com/lGRfE.png)

They will generate some baisc C++ code, a root `CMakeLists.txt` and a `CMakeSettings.json` file in the given folders and then will use "Open Folder" on it.

The CMake settings use Ninja as a build system and it's recommended to have at least Visual Studio 2017 15.4 to get the expected results.

