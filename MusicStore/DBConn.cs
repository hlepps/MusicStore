﻿using System;
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
        private MySqlConnection conn;
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
        
        public bool Login(string username, string password)
        {
            string passhash = Security.DoubleHash(password);

            if (Security.IsAlphanumeric(username) && Security.IsAlphanumeric(password))
            {
                string sql = $"SELECT username, passhash, permission, wallet, library FROM users WHERE username='{username}' AND passhash='{passhash}'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    //System.Windows.MessageBox.Show($"user: {rdr[0]}, permission: {rdr[2]}, wallet: {rdr[3]}zł, library: {rdr[4]}", "Login", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    currentUser = new DB.DBUser();
                    currentUser.username = username;
                    currentUser.wallet = (double)rdr[3];
                    currentUser.permission = (int)rdr[2];
                    ReloadUserLibrary();
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

        public void ReloadUserLibrary()
        {
            DBConn.instance.currentUser.library.itemlist.Clear();
            string sql = $"SELECT library FROM users WHERE username='{DBConn.instance.currentUser.username}'";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            MySqlDataReader rdr = cmd.ExecuteReader();
            if(rdr.HasRows)
            {
                rdr.Read();
                string[] entries = rdr[0].ToString().Split(',');
                foreach(string entry in entries)
                {
                    char type = entry[0];
                    int id = int.Parse(entry.Substring(1));

                    switch(type)
                    {
                        case 's':
                            MySqlCommand encmd = new MySqlCommand($"SELECT songname, image_id FROM songs where id={id}", conn);
                            MySqlDataReader enrdr = encmd.ExecuteReader();
                            if(rdr.HasRows)
                            {
                                rdr.Read();
                            }
                            break;

                        case 'a':

                            break;
                    }
                }
            }
            conn.Close();
            
        }
    }
}
