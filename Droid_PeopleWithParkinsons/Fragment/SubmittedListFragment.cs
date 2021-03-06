using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SpeechingShared;
using System;
using System.Threading.Tasks;

namespace DroidSpeeching
{
    public class SubmittedListFragment : Android.Support.V4.App.Fragment
    {
        private ListView exportList;
        private IResultItem[] results;
        private IResultItem res;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.SubmittedFragment, container, false);

            View header = Activity.LayoutInflater.Inflate(Resource.Layout.SubmittedHeader, null);

            exportList = view.FindViewById<ListView>(Resource.Id.submitted_list);
            exportList.AddHeaderView(header, null, false);
            LoadData();
            exportList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                // The list's header borks indexing
                res = results[args.Position - 1];

                View alertView = Activity.LayoutInflater.Inflate(Resource.Layout.SubmittedAlert, null);

                Button feedbackBtn = alertView.FindViewById<Button>(Resource.Id.submittedAlert_feedbackBtn);
                feedbackBtn.Click += feedbackBtn_Click;

                Button permissionsBtn = alertView.FindViewById<Button>(Resource.Id.submittedAlert_permission);
                permissionsBtn.Click += permissionsBtn_Click;

                Android.Support.V7.App.AlertDialog alert = new Android.Support.V7.App.AlertDialog.Builder(Activity)
                .SetTitle("What would you like to do with this submission?")
                .SetView(alertView)
                .SetCancelable(true)
                .SetNegativeButton("Delete", (EventHandler<DialogClickEventArgs>)null)
                .SetNeutralButton("Close", (s, a) => { })
                .Create();

                alert.Show();

                // A second alert dialogue, confirming the decision to delete
                Button deleteBtn = alert.GetButton((int)DialogButtonType.Negative);
                deleteBtn.Click += delegate(object s, EventArgs e)
                {
                    Android.Support.V7.App.AlertDialog.Builder confirm = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                    confirm.SetTitle("Are you sure?");
                    confirm.SetMessage("The recorded data will be deleted from the server and irrecoverably lost. Continue?");
                    confirm.SetPositiveButton("Delete", (senderAlert, confArgs) =>
                    {
                        ServerData.PushResultDeletion(res);

                        exportList.Adapter = null;
                        LoadData();

                        alert.Dismiss();
                    });
                    confirm.SetNegativeButton("Cancel", (senderAlert, confArgs) => { });
                    confirm.Show();
                };     
            };

            return view;
        }

        private async void LoadData()
        {
            results = null;//await ServerData.FetchSubmittedList();

            if(results != null)
                exportList.Adapter = new ExportedListAdapter(Activity, Resource.Id.submitted_list, results);
        }

        void permissionsBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Activity, typeof(UploadsActivity)); //TODO
            intent.PutExtra("ResultId", res.Id);
            StartActivity(intent);
        }

        void feedbackBtn_Click(object sender, EventArgs e)
        {
            Toast.MakeText(Activity, "Feedback", ToastLength.Short).Show();
        }
    }
}