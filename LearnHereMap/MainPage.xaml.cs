using System;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace LearnHereMap
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            mapControl.DataContext = new ViewModel();
        }

        private void mapControl_TransformOriginChanged(MapControl sender, object args)
        {
            Debug.WriteLine(mapControl.TransformOrigin.ToString());
        }


        private void AddMarker()
        {
            var mapIcon1 = new MapIcon
            {
                //geopoint配合定位namespace

                Location = new Geopoint(new BasicGeoposition
                {
                    Latitude = 39.9,
                    Longitude = 116.3
                }),
                //锚点
                NormalizedAnchorPoint = new Point(0, 0),
                Title = "Space Needle",
                ////图像URI上面是这样的
                //Image =
                //    RandomAccessStreamReference.CreateFromUri(new Uri(
                //        "ms-appx:///Assets/kfc.jpg", UriKind.RelativeOrAbsolute))
                Image =
                    RandomAccessStreamReference.CreateFromUri(new Uri(
                        "http://i.stack.imgur.com/mM3h7.jpg?s=32&g=1", UriKind.RelativeOrAbsolute))
            };
            mapControl.MapElements.Add(mapIcon1);
        }


        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.
        ///     This parameter is typically used to configure the page.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void Grid_OnLoaded(object sender, RoutedEventArgs e)
        {
            //如何设置地图中心点
            var center = new BasicGeoposition { Longitude = 116.3, Latitude = 39.9 };
            mapControl.Center = new Geopoint(center);
            mapControl.ZoomLevel = 10;


            //Learn property changed
            mapControl.CenterChanged += mapControl_CenterChanged;


            //尝试理解
            mapControl.TransformOriginChanged += mapControl_TransformOriginChanged;


            mapControl.WatermarkMode = MapWatermarkMode.Automatic;


            //3d 建筑 双指下滑
            mapControl.LandmarksVisible = false;


            //AddMarker();

            //AddOverlay();

            //Geocode();

            //GetRouteAndDirections();

            //MoveCamera();


            MoveCameraBounds();
        }

        private void MoveCameraBounds()
        {
            var center = new BasicGeoposition { Longitude = 116.3, Latitude = 25 };
            var geopoint = new Geopoint(center);
            mapControl.TrySetViewAsync(geopoint, 6, 0, 0, MapAnimationKind.Bow);


            var westNorth = new BasicGeoposition
            {
                Longitude = 110,
                Latitude = 40
            };


            var eastSouth = new BasicGeoposition { Longitude = 111, Latitude = 39 };
            var bound = new GeoboundingBox(westNorth, eastSouth);
            mapControl.TrySetViewBoundsAsync(bound, new Thickness(0,0,0,0), MapAnimationKind.None);
        }

        private void MoveCamera()
        {
            var center = new BasicGeoposition { Longitude = 116.3, Latitude = 25 };
            var geopoint = new Geopoint(center);
            mapControl.TrySetViewAsync(geopoint, 6, 0, 0, MapAnimationKind.Bow);
        }

        private void mapControl_CenterChanged(MapControl sender, object args)
        {
            Debug.WriteLine(sender.Center.Position.Longitude);
        }

        private void AddOverlay()
        {
            var fence1 = new Ellipse { Fill = new SolidColorBrush(Colors.BlueViolet), Height = 20, Width = 20 };
            mapControl.Children.Add(fence1);
            var basic = new BasicGeoposition { Longitude = 116.3, Latitude = 39.9 };
            var point = new Geopoint(basic);
            MapControl.SetLocation(fence1, point);
            MapControl.SetNormalizedAnchorPoint(fence1, new Point(0.5, 0.5));


            var fence = new Ellipse { Fill = new SolidColorBrush(Colors.Red), Height = 20, Width = 20 };
            mapControl.Children.Add(fence);
            MapControl.SetLocation(fence, point);
            MapControl.SetNormalizedAnchorPoint(fence, new Point(0.5, 1));
        }


        private async void Geocode()
        {
            // Address or business to geocode.
            string addressToGeocode = "Apple";

            // Nearby location to use as a query hint.
            var queryHint = new BasicGeoposition();
            queryHint.Latitude = 39.9;
            queryHint.Longitude = 116.3;
            var hintPoint = new Geopoint(queryHint);

            // Geocode the specified address, using the specified reference point
            // as a query hint. Return no more than 3 results.
            MapLocationFinderResult result =
                await MapLocationFinder.FindLocationsAsync(
                    addressToGeocode,
                    hintPoint,
                    3);


            result =
                await MapLocationFinder.FindLocationsAtAsync(
                    hintPoint
                    );

            // If the query returns results, display the coordinates
            // of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
            {
                string rst = "result = (" +
                             result.Locations[0].Point.Position.Latitude + "," +
                             result.Locations[0].Point.Position.Longitude + ")";

                Debug.WriteLine(rst);
            }
        }


        private async void GetRouteAndDirections()
        {
            // Start at Microsoft in Redmond, Washington.
            var startLocation = new BasicGeoposition();
            startLocation.Latitude = 47.643;
            startLocation.Longitude = -122.131;
            var startPoint = new Geopoint(startLocation);

            // End at the city of Seattle, Washington.
            var endLocation = new BasicGeoposition();
            endLocation.Latitude = 47.604;
            endLocation.Longitude = -122.329;
            var endPoint = new Geopoint(endLocation);

            // Get the route between the points.
            MapRouteFinderResult routeResult =
                await MapRouteFinder.GetDrivingRouteAsync(
                    startPoint,
                    endPoint,
                    MapRouteOptimization.Time,
                    MapRouteRestrictions.None);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                if (routeResult.Status == MapRouteFinderStatus.Success)
                {
                    // Use the route to initialize a MapRouteView.
                    var viewOfRoute = new MapRouteView(routeResult.Route)
                    {
                        RouteColor = Colors.Yellow,
                        OutlineColor = Colors.Black
                    };

                    // Add the new MapRouteView to the Routes collection
                    // of the MapControl.
                    mapControl.Routes.Add(viewOfRoute);

                    // Fit the MapControl to the route.
                    await mapControl.TrySetViewBoundsAsync(
                        routeResult.Route.BoundingBox,
                        null,
                        MapAnimationKind.Bow);
                }


                // Display summary info about the route.
                tbOutputText.Inlines.Add(new Run
                {
                    Text = "Total estimated time (minutes) = "
                           + routeResult.Route.EstimatedDuration.TotalMinutes
                });
                tbOutputText.Inlines.Add(new LineBreak());
                tbOutputText.Inlines.Add(new Run
                {
                    Text = "Total length (kilometers) = "
                           + (routeResult.Route.LengthInMeters / 1000)
                });
                tbOutputText.Inlines.Add(new LineBreak());
                tbOutputText.Inlines.Add(new LineBreak());

                // Display the directions.
                tbOutputText.Inlines.Add(new Run
                {
                    Text = "DIRECTIONS"
                });
                tbOutputText.Inlines.Add(new LineBreak());

                foreach (MapRouteLeg leg in routeResult.Route.Legs)
                {
                    foreach (MapRouteManeuver maneuver in leg.Maneuvers)
                    {
                        tbOutputText.Inlines.Add(new Run
                        {
                            Text = maneuver.InstructionText
                        });
                        tbOutputText.Inlines.Add(new LineBreak());
                    }
                }
            }
            else
            {
                tbOutputText.Text =
                    "A problem occurred: " + routeResult.Status;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            MoveCamera();
        }
    }
}