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
using Android.Content.PM;

namespace Final_camera
{
    [Activity(Label = "ScareCoew", ScreenOrientation = ScreenOrientation.Portrait)]
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

        //ListView
        private List<string> FilterList;
        private ListView nListView;
        private ListViewAdapter nAdapter;
        private LinearLayout nContainer;


        // temp bit for filters
        Android.Graphics.Bitmap temp_bit;



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

            //Hide Container
            nContainer.Enabled = false;
            nContainer.Visibility = ViewStates.Invisible;
            bool nContainerUp = false;

            //Populate FitlerList
            FilterList = new List<string>();
            FilterList.Add("@drawable/gray");
            FilterList.Add("@drawable/sepia");
            FilterList.Add("@drawable/blue");
            FilterList.Add("@drawable/red");
            FilterList.Add("@drawable/green");
            FilterList.Add("@drawable/negative");
            FilterList.Add("@drawable/cancel");

            //Set the Custom Adapter and then the adapter to the ListView
            nAdapter = new ListViewAdapter(this, FilterList);
            nListView.Adapter = nAdapter;
            nListView.VerticalScrollBarEnabled = false;
            

           

            //Click on a ListView Item
            nListView.ItemClick += NListView_ItemClick;

            //Initial Bitmap
            bitmap_img = Android.Graphics.BitmapFactory.DecodeFile(absolut_path_pic);
            //TEMP bitmap

            //added , try to solve bug_________
            temp_bit = bitmap_img;
            gimg_view.SetImageBitmap(bitmap_img);
            //end here _________


            gimg_view.SetImageBitmap(bitmap_img);

            //for throw button
            delete_pic_if_needed_throw(bitmap_img);
            //for save but
            //save_picture(bitmap_img);

           


            save_picture(temp_bit);

            gEdit.Click += delegate
            {
                if(nContainerUp == false)
                {
                    nContainer.Enabled = true;
                    nContainer.Visibility = ViewStates.Visible;
                    nContainerUp = true;
                }
                else
                {
                    nContainer.Enabled = false;
                    nContainer.Visibility = ViewStates.Invisible;
                    nContainerUp = false;
                }
            };

        }

        private void NListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if(nAdapter[e.Position] == "@drawable/")
            {
                gimg_view.SetImageResource(this.Resources.GetIdentifier(nAdapter[e.Position], "drawable", this.PackageName));
            }

            switch (nAdapter[e.Position])
            {
                case "@drawable/gray":
                    {
                        temp_bit = toGrayscale(bitmap_img);
                        gimg_view.SetImageBitmap(temp_bit);
                        break;
                    }
                case "@drawable/sepia":
                    {
                        temp_bit = createSepia_and_RBG(bitmap_img,1);
                        gimg_view.SetImageBitmap(temp_bit);
                        break;
                    }
                case "@drawable/blue":
                    {
                        temp_bit = createSepia_and_RBG(bitmap_img, 2);
                        gimg_view.SetImageBitmap(temp_bit);
                        break;
                    }
                case "@drawable/red":
                    {
                        temp_bit = createSepia_and_RBG(bitmap_img, 4);
                        gimg_view.SetImageBitmap(temp_bit);
                        break;
                    }
                case "@drawable/green":
                    {
                        temp_bit = createSepia_and_RBG(bitmap_img, 3);
                        gimg_view.SetImageBitmap(temp_bit);
                        break;
                    }
                case "@drawable/negative":
                    {
                        temp_bit = createInverse(bitmap_img);
                        gimg_view.SetImageBitmap(temp_bit);
                        break;
                    }
                case "@drawable/cancel":
                    {
                        temp_bit = bitmap_img;
                        gimg_view.SetImageBitmap(bitmap_img);
                        break;
                    }
            }
        }

        private void FindID(Context context)
        {
            gKeep = FindViewById<Button>(Resource.Id.keep);
            gEdit = FindViewById<Button>(Resource.Id.edit);
            gThrow = FindViewById<Button>(Resource.Id.throww);
            gimg_view = FindViewById<ImageView>(Resource.Id.imageView1);
            nListView = FindViewById<ListView>(Resource.Id.myListView);
            nContainer = FindViewById<LinearLayout>(Resource.Id.llContainer);
        }


        private void delete_pic_if_needed_throw(Android.Graphics.Bitmap bit)
        {
            gThrow.Click += delegate
            {
                Java.IO.File fis = new Java.IO.File(absolut_path_pic);


                if (fis.Exists())
                    fis.Delete();

                Intent intent = new Intent(this, typeof(CameraSurface));

                StartActivity(intent);

                Finish();
            };
        }


        private void delete_pic()
        {
            Java.IO.File fis = new Java.IO.File(absolut_path_pic);


            if (fis.Exists())
                fis.Delete();
        }


        private void save_picture(Android.Graphics.Bitmap bit)
        {

            gKeep.Click += delegate
            {
                delete_pic();

                bit = temp_bit;

                the_proper_save(bit);

            };

        }


        void the_proper_save(Android.Graphics.Bitmap bit)
        {

            Java.IO.File fil = new Java.IO.File(path_file);

            if (!fil.Exists())
                fil.Mkdir();

            string cur_time_milisec = System.DateTime.Now.Millisecond.ToString();
            string cur_time_sec = System.DateTime.Now.Second.ToString();

            File ceva = new Java.IO.File(path_file, "image" + cur_time_milisec + cur_time_sec + ".jpg");

            if (!ceva.Exists())
                ceva.CreateNewFile();


            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            bit.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
            byte[] byte_array = stream.ToArray();

            if (ceva != null)
            {
                FileOutputStream outstream = new FileOutputStream(ceva);


                outstream.Write(byte_array);
                outstream.Flush();
                outstream.Close();

            }

            Intent intent = new Intent(this, typeof(CameraSurface));

            StartActivity(intent);

            Finish();

        }


        //Gray Filter
        public Android.Graphics.Bitmap toGrayscale(Android.Graphics.Bitmap bmpOriginal)
        {
            int width, height;
            height = bmpOriginal.Height;
            width = bmpOriginal.Width;

            Android.Graphics.Bitmap bmpGrayscale = Android.Graphics.Bitmap.CreateBitmap(width, height, Android.Graphics.Bitmap.Config.Argb8888);
            Android.Graphics.Canvas c = new Android.Graphics.Canvas(bmpGrayscale);
            Android.Graphics.Paint paint = new Android.Graphics.Paint();
            Android.Graphics.ColorMatrix cm = new Android.Graphics.ColorMatrix();
            cm.SetSaturation(0);
            Android.Graphics.ColorMatrixColorFilter f = new Android.Graphics.ColorMatrixColorFilter(cm);
            paint.SetColorFilter(f);
            c.DrawBitmap(bmpOriginal, 0, 0, paint);
            return bmpGrayscale;
        }


        //sepia si prietenii 
        private Android.Graphics.Bitmap createSepia_and_RBG(Android.Graphics.Bitmap src, int choise)
        {
            Android.Graphics.ColorMatrix colorMatrix_Sepia = new Android.Graphics.ColorMatrix();
            colorMatrix_Sepia.SetSaturation(0);

            Android.Graphics.ColorMatrix colorScale = new Android.Graphics.ColorMatrix();


            if (choise == 1)
                colorScale.SetScale(1, 1f, 0.7f, 1); //sepia
            else
                if (choise == 2)
                colorScale.SetScale(1, 1f, 2f, 1); //albastru
            else
                if (choise == 3)
                colorScale.SetScale(1, 2f, 1f, 1); //verde
            else
                if (choise == 4)
                colorScale.SetScale(2f, 1, 1f, 1); //rosu


            //0.7 sepia ch =1
            //0.4 ceva verziu
            //2 albastru fain
            //5 albastru neon

            colorMatrix_Sepia.PostConcat(colorScale);

            Android.Graphics.ColorFilter ColorFilter_Sepia = new Android.Graphics.ColorMatrixColorFilter(
              colorMatrix_Sepia);

            Android.Graphics.Bitmap bitmap = Android.Graphics.Bitmap.CreateBitmap(src.Width, src.Height,
              Android.Graphics.Bitmap.Config.Argb8888);
            Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(bitmap);

            Android.Graphics.Paint paint = new Android.Graphics.Paint();

            paint.SetColorFilter(ColorFilter_Sepia);
            canvas.DrawBitmap(src, 0, 0, paint);

            return bitmap;
        }


        //Negativ
        private Android.Graphics.Bitmap createInverse(Android.Graphics.Bitmap src)
        {
            float[] colorMatrix_Negative = {
                                    -1.0f, 0, 0, 0, 255, //red
                                        0, -1.0f, 0, 0, 255, //green
                                        0, 0, -1.0f, 0, 255, //blue
                                        0, 0, 0, 1.0f, 0 //alpha  
                                      };

            Android.Graphics.Paint MyPaint_Normal = new Android.Graphics.Paint();
            Android.Graphics.Paint MyPaint_Negative = new Android.Graphics.Paint();
            Android.Graphics.ColorFilter colorFilter_Negative = new Android.Graphics.ColorMatrixColorFilter(colorMatrix_Negative);
            MyPaint_Negative.SetColorFilter(colorFilter_Negative);

            Android.Graphics.Bitmap bitmap = Android.Graphics.Bitmap.CreateBitmap(src.Width, src.Height,
              Android.Graphics.Bitmap.Config.Argb8888);
            Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(bitmap);

            MyPaint_Negative.SetColorFilter(colorFilter_Negative);
            canvas.DrawBitmap(src, 0, 0, MyPaint_Negative);

            return bitmap;

            return bitmap;
        }
    }

}
