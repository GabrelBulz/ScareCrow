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

    [Activity(Label = "ScareCrow", MainLauncher = false, Icon = "@drawable/xs")]
    class SignUp : Activity
    {

        private EditText UserName;
        private EditText Password;
        private EditText Email;
        private Button Sign_Up;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SignUp);
            

            //Hide Keyboard
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);

            UserName = FindViewById<EditText>(Resource.Id.editUserName);
            Password = FindViewById<EditText>(Resource.Id.editPassword);
            Email = FindViewById<EditText>(Resource.Id.editEmail);
            Sign_Up = FindViewById<Button>(Resource.Id.btnSignUp);

            //Click on Sign Up
            Sign_Up.Click += Sign_Up_Click;

        }

        private void Sign_Up_Click(object sender, EventArgs e)
        {
            UserDataBase database = new UserDataBase(this);//Used to check if the username and/or email is already in the DB
            if (UserName.Text.ToString().Length == 0)
            {
                Toast.MakeText(this, "Please enter a valid User Name", ToastLength.Short).Show();
            }
            else if (Password.Text.ToString().Length == 0)
            {
                Toast.MakeText(this, "Please enter a valid Password", ToastLength.Short).Show();
            }
            else if (Email.Text.ToString().Length == 0 || !Android.Util.Patterns.EmailAddress.Matcher(Email.Text.ToString()).Matches())
            {
                Toast.MakeText(this, "Please enter a valid Email", ToastLength.Short).Show();
            }
            else if (database.CheckExistingUserNameOrEmail(UserName.Text.ToString(), Email.Text.ToString()))
            {
                Toast.MakeText(this, "This User Name or Email is already registered", ToastLength.Short).Show();
            }
            else
            {

                User NewUser = new User();
                NewUser.UserName = UserName.Text.ToString();
                NewUser.Password = Password.Text.ToString();
                NewUser.Email = Email.Text.ToString();

                database.AddNewUser(NewUser);

                Intent intent = new Intent(this, typeof(LogIn));
                StartActivity(intent);
                //TO DECIDE ---------> With or without Finish?
                //Finish();

            }
        }
    }

}