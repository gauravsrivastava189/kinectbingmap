using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kinectaudiostream;
using System.Windows;
using System.ComponentModel;
using Microsoft.Kinect;
using System.Runtime.InteropServices;
using speechlibrary.GeocodeService;
using speechlibrary.RouteService;
using Microsoft.Maps.MapControl.WPF;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Imtcspeechservice;
using BingMapsRESTService;
using BingMapsRESTService.Common.JSON;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;

namespace speechlibrary
{
      
   using System.Runtime.InteropServices;
   using Microsoft.Speech.AudioFormat;
   using Microsoft.Speech.Recognition;
  
   
    
    public class kinectandspeech : speechinterface
    {
        public KinectAudioStream convertStream = null;
        public SpeechRecognitionEngine speechEngine = null;
        public KinectSensor kinectSensor = null;
        public delegate void mydelegate(ref string[] sentence , ref string command);
        public LocationCollection locs = new LocationCollection();
        mydelegate local_speechrecognized_delegate;

        string key = "Alb8_m-LNHfEuGq-hXrdCNVYiqLKvzIZd3ZImsYlF1zHl1J1lCNEr_vtjPehn6t3";

      public static RecognizerInfo TryGetKinectRecognizer()
        {
            
            IEnumerable<RecognizerInfo> recognizers;
           

            // This is required to catch the case when an expected recognizer is not installed.
            // By default - the x86 Speech Runtime is always expected. 
            try
            {
              
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
                
               
                
               
                
            }
            catch (COMException)
            {
                
                return null;
            }
         

            foreach (RecognizerInfo recognizer in recognizers)
            {
              // MessageBox.Show(" recognizer ");

                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) &&
                    "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                   // MessageBox.Show(" working ");

                    return recognizer;
                }
                
            }

            return null;
        }

      public void Add_Grammer(ref int result)
      {
          kinectSensor = KinectSensor.GetDefault();
          if (kinectSensor != null)
          {
             
              // open the sensor
              kinectSensor.Open();

              // grab the audio stream
              IReadOnlyList<AudioBeam> audioBeamList = kinectSensor.AudioSource.AudioBeams;
              System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

              // create the convert stream
              this.convertStream = new KinectAudioStream(audioStream);
          }
          else
          {
              result = 2;
              return ;
          }
          
              RecognizerInfo ri = kinectandspeech.TryGetKinectRecognizer();

              speechEngine = new SpeechRecognitionEngine(ri.Id);

               if(ri == null)
               {
                   result = 0;
                   return;
               }
               
              var directions = new Choices();


              StreamReader reader = new StreamReader("input.txt");
              string line;

              while ((line = reader.ReadLine()) != null)
              {
                  // Do something with the line.
                  string[] parts = line.Split('\n');
                  if (parts.Length == 1)
                  {
                      // MessageBox.Show(parts.Length.ToString());
                      //gb.Append(parts[0].ToString());
                      directions.Add(parts[0].ToString());
                      //  MessageBox.Show(parts[0].ToString());

                  }
              }

              var gb = new GrammarBuilder { Culture = ri.Culture };
              gb.Append(directions);


              var g = new Grammar(gb);
              speechEngine.LoadGrammar(g);

              var gb1 = new GrammarBuilder { Culture = ri.Culture };
              gb1.Append("take");
              gb1.Append("me");
              gb1.Append("from");
              gb1.Append("mumbai");
              gb1.Append("to");
              gb1.Append("goa");
              var g1 = new Grammar(gb1);
              speechEngine.LoadGrammar(g1);

              var gb2 = new GrammarBuilder { Culture = ri.Culture };
              gb2.Append("show");
              gb2.Append("me");
              gb2.Append("the");
              gb2.Append("direction");
              var g2 = new Grammar(gb2);
              speechEngine.LoadGrammar(g2);

              var gb3 = new GrammarBuilder { Culture = ri.Culture };
              gb3.Append("take");
              gb3.Append("me");
              gb3.Append("from");
              gb3.Append("delhi");
              gb3.Append("to");
              gb3.Append("agra");
              var g3 = new Grammar(gb3);
              speechEngine.LoadGrammar(g3);

              var gb4 = new GrammarBuilder { Culture = ri.Culture };
              gb4.Append("take");
              gb4.Append("me");
              gb4.Append("from");
              gb4.Append("newyork");
              gb4.Append("to");
              gb4.Append("chicago");
              var g4 = new Grammar(gb4);
             speechEngine.LoadGrammar(g4);

             var gb5 = new GrammarBuilder { Culture = ri.Culture };
             gb5.Append("take");
             gb5.Append("me");
             gb5.Append("from");
             gb5.Append("prague");
             gb5.Append("to");
             gb5.Append("vienna");
             var g5 = new Grammar(gb5);
             speechEngine.LoadGrammar(g5);

             
             speechEngine.SpeechRecognized += this.speechEngine_SpeechRecognized;
             speechEngine.SpeechRecognitionRejected += this.speechEngine_SpeechRecognitionRejected;

             speechEngine.SetInputToDefaultAudioDevice();
             // let the convertStream know speech is going active
             this.convertStream.SpeechActive = true;

             // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
             // This will prevent recognition accuracy from degrading over time.
        //    this.speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

             speechEngine.SetInputToAudioStream(
                 this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
             speechEngine.RecognizeAsync(RecognizeMode.Multiple);  
      
          
      }
      public void call_after_speech_recognition_function(mydelegate speechrecognized_delegate)
      {
          local_speechrecognized_delegate = speechrecognized_delegate;
      }

      public  void speechlibrray_windowclosing()
        {
           
               if (null != this.convertStream)
              {                 
                  this.convertStream.SpeechActive = false;
              }

               if (null != this.speechEngine)
              {
                  this.speechEngine.SpeechRecognized -= this.speechEngine_SpeechRecognized;
                  this.speechEngine.SpeechRecognitionRejected -= this.speechEngine_SpeechRecognitionRejected;
                  this.speechEngine.RecognizeAsyncStop();
              }
          
        }

      void speechEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
      {
          Console.WriteLine("REJECTED");
      }

      void speechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
      {

         string[] sentence = new string[6] ;
         string command ="";
          const double ConfidenceThreshold = 0.5;
          

          if (e.Result.Confidence >= ConfidenceThreshold)
          {
              if (e.Result.Words[0].Text != "take" && e.Result.Words[0].Text != "show")
              {
                  command = e.Result.Text;
              }
              
              else if ("takemefrom" == e.Result.Words[0].Text + e.Result.Words[1].Text + e.Result.Words[2].Text)
              {
                  
                  sentence[0] = e.Result.Words[0].Text.ToString();
                  sentence[1] = e.Result.Words[1].Text.ToString();
                  sentence[2] = e.Result.Words[2].Text.ToString();
                  sentence[3] = e.Result.Words[3].Text.ToString();
                  sentence[4] = e.Result.Words[4].Text.ToString();
                  sentence[5] = e.Result.Words[5].Text.ToString();
                  command = "";
              }
              else if ("showmethedirection" == e.Result.Words[0].Text + e.Result.Words[1].Text + e.Result.Words[2].Text + e.Result.Words[3].Text)
              {
                  sentence[0] = e.Result.Words[0].Text.ToString();
                  sentence[1] = e.Result.Words[1].Text.ToString();
                  sentence[2] = e.Result.Words[2].Text.ToString();
                  sentence[3] = e.Result.Words[3].Text.ToString();
                  sentence[4] = "";
                  sentence[5] = "";
              }
              else
              command = "";



          }

         


          local_speechrecognized_delegate(ref sentence , ref command);
      }

      public Microsoft.Maps.MapControl.WPF.Location getlocation(string input , ref string results)
      {
         
          GeocodeService.Location point = new GeocodeService.Location();
          GeocodeResponse geocodeResponse = GeocodeAddressGeocodeResponse(input);

          if (geocodeResponse.Results.Length > 0)
              results = String.Format("Latitude: {0}\nLongitude: {1}",
                geocodeResponse.Results[0].Locations[0].Latitude,
                geocodeResponse.Results[0].Locations[0].Longitude);
          else
              results = "No Results Found";
        
          speechlibrary.GeocodeService.Location myLoc = new speechlibrary.GeocodeService.Location();

          try
          {
              myLoc.Latitude = geocodeResponse.Results[0].Locations[0].Latitude;
              myLoc.Longitude = geocodeResponse.Results[0].Locations[0].Longitude;
          }
          catch( System.IndexOutOfRangeException e )
          {
              results = "indexoutofrange";
              return null;
          }
          Microsoft.Maps.MapControl.WPF.Location loc = new Microsoft.Maps.MapControl.WPF.Location();
          loc.Latitude = geocodeResponse.Results[0].Locations[0].Latitude;
          loc.Longitude = geocodeResponse.Results[0].Locations[0].Longitude;

          return loc;
      }

      public GeocodeResponse GeocodeAddressGeocodeResponse(string address)
      {
          GeocodeRequest geocodeRequest = new GeocodeRequest();


          // Set the credentials using a valid Bing Maps key
          geocodeRequest.Credentials = new speechlibrary.GeocodeService.Credentials();
          geocodeRequest.Credentials.ApplicationId = key;

          // Set the full address query
          geocodeRequest.Query = address;




          // Set the options to only return high confidence results 
          ConfidenceFilter[] filters = new ConfidenceFilter[1];
          filters[0] = new ConfidenceFilter();
          filters[0].MinimumConfidence = GeocodeService.Confidence.High;

          // Add the filters to the options
          GeocodeOptions geocodeOptions = new GeocodeOptions();
          geocodeOptions.Filters = filters;
          geocodeRequest.Options = geocodeOptions;

          // Make the geocode request
          GeocodeServiceClient geocodeService = new GeocodeServiceClient();
          GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);
          return geocodeResponse;
      }

    
      public void CalculateRouteLocation(string from, string to)
      {
         // LocationCollection loc = new LocationCollection();

          if (!string.IsNullOrWhiteSpace(from))
          {
              if (!string.IsNullOrWhiteSpace(to))
              {
                  //Create the Request URL for the routing service
                  Uri routeRequest = new Uri(string.Format("http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&rpo=Points&key={2}",
                     from, to, "Alb8_m-LNHfEuGq-hXrdCNVYiqLKvzIZd3ZImsYlF1zHl1J1lCNEr_vtjPehn6t3"));



                  //Make a request and get the response 
                    GetResponse(routeRequest, (x) =>
                    {
                        Console.WriteLine(x.ResourceSets[0].Resources.Length + " result(s) found.");

                        //displauMsgBox(x.ResourceSets[0].Resources[0];
                        //Location loc = new Location();
                        if (x != null &&
                             x.ResourceSets != null &&
                             x.ResourceSets.Length > 0 &&
                             x.ResourceSets[0].Resources != null &&
                             x.ResourceSets[0].Resources.Length > 0)
                        {
                            Route route = x.ResourceSets[0].Resources[0] as Route;

                           
                            double[][] routePath1 = route.RoutePath.Line.Coordinates;

                            // ---------------.....................................---creating road map --...........-------------------------------------

                            
                           // Console.WriteLine("************length = "+ routePath1.Length );
                            for (int i = 0; i < routePath1.Length; i++)
                            {
                                if (routePath1[i].Length >= 2)
                                {
                                    locs.Add(new Microsoft.Maps.MapControl.WPF.Location(routePath1[i][0], routePath1[i][1]));
                                }
                            }
                           // Console.WriteLine("************count = " + locs.Count.ToString());
                        }
                    });
              }
          }
          return ;
      }

      private void GetResponse(Uri uri, Action<Response> callback)
      {
          WebClient wc = new WebClient();
          wc.OpenReadCompleted += (o, a) =>
          {
              if (callback != null)
              {
                  DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                  callback(ser.ReadObject(a.Result) as Response);
              }
          };
          wc.OpenReadAsync(uri);
      }

        public string getdirection(string start , string end ,ref string distance)
      {
          
          string results = "";
          RouteRequest routeRequest = new RouteRequest();

          // Set the credentials using a valid Bing Maps key
          routeRequest.Credentials = new Microsoft.Maps.MapControl.WPF.Credentials();
          routeRequest.Credentials.ApplicationId = key;

          //Parse user data to create array of waypoints
          string[] points = new string[] { start, end };
          Waypoint[] waypoints = new Waypoint[points.Length];


          int pointIndex = -1;
          foreach (string point in points)
          {
              pointIndex++;
              waypoints[pointIndex] = new Waypoint();
              string[] digits = point.Split(',');

              waypoints[pointIndex].Location = new RouteService.Location();
              waypoints[pointIndex].Location.Latitude = double.Parse(digits[0].Trim());
              waypoints[pointIndex].Location.Longitude = double.Parse(digits[1].Trim());

              if (pointIndex == 0)
                  waypoints[pointIndex].Description = "Start";
              else if (pointIndex == points.Length)
                  waypoints[pointIndex].Description = "End";
              else
                  waypoints[pointIndex].Description = string.Format("Stop #{0}", pointIndex);
          }

          routeRequest.Waypoints = waypoints;

          // Make the calculate route request
          RouteServiceClient routeService = new RouteServiceClient();
          RouteResult routeresult = new RouteResult();

          routeRequest.UserProfile = new RouteService.UserProfile
          {
              DistanceUnit = speechlibrary.RouteService.DistanceUnit.Kilometer
          };
          RouteResponse routeResponse = routeService.CalculateRoute(routeRequest);
          routeresult = routeResponse.Result;

          //  MessageBox.Show( routeresult.RoutePath.Points.Length.ToString() );

          // labeldistance.Visibility = Visibility.Visible;
          distance = "Distance = " + routeResponse.Result.Summary.Distance.ToString() + " Km";

          //MessageBox.Show(routeResponse.Result.Summary.Distance.ToString());
          //MessageBox.Show(routeResponse.Result.RoutePath.Points.Count().ToString());



          // Iterate through each itinerary item to get the route directions
          StringBuilder directions = new StringBuilder("");

          if (routeResponse.Result.Legs.Length > 0)
          {
              int instructionCount = 0;
              int legCount = 0;

              foreach (speechlibrary.RouteService.RouteLeg leg in routeResponse.Result.Legs)
              {
                  legCount++;
                  directions.Append(string.Format("Leg #{0}\n", legCount));

                  foreach (speechlibrary.RouteService.ItineraryItem item in leg.Itinerary)
                  {
                      instructionCount++;
                      directions.Append(string.Format("{0}. {1}\n",
                          instructionCount, item.Text));
                  }
              }
              //Remove all Bing Maps tags around keywords.  
              //If you wanted to format the results, you could use the tags
              Regex regex = new Regex("<[/a-zA-Z:]*>",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
              results = regex.Replace(directions.ToString(), string.Empty);
          }
          else
              results = "No Route found";

          return results;
      }

    }
}
