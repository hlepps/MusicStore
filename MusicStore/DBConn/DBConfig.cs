using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace MusicStore.DB
{
    public class DBConfig
    {
        public static string studioName;
        public static DBImage studioLogo;
        public static DBImage studioBanner;
        

        public static void UploadConfig(string sname, DBImage slogo, DBImage sbanner)
        {
            MySqlCommand cmd = new MySqlCommand($"UPDATE config SET studio_name=@sn,logo=@l,banner=@b WHERE id=1", DBConn.instance.conn);
            cmd.Parameters.AddWithValue("@sn", sname);
            cmd.Parameters.AddWithValue("@l", DB.DBImage.GetBytesFromBitmap(slogo.bitmap));
            cmd.Parameters.AddWithValue("@b", DB.DBImage.GetBytesFromBitmap(sbanner.bitmap));
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();
            DBConn.instance.conn.Close();

            RefreshConfig();
        }

        public static void RefreshConfig()
        {
            MySqlCommand cmd = new MySqlCommand($"SELECT studio_name, logo, banner FROM config WHERE id=1", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();

            studioName = (string)rdr[0];
            studioLogo = new DBImage(DBImage.GetBitmapFromBytes((byte[])rdr[1]));
            studioBanner = new DBImage(DBImage.GetBitmapFromBytes((byte[])rdr[2]));

            DBConn.instance.conn.Close();
        }
    }
}
