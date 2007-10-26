This is Geopochi v2. It acts as a proxy between Google Earth and the second version of the IGN Geoportail. Compared to the first version, it should be noticeably faster in displaying the tiles (the images of the Géoportail now are in the same type of projection as the one used by Google Earth, so there is no expensive computation to do). Also it now uses the Erlang programming language and the Yaws web server as the front end. Beacause it does not use any native code anymore, it should normally run under most ocmmon platforms (Windows, Mac OS X and Linux) unmodified, although I was unable to check this by myself.

*** Installation Instructions ***
- Download the Erlang/OTP distribution :
	- For Windows users, go to http://erlang.org/download.html and download the first link in the "Windows binary" column. 
	- For Linux usersn it is likely your distribution provides a precompiled Erlang distribution you can download with a package manager. 
	- For Mac OS X users, the Darwin Ports project has a package
	- In any case, the Erlang source is available at http://erlang.org/download.html and can be compiled for your platform.
- Install Google Earth 4 or above.
- Double click on launch.bat at the root of the archive. It should create a Dos-style window. 
- If you get a message about 'erl not found', add the directory where the erl.exe file (that comes with the Erlang/OTP distribution) is in to your PATH env var.
- Double click on geopochiv2.kml at the root of the archive. Google Earth should open. Go to one of the 2 currently supported territories. The images won't appaer until you zoom on the territory. If nothing gets displayed, zoom some more.

*** Usage Instructions ***
- The images downloaded from the Geoportail are put in the cache folder in order no to run the same query multiple times. You can empty it from time to time but be sure to keep the empty.png file there since it is needed by the application.

***Current Limitations ***
- Only Metropolitan France (Aerial and map scans) and Saint-Pierre-et-Miquelon (Aerial and map scans) are currently supported, although it is not a technical problem, more of a motivation one...
