﻿using System;
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

        public DBLibrary()
        {
            itemlist = new List<DBLibraryObject>();
        }


        public bool ContainsID(Type type, int id)
        {
            foreach (DBLibraryObject dbo in itemlist)
            {
                if (dbo.id == id && dbo.GetType() == type)
                    return true;
            }
            return false;
        }
        public DBLibraryObject GetByID(Type type, int id)
        {
            foreach (DBLibraryObject dbo in itemlist)
            {
                if (dbo.id == id && dbo.GetType() == type)
                    return dbo;
            }
            return null;
        }
        public void RemoveByID(Type type, int id)
        {
            foreach (DBLibraryObject dbo in itemlist)
            {
                if (dbo.id == id && dbo.GetType() == type)
                {
                    itemlist.Remove(dbo);
                    break;
                }
            }
        }

        public void ReloadLibrary()
        {
            DBConn.instance.currentUser.library.itemlist.Clear();
            string sql = $"SELECT library FROM users WHERE username='{DBConn.instance.currentUser.username}'";

            MySqlCommand cmd = new MySqlCommand(sql, DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                string[] entries = ((string)rdr[0]).Split(',');
                foreach (string entry in entries)
                {
                    if (entry == "")
                        continue;
                    char type = entry[0];
                    int id = int.Parse(entry.Substring(1));

                    DBConn.instance.conn.Close();

                    switch (type)
                    {
                        case 's':
                            DBConn.instance.currentUser.library.itemlist.Add(DB.DBSongsSaved.Get(id));
                            break;

                        case 'a':
                            DBConn.instance.currentUser.library.itemlist.Add(DB.DBAlbumsSaved.Get(id));
                            break;
                    }
                }
            }

        }
    }

    public class DBLibraryObject
    {
        public int id;
    }
}
