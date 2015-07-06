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
using Microsoft.Kinect.Wpf.Controls;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Imtcspeechservice;
using Microsoft.Maps.MapControl.WPF;
using speechlibrary;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using BingMapsRESTService.Common.JSON;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;

namespace WpfApplication1
{
 
    using System.Runtime.InteropServices;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;

   public partial class MainWindow : Window 
    {
        IList<Body> _bodies;
        
        MultiSourceFrameReader _reader;
       
       int toggle_flag = 1;
       
        
        public KinectSensor kinectSensor = null;
        
        public kinectandspeech addgrammer = new kinectandspeech();
        
        bool flag_engage = false;
        ulong trackingid = 0;
        string start;
        string end;
        public bool flag_mode = false;

        
       

        Microsoft.Maps.MapControl.WPF.Location image_location = new Microsoft.Maps.MapControl.WPF.Location();
        Microsoft.Maps.MapControl.WPF.Pushpin myPin_start = new Microsoft.Maps.MapControl.WPF.Pushpin();
        Microsoft.Maps.MapControl.WPF.Pushpin myPin_end = new Microsoft.Maps.MapControl.WPF.Pushpin();
        System.Windows.Controls.Label label_start = new System.Windows.Controls.Label();
        System.Windows.Controls.Label label_end = new System.Windows.Controls.Label();
        MapPolyline routeLine = new MapPolyline();

    

       SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
        // Key for bing maps 
       string key = "Alb8_m-LNHfEuGq-hXrdCNVYiqLKvzIZd3ZImsYlF1zHl1J1lCNEr_vtjPehn6t3";

       public MainWindow()
        {            
           InitializeComponent();
           myMap.Focus();
           KinectRegion.SetKinectRegion(this, kinectRegion);                   
           this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
        }
     
       public void openvoiceinterface()
        {
               
            int result = 1;
           
            test.Text = " window loaded ";
         
            addgrammer.call_after_speech_recognition_function(SpeechRecognized);
               
            addgrammer.Add_Grammer(ref result );

               
            if( result == 0 )
                {
                    test.AppendText("\n\nno speech recognizer ");                        
                 }
                else if(result == 2)
                {
                    test.Text = " kinect is not ready  ";
                }
              
           
        }

       private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            kinectSensor = KinectSensor.GetDefault();
            button_handgesture.Visibility = Visibility.Hidden;
            System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
            if (kinectSensor != null)
            {
                kinectSensor.Open();

                _reader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }

            openvoiceinterface();
           
            
        }
     
       
       void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();
            System.Windows.Point p;
            myMap.TryLocationToViewportPoint(myMap.Center, out p);

            int main_body_lost = 0;
          
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];
                   
                    frame.GetAndRefreshBodyData(_bodies);
                    
                    if (!flag_engage)
                    {
                        foreach (var body in _bodies)
                        {
                            if (body != null)
                            {
                                if (body.IsTracked)
                                {
                                    //hand control gesture code 
                                    Joint righthand = body.Joints[JointType.HandRight];
                                    Joint head = body.Joints[JointType.Head];
                                    Joint lefthand = body.Joints[JointType.HandLeft];
                                    Joint shoulderRight = body.Joints[JointType.ShoulderRight];

                                    

                                    if (righthand.Position.Y > shoulderRight.Position.Y && lefthand.Position.Y > shoulderRight.Position.Y && righthand.Position.Y < head.Position.Y && lefthand.Position.Y < head.Position.Y)
                                    {
                                        
                                        flag_engage = true;
                                        
                                        trackingid = body.TrackingId;
                                        test.Text = trackingid.ToString();
                                        image_engage.Visibility = Visibility.Hidden;
                                        speechSynthesizer.Speak("welcome to microsoft bing maps.");
                                        
                                        break;
                                    }



                                }
                            }
                        }
                    }
                    else
                    {
                        
                        foreach ( var main_body in _bodies)
                        {
                          //  test1.Text = trackingid.ToString();
                            
                            
                            if (main_body != null)
                            {
                                if (main_body.IsTracked && main_body.TrackingId == trackingid)
                                {
                                    main_body_lost = 1;
                                   // test2.Text = main_body.TrackingId.ToString();
                                    //hand control gesture code 
                                    Joint righthand = main_body.Joints[JointType.HandRight];
                                    Joint head = main_body.Joints[JointType.Head];
                                    Joint lefthand = main_body.Joints[JointType.HandLeft];
                                    Joint rightshoulder = main_body.Joints[JointType.ShoulderRight];
                                    Joint leftshoulder = main_body.Joints[JointType.ShoulderLeft];
                                    Joint spinemid = main_body.Joints[JointType.SpineMid];
                                    Joint spinebase = main_body.Joints[JointType.SpineBase];                                                               
                                    //  test1.Text = righthand.Position.X.ToString();
                                    // test2.Text = lefthand.Position.X.ToString();
                                    //test1.Visibility = Visibility.Hidden;
                                    // test2.Visibility = Visibility.Hidden;
                                    // testing purpose


                                    if (righthand.Position.Y > spinebase.Position.Y && righthand.Position.Y < spinemid.Position.Y && righthand.Position.X > rightshoulder.Position.X + 0.150  &&  main_body.HandRightState == HandState.Closed)
                                    {

                                        addgrammer.speechlibrray_stopaudio();
                                            toggle_flag = 2 ;
                                            ellipse_mute.Fill = new SolidColorBrush(Colors.Red);
                                            test.Text = toggle_flag.ToString();
                                    }
                                    if (lefthand.Position.Y > spinebase.Position.Y && lefthand.Position.Y < spinemid.Position.Y && lefthand.Position.X - leftshoulder.Position.X < -0.250)                                       
                                        {
                                            
                                            addgrammer.speechlibrray_resumeaudio();
                                            toggle_flag = 1;
                                            test.Text = toggle_flag.ToString();
                                            ellipse_mute.Fill = new SolidColorBrush(Colors.Green);
                                        }

                                   
                                   
                                    
                                   

                                    // for moving fast 
                                    if (main_body.HandRightState == HandState.Closed && main_body.HandLeftState != HandState.Closed)
                                    {
                                        if (righthand.Position.X > (rightshoulder.Position.X + 0.300))
                                        {
                                            p.X += 40;
                                        }

                                        if (righthand.Position.Y > (head.Position.Y + 0.100))
                                        {
                                            p.Y -= 40;
                                        }
                                       
                                        if (rightshoulder.Position.Y - righthand.Position.Y > 0.100 && rightshoulder.Position.Y - righthand.Position.Y < 0.35 && righthand.Position.Y > spinebase.Position.Y )                                       
                                        {
                                            p.Y += 40;
                                        }
                                    }
                                    if (main_body.HandLeftState == HandState.Closed && main_body.HandRightState != HandState.Closed)
                                    {
                                        if (lefthand.Position.X - leftshoulder.Position.X < -0.250 && lefthand.Position.Y < (rightshoulder.Position.Y + 0.200))
                                        {
                                            p.X -= 40;
                                        }
                                        if (lefthand.Position.Y > (head.Position.Y + 0.100) && lefthand.Position.Y > (rightshoulder.Position.Y + 0.200))
                                        {
                                            p.Y -= 40;
                                            p.X -= 40;
                                        }
                                        if (leftshoulder.Position.Y - lefthand.Position.Y > 0.100 && lefthand.Position.X - leftshoulder.Position.X < -0.250 && lefthand.Position.Y > spinebase.Position.Y)
                                        {
                                            p.Y += 40;
                                            p.X -= 40;
                                        }
                                    }
                                    // for moving slow 

                                    if (main_body.HandRightState == HandState.Open && main_body.HandLeftState != HandState.Closed)
                                    {
                                        if (righthand.Position.X > (rightshoulder.Position.X + 0.300))
                                        {
                                            p.X += 25;
                                        }

                                        if (righthand.Position.Y > (head.Position.Y + 0.100))
                                        {
                                            p.Y -= 25;
                                        }
                                        if (rightshoulder.Position.Y - righthand.Position.Y > 0.100 && rightshoulder.Position.Y - righthand.Position.Y < 0.35 && righthand.Position.Y > spinebase.Position.Y)
                                        {
                                            p.Y += 25;
                                        }
                                    }
                                    if (main_body.HandLeftState == HandState.Open && main_body.HandRightState != HandState.Closed)
                                    {
                                        if (lefthand.Position.X - leftshoulder.Position.X < -0.250 && lefthand.Position.Y < (rightshoulder.Position.Y + 0.200))
                                        {
                                            p.X -= 25;
                                        }
                                        if (lefthand.Position.Y > (head.Position.Y + 0.100) && lefthand.Position.Y > (rightshoulder.Position.Y + 0.200))
                                        {
                                            p.Y -= 25;
                                            p.X -= 25;
                                        }
                                        if (leftshoulder.Position.Y - lefthand.Position.Y > 0.100 && lefthand.Position.X - leftshoulder.Position.X < -0.250  && lefthand.Position.Y > spinebase.Position.Y )
                                        {
                                            p.Y += 25;
                                            p.X -= 20;
                                        }
                                    }

                                    Microsoft.Maps.MapControl.WPF.Location l;
                                    myMap.TryViewportPointToLocation(p, out l);
                                    myMap.SetView(l, myMap.ZoomLevel);


                                    // for zoom up and zoom down 

                                    if ((lefthand.Position.Z - righthand.Position.Z) > 0.2 && main_body.HandLeftState == HandState.Closed && righthand.Position.X < rightshoulder.Position.X + 0.10 && lefthand.Position.X > leftshoulder.Position.X - 0.10)
                                    {
                                        myMap.ZoomLevel += 0.2;
                                    }
                                    else if ((lefthand.Position.Z - righthand.Position.Z) < -0.2 && main_body.HandLeftState == HandState.Closed && righthand.Position.X < rightshoulder.Position.X + 0.10 && lefthand.Position.X > leftshoulder.Position.X - 0.10)
                                    {
                                        myMap.ZoomLevel -= 0.2;

                                    }
                                    // reset the map 
                                    if (lefthand.Position.X - righthand.Position.X > 0.200 && lefthand.Position.Y >= rightshoulder.Position.Y && righthand.Position.Y >= rightshoulder.Position.Y )
                                    {
                                        Microsoft.Maps.MapControl.WPF.Location loc1 = new Microsoft.Maps.MapControl.WPF.Location();
                                        loc1.Latitude = 12.9667;
                                        loc1.Longitude = 77.5667;
                                        myMap.SetView(loc1, Convert.ToDouble(4), Convert.ToDouble(0));



                                    }
                                }
                            }
                        }

                        if (main_body_lost == 0)
                        { 
                            flag_engage = false;
                            image_engage.Visibility = Visibility.Visible;
                           
                           
                        }
                    }
                }
              }
        }
      
       private void WindowClosing(object sender, CancelEventArgs e)
        {
            addgrammer.speechlibrray_windowclosing();

            if (null != this.kinectSensor)
            {
                this.kinectSensor.Close(); 
                this.kinectSensor = null;
            }
        }
         
       public void SpeechRecognized(ref string[] sentence ,ref string command )
        {
            
            test.Text = " speech recognized ";
            label_speakagain.Visibility = Visibility.Hidden;
            System.Windows.Point p;
            myMap.TryLocationToViewportPoint(myMap.Center, out p);
            Microsoft.Maps.MapControl.WPF.Location l;

            test.Text = sentence[0] + sentence[1] + sentence[2]; 
                if (sentence[0] != "take" && sentence[0] != "show")
                {
                    switch (command)
                    {
                        case "zoom":
                            myMap.ZoomLevel += 2;
                            
                        
                            break;

                        case "back":
                            myMap.ZoomLevel -= 2;
                            break;

                        case "find distance":
                            labeldistance.Foreground = Brushes.Red;
                            labeldistance.Visibility = Visibility.Visible;
                            break;
                     
                        case "hide direction":
                            labelResults.Visibility = Visibility.Hidden;
                            break;

                        case "bye":
                            speechSynthesizer.Speak("thank you for using microsoft bing maps");
                            System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
                            
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

                        case "Clear map":
                            myMap.Children.Clear();
                            add_item_over_map(); 
                       
                        break;

                        case "remove":
                            myMap.Children.Clear();
                            add_item_over_map();
                           

                                 break;

                        case "get image":
                                 addImageToMap(input.Text);
                                 break;

                        case "right" :
                              p.X += 300;
                             myMap.TryViewportPointToLocation(p, out l);
                             myMap.SetView(l, myMap.ZoomLevel);
                             break;

                        case "left":
                             p.X -= 300;
                             myMap.TryViewportPointToLocation(p, out l);
                             myMap.SetView(l, myMap.ZoomLevel);
                             break;

                        case "up":
                             p.Y -= 300;
                             myMap.TryViewportPointToLocation(p, out l);
                             myMap.SetView(l, myMap.ZoomLevel);
                             break;

                        case "down":                             
                             p.Y += 300;
                             myMap.TryViewportPointToLocation(p, out l);
                             myMap.SetView(l, myMap.ZoomLevel);
                             break;

                        case "button mode" :
                             debug_mode();
                             break;

                        case "hide button":
                             hide_button();
                             break;
                        default:
                            try
                            {
                                if ("" != command)
                                {
                                    input.Text = command;
                                    GetMyLocation();
                                }
                            }
                            catch (SystemException err)
                            {
                               
                                string errMsg = "Unable to get your location: " + err.Message.ToString();
                                label_speakagain.Visibility = Visibility.Visible;
                                label_speakagain.Content = errMsg;
                               
                            }
                            break;

                    }

                }
                else
                {
                   

                    if ("takemefrom" == sentence[0] + sentence[1] + sentence[2]   )
                    {                                                                                
                        input.Text = sentence[3];
                        input_des.Text = sentence[5];
                        test.Text = sentence[0] + sentence[1] + sentence[2] + sentence[3] + sentence[5];
                        GetMyRoute();
                    }

                    if ("showmethedirection" == sentence[0] + sentence[1] + sentence[2] + sentence[3])
                    {
                        
                        labelResults.Visibility = Visibility.Visible;
                        labelResults.Content = GetDirections();
                        
                    }

                }
                   
                
           
        }
         
       private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            return;
        }
       public void add_item_over_map()
       {
           myMap.Children.Add(labelResults);
           labelResults.Visibility = Visibility.Hidden;
           myMap.Children.Add(labeldistance);
           labeldistance.Visibility = Visibility.Hidden;
           myMap.Children.Add(input);
           myMap.Children.Add(input_des);
           myMap.Children.Add(location);
           myMap.Children.Add(button_direction);
           myMap.Children.Add(zoomdown);
           myMap.Children.Add(zoomup);
           myMap.Children.Add(route);
           myMap.Children.Add(test);
           myMap.Children.Add(getlocation);           
          myMap.Children.Add(image_engage);
          myMap.Children.Add(ellipse_mute);



       }
       public void addImageToMap(string place)
       {
           MapLayer imageLayer = new MapLayer();
           Image image = new Image();
           image.Height = 350;
           
           //Define the URI location of the image
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            if (place == "Victoria fall")
                myBitmapImage.UriSource = new Uri("Victoria_Falls.jpg", UriKind.Relative);
            else if (place == "India gate")
            {
                myBitmapImage.UriSource = new Uri("indiagate.jpg", UriKind.Relative);
                image.Name = "india";
            }
            else if (place == "Himalayas")
                myBitmapImage.UriSource = new Uri("himalayas.jpg", UriKind.Relative);
            else
                return;
          // myBitmapImage.UriSource = new Uri("http://vignette3.wikia.nocookie.net/cryptidz/images/7/79/Himalayas.jpg/revision/latest?cb=20140611014713");
           
           myBitmapImage.DecodePixelHeight = 350;
           myBitmapImage.EndInit();
           image.Source = myBitmapImage;
           image.Opacity = 0.8;
           image.Stretch = System.Windows.Media.Stretch.None;

           //The map location to place the image at
           //Center the image around the location specified
           PositionOrigin position = PositionOrigin.Center;

           //Add the image to the defined map layer
           imageLayer.AddChild(image, image_location, position);
           //Add the image layer to the map
           myMap.Children.Add(imageLayer);
          
       }
       
       private void getlocation_Click(object sender, RoutedEventArgs e)
        {
            
                GetMyLocation();
         
        }
 
       public  void GetMyLocation()
        {
            string results = "";
            var getgeocode = new kinectandspeech();
               
            Microsoft.Maps.MapControl.WPF.Location loc = new Microsoft.Maps.MapControl.WPF.Location();

            loc = getgeocode.getlocation(input.Text, ref results);
            image_location = loc;
            location.Text = results;
            if(location.Text == "No Result Found")
            {
                test.Text = " No Result Found ";
                return;
            }
          
             myMap.SetView( loc ,Convert.ToDouble(7), Convert.ToDouble(0));
         
                 
         Microsoft.Maps.MapControl.WPF.Pushpin myPin = new  Microsoft.Maps.MapControl.WPF.Pushpin();
         MapLayer.SetPosition(myPin ,loc );
         myMap.Children.Add(myPin);

         System.Windows.Controls.Label label = new System.Windows.Controls.Label();
         label.Content = input.Text +"\nLAT. ="+ loc.Latitude.ToString() +"\nLONG. ="+ loc.Longitude.ToString();
         label.Foreground = new SolidColorBrush(Colors.DarkBlue);
         label.Background = new SolidColorBrush(Colors.LawnGreen);
         label.FontSize = 25;
         label.Opacity = 0.6;
         MapLayer.SetPosition(label , loc);
         myMap.Children.Add(label);  

        }

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

       public  void GetMyRoute()
        {
           
            
            clear_map();            // to remove the pushpins and routline existing on the map 
            string start_from;
            string end_to;
            string results = "";
            var getgeocode = new kinectandspeech();

            Microsoft.Maps.MapControl.WPF.Location loc1 = new Microsoft.Maps.MapControl.WPF.Location();

            loc1 = getgeocode.getlocation(input.Text, ref results);
          
           
            if (results == "No Result Found")
            {
                location.Text = results;
                return;
            }
            else 
            {
                if (loc1 != null)
                {
                    start = loc1.Latitude.ToString() + "," + loc1.Longitude.ToString();
                    start_from = String.Format("{0},{1}", loc1.Latitude, loc1.Longitude);
                }
                else
                    return;
            }
               
            /* ---------------------------------adding pushpin to start location ------------------------------------------- */
    
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
                  
            Microsoft.Maps.MapControl.WPF.Location loc = new Microsoft.Maps.MapControl.WPF.Location();

            loc = getgeocode.getlocation(input_des.Text, ref results);
           
            if (results == "No Result Found")
            {
                location.Text = " No Result Found ";
                return;
            }
            else if(results == "indexoutofrange" )
            {
                return;
            }
            else
            {
                if (loc != null)
                {
                    end = loc.Latitude.ToString() + "," + loc.Longitude.ToString();
                    end_to = String.Format("{0},{1}", loc.Latitude, loc.Longitude);
                }
                else
                    return;
            }
           

            /* ---------------------------------adding pushpin to end location ------------------------------------------- */
   
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
     
            /*---------------------------------------- adding route on map ----------------------------------------------- */

            init(start_from, end_to);    // this funciton will add route on the map 
                              
        }

       private void route_Click(object sender, RoutedEventArgs e)
       {
           GetMyRoute();
       }

       public string GetDirections()
       {
           kinectandspeech direction = new kinectandspeech() ;
           
           Microsoft.Maps.MapControl.WPF.Location loc = new Microsoft.Maps.MapControl.WPF.Location();
           string locationresult ="" ;

           if( string.IsNullOrEmpty(start))
            {
                loc = direction.getlocation(input.Text, ref locationresult);
               start = loc.Latitude.ToString() + "," + loc.Longitude.ToString();
                 if (locationresult == "No Result Found")
                 {
                     location.Text = locationresult ;
                     return "" ;
                 }
             }

           if (string.IsNullOrEmpty(end))
           {
               try
               {
                    loc = direction.getlocation(input.Text, ref locationresult);
                  
                   end = loc.Latitude.ToString() + "," + loc.Longitude.ToString();
               }
               catch(NullReferenceException e)
               {
                   return "";
               }
               
               if (locationresult == "No Result Found")
               {
                   location.Text = locationresult;
                   return "";
               }

           }

           
          string results = "";
          string distance = "";

          results = direction.getdirection(start, end, ref distance);
          labeldistance.Content = distance;
        
           return results;                    
       }
     
       private void init(string from , string to)
       {
           var getlocation = new kinectandspeech();
                  
          if (!string.IsNullOrWhiteSpace(from))
           {
               if (!string.IsNullOrWhiteSpace(to))
               {
           

                           getlocation.CalculateRouteLocation(from, to);

                           routeLine.Locations = getlocation.locs ;
                           routeLine.Stroke = new SolidColorBrush(Colors.Red);
                           routeLine.StrokeThickness = 4 ;

                           myMap.Children.Add(routeLine);
                        
                           myMap.ZoomLevel = 5;
                           
                 }
                 else
                 {
                           MessageBox.Show("No Results found.");
                 }                                                  
           }
       }

       private void button_startvoice_Click(object sender, RoutedEventArgs e)
       {
           openvoiceinterface();
           button_startvoice.Visibility = Visibility.Hidden;
       }

       private void button_direction_Click(object sender, RoutedEventArgs e)
       {
           labelResults.Visibility = Visibility.Visible;
           labelResults.Content = GetDirections();
       }

       private void button_handgesture_Click(object sender, RoutedEventArgs e)
       {
           kinectSensor = KinectSensor.GetDefault();
           button_handgesture.Visibility = Visibility.Hidden;
           if (kinectSensor != null)
           {
               kinectSensor.Open();

               _reader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
               _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
           }
       }
      
       public void debug_mode()
       {
           getlocation.Visibility = Visibility.Visible;
           button_direction.Visibility = Visibility.Visible;
         //  button_handgesture.Visibility = Visibility.Visible;
         //  button_startvoice.Visibility = Visibility.Visible;
           zoomdown.Visibility = Visibility.Visible;
           zoomup.Visibility = Visibility.Visible;
           mode.Visibility = Visibility.Visible;
           route.Visibility = Visibility.Visible;          

           input.Visibility = Visibility.Visible;
           input_des.Visibility = Visibility.Visible;
           location.Visibility = Visibility.Visible;
           test.Visibility = Visibility.Visible;
       }

       public void hide_button()
        {
            getlocation.Visibility = Visibility.Hidden;
            button_direction.Visibility = Visibility.Hidden;
            button_handgesture.Visibility = Visibility.Hidden;
            button_startvoice.Visibility = Visibility.Hidden;
            zoomdown.Visibility = Visibility.Hidden;
            zoomup.Visibility = Visibility.Hidden;
            mode.Visibility = Visibility.Hidden;
            route.Visibility = Visibility.Hidden;

            input.Visibility = Visibility.Hidden;
            input_des.Visibility = Visibility.Hidden;
            location.Visibility = Visibility.Hidden;
            test.Visibility = Visibility.Hidden;
        }

    }

    
}
