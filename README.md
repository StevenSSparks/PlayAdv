# Play Adventure Server Client 
This is a .EXE client for the Adcenture Server Project. You can build this and run from a command prompt to play the Adventure House game. Future releases will include game selection. 

# Releases
See the latest releases for more deails - https://github.com/StevenSSparks/PlayAdv/releases

#Goals
* Learn to create a stand alone .EXE file using .Net 5
* Have fun with lots of colors and make the game text visually appealing 

# Build Details 
* Visual Studio 2022 latest release 
* .NET 5.6 current release 
* The creation of the EXE requires a command line publish
   *  dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true

