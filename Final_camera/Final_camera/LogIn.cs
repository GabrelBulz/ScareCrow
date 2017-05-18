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

namespace Final_camera
{
    [Activity(Label = "ScareCrow", MainLauncher = true, Icon = "@drawable/xs")]
    class LogIn : Activity
    {
        private Button SignIn;
        private EditText UserName;
        private EditText Password;
        private TextView SignUp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LogIn);

            //Hide Keyboard
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);

            SignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            UserName = FindViewById<EditText>(Resource.Id.editUserName);
            Password = FindViewById<EditText>(Resource.Id.editPassword);
            SignUp = FindViewById<TextView>(Resource.Id.txtSignUp);

            //Click on Sign In
            SignIn.Click += SignIn_Click;

            //Click on Sign Up
            SignUp.Click += SignUp_Click;
        }

        private void SignUp_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SignUp));
            StartActivity(intent);
            //TO DECIDE ---------> With or without Finish?
            //Finish();
        }

        private void SignIn_Click(object sender, EventArgs e)
        {
            if (UserName.Text.ToString().Length == 0)
            {
                Toast.MakeText(this, "Invalid User Name", ToastLength.Short).Show();
            }
            else if (Password.Text.ToString().Length == 0)
            {
                Toast.MakeText(this, "Invalid Password", ToastLength.Short).Show();
            }
            else
            {
                //! Check is strings are Valid!
                UserDataBase database = new UserDataBase(this);
                bool valid = database.Validity(UserName.Text.ToString().Trim(), Password.Text.ToString().Trim());
                if (valid)
                {
                    Intent intent = new Intent(this, typeof(CameraSurface)).PutExtra("StringName", UserName.Text.ToString());
                    StartActivity(intent);
                    //TO DECIDE ---------> With or without Finish?
                    //Finish();

                }
                else
                {
                    Toast.MakeText(this, "Wrong User Name or Password", ToastLength.Short).Show();
                }

            }
        }
    }

}