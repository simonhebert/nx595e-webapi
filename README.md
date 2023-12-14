# nx595e-webapi
Interlogix NetworX NX-595E REST Web API

Compatible with Interlogix NetworX V2 security control panels (NX-4, NX-6, NX-8, NX-8E).

This is a Web interface to access, view and control NX alarm system from the network with the IP Communication Module NX-595E / UltraSync Interface.

Only one Area/Partition is currently supported, with the Zones and Outputs associated.

No installer access or programming code is required. The security panel may continue to be connected to the alarm central and all that is needed is the user credentials authorized to access and control the standard features of the system.

Created to run across multiple platforms (Windows, Linux, Mac) and built with ASP.NET Core 2.2. Tested and running on Debian Linux 64-bit (Synology NAS) within a Docker Container using the "microsoft/dotnet:2.2.1-aspnetcore-runtime" image. Currently tested and fonctionnal on a NX-595E Module with software version v_CN_0.108-O (North America).