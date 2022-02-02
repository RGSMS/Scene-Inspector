# Scene Inspector 🎮

![Unity](https://img.shields.io/badge/Unity-100000?style=flat-square&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)
[![Unity 2019.4+](https://img.shields.io/badge/unity-2019.4%2B-blue.svg)](https://unity3d.com/get-unity/download)

![GitHub last commit](https://img.shields.io/github/last-commit/RGSMS/Scene-Inspector)
![GitHub all releases](https://img.shields.io/github/downloads/RGSMS/Scene-Inspector/total)
![GitHub](https://img.shields.io/github/license/RGSMS/Scene-Inspector)

[![Twitter Follow](https://img.shields.io/twitter/follow/BillyCoenWU?style=social)](https://twitter.com/BillyCoenWU)

## A custom serialization of a scene's information in the unity inspector.

You will no longer insert the wrong scene name when typing it in the string field. In addition, you can decide to load a scene using the _Path_, _Scene Name_ or _Build Index_, without worrying about changing scene information in case something changes in the project.

_What this allows you to do:_
1. `Select scenes from any project folder and serialize the information in the Unity inspector`
2. `Stores the _Path_, _Scene Name_ and _Build Index_ informations of the selected scene`
3. `Keeps your selected scene's Build Index updated if you change the order of scenes in the build settings`

## How to use:

```c#
[SerializeField]
private SceneInspector _scene = null;
```
![ ](https://github.com/RGSMS/prints/blob/main/printjpg.jpg)
```c#
SceneManager.LoadSceneAsync(_scene.BuildIndex);
//or
SceneManager.LoadSceneAsync(_scene.Name);
//or
SceneManager.LoadSceneAsync(_scene.Path);
```

## Future Updates:
1. More Examples
2. Inspector Improvements
