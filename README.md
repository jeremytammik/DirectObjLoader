DirectObjLoader
===============

Revit add-in to load a WaveFront OBJ model and generate a DirectShape element from it.

The initial implementation and development history is documented in the article 
[From Hack to App - OBJ Mesh Import to DirectShape](http://thebuildingcoder.typepad.com/blog/2015/02/from-hack-to-app-obj-mesh-import-to-directshape.html).

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/DirectObjLoader_app_2.png)

Sample fire hydrant OBJ file:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/fire_hydrant_closed_render.jpg)

Resulting DirectShape element in Revit model:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/fire_hydrant_closed_directshape_rvt.jpg)

Input scaling factor 1 versus 0.5 happily produces a gargoyle and a half:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/gargoyle2.png)

OBJ files defining groups generate a separate DirectShape element for each one:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/cart_groups_3.png)

After adding support for faces with more than four vertices, the sandal.obj test file is loaded successfully, albeit with some missing faces:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/sandal_with_gaps.png)

Switched from TessellatedShapeBuilder target Mesh to AnyGeometry generated more internal model structure from the sandal.obj test file, still with some missing faces:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/sandal_with_gaps_anygeometry.png)

Release 2015.0.0.17 improved error handling on degenerate faces:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/high_ball_glass.png)


Wish List
---------

- Progress bar
- Support for materials, minimally colour, preferably textures
- Support for the options provided by the [StlImport](https://github.com/jeremytammik/StlImport) StlImportProperties class


Author
------

Jeremy Tammik, [The Building Coder](http://thebuildingcoder.typepad.com), Autodesk Inc.


Dependencies
------------

DirectObjLoader uses the
[FileFormatWavefront](http://nugetmusthaves.com/Package/FileFormatWavefront) NuGet package based on Dave Kerr's
[file-format-wavefront](https://github.com/dwmkerr/file-format-wavefront) GitHub library.


License
-------

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.
