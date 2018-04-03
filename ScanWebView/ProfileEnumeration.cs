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

namespace ScanWebView
{
    public class ProfileEnumeration : Enumeration<int>
    {
        public ProfileEnumeration(){ }
        public ProfileEnumeration(int key,string displayName):base(key,displayName) { }

        public static readonly ProfileEnumeration Default = new ProfileEnumeration(0, "WebView_Default");
        public static readonly ProfileEnumeration Barcode1 = new ProfileEnumeration(1, "WebView_Barcode1");
        public static readonly ProfileEnumeration Barcode2 = new ProfileEnumeration(2, "WebView_Barcode2");
        public static readonly ProfileEnumeration Barcode3 = new ProfileEnumeration(3, "WebView_Barcode3");
        public static readonly ProfileEnumeration Barcode4 = new ProfileEnumeration(4, "WebView_Barcode4");
        public static readonly ProfileEnumeration Barcode5 = new ProfileEnumeration(5, "WebView_Barcode5");
        public static readonly ProfileEnumeration Barcode6 = new ProfileEnumeration(6, "WebView_Barcode6");
        public static readonly ProfileEnumeration Barcode7 = new ProfileEnumeration(7, "WebView_Barcode7");
        public static readonly ProfileEnumeration Barcode8 = new ProfileEnumeration(8, "WebView_Barcode8");
        public static readonly ProfileEnumeration Barcode9 = new ProfileEnumeration(9, "WebView_Barcode9");
        public static readonly ProfileEnumeration Barcode10 = new ProfileEnumeration(10, "WebView_Barcode10");
    }
}