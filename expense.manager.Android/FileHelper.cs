using System;
using System.IO;
using expense.manager.Data;
using expense.manager.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]

namespace expense.manager.Droid
{


    public class FileHelper: IFileHelper
    {
        public string GetConnection()
        {
            var fileName = "xpenseLocalDb.db3";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, fileName);

        }

        public string GetCsvFile()
        {
            var fileName = "Expenses_backup.csv";
            string path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            return Path.Combine(path, fileName);

        }
    }
}