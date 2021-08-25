# DirectorySync
Simple folder mirror console app

CommandLineParser nuget package used for the purpose of parsing Command Line arguments

To build the .exe application, open the DirectorySync.sln file in visual studio (tested in 2019) and build the solution

DirectorySync.exe --help - for help

Usage example: DirectorySync.exe -i "SourceFolder" -o "DestinationFolder" -l "PathToLogFile" -p "TimeInSeconds"
  
DirectorySync.exe -i  C:/source  -o  C:/dest  -l  C:/  -p  600
