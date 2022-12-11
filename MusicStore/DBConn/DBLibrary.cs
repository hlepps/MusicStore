using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace MusicStore.DB
{
    public class DBLibrary
    {
        public List<DBLibraryObject> itemlist;

        public void ReloadLibrary()
        {
            DBConn.instance.currentUser.library.itemlist.Clear();
            string sql = $"SELECT library FROM users WHERE username='{DBConn.instance.currentUser.username}'";

            MySqlCommand cmd = new MySqlCommand(sql, DBConn.instance.conn);
            DBConn.instance.conn.Open();
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                string[] entries = ((string)rdr[0]).Split(',');
                foreach (string entry in entries)
                {
                    char type = entry[0];
                    int id = int.Parse(entry.Substring(1));

                    DBConn.instance.conn.Close();

                    switch (type)
                    {
                        case 's':
                            MySqlCommand encmd = new MySqlCommand($"SELECT songname, image_id FROM songs where id={id}", DBConn.instance.conn);
                            MySqlDataReader enrdr = encmd.ExecuteReader();
                            enrdr.Read();

                            DB.DBSong song = new DB.DBSong();
                            song.name = (string)enrdr[0];
                            song.image = DB.DBImagesSaved.Get((int)enrdr[1]);

                            MySqlCommand aucmd = new MySqlCommand($"SELECT author_id FROM songauthors where song_id={id}", DBConn.instance.conn);
                            MySqlDataReader aurdr = encmd.ExecuteReader();
                            
                            while(aurdr.HasRows)
                            {
                                aurdr.Read();
                                song.authors.Add(DBAuthorsSaved.Get((int)aurdr[0]));
                            }
                            


                            break;

                        case 'a':

                            break;
                    }
                }
            }

        }
    }

    public class DBLibraryObject
    {

    }
}
