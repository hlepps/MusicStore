using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace MusicStore.DB
{
    public class DBAlbum : DBLibraryObject
    {
        public string name;
        public DBAuthor author;
        public List<DBLibraryObject> songs;
        public DBImage image;
        public double price;
    }


    public class DBAlbumsSaved
    {
        /// <summary>
        /// Lokalna baza, może nie być aktualna
        /// </summary>
        public static Dictionary<int, DBAlbum> dictionary;

        public static DBAlbum Get(int id)
        {
            if (dictionary == null)
                dictionary = new Dictionary<int, DBAlbum>();

            if (dictionary.ContainsKey(id))
                return dictionary[id];

            DBConn.instance.PrepareConnection();
            MySqlCommand aencmd = new MySqlCommand($"SELECT name, image_id, mainauthor_id, price FROM albums where id={id}", DBConn.instance.conn);
            MySqlDataReader aenrdr = aencmd.ExecuteReader();
            aenrdr.Read();
            object[] a = { aenrdr[0], aenrdr[1], aenrdr[2], aenrdr[3] };
            DB.DBAlbum album = new DB.DBAlbum();
            album.id = id;
            album.name = (string)a[0];
            album.image = DB.DBImagesSaved.Get((int)a[1]);
            album.author = DB.DBAuthorsSaved.Get((int)a[2]);
            album.price = (double)a[3];
            album.songs = new List<DBLibraryObject>();

            DBConn.instance.PrepareConnection();
            MySqlCommand songscmd = new MySqlCommand($"SELECT song_id FROM songsinalbums where album_id={id}", DBConn.instance.conn);
            
            DataTable dt = new DataTable();
            dt.Load(songscmd.ExecuteReader());
            foreach (DataRow row in dt.Rows)
            {
                album.songs.Add(DBSongsSaved.Get((int)row[0]));
            }


            dictionary.Add(id, album);

            return album;
        }

    }
}
