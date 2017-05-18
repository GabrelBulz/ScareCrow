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
using Android.Database.Sqlite;
using Android.Database;

namespace Final_camera
{
    public class UserDataBase : SQLiteOpenHelper
    {
        private const string APP_DATABASENAME = "UserDataBase.db";
        private const int APP_DATABASE_VERSION = 1;

        public UserDataBase(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(@"CREATE TABLE IF NOT EXISTS UserDataBase(
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            UserName TEXT NOT NULL,
                            Password  TEXT NOT NULL,
                            Email   TEXT NULL)");

            db.ExecSQL("Insert into UserDataBase(UserName,Password,Email)VALUES('Admin','Admin123','sharad.pyakurel@gmail.com')");
            db.ExecSQL("Insert into UserDataBase(UserName,Password,Email)VALUES('Fugi','123','sharad.pyakurel@gmail.com')");

        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL("DROP TABLE IF EXISTS UserDataBase");
            OnCreate(db);
        }

        //! Login validity checker

        public bool Validity(string username, string password)
        {
            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query("UserDataBase", new string[] { "UserName", "Password", "Email" }, "UserName =? and Password =?", new string[] { username, password }, null, null, null, null);

            bool validity = new bool();

            while (c.MoveToNext())
            {
                if (string.Compare(c.GetString(0), username) == 0 && string.Compare(c.GetString(1), password) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            c.Close();
            db.Close();

            return false;

        }

        //TO DO -------> Split UserName and Email!

        public bool CheckExistingUserNameOrEmail(string username, string email)
        {
            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query("UserDataBase", new string[] { "UserName", "Password", "Email" }, "UserName =? or Email =?", new string[] { username, email }, null, null, null, null);

            while (c.MoveToNext())
            {
                if (string.Compare(c.GetString(0), username) == 0 || string.Compare(c.GetString(2), email) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            c.Close();
            db.Close();

            return false;
        }


        //Add New User!!!!!
        public void AddNewUser(User NewUser)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("UserName", NewUser.UserName);
            vals.Put("Password", NewUser.Password);
            vals.Put("Email", NewUser.Email);
            db.Insert("UserDataBase", null, vals);
        }
    }

}