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

using Java.IO;

namespace Final_camera
{
   public  class the_image
    {
        public File gdirectory { get; set; }
        public File temp_storage { get; set; }
        public string path_file { get; set; }
        public Android.Graphics.Bitmap bitma_pic { get; set; }
        public string absolut_path_pic { get; set; }

    }
}