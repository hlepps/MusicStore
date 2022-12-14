using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace MusicStore
{
    class DBConn
    {
        public static DBConn instance;
        public MySqlConnection conn;
        public DB.DBUser currentUser;
        public DBConn() // konstruktor połączenia, wywoływany raz
        {
            if (instance == null)
                instance = this;
            else
                return;

            string connStr = "server=192.166.219.220;user=vinyl;database=vinyl;port=3306;password=vinyl1212";
            conn = new MySqlConnection(connStr);
            try
            {
                Trace.WriteLine("Connecting to MySQL...");
                conn.Open();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            conn.Close();
        }

        ~DBConn()
        {
            conn.Close();
        }

        public void PrepareConnection()
        {
            conn.Close();
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
        }
        
        public bool Login(string username, string password)
        {
            string passhash = Security.DoubleHash(password);

            if (Security.IsAlphanumeric(username) && Security.IsAlphanumeric(password))
            {
                string sql = $"SELECT username, passhash, permission, wallet, library, avatar_id FROM users WHERE username='{username}' AND passhash='{passhash}'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    object[] a = { rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5] };
                    //System.Windows.MessageBox.Show($"user: {rdr[0]}, permission: {rdr[2]}, wallet: {rdr[3]}zł, library: {rdr[4]}", "Login", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    currentUser = new DB.DBUser();
                    currentUser.username = username;
                    currentUser.wallet = (double)a[3];
                    currentUser.permission = (int)a[2];
                    currentUser.library = new DB.DBLibrary();
                    currentUser.library.ReloadLibrary();
                    currentUser.avatar = DB.DBImagesSaved.Get((int)a[5]);
                    return true;

                }
                else
                {
                    System.Windows.MessageBox.Show("Wrong username or password", "Login", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                conn.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Use only letters, numbers and '_'", "Login", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

            }
            return false;
        }

        
    }
}
