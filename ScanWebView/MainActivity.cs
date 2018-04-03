using System;
using Android.App;
using Android.Content;
using Android.Webkit;
using Android.OS;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScanWebView.Views;
using Java.Lang;
using Android.Views;
using Android.Runtime;

namespace ScanWebView
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class SampleReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            string command = intent.GetStringExtra("COMMAND").Equals("") ? "EMPTY" : intent.GetStringExtra("COMMAND");
            string commandIdentifier = intent.GetStringExtra("COMMAND_IDENTIFIER").Equals("") ? "EMPTY" : intent.GetStringExtra("COMMAND_IDENTIFIER");
            string result = intent.GetStringExtra("RESULT").Equals("") ? "EMPTY" : intent.GetStringExtra("RESULT");

            Bundle bundle;
            string resultInfo = "";
            {
                bundle = intent.GetBundleExtra("RESULT_INFO");
                ICollection<string> keys = bundle.KeySet();
                foreach (string key in keys)
                {
                    resultInfo += key + ": " + bundle.GetString(key) + "\n";
                }
            }
            string text = "\n" + "Command:      " + command + "\n" +
                                "Result:       " + result + "\n" +
                                "Result Info:  " + resultInfo + "\n" +
                                "CID:          " + commandIdentifier;
            //Log.d("TAG",text);
        }
    }
    [Activity(Label = "ScanWebView", MainLauncher = true,ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private const string DATAWEDGE_API_ACTION_SWITCH_TO_PROFILE = "com.symbol.datawedge.api.ACTION_SWITCHTOPROFILE";
        private const string DATAWEDGE_API_EXTRA_PROFILENAME = "com.symbol.datawedge.api.EXTRA_PROFILENAME";
        private const string DATAWEDGE_API_ACTION_DEFAULT_PROFILE = "com.symbol.datawedge.api.ACTION_SETDEFAULTPROFILE";
        private const string DATAWEDGE_SET_CONFIG = "com.symbol.datawedge.api.SET_CONFIG";

        private Dictionary<string, Action<object>> inputCommandsCollection;

        public delegate void OnSentInputCommandHandler(string url);

        public delegate void OnInitDefaultProfileHandler();

        private SampleReceiver sampleReceiver;

        public ConfigFile config { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            config = ConfigFile.InitializeConfig();

            base.OnCreate(bundle);

            //Intent i = new Intent();
            //i.SetAction(DATAWEDGE_API_ACTION);
            //i.PutExtra("com.symbol.datawedge.api.ENABLE_DATAWEDGE", true);
            //this.SendBroadcast(i);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.SaveFormData = false;
  
            // Use subclassed WebViewClient to intercept hybrid native calls

            HybridWebViewClient hybridWebViewClient = new HybridWebViewClient();
            hybridWebViewClient.initDefaultProfileHandler += new OnInitDefaultProfileHandler(InitDefaultProfile);
            hybridWebViewClient.sentInputCommandHandler += new OnSentInputCommandHandler(SendInputCommand);

            webView.SetWebViewClient(hybridWebViewClient);

            inputCommandsCollection = new Dictionary<string, Action<object>>();

            inputCommandsCollection.Add("multibarcode", (param) => callBarcodeScan(param));
            var template = new Test();
            var page = template.GenerateString();
            // Load the rendered HTML into the view with a base URL 
            // that points to the root of the bundled Assets folder
            //webView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);
            //webView.LoadDataWithBaseURL(Android.Net.Uri.Parse(@"/sdcard/www/index.html").ToString(), page, "text/html", "UTF-8", null);
            //webView.LoadUrl("file:///android_asset/Test.html");
            webView.LoadUrl(config.Url);
            webView.KeyPress += WebView_KeyPress;
            //webView.LoadUrl("file:///android_asset/PaginaBarCode.htm");
            //webView.LoadUrl("file:///storage/sdcard0/www/index.html");
            //registerReceivers();

            IntentFilter filter = new IntentFilter();
            filter.AddAction("com.symbol.datawedge.api.RESULT_ACTION");
            filter.AddCategory("android.intent.category.DEFAULT");

            sampleReceiver = new SampleReceiver();
            RegisterReceiver(sampleReceiver, filter);
        }

        private void WebView_KeyPress(object sender, View.KeyEventArgs e)
        {
            WebView webView = sender as WebView;
            if (e.Event.Action == KeyEventActions.Down )
            {
                switch (e.KeyCode)
                {
                    case Keycode.Back: //KeyEvent.KEYCODE_BACK:
                        if (webView.CanGoBack())
                        {
                            webView.GoBack();
                        }
                        else
                        {
                            Finish();
                        }
                        break;
                }
            }
        }
       #region private method
        private void callBarcodeScan(object param)
        {
            var parameters = System.Web.HttpUtility.ParseQueryString(param.ToString());

            int num = 0;
            if (parameters["num"] == null)
                return;
            num = int.Parse(parameters["num"]);
            Intent intent = new Intent();
            intent.SetAction(DATAWEDGE_API_ACTION_SWITCH_TO_PROFILE);
            intent.PutExtra(DATAWEDGE_API_EXTRA_PROFILENAME, ProfileEnumeration.FromKey<ProfileEnumeration>(num).Value);
            SendBroadcast(intent);

            Intent i = new Intent();
            i.SetAction("com.symbol.datawedge.api.ACTION");
            i.PutExtra("com.symbol.datawedge.api.GET_ACTIVE_PROFILE", "");
            //send intent


            //Bundle bundleApp1 = new Bundle();
            //bundleApp1.PutString("PACKAGE_NAME", "it.xerum.ScanWebView");
            //bundleApp1.PutStringArray("ACTIVITY_LIST", new String[] { "*" });

            //Bundle mainBundle = new Bundle();
            //mainBundle.PutString("PROFILE_NAME", ProfileEnumeration.FromKey<ProfileEnumeration>(num).Value + "_Test");
            //mainBundle.PutString("PROFILE_ENABLED", "true");
            //mainBundle.PutString("CONFIG_MODE", "CREATE_IF_NOT_EXIST");
            //mainBundle.PutParcelable("APP_LIST", bundleApp1);

            //Intent configIntent = new Intent();
            //configIntent.SetAction(DATAWEDGE_API_ACTION_SWITCH_TO_PROFILE);
            //configIntent.PutExtra("com.symbol.datawedge.api.SET_CONFIG", mainBundle);
            //SendBroadcast(configIntent);

            //var scanIntent = new Intent();
            //scanIntent.SetAction("com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER");
            //scanIntent.PutExtra("com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER", "TOGGLE_SCANNING");
            //SendBroadcast(scanIntent);

        }
        #endregion
        protected override void OnNewIntent(Intent i)

        {

            handleDecodeData(i);
            string profile = i.Extras.GetBundle("com.symbol.datawedge.api.RESULT_GET_ACTIVE_PROFILE").ToString();
        }

        private void handleDecodeData(Intent i)
        {
            
        }

        public void InitDefaultProfile()
        {
            Intent intent = new Intent();
            intent.SetAction(DATAWEDGE_API_ACTION_SWITCH_TO_PROFILE);
            intent.PutExtra(DATAWEDGE_API_EXTRA_PROFILENAME, ProfileEnumeration.Default.Value);
            SendBroadcast(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
        public void SendInputCommand(string urlstring)
        {
            var resources = urlstring.Split('?');
            var method = resources[0];
            var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);
            if (inputCommandsCollection.ContainsKey(method))
            {
                Task task = new Task(inputCommandsCollection.GetValueOrDefault(method), resources[1]);
                task.Start();
            }
        }


        


        private class HybridWebViewClient : WebViewClient
        {
            public OnInitDefaultProfileHandler initDefaultProfileHandler;
            public OnSentInputCommandHandler sentInputCommandHandler;

            public override bool ShouldOverrideUrlLoading(WebView webView, string url)
            {

                // If the URL is not our own custom scheme, just let the webView load the URL as usual
                var scheme = "xerum:";

                if (!url.StartsWith(scheme))
                {
                    this.initDefaultProfileHandler();
                    return false;
                }
                // This handler will treat everything between the protocol and "?"
                // as the method name.  The querystring has all of the parameters.
                sentInputCommandHandler(url.Substring(scheme.Length));

                return true;
            }
        }
    }
}

