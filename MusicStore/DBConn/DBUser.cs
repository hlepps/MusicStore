using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.DB
{
    public class DBUser
    {
        public string username;
        public DBLibrary library;
        public double wallet;
        public int permission;
        public DBImage avatar;

        public static DBUser GetUserInfo(string username)
        {
            DBUser temp = new DBUser();
            temp.username = "~";
            string sql = $"SELECT username, passhash, permission, wallet, library, avatar_id FROM users WHERE username='{username}'";

            MySqlCommand cmd = new MySqlCommand(sql, DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                object[] a = { rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5] };
                //System.Windows.MessageBox.Show($"user: {rdr[0]}, permission: {rdr[2]}, wallet: {rdr[3]}zł, library: {rdr[4]}", "Login", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                temp.username = username;
                temp.wallet = (double)a[3];
                temp.permission = (int)a[2];
                temp.library = new DB.DBLibrary();
                temp.avatar = DB.DBImagesSaved.Get((int)a[5]);

            }

            if (temp.username == "~")
                return null;

            return temp;
        }

        public void ChangeAvatar(int imageID)
        {
            string sql = $"UPDATE users SET avatar_id = '{imageID}' WHERE username = '{username}'";

            MySqlCommand cmd = new MySqlCommand(sql, DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();
            avatar = DBImagesSaved.Get(imageID);
        }

    }
}
