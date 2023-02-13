using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;

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
        public static Dictionary<int, DBSong> dictionary = new Dictionary<int, DBSong>();
        public static List<DBLibraryObject> GetAllFromDictionary()
        {
            List<DBLibraryObject> temp = new List<DBLibraryObject>();
            foreach (System.Collections.Generic.KeyValuePair<int, DBSong> a in dictionary)
            {
                temp.Add(a.Value);
            }
            return temp;
        }


        public static void PreloadAll()
        {
            DBConn.instance.PrepareConnection();
            MySqlCommand encmd = new MySqlCommand($"SELECT id FROM songs", DBConn.instance.conn);
            MySqlDataReader enrdr = encmd.ExecuteReader();

            while(enrdr.Read())
            {
                /// 0-songname 1-image_id 2-price 3-mp3_id 4-id
                object[] a = {enrdr[0]};
                if (!dictionary.ContainsKey((int)enrdr[0]))
                    Get((int)enrdr[0]);

                if (enrdr.IsClosed)
                    break;
            }

        }

        public static void Remove(int id)
        {
            DBConn.instance.PrepareConnection();
            MySqlCommand a = new MySqlCommand($"DELETE FROM songsinalbums WHERE song_id = '{id}'", DBConn.instance.conn);
            a.ExecuteNonQuery();
            DBConn.instance.PrepareConnection();
            MySqlCommand b = new MySqlCommand($"DELETE FROM songauthors WHERE song_id = '{id}'", DBConn.instance.conn);
            b.ExecuteNonQuery();
            DBConn.instance.PrepareConnection();
            MySqlCommand c = new MySqlCommand($"DELETE FROM songs WHERE id = '{id}'", DBConn.instance.conn);
            c.ExecuteNonQuery();
            DBConn.instance.PrepareConnection();
            MySqlCommand d = new MySqlCommand($"SELECT id, library FROM users", DBConn.instance.conn);
            DataTable dt = new DataTable();
            dt.Load(d.ExecuteReader());
            foreach (DataRow row in dt.Rows)
            {
                object[] temp = { row[0], row[1] };
                string idtxt = "s" + id;
                if (((string)temp[1]).Contains(idtxt))
                {
                    string t = (string)row[1];
                    Trace.WriteLine(t);
                    if (t.Contains("," + idtxt))
                    {
                        t = t.Replace("," + idtxt, "");
                    }
                    else if (t.Contains(idtxt + ","))
                    {
                        t = t.Replace(idtxt + ",", "");
                    }
                    else if (t.Contains(idtxt))
                    {
                        t = t.Replace(idtxt, "");
                    }
                    Trace.WriteLine(t);

                    DBConn.instance.PrepareConnection();
                    MySqlCommand e = new MySqlCommand($"UPDATE users SET library = '{t}' WHERE id='{(int)temp[0]}'", DBConn.instance.conn);
                    e.ExecuteNonQuery();
                    dictionary.Remove(id);

                }
            }
            if (DBConn.instance.currentUser.library.ContainsID(typeof(DBSong), id))
                DBConn.instance.currentUser.library.RemoveByID(typeof(DBSong), id);

        }


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
            song.id = id;
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

        public static void GetAll()
        {
            DBConn.instance.PrepareConnection();
            MySqlCommand authorcmd = new MySqlCommand($"SELECT id, songname, image_id, price, mp3_id FROM songs", DBConn.instance.conn);
            DataTable dt = new DataTable();
            dt.Load(authorcmd.ExecuteReader());
            foreach (DataRow row in dt.Rows)
            {
                if (!dictionary.ContainsKey((int)row[0]))
                {
                    DBSong temp = new DBSong();
                    temp.id = (int)row[0];
                    temp.name = (string)row[1];
                    temp.image = DBImagesSaved.Get((int)row[2]);
                    temp.price = (double)row[3];
                    temp.songurlid = (string)row[4];
                    dictionary.Add(temp.id, temp);
                    System.Diagnostics.Trace.WriteLine(temp.name);
                }
            }
        }

        public static int UploadMP3(byte[] bytes)
        {
            MySqlCommand cmd = new MySqlCommand($"INSERT INTO mp3s (mp3) VALUES (@data)", DBConn.instance.conn);
            cmd.Parameters.AddWithValue("data", bytes);
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public static int Add(string name, int image_id, double price, int songid, List<int> authorsIDs)
        {
            MySqlCommand cmd = new MySqlCommand($"INSERT INTO songs (songname, image_id, price, mp3_id) VALUES ('{name}', {image_id}, {price}, {songid})", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();
            int id = (int)cmd.LastInsertedId;
            Get(id);

            foreach(int authorID in authorsIDs)
            {
                MySqlCommand a = new MySqlCommand($"INSERT INTO songauthors (song_id, author_id) VALUES ({id}, {authorID})", DBConn.instance.conn);
                DBConn.instance.PrepareConnection();
                a.ExecuteNonQuery();
            }

            return id;
        }

        public static void Update(int id, string name, int image_id, double price, int songid, List<int> authorsIDs)
        {
            MySqlCommand cmd = new MySqlCommand($"UPDATE authors SET name = '{name}', image_id='{image_id}', price = {price}, mp3_id = {songid} WHERE id='{id}'", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();

            MySqlCommand b = new MySqlCommand($"DELETE FROM songauthors WHERE song_id={id}", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            b.ExecuteNonQuery();

            foreach (int authorID in authorsIDs)
            {
                MySqlCommand a = new MySqlCommand($"INSERT INTO songauthors (song_id, author_id) VALUES ({id}, {authorID})", DBConn.instance.conn);
                DBConn.instance.PrepareConnection();
                a.ExecuteNonQuery();
            }

        }

    }
}
