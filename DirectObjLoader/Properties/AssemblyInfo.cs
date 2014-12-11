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
[assembly: AssemblyCopyright( "Copyright 2014 © Eric Boehlke and Jeremy Tammik Autodesk Inc." )]
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
//
[assembly: AssemblyVersion( "2015.0.0.4" )]
[assembly: AssemblyFileVersion( "2015.0.0.4" )]
