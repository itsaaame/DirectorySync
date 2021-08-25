# DirectorySync
Simple folder mirror console app

CommandLineParser nuget package used for the purpose of parsing Command Line arguments

To build the .exe application, open the DirectorySync.sln file in visual studio (tested in 2019) and build the solution

DirectorySync.exe --help - for help

Usage example: DirectorySync.exe -i "SourceFolder" -o "DestinationFolder" -l "PathToLogFile" -p "TimeInSeconds"
  
DirectorySync.exe -i  C:/source  -o  C:/dest  -l  C:/  -p  600

There is also another version of the app created with help of event-driven watchers (EventFileWatcher), feel free to play around with it, though the downside of it is a small bounded buffer which may be overflown when there are too many events (many files are changed in the short amount of time)
