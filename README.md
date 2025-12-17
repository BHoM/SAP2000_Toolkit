[![License: LGPL v3](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](https://www.gnu.org/licenses/lgpl-3.0) [![Build status](https://ci.appveyor.com/api/projects/status/plmqiho414qw9oko/branch/master?svg=true)](https://ci.appveyor.com/api/projects/status/sap2000_toolkit/branch/master) [![Build Status](https://dev.azure.com/BHoMBot/BHoM/_apis/build/status/SAP2000_Toolkit/SAP2000_Toolkit.CheckCore?branchName=master)](https://dev.azure.com/BHoMBot/BHoM/_build/latest?definitionId=202&branchName=master)

# SAP2000_Toolkit

This toolkit allows interoperability between the BHoM and SAP2000. Enables creation and reading of structural finite element analysis models and analysis results. 

[SAP2000 Product Website](https://www.csiamerica.com/products/sap2000)

### Known Versions of Software Supported
The adapter is intended to support SAP2000 version 21 and all future versions. It is currently tested on version 23 exclusively.
SAP2000 v26 is not currently supported.

### Net runtime issues

There are currently some internal failures in the SAP2000 API when called in a NET Core environment. For this reason, running the ETABSAdapter in runtimes above NET4 is disabled.

If you are using the SAP2000 Adapter with Grasshopper in Rhino 8 you can change the runtime used by Rhino to framework. To do this, please see this link: https://www.rhino3d.com/en/docs/guides/netcore/#to-change-rhino-to-always-use-net-framework

A fix to allow for higher net runtimes is being worked on.

### Documentation
For more information about functionality see [Object Relation Table](https://github.com/BHoM/SAP2000_Toolkit/wiki/BHoM-SAP2000-Object-Relations)

---
This toolkit is part of the Buildings and Habitats object Model. Find out more on our [wiki](https://github.com/BHoM/documentation/wiki) or at [https://bhom.xyz](https://bhom.xyz/)

## Quick Start 🚀 

Grab the [latest installer](https://bhom.xyz/) and a selection of [sample scripts](https://github.com/BHoM/samples).


## Getting Started for Developers 🤖 

If you want to build the BHoM and the Toolkits from source, it's hopefully easy! 😄 
Do take a look at our specific wiki pages here: [Getting Started for Developers](https://bhom.xyz/documentation/Guides-and-Tutorials/Coding-with-BHoM/)


## Want to Contribute? ##

BHoM is an open-source project and would be nothing without its community. Take a look at our contributing guidelines and tips [here](https://github.com/BHoM/BHoM/blob/main/CONTRIBUTING.md).


## License ##

BHoM is free software licensed under GNU Lesser General Public License - [https://www.gnu.org/licenses/lgpl-3.0.html](https://www.gnu.org/licenses/lgpl-3.0.html)  
Each contributor holds copyright over their respective contributions.
The project versioning (Git) records all such contribution source information.
See [LICENSE](https://github.com/BHoM/BHoM/blob/main/LICENSE) and [COPYRIGHT_HEADER](https://github.com/BHoM/BHoM/blob/main/COPYRIGHT_HEADER.txt).
