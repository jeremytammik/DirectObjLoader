using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "DirectObjLoader" )]
[assembly: AssemblyDescription( "Revit add-in to load a WaveFront OBJ file into a DirectShape element" )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "Autodesk Inc." )]
[assembly: AssemblyProduct( "DirectObjLoader" )]
[assembly: AssemblyCopyright( "Copyright 2014-2015 © Eric Boehlke and Jeremy Tammik Autodesk Inc." )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "321044f7-b0b2-4b1c-af18-e71a19252be0" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
//
// Release history:
//
// 2014-12-02 2015.0.0.0 initial version with Eric Boehlke loaded fire hydrant
// 2014-12-03 2015.0.0.1 added config class to store last folder between sessions
// 2014-12-05 2015.0.0.2 use mesh + salvage instead of solid + abort when building the DirectShape per suggestion from angel velez via eric boehlke and gargoyle is now successfully loaded
// 2014-12-10 2015.0.0.3 implemented input scaling factor stored in config file, cf. gargoyle2.png
// 2014-12-11 2015.0.0.4 added face count and error message on zero faces
// 2014-12-11 2015.0.0.5 added initial primitive test support for groups as well as ungrouped faces, result directobjloader_shopping_cart_groups.png
// 2014-12-11 2015.0.0.6 call OpenConnectedFaceSet for each OBJ group, result directobjloader_shopping_cart_groups_2.png
// 2014-12-11 2015.0.0.6 name DirectShape element same as input file
// 2014-12-12 2015.0.0.7 create separate DirectShape element for each OBJ group, add appGuid and name shapes better, result directobjloader_shopping_cart_groups_3.png
// 2014-12-12 2015.0.0.8 capitalise and replace underscore by space in DirectShape element name
// 2015-01-02 2015.0.0.9 added support for faces with more than four vertices, enabling succesful load of sandal.obj
// 2015-01-02 2015.0.0.10 switched TessellatedShapeBuilder target from Mesh to AnyGeometry
// 2015-01-02 2015.0.0.11 set TessellatedShapeBuilder LogString and LogInteger properties
// 2015-01-02 2015.0.0.12 abort and display error message on invalid OBJ file due to face vertex index exceeding total vertex count
// 2015-01-05 2015.0.0.13 added two exception handlers for loading OBJ file and generating DirectShape
// 2015-01-12 2015.0.0.14 removed external command from add-in manifest, leaving only the external application
// 2015-01-12 2015.0.0.15 fixed a logical error handling nFaces and nFacesTotal count
// 2015-01-15 2015.0.0.16 display command button icon stored in embedded resources
// 2015-01-23 2015.0.0.17 wrapped call to AddFace in an own exception handler and added a debug log reporting count of faces added and failed
// 2015-02-15 2015.0.0.18 implemented Config.MaxNumberOfVertices and graceful exit on too many mesh vertices
//
[assembly: AssemblyVersion( "2015.0.0.18" )]
[assembly: AssemblyFileVersion( "2015.0.0.18" )]
