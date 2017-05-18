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
using Newtonsoft.Json;
using Java.IO;

namespace Final_camera
{
    [Activity(Label = "ScareCoew")]
    public class PhotoView : Activity
    {
        Button gKeep;
        Button gEdit;
        Button gThrow;
        ImageView gimg_view;
        private Android.Graphics.Bitmap bitmap_img;

        //Data From CameraSurface Activity
        public File gdirectory;
        public File temp_storage;
        public string path_file;
        public Android.Graphics.Bitmap bitma_pic;
        public string absolut_path_pic;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PhotoView);

            //Set Data from CameraSurface Acivity in this Activity
            gdirectory = (File)Intent.GetStringExtra("ImageDirectory");
            temp_storage = (File)Intent.GetStringExtra("Temp_Storage");
            path_file = (string)Intent.GetStringExtra("PathFile");
            absolut_path_pic = (string)Intent.GetStringExtra("AbsolutePath");

            FindID(this);

            bitmap_img = Android.Graphics.BitmapFactory.DecodeFile(absolut_path_pic);

            //Android.Graphics.Color

            gimg_view.SetImageBitmap(bitmap_img);

            gThrow.Click += delegate 
            {
                Intent intent = new Intent(this, typeof(CameraSurface));

                StartActivity(intent);

                Finish();
            };

            // Create your application here
        }

        private void FindID(Context context)
        {
            gKeep = FindViewById<Button>(Resource.Id.keep);
            gEdit = FindViewById<Button>(Resource.Id.edit);
            gThrow = FindViewById<Button>(Resource.Id.throww);
            gimg_view = FindViewById<ImageView>(Resource.Id.imageView1);
        }
    }

}