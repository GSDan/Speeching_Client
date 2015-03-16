using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using System.Threading.Tasks;
using System.Net.Http;
using SpeechingCommon;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.Gms.Maps.Model;

namespace Droid_PeopleWithParkinsons
{
    [Activity(Label = "LocationActivity")]
    public class LocationActivity : Activity, Android.Gms.Maps.IOnMapReadyCallback, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
    {
        MapFragment mapFragment;
        GoogleMap map;
        IGoogleApiClient apiClient;
        Location lastLoc;
        GooglePlace[] nearby;
        bool placed = false;

        ListView mainList;
        ProgressBar spinner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.LocationActivity);
            mapFragment = MapFragment.NewInstance();
            FragmentTransaction tx = FragmentManager.BeginTransaction();
            tx.Add(Resource.Id.map_fragment_container, mapFragment);
            tx.Commit();

            spinner = FindViewById<ProgressBar>(Resource.Id.placesProgress);
            spinner.Visibility = ViewStates.Visible;

            mainList = FindViewById<ListView>(Resource.Id.placesList);
            View header = LayoutInflater.Inflate(Resource.Layout.PlacesListHeader, null);
            mainList.AddHeaderView(header, null, false);
            mainList.Visibility = ViewStates.Gone;

            mapFragment.GetMapAsync(this);
            ReadyGoogleApi();
        }

        private void ReadyGoogleApi()
        {
            GoogleApiClientBuilder builder = new GoogleApiClientBuilder(this)
            .AddConnectionCallbacks(this)
            .AddOnConnectionFailedListener(this)
            .AddApi(LocationServices.Api);

            apiClient = builder.Build();
            apiClient.Connect();
        }

        public void OnMapReady(GoogleMap finalMap)
        {
            map = finalMap;

            if(nearby != null && !placed)
            {
                PopulateMap();
                PopulateList();
            }
        }

        /// <summary>
        /// Zoom the map to focus on this location
        /// </summary>
        /// <param name="loc"></param>
        private void ZoomToLoc(double latitude, double longitude, int zoomLevel)
        {
            if(map == null) return;
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(latitude, longitude));
            builder.Zoom(zoomLevel);

            CameraPosition camPos = builder.Build();
            CameraUpdate camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);
            map.MoveCamera(camUpdate);
        }

        /// <summary>
        /// We'v successfully connected to the Google Play API, so we can get the last known location of the device
        /// </summary>
        /// <param name="connectionHint"></param>
        public void OnConnected(Bundle connectionHint)
        {
            lastLoc = LocationServices.FusedLocationApi.GetLastLocation(apiClient);

            if(lastLoc != null)
            {
                ServerData.FetchPlaces(lastLoc.Latitude.ToString(), lastLoc.Longitude.ToString(), 500, OnPlacesReturned);
            }
        }

        /// <summary>
        /// Add markers to the map fragment
        /// </summary>
        public void PopulateMap()
        {
            placed = true; // so this doesn't get called twice
            map.Clear();

            if (lastLoc != null)
            {
                // Zoom to the device's location and add a marker on the map
                ZoomToLoc(lastLoc.Latitude, lastLoc.Longitude, 15);
                MarkerOptions marker = new MarkerOptions();
                marker.SetPosition(new LatLng(lastLoc.Latitude, lastLoc.Longitude));
                marker.SetTitle("Your Location");
                marker.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(180f));
                map.AddMarker(marker);
            }

            foreach (GooglePlace place in nearby)
            {
                MarkerOptions marker = new MarkerOptions();
                marker.SetPosition(new LatLng(place.geometry.location.lat, place.geometry.location.lng));
                marker.SetTitle(place.name);
                map.AddMarker(marker);
            }

            spinner.Visibility = ViewStates.Gone;
        }

        /// <summary>
        /// Create the list using the found nearby locations
        /// </summary>
        public void PopulateList()
        {
            mainList.Adapter = new PlacesListAdapter(this, Resource.Id.placesList, nearby);
            mainList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                GooglePlace thisPlace = nearby[args.Position - 1];
                ZoomToLoc(thisPlace.geometry.location.lat, thisPlace.geometry.location.lng, 17);
            };
            mainList.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Locations returned from Google Places API call
        /// </summary>
        /// <param name="places"></param>
        public void OnPlacesReturned(GooglePlace[] places)
        {
            nearby = places;

            if(map != null && !placed)
            {
                PopulateMap();
                PopulateList();
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            // TODO
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            //TODO show alert
            throw new NotImplementedException();
        }

        protected override void OnPause()
        {
            base.OnPause();
            AppData.SaveCurrentData();
        }

        public class PlacesListAdapter : BaseAdapter<GooglePlace>
        {
            Activity context;
            GooglePlace[] places;

            public PlacesListAdapter(Activity context, int resource, GooglePlace[] data)
            {
                this.context = context;
                this.places = data;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override GooglePlace this[int position]
            {
                get { return places[position]; }
            }

            public override int Count
            {
                get { return places.Length; }
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {

                View view = convertView;

                if (view == null)
                {
                    view = context.LayoutInflater.Inflate(Resource.Layout.PlacesListItem, null);
                }

                view.FindViewById<TextView>(Resource.Id.placesList_name).Text = places[position].name;

                ImageView photoView = view.FindViewById<ImageView>(Resource.Id.placesList_photo);
                photoView.Visibility = ViewStates.Invisible;

                if (places[position].photos != null && places[position].photos.Length > 0)
                {
                    // A photo is available to show!
                    LoadImage(photoView, places[position].photos[0].photo_reference);
                }

                return view;
            }

            private async Task LoadImage(ImageView image, string photoRef)
            {
                string imageLoc = await ServerData.FetchPlacePhoto(photoRef, 120, 120);
                image.SetImageURI(Android.Net.Uri.FromFile(new Java.IO.File(imageLoc)));
                image.Visibility = ViewStates.Visible;
            }
        }
    }
}