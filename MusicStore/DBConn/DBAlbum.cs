using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

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
        public static Dictionary<int, DBAlbum> dictionary = new Dictionary<int, DBAlbum>();
        public static List<DBLibraryObject> GetAllFromDictionary()
        {
            List<DBLibraryObject> temp = new List<DBLibraryObject>();
            foreach(System.Collections.Generic.KeyValuePair<int, DBAlbum> a in dictionary)
            {
                temp.Add(a.Value);
            }
            return temp;
        }

        public static void PreloadAll()
        {
            DBConn.instance.PrepareConnection();
            MySqlCommand encmd = new MySqlCommand($"SELECT id FROM albums", DBConn.instance.conn);
            MySqlDataReader enrdr = encmd.ExecuteReader();

            while (enrdr.Read())
            {
                if (!enrdr.HasRows)
                    continue;
                object[] a = { enrdr[0] };
                if (!dictionary.ContainsKey((int)enrdr[0]))
                    Get((int)enrdr[0]);

                if(enrdr.IsClosed)
                    break;
                
            }

        }

        public static void Remove(int id)
        {
            DBConn.instance.PrepareConnection();
            MySqlCommand a = new MySqlCommand($"DELETE FROM songsinalbums WHERE album_id = '{id}'", DBConn.instance.conn);
            a.ExecuteNonQuery();
            DBConn.instance.PrepareConnection();
            MySqlCommand b = new MySqlCommand($"DELETE FROM albums WHERE id = '{id}'", DBConn.instance.conn);
            b.ExecuteNonQuery();
            DBConn.instance.PrepareConnection();
            MySqlCommand c = new MySqlCommand($"SELECT id, library FROM users", DBConn.instance.conn);
            DataTable dt = new DataTable();
            dt.Load(c.ExecuteReader());
            foreach (DataRow row in dt.Rows)
            {
                object[] temp = { row[0], row[1] };
                string idtxt = "a" + id;
                if (((string)temp[1]).Contains(idtxt))
                {
                    string t = (string)row[1];
                    Trace.WriteLine(t);
                    if (t.Contains("," + idtxt))
                    {
                        t = t.Replace("," + idtxt, "");
                    }
                    else if(t.Contains(idtxt + ","))
                    {
                        t = t.Replace(idtxt + ",", "");
                    }
                    else if(t.Contains(idtxt))
                    {
                        t = t.Replace(idtxt, "");
                    }
                    Trace.WriteLine(t);

                    DBConn.instance.PrepareConnection();
                    MySqlCommand d = new MySqlCommand($"UPDATE users SET library = '{t}' WHERE id='{(int)temp[0]}'", DBConn.instance.conn);
                    d.ExecuteNonQuery();
                    dictionary.Remove(id);

                }
            }

        }

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
