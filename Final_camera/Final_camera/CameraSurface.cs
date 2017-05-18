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


using Android.Hardware;

using System.Collections;
using System.Drawing;
using Android.Util;
using static Android.Hardware.Camera;
//using Android.Graphics; // cu asta inclus da ambiguous la camera cu android.hardware
using Android.Nfc;
using Java.IO;
using Newtonsoft.Json;
using Android.Media;
using System.IO;

namespace Final_camera
{
    [Activity(Label = "ScareCrow", MainLauncher = false, Icon = "@drawable/icon",
       Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class CameraSurface : Activity, IPictureCallback

    {
        private Button gtake_pic;
        private Button gswitch_cam;
        public Camera gCam;
        private int gNr_of_cam;
        private int gDef_front_cam;
        private int gcur_cam;
        private SurfaceView gsurf_view;
        private Camera.Parameters gparam;
        Preview gprew_cam;

        ///these can be erased if u manage to use the_image class
        private Java.IO.File gdirectory;
        private Java.IO.File temp_storage;
        private string path_file;
        private Android.Graphics.Bitmap bitma_pic;
        ///until here


        //this class is used to store the path, file, and bitmap of a picture, which need to be sended to photo view to display the proper image
        private the_image img_container;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.CameraSurface);

            FindInstance();
            ///#############################  switch cam_function to be implemented from this prototype
            gswitch_cam.Click += delegate
            {
                if (gNr_of_cam == 1) ///am doar o camera
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetMessage("Alert")
                        .SetNeutralButton("Close", (Android.Content.IDialogInterfaceOnClickListener)null);
                    AlertDialog alert = builder.Create();
                    alert.Show();
                }
                else
                {
                    if (gCam != null)
                    {
                        gCam.StopPreview();
                        gprew_cam.PrewCam = null;
                        gCam.Release();
                        gCam = null;
                    }

                    gCam = Camera.Open((gcur_cam + 1) % gNr_of_cam);
                    gcur_cam = (gcur_cam + 1) % gNr_of_cam;

                    gprew_cam.Switch_cam(gCam);

                    gCam.StartPreview();
                }
            };/// ###################   end switch cam

            gtake_pic.Click += delegate
            {
                img_container = new the_image();

                create_folder();

                gCam.TakePicture(null, null, this);



                //Intent intent = new Intent(this, typeof(PhotoView));
                //intent.PutExtra("img_pack", JsonConvert.SerializeObject(img_container.bitma_pic));

                //StartActivity(intent);

                //Finish();
            };

            gNr_of_cam = Camera.NumberOfCameras;   ///daca nr de cam =1 atunci va fi nevoie sa dam disable la butonu de switch cam, care trebe si ala facut

            CameraInfo cam_info = new Camera.CameraInfo();

            for (int i = 0; i < gNr_of_cam; i++)
            {
                Camera.GetCameraInfo(i, cam_info);
                if (cam_info.Facing == CameraFacing.Front)
                    gDef_front_cam = i;
            }

            gCam = Camera.Open(gDef_front_cam);

            //if(gCam == null)
            //{
            //    gtake_pic.Text = "NULL";
            //} // am verificat daca am camera, sigur am

            gcur_cam = gDef_front_cam;

            gparam = gCam.GetParameters();
            gCam.SetParameters(gparam);

            gprew_cam = new Preview(this, gsurf_view);


            gCam.StartPreview();


        }


        protected override void OnResume()
        {
            base.OnResume();

            if (gCam == null)
            {
                gCam = Camera.Open(gDef_front_cam);
                gcur_cam = gDef_front_cam;
                gprew_cam.PrewCam = gCam;
            }
            else
            {
                gCam.Release();

                gCam = Camera.Open(gDef_front_cam);
                gcur_cam = gDef_front_cam;
                gprew_cam.PrewCam = gCam;
            }

            gparam = gCam.GetParameters();
            gCam.SetParameters(gparam);

            gCam.StartPreview();
            ///preview-ul de gatat
        }


        protected override void OnPause()
        {
            base.OnPause();

            if (gCam != null)
            {
                gCam.Release();
                gprew_cam.PrewCam = null;
                gCam = null;

                gcur_cam = -1;
            }
        }


        private void FindInstance()
        {
            gtake_pic = FindViewById<Button>(Resource.Id.take_pic);
            gsurf_view = FindViewById<SurfaceView>(Resource.Id.surfaceView1);
            gswitch_cam = FindViewById<Button>(Resource.Id.switch_cam);
            ///sufr takeIdBy
        }

        protected void create_folder()
        {
            img_container.gdirectory = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "CameraTest_2");

            if (!img_container.gdirectory.Exists())
                img_container.gdirectory.Mkdir();

            img_container.path_file = img_container.gdirectory.AbsolutePath;

        }

        public async void OnPictureTaken(byte[] data, Camera camera)
        {
            img_container.bitma_pic = Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length);


            string cur_time_milisec = System.DateTime.Now.Millisecond.ToString();
            string cur_time_sec = System.DateTime.Now.Second.ToString();

            img_container.temp_storage = new Java.IO.File(img_container.path_file, "image" + cur_time_milisec + cur_time_sec + ".jpg");
            if (!img_container.temp_storage.Exists())
                img_container.temp_storage.CreateNewFile();

            img_container.absolut_path_pic = img_container.temp_storage.AbsolutePath;

            Android.Graphics.Matrix rotation_matrix = new Android.Graphics.Matrix();


            //caz ii camera spate
            if (gcur_cam != gDef_front_cam)
                rotation_matrix.PreRotate(90);
            else //camera frontala
                rotation_matrix.PreRotate(270);


            img_container.bitma_pic = Android.Graphics.Bitmap.CreateBitmap(img_container.bitma_pic, 0, 0, img_container.bitma_pic.Width, img_container.bitma_pic.Height, rotation_matrix, true);


            MemoryStream stream = new MemoryStream();
            Android.Graphics.Bitmap bmp = img_container.bitma_pic;
            bmp.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
            byte[] byte_array = stream.ToArray();

       

            FileOutputStream outstream = new FileOutputStream(img_container.temp_storage);

            outstream.Write(byte_array);
            outstream.Flush();
            outstream.Close();

        }



    }



    /// ############################# CLASS PREVIEW

    class Preview : ViewGroup, ISurfaceHolderCallback
    {

        SurfaceView gSurface_view;
        ISurfaceHolder gHolder;
        bool surf_destroyed = true;
        Camera cam_prew;


        public Camera PrewCam
        {
            get { return cam_prew; }
            set
            {
                cam_prew = value;
                if (cam_prew != null)
                { RequestLayout(); }
            }
        }

        public Preview(Context context, SurfaceView srf_view) : base(context)
        {
            gSurface_view = srf_view;

            gHolder = gSurface_view.Holder;
            gHolder.AddCallback(this);
            gHolder.SetType(SurfaceType.PushBuffers);

        }


        public void Switch_cam(Camera camm)
        {
            PrewCam = camm;

            camm.SetPreviewDisplay(gHolder);
            PrewCam.SetDisplayOrientation(90);

            Camera.Parameters param = camm.GetParameters();
            RequestLayout();

            camm.SetParameters(param);
        }


        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            throw new NotImplementedException();
        }


        public void SurfaceCreated(ISurfaceHolder holder)
        {
            surf_destroyed = false;

            try
            {
                if (PrewCam != null)
                {
                    PrewCam.SetPreviewDisplay(holder);
                    PrewCam.SetDisplayOrientation(90);
                }
            }
            catch (Java.IO.IOException exception)
            {
                //null
            }

        }


        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            surf_destroyed = true;

            if (PrewCam != null)
            {
                PrewCam.StopPreview();
            }

        }

        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int x, int y)
        {
            Camera.Parameters parameters = PrewCam.GetParameters();

            RequestLayout();

            PrewCam.StartPreview();
        }

    }
}