DirectObjLoader
===============

Revit add-in to load a WaveFront OBJ model and generate a DirectShape element from it.

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/DirectObjLoader_app.png)

Sample fire hydrant OBJ file:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/fire_hydrant_closed_render.jpg)

Resulting DirectShape element in Revit model:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/fire_hydrant_closed_directshape_rvt.jpg)

Input scaling factor 1 versus 0.5 happily produces a gargoyle and a half:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/gargoyle2.png)

OBJ files defining groups generate a separate DirectShape element for each one:

![Image](https://github.com/jeremytammik/DirectObjLoader/blob/master/img/cart_groups_3.png)


Wish List
---------

- Progress bar
- Support for materials, minimally colour, preferably textures
- Support for the options provided by [StlImport](https://github.com/jeremytammik/StlImport)


Author
------

Jeremy Tammik, [The Building Coder](http://thebuildingcoder.typepad.com), Autodesk Inc.


License
-------

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.
