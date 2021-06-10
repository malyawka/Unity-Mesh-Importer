# Unity Mesh Importer

Unity Mesh Importer is a utility that applies modifiers to its meshes at the time of importing a 3d model.
![Unity Mesh Importer](/../pics/pics/All.png?raw=true "Mesh Importer")

List of modifiers:
------------------
* Combine - allows you to combine two arrays of UV coordinates into one array (provided that these two are two-dimensional). The X and Y fields of the second array will be written to the Z and W fields of the first array. The second array will be cleared.

* Manual - allows you to write a specific value to any mesh array. This will allow it to be used, for example, as an origin point.

* Mesh - allows you to transfer data from an external mesh to this mesh. For example, you can replace the Tangent array of this mesh with the Normal array of another mesh.

* Bounds - allows you to set the position and size of the border of this mesh. Useful in case you are animating a mesh and it goes beyond the original boundaries, which can lead to the camera clipping the render.

How to use:
-----------
To apply import modifiers to a mesh, it is necessary to select not the model object in the project, but the mesh itself inside it. This utility overrides the default Mesh Inspector behavior.

Notes:
------
This utility stores import settings in the "meta" file. If there are any errors, then remove the line "userData: ..." from the "meta" file.
