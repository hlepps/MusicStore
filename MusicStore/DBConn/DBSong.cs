using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace MusicStore.DB
{
    public class DBSong : DBLibraryObject
    {
        public string name;
        public List<DBAuthor> authors;
        public DBImage image;
        public double price;
        public string songurlid;
    }
    public class DBSongsSaved
    {
        /// <summary>
        /// Lokalna baza, może nie być aktualna
        /// </summary>
        public static Dictionary<int, DBSong> dictionary;

        /// <summary>
        /// Pobiera z bazy danych i aktualizuje lokalną bazę
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DBSong Get(int id)
        {
            if (dictionary == null)
                dictionary = new Dictionary<int, DBSong>();


            if (dictionary.ContainsKey(id))
                return dictionary[id];

            DBConn.instance.PrepareConnection();
            MySqlCommand encmd = new MySqlCommand($"SELECT songname, image_id, price, mp3_id FROM songs where id={id}", DBConn.instance.conn);
            MySqlDataReader enrdr = encmd.ExecuteReader();
            enrdr.Read();

            object[] a = { enrdr[0], enrdr[1], enrdr[2], enrdr[3] };
            DB.DBSong song = new DB.DBSong();
            song.name = (string)a[0];
            song.image = DB.DBImagesSaved.Get((int)a[1]);
            song.price = (double)a[2];
            song.authors = new List<DBAuthor>();
            song.songurlid = (string)a[3];

            DBConn.instance.PrepareConnection();
            MySqlCommand authorcmd = new MySqlCommand($"SELECT author_id FROM songauthors where song_id={id}", DBConn.instance.conn);
            DataTable dt = new DataTable();
            dt.Load(authorcmd.ExecuteReader());
            foreach(DataRow row in dt.Rows)
            {
                //System.Diagnostics.Trace.WriteLine((int)row[0]);
                song.authors.Add(DBAuthorsSaved.Get((int)row[0]));
            }


            dictionary.Add(id, song);
            return song;
        }

    }
}
