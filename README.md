# kinectbingmap
voice interface on bing maps

NUI fir Bing Maps

1.	Description 
In this project we have made voice command and hand gesture interface to the Microsoft Bing maps. A person who get engage to the map can control navigation on map through his hand gesture 
•	Navigating Bing Maps using Voice and Gestures
•	It uses Kinect hardware to process voice as well as hand gesture and translates them into relevant Bing Service Calls.
•	Ideally Solution: Project Oxford for Voice Recognition. 
•	Currently using Microsoft Speech Library as Project Oxford is unavailable in our region.

2.	Resource

•	Built using visual studios 2013.
•	Microsoft speech platform
•	Microsoft Bing Maps control
•	Kinect v2 for windows.
•	Geolocation and Routing API services. 

3.	Features

•	We can navigate on the Bing maps with Voice command and Hand Gesture
•	Can go to any place just by calling the name.
•	Get image of the desired place.
•	Zoom in and out using Hand Gestures
•	Find routes between the two points.
•	Find Driving Direction between them.
Calculate distance between two points.

4.	Hand Gestures 

Following are the hand gesture that can be used.
•	Stretch your arm with open fist to all eight direction to navigate on map.
•	Close your fist with stretched arm to increase the pace of navigation.
•	If lost while in navigation, we can reset the map by bringing your right fist in front of left shoulder and left one in front of right shoulder i.e. cross being made by arms at the height of chest with open fists.
•	To zoom up close your left fist and slight push right hand with open fist toward Kinect.
•	To Zoom back close your left fist and slightly bring right hand with open fist toward your chest. 
•	To close voice intake close your right fist slightly push it perpendicular to spine base in right direction. Color of the ellipse in top left corner will change to red.
•	To resume your voice intake slightly push the left hand with open fist perpendicular to spine base in left direction. Color of the ellipse in top left corner will change to green.


5.	Voice command

•	Speak the name of the location map will navigate to the place will show its latitude and longitude.
•	Speak command “get image” to the image of the place if it is added in the code.
•	Speak command “zoom”, “back”, “up”, “down”, “right”, “left” to navigate on the map.
•	Speak a phrase “take me from new York to Chicago “to get the route map between the two places. Places can be changed. It’s been coded for New York to Chicago, Prague to Vienna, and Mumbai to goa. For other places either add the phase accordingly as a grammar in library to add some ape services which can change the voice into text. This code is using Microsoft.Speech for this purpose.
•	Speak “show me the direction” to get the driving direction between the two places.
•	Speak “find distance” to calculate distance between the two. 
•	Speak command “button mode” to make buttons visible on the map and we can use the maps though the button. Can go to any location, can find routes, find driving direction, and zoom up and down.


6.	Code design 
Requires a reference to 

•	Microsoft.Kinect
•	Microsoft.Kinect.Wpf.control
•	Microsoft.Maps.Mapscontrol.Wpf
•	Microsft.Speech 
•	Library Imtcspeechservices.
Service references
Using this we get latitude and longitude of places and route location collection between two places.
1.	http://dev.virtualearth.net/webservices/v1/geocodeservice/geocodeservice.svc/mex
2.	http://dev.virtualearth.net/webservices/v1/routeservice/routeservice.svc/mex
Functions
1.	MainWindow()
It initializes the components as well as map and set the Kinect region.
2.	Openvoiceinterface()
Calls the function add_grammer of the library where speech engine is configured, grammar is loaded on to the speech engine using file input.txt present in debug folder of the solution and speech is recognized. This function also sends the reference of the function which is to be called by the library when speech is recognized.

To handle speech events a library is created, speech library which handles recognize the speech convert it into text and call the function whose reference it has already received and sends this text as a parameter.
3.	Windowloaded()
It configure the Kinect sensor and opens it up and call “openvoiceinterface” function.
4.	Reader_MultiSourceFrameArrived()
This function is called when Kinect frame arrives. It handles the body gesture and calls the appropriate function.
5.	SpeechRecognized()
This is the function which the speech library call when speech is identified and converted into text.
6.	AddImageToMap()
it add image on the map , present in the solution when voice “ get image” is recognized then this function is called.
7.	GetMyLocation()
It will cal the getlocation function of the speech library and set the pushpin and labels on the map.
8.	GetMyRoute()
It calls the appropriate function to get the routes between the two location and the add polyline accordingly on the map and pushpin and labels as well.
9.	GetDirections()
It returns the driving direction between the two places by calling appropriate function in the library.

Speech Library 
It implements the Imtcspeechservices interface. If we want to add different handling for speech we can implements this interface and can use it instead of this library.
This library completely handles the speech handling features of the map and return the text format to the main page where appropriate action is taken.

Function
1.	Add_Grammer()
It create speech engine using Microsoft.speech and load grammar and voice command to be recognized using text file “input.txt” on to it. And call a appropriate function whenever the speech is recognized.
2.	Call_after_speech_recognition_function(mydelegatespeechrecognized_delegate)
Receives the reference of the function as delegate from the main code which is to be called when speech is recognized.
3.	Speechlibrray_stopaudio()

Function called when we want to pause the audio intake.
4.	Speechlibrray_resumeaudio()

Function called when we want to resume the audio intake.
5.	SpeechEngine_SpeechRecognitionRejected()

Called when speech is rejected.
6.	SpeechEngine_SpeechRecognized()

Called when speech is recognized.
7.	Getlocation()

It calls the SOAP services to get the location of the places and return the latitude and longitude of the place
8.	CalculateRouteLocation()

It calls the REST services the get the route path leg between the two location and return the collection of latitude and longitude.
9.	Getdirection()

Use SOAP services to get the driving direction between the places.

INTERFACE: Imtcspeechservices
This interface defines the function calls that is required to add other library which can handle speech.
1.	Add_Grammer()
This function requires the implementation of speech handling and call appropriate function when speech is recognized.
2.	Getlocation()

Call some API to get the location of a place.
3.	CalculateRouteLocation()
To get the collection of location representing routes between the two point.
4.	Speechlibrray_windowclosing()

When application closes, this function defines a way to close the speech handling events and engines related.
5.	Getdirection()

 Defines way to get the driving direction between the two places and return as a string.


To debug we can use button mode.

Regards
Gaurav Srivastava







