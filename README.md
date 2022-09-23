# Unity Mesh Importer

<b>Unity Mesh Importer is a utility that applies modifiers to its meshes at the time of importing a 3d model.</b>

<img src="/../pics/pics/All.png" width="50%" height="50%">

List of modifiers:
------------------
* <b>Combine</b> - allows you to combine two arrays of UV coordinates into one array (provided that these two are two-dimensional). The X and Y fields of the second array will be written to the Z and W fields of the first array. The second array will be cleared. <br> <img src="/../pics/pics/Combine.png" width="50%" height="50%">

* <b>Manual</b> - allows you to write a specific value to any mesh array. This will allow it to be used, for example, as an origin point. <br> <img src="/../pics/pics/Manual.png" width="50%" height="50%">

* <b>Mesh</b> - allows you to transfer data from an external mesh to this mesh. For example, you can replace the Tangent array of this mesh with the Normal array of another mesh. <br> <img src="/../pics/pics/Mesh.png" width="50%" height="50%">

* <b>Collapse</b> - сoncatenates all untouched UV arrays in this mesh. This parameter depends on the [generateSecondaryUV](https://docs.unity3d.com/ScriptReference/ModelImporter-generateSecondaryUV.html) value in the model import settings. <br> <img src="/../pics/pics/Collapse.png" width="50%" height="50%">

* <b>Bounds</b> - allows you to set the position and size of the [bounds](https://docs.unity3d.com/ScriptReference/Mesh-bounds.html) of this mesh. Useful in case you are animating a mesh and it goes beyond the original boundaries, which can lead to the camera clipping the render. <br> <img src="/../pics/pics/Bounds.png" width="50%" height="50%">

How to use:
-----------
To apply import modifiers to a mesh, it is necessary to select not the model object in the project, but the mesh itself inside it. This utility overrides the default Mesh Inspector behavior.

Project Setttings:
-----------------
<img src="/../pics/pics/ProjectSettings.png" width="50%" height="50%">
To combine all untouched UV arrays in all meshes in the project, there is a corresponding option in the project settings.

Notes:
------
This utility stores import settings in the [meta file](https://docs.unity3d.com/2018.4/Documentation/Manual/BehindtheScenes.html). If there are any errors, then remove the line [userData: ...](https://docs.unity3d.com/ScriptReference/AssetImporter-userData.html) from the [meta" file](https://docs.unity3d.com/2018.4/Documentation/Manual/BehindtheScenes.html).
