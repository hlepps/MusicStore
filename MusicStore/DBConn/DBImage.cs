using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace MusicStore.DB
{
    public class DBImage
    {
        public BitmapImage bitmap;
    }

    public class DBImagesSaved
    {
        private static Dictionary<int, DBImage> dictionary;

        public static DBImage Get(int id)
        {
            if (dictionary == null)
                dictionary = new Dictionary<int, DBImage>();


            MySqlCommand cmd = new MySqlCommand($"SELECT image FROM images WHERE id={id}", DBConn.instance.conn);
            DBConn.instance.conn.Open();
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            
            byte[] buffer = (byte[])rdr[0];
            MemoryStream memoryStream = new MemoryStream(buffer);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.DecodePixelWidth = 180;
            bitmap.DecodePixelHeight = 180;
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            bitmap.Freeze();

            DBImage temp = new DBImage();
            temp.bitmap = bitmap;

            if (dictionary.ContainsKey(id))
                dictionary[id] = temp;
            else
                dictionary.Add(id, temp);

            DBConn.instance.conn.Close();
            return dictionary[id];
        }

    }
}
