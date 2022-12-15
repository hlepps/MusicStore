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

        public DBImage()
        {

        }

        public DBImage(BitmapImage bi)
        {
            bitmap = bi;
        }
        public static BitmapImage GetBitmapFromBytes(byte[] buffer)
        {
            MemoryStream memoryStream = new MemoryStream(buffer);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.DecodePixelWidth = 180;
            bitmap.DecodePixelHeight = 180;
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
        public static byte[] GetBytesFromBitmap(BitmapImage bitmapImage)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
    }

    public class DBImagesSaved
    {
        /// <summary>
        /// Lokalna baza, może nie być aktualna
        /// </summary>
        public static Dictionary<int, DBImage> dictionary;

        /// <summary>
        /// Pobiera z bazy danych i aktualizuje lokalną bazę
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DBImage Get(int id)
        {
            if (dictionary == null)
                dictionary = new Dictionary<int, DBImage>();

            if (dictionary.ContainsKey(id))
                return dictionary[id];

            MySqlCommand cmd = new MySqlCommand($"SELECT image FROM images WHERE id={id}", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            
            byte[] buffer = (byte[])rdr[0];
            

            DBImage temp = new DBImage();
            temp.bitmap = DBImage.GetBitmapFromBytes(buffer);

            dictionary.Add(id, temp);
            DBConn.instance.conn.Close();
            return dictionary[id];
        }

    }

}
