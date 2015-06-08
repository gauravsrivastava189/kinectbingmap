using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
//using Microsoft.Speech;

using WpfApplication1.GeocodeService;
using WpfApplication1.SearchService;
using WpfApplication1.ImageryService;
using WpfApplication1.RouteService;
using Microsoft.Maps.MapControl.WPF;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
// to add route 
using BingMapsRESTService;
using BingMapsRESTService.Common.JSON;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;





namespace WpfApplication1
{
   // using System;
   // using System.Collections.Generic;
    //using System.ComponentModel;
   // using System.IO;
    using System.Runtime.InteropServices;
//using System.Text;
   // using System.Windows;
   // using System.Windows.Documents;
   // using System.Windows.Media;

   using Microsoft.Speech.AudioFormat;
     using Microsoft.Speech.Recognition;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Stream for 32b-16b conversion.
        /// </summary>
      
      private KinectAudioStream convertStream = null;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine = null;


        /// <summary>
        /// List of all UI span elements used to select recognized text.
        /// </summary>
    //    private List<Span> recognitionSpans;


// Key for bing maps 
        string key = "Alb8_m-LNHfEuGq-hXrdCNVYiqLKvzIZd3ZImsYlF1zHl1J1lCNEr_vtjPehn6t3";
        public MainWindow()
        {
            InitializeComponent();
             myMap.Focus();
            
        }

//------------------------------------------------- Voice handling start -----------------------------------------------------
        private static RecognizerInfo TryGetKinectRecognizer()
        {
            
            IEnumerable<RecognizerInfo> recognizers;
           

            // This is required to catch the case when an expected recognizer is not installed.
            // By default - the x86 Speech Runtime is always expected. 
            try
            {
              
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
                
                // here is the problem 
                
               
                
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
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            test.Text = " window loaded ";
            // Only one sensor is supported
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                test.Text = " kinect sensor availaible ";
                // open the sensor
                this.kinectSensor.Open();

                // grab the audio stream
                IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                // create the convert stream
                this.convertStream = new KinectAudioStream(audioStream);
            }
            else
            {
                test.Text = " kinect is not ready  ";
                // on failure, set the status text
                //this.statusBarText.Text = Properties.Resources.NoKinectReady;
                return;
            }

            RecognizerInfo ri = TryGetKinectRecognizer();
            
            if (null != ri)
            {
                test.Text = " got recognizer ";
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                var directions = new Choices();
                directions.Add("india");
                directions.Add("jaipur");               
                directions.Add("delhi");
                directions.Add("new york");
                directions.Add("himalayas");
                directions.Add("microsoft");
                directions.Add("google");
                directions.Add("zoom");
                directions.Add("back");
                directions.Add("change mode");
                directions.Add("hide direction");
                directions.Add("find distance");

               

                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(directions);


                var g = new Grammar(gb);
                this.speechEngine.LoadGrammar(g);

                var gb1 = new GrammarBuilder { Culture = ri.Culture };
                gb1.Append("take");
                gb1.Append("me");
                gb1.Append("from");
                gb1.Append("jaipur");
                gb1.Append("to");
                gb1.Append("agra");
                var g1 = new Grammar(gb1);
                this.speechEngine.LoadGrammar(g1);

                var gb2 = new GrammarBuilder { Culture = ri.Culture };
                gb2.Append("show");
                gb2.Append("me");
                gb2.Append("the");
                gb2.Append("direction");
                var g2 = new Grammar(gb2);
                this.speechEngine.LoadGrammar(g2);

                var gb3 = new GrammarBuilder { Culture = ri.Culture };
                gb3.Append("take");
                gb3.Append("me");
                gb3.Append("from");
                gb3.Append("delhi");
                gb3.Append("to");
                gb3.Append("agra");
                var g3 = new Grammar(gb3);
                this.speechEngine.LoadGrammar(g3);

                var gb4 = new GrammarBuilder { Culture = ri.Culture };
                gb4.Append("take");
                gb4.Append("me");
                gb4.Append("from");
                gb4.Append("newyork");
                gb4.Append("to");
                gb4.Append("chicago");
                var g4 = new Grammar(gb4);
                this.speechEngine.LoadGrammar(g4);
            
               /* GrammarBuilder startStop = new GrammarBuilder();
                GrammarBuilder dictation = new GrammarBuilder();
                dictation.AppendDictation();

                startStop.Append(new SemanticResultKey("StartDictation", new SemanticResultValue("Start Dictation", true)));
                startStop.Append(new SemanticResultKey("DictationInput", dictation));
                startStop.Append(new SemanticResultKey("StopDictation", new SemanticResultValue("Stop Dictation", false)));
                Grammar grammar = new Grammar(startStop);
                grammar.Enabled = true;
                grammar.Name = " Free-Text Dictation ";
                this.speechEngine.LoadGrammar(grammar);  */


                   // this.speechEngine.LoadGrammar(new Grammar(new GrammarBuilder("exit")));
                   // this.speechEngine.LoadGrammar(new DictationGrammar());

           


                this.speechEngine.SpeechRecognized += this.SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected += this.SpeechRejected;

                // let the convertStream know speech is going active
                this.convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                this.speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                test.AppendText( "\n\nno speech recognizer ");
                //this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }
        }
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.convertStream)
            {
                this.convertStream.SpeechActive = false;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= this.SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= this.SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }

            if (null != this.kinectSensor)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        string start;
        string end;
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            
            test.Text = " speech recognized ";
           // test.Text = " speech recognized ";
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            // Number of degrees in a right angle.
           // const int DegreesInRightAngle = 90;

            // Number of pixels turtle should move forwards or backwards each time.
           // const int DisplacementAmount = 60;

          //  this.ClearRecognitionHighlights();


            if (e.Result.Confidence >= ConfidenceThreshold)
            {

                if (e.Result.Words[0].Text != "take" && e.Result.Words[0].Text != "show")
                {
                    switch (e.Result.Text)
                    {
                        case "zoom":
                            myMap.ZoomLevel += 3;
                            break;

                        case "back":
                            myMap.ZoomLevel -= 3;
                            break;

                        case "find distance":
                            labeldistance.Foreground = Brushes.Red;
                            labeldistance.Visibility = Visibility.Visible;
                            break;
                     
                        case "hide direction":
                            labelResults.Visibility = Visibility.Hidden;
                            break;
 
                        case "change mode" :
                                  
                        if (flag_mode == false)
                             {
                                 myMap.Mode = new AerialMode();
                                    flag_mode = true;
                             }
                         else
                             {
                                myMap.Mode = new RoadMode();
                                flag_mode = false;
                             }
                        break;
                        default:
                            try
                            {
                                input.Text = e.Result.Text;
                                GetMyLocation();
                            }
                            catch (SystemException err)
                            {
                                string errMsg = "Unable to get your location: " + err.Message.ToString();
                                MessageBox.Show(errMsg);
                            }
                            break;

                    }

                }
                else
                {

                    if ("takemefrom" == e.Result.Words[0].Text + e.Result.Words[1].Text + e.Result.Words[2].Text)
                    {
                        input.Text = e.Result.Words[3].Text.ToString();
                        input_des.Text = e.Result.Words[5].Text.ToString();
                        test.Text = e.Result.Words[0].Text + e.Result.Words[1].Text + e.Result.Words[2].Text + e.Result.Words[3].Text + e.Result.Words[5].Text;
                        GetMyRoute();
                    }

                    if ("showmethedirection" == e.Result.Words[0].Text + e.Result.Words[1].Text + e.Result.Words[2].Text + e.Result.Words[3].Text)
                    {
                        labelResults.Visibility = Visibility.Visible;
                        labelResults.Content = GetDirections();
                    }

                }
                   
                }
           
        }
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            return;
        }

 ///------------------------------------------------ Voice handling end------------------------------------------------------- 
    /// <summary>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void getlocation_Click(object sender, RoutedEventArgs e)
        {
          //  try
       //     {
                GetMyLocation();
         //   }
         /*  catch (SystemException err)
            {
                string errMsg = "Unable to get your location: " + err.Message.ToString();
                MessageBox.Show(errMsg);
            }*/
        }


      



      public  void GetMyLocation()
        {
            string results = "";
            GeocodeService.Location point = new GeocodeService.Location();
            GeocodeResponse geocodeResponse = GeocodeAddressGeocodeResponse(input.Text);

            if (geocodeResponse.Results.Length > 0)
                results = String.Format("Latitude: {0}\nLongitude: {1}",
                  geocodeResponse.Results[0].Locations[0].Latitude,
                  geocodeResponse.Results[0].Locations[0].Longitude);
            else
                results = "No Results Found";

            location.Text = results;


           WpfApplication1.GeocodeService.Location myLoc = new WpfApplication1.GeocodeService.Location();
           

         myLoc.Latitude = geocodeResponse.Results[0].Locations[0].Latitude;
         myLoc.Longitude = geocodeResponse.Results[0].Locations[0].Longitude;

         Microsoft.Maps.MapControl.WPF.Location loc = new Microsoft.Maps.MapControl.WPF.Location();
         loc.Latitude = geocodeResponse.Results[0].Locations[0].Latitude;
         loc.Longitude = geocodeResponse.Results[0].Locations[0].Longitude;
         myMap.SetView( loc ,Convert.ToDouble(7), Convert.ToDouble(0));
         
                 
         Microsoft.Maps.MapControl.WPF.Pushpin myPin = new  Microsoft.Maps.MapControl.WPF.Pushpin();
         MapLayer.SetPosition(myPin ,loc );
         myMap.Children.Add(myPin);

         System.Windows.Controls.Label label = new System.Windows.Controls.Label();
         label.Content = input.Text;
         label.Foreground = new SolidColorBrush(Colors.DarkBlue);
         label.Background = new SolidColorBrush(Colors.WhiteSmoke);
         label.FontSize = 30;
         MapLayer.SetPosition(label , loc);
         myMap.Children.Add(label);  

        }




       public GeocodeResponse GeocodeAddressGeocodeResponse(string address)
        {
            GeocodeRequest geocodeRequest = new GeocodeRequest();
          

            // Set the credentials using a valid Bing Maps key
            geocodeRequest.Credentials = new Microsoft.Maps.MapControl.WPF.Credentials();
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
       bool flag_mode= false;

       private void mode_Click(object sender, RoutedEventArgs e)
       {
           if (flag_mode == false)
           {
               myMap.Mode = new AerialMode();
               flag_mode = true;
           }
           else
           {
               myMap.Mode = new RoadMode();
               flag_mode = false;
           }
       }

       private void zoomup_Click(object sender, RoutedEventArgs e)
       {
           myMap.ZoomLevel += 2;
       }

       private void zoomdown_Click(object sender, RoutedEventArgs e)
       {
           myMap.ZoomLevel -= 2;
       }

 //-------------------------------Route calculation --------------------------------------------------------------
 
        void clear_map()
       {
           myMap.Children.Remove(myPin_end);
           myMap.Children.Remove(myPin_start);
           myMap.Children.Remove(label_start);
           myMap.Children.Remove(label_end);
           myMap.Children.Remove(routeLine);
           labelResults.Visibility = Visibility.Hidden;
           labeldistance.Visibility = Visibility.Hidden;
       }
       
        
        Microsoft.Maps.MapControl.WPF.Pushpin myPin_start = new Microsoft.Maps.MapControl.WPF.Pushpin();
        Microsoft.Maps.MapControl.WPF.Pushpin myPin_end = new Microsoft.Maps.MapControl.WPF.Pushpin();
        System.Windows.Controls.Label label_start = new System.Windows.Controls.Label();
        System.Windows.Controls.Label label_end = new System.Windows.Controls.Label();

       public  void GetMyRoute()
        {
            clear_map();            // to remove the pushpins and routline existing on the map 
            string start_from;
            string end_to;

            //string[] input1 = input.Text.Split(';');
            GeocodeResponse resp = GeocodeAddressGeocodeResponse(input.Text);
           

            //  if (resp.Results.Length > 0)
            if (resp.Results.Length == 0)
            {
                MessageBox.Show("No result found ");
                return;
            }
            else
            {
                start = resp.Results[0].Locations[0].Latitude.ToString() + "," + resp.Results[0].Locations[0].Longitude.ToString();
                start_from = String.Format("{0},{1}",
                resp.Results[0].Locations[0].Latitude,
                resp.Results[0].Locations[0].Longitude);
            }



            /* ---------------------------------adding pushpin to start location ------------------------------------------- */

            Microsoft.Maps.MapControl.WPF.Location loc1 = new Microsoft.Maps.MapControl.WPF.Location();
            loc1.Latitude = resp.Results[0].Locations[0].Latitude;
            loc1.Longitude = resp.Results[0].Locations[0].Longitude;
            myMap.SetView(loc1, Convert.ToDouble(7), Convert.ToDouble(0));



            MapLayer.SetPosition(myPin_start, loc1);
            myMap.Children.Add(myPin_start);


            label_start.Content = input.Text;
            label_start.Foreground = new SolidColorBrush(Colors.DarkBlue);
            label_start.Background = new SolidColorBrush(Colors.WhiteSmoke);
            label_start.FontSize = 30;
            MapLayer.SetPosition(label_start, loc1);
            myMap.Children.Add(label_start);
            /* -------------------------------------------end ------------------------------------------------------------------*/
            resp = GeocodeAddressGeocodeResponse(input_des.Text);
            

            if (resp.Results.Length == 0)
            {
                MessageBox.Show("no result found ");
                return;
            }
            else
            {
                end = resp.Results[0].Locations[0].Latitude.ToString() + "," + resp.Results[0].Locations[0].Longitude.ToString();
                end_to = String.Format("{0},{1}",
                resp.Results[0].Locations[0].Latitude,
                resp.Results[0].Locations[0].Longitude);
            }




            /* ---------------------------------adding pushpin to end location ------------------------------------------- */
            Microsoft.Maps.MapControl.WPF.Location loc = new Microsoft.Maps.MapControl.WPF.Location();
            loc.Latitude = resp.Results[0].Locations[0].Latitude;
            loc.Longitude = resp.Results[0].Locations[0].Longitude;
            myMap.SetView(loc, Convert.ToDouble(7), Convert.ToDouble(0));



            MapLayer.SetPosition(myPin_end, loc);
            myPin_end.Tag = "IamEnd";
            myMap.Children.Add(myPin_end);


            label_end.Content = input_des.Text;
            label_end.Foreground = new SolidColorBrush(Colors.DarkBlue);
            label_end.Background = new SolidColorBrush(Colors.WhiteSmoke);
            label_end.FontSize = 30;
            MapLayer.SetPosition(label_end, loc);
            myMap.Children.Add(label_end);
            /* ---------------------------------end ------------------------------------------- */


            //labelResults.Visibility = Visibility.Visible;
            // labelResults.Content = GetDirections();

            /*---------------------------------------- adding route on map ----------------------------------------------- */

            init(start_from, end_to);    // this funciton will add route on the map 

            // myMap.Children.Remove();
         
           
        }
       private void route_Click(object sender, RoutedEventArgs e)
       {
           GetMyRoute();
       }
       private string GetDirections()
       {
         /*  GeocodeResponse resp = GeocodeAddressGeocodeResponse(input.Text);
           string start = resp.Results[0].Locations[0].Latitude.ToString() + "," + resp.Results[0].Locations[0].Longitude.ToString();

           resp = GeocodeAddressGeocodeResponse(input_des.Text);
           string end = resp.Results[0].Locations[0].Latitude.ToString() + "," + resp.Results[0].Locations[0].Longitude.ToString();
       */   
           string results = "";
           RouteRequest routeRequest = new RouteRequest();

           // Set the credentials using a valid Bing Maps key
           routeRequest.Credentials = new Credentials();
           routeRequest.Credentials.ApplicationId = key;

           //Parse user data to create array of waypoints
           string[] points = new string[] { start, end };
           Waypoint[] waypoints = new Waypoint[points.Length];

           int pointIndex = -1;
           foreach (string point in points)
           {
               pointIndex++;
               waypoints[pointIndex] = new Waypoint();
               string[] digits = point.Split(','); waypoints[pointIndex].Location = new RouteService.Location();
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
               DistanceUnit =WpfApplication1.RouteService.DistanceUnit.Kilometer 
           };
           RouteResponse routeResponse = routeService.CalculateRoute(routeRequest);
           routeresult = routeResponse.Result;

         //  MessageBox.Show( routeresult.RoutePath.Points.Length.ToString() );

          // labeldistance.Visibility = Visibility.Visible;
           labeldistance.Content = "Distance = " + routeResponse.Result.Summary.Distance.ToString() + " Km";

           //MessageBox.Show(routeResponse.Result.Summary.Distance.ToString());
           //MessageBox.Show(routeResponse.Result.RoutePath.Points.Count().ToString());

          

           // Iterate through each itinerary item to get the route directions
           StringBuilder directions = new StringBuilder("");

           if (routeResponse.Result.Legs.Length > 0)
           {
               int instructionCount = 0;
               int legCount = 0;

               foreach (WpfApplication1.RouteService.RouteLeg leg in routeResponse.Result.Legs)
               {
                   legCount++;
                   directions.Append(string.Format("Leg #{0}\n", legCount));

                   foreach (WpfApplication1.RouteService.ItineraryItem item in leg.Itinerary)
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

       MapPolyline routeLine = new MapPolyline();
       private void init(string from , string to)
       {

          //   string from = "26.9122,75.8060";
        //  string to = "28.6432,77.1158";     
   

        //   GeocodeResponse resp = GeocodeAddressGeocodeResponse(input.Text);
       //    string from = resp.Results[0].Locations[0].Latitude.ToString() + "," + resp.Results[0].Locations[0].Longitude.ToString();

          // resp = GeocodeAddressGeocodeResponse(input_des.Text);
          // string to = resp.Results[0].Locations[0].Latitude.ToString() + "," + resp.Results[0].Locations[0].Longitude.ToString();
         
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
                           // -------------------------------------------- Adding pushpin to start and end location ---------------------------------




                           // -------------------------------------------------- Pushpin Added ---------------------------------------------------------

                           // ---------------.....................................---creating road map --...........-------------------------------------

                           LocationCollection locs = new LocationCollection();

                           for (int i = 0; i < routePath1.Length; i++)
                           {
                               if (routePath1[i].Length >= 2)
                               {
                                   locs.Add(new Microsoft.Maps.MapControl.WPF.Location(routePath1[i][0], routePath1[i][1]));
                               }
                           }

                          /* MapPolyline routeLine = new MapPolyline()
                           {
                               Locations = locs,
                               Stroke = new SolidColorBrush(Colors.Blue),
                               StrokeThickness = 5
                           };*/
                           routeLine.Locations = locs ;
                           routeLine.Stroke = new SolidColorBrush(Colors.Red);
                           routeLine.StrokeThickness = 3 ;

                           myMap.Children.Add(routeLine);
                          

                           myMap.SetView(locs, new Thickness(17), 0);
                       }
                       else
                       {
                           MessageBox.Show("No Results found.");
                       }


                       //........................................................ road map creation end ..........................................              


                       


                       //displauMsgBox(x.ResourceSets[0].Resources.Length.ToString());
                   });

               }

           }
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

       private void GetPOSTResponse(Uri uri, string data, Action<Response> callback)
       {
           HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

           request.Method = "POST";
           request.ContentType = "text/plain;charset=utf-8";

           System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
           byte[] bytes = encoding.GetBytes(data);

           request.ContentLength = bytes.Length;

           using (Stream requestStream = request.GetRequestStream())
           {
               // Send the data.
               requestStream.Write(bytes, 0, bytes.Length);
           }

           request.BeginGetResponse((x) =>
           {
               using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
               {
                   if (callback != null)
                   {
                       DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                       callback(ser.ReadObject(response.GetResponseStream()) as Response);
                   }
               }
           }, null);
       }
      
    }
}
