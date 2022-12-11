using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace MusicStore.DB
{
    public class DBAuthor
    {
        public string name;
        public DBImage image;
    }

    public class DBAuthorsSaved
    {
        private static Dictionary<int, DBAuthor> dictionary;

        public static DBAuthor Get(int id)
        {
            if (dictionary == null)
                dictionary = new Dictionary<int, DBAuthor>();


            MySqlCommand cmd = new MySqlCommand($"SELECT name, image_id FROM authors WHERE id={id}", DBConn.instance.conn);
            DBConn.instance.conn.Open();
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();

            DBAuthor temp = new DBAuthor();
            temp.name = (string)rdr[0];
            temp.image = DBImagesSaved.Get((int)rdr[1]);

            if (dictionary.ContainsKey(id))
                dictionary[id] = temp;
            else
                dictionary.Add(id, temp);

            DBConn.instance.conn.Close();
            return dictionary[id];
        }

    }
}