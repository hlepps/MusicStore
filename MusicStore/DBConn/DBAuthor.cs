using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace MusicStore.DB
{
    public class DBAuthor
    {
        public int id;
        public string name;
        public DBImage image;
    }

    public class DBAuthorsSaved
    {
        /// <summary>
        /// Lokalna baza, może nie być aktualna
        /// </summary>
        public static Dictionary<int, DBAuthor> dictionary;

        /// <summary>
        /// Pobiera z bazy danych i aktualizuje lokalną bazę
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DBAuthor Get(int id)
        {
            if (dictionary == null)
                dictionary = new Dictionary<int, DBAuthor>();
            
            if (dictionary.ContainsKey(id))
                return dictionary[id];

            MySqlCommand cmd = new MySqlCommand($"SELECT name, image_id, id FROM authors WHERE id={id}", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();

            object[] a = { rdr[0], rdr[1], rdr[2] };
            DBAuthor temp = new DBAuthor();
            temp.name = (string)a[0];
            temp.image = DBImagesSaved.Get((int)a[1]);
            temp.id = (int)a[2];


            dictionary.Add(id, temp);

            DBConn.instance.conn.Close();
            return dictionary[id];
        }

        public static void GetAll()
        {
            DBConn.instance.PrepareConnection();
            MySqlCommand authorcmd = new MySqlCommand($"SELECT id, name, image_id FROM authors", DBConn.instance.conn);
            DataTable dt = new DataTable();
            dt.Load(authorcmd.ExecuteReader());
            foreach (DataRow row in dt.Rows)
            {
                if (!dictionary.ContainsKey((int)row[0]))
                {
                    DBAuthor temp = new DBAuthor();
                    temp.id = (int)row[0];
                    temp.name = (string)row[1];
                    temp.image = DBImagesSaved.Get((int)row[2]);
                    dictionary.Add(temp.id, temp);
                    System.Diagnostics.Trace.WriteLine(temp.name);
                }
            }
        }

        /// <summary>
        /// returns inserted id
        /// </summary>
        /// <param name="name"></param>
        /// <param name="image_id"></param>
        /// <returns></returns>
        public static int Add(string name, int image_id)
        {
            MySqlCommand cmd = new MySqlCommand($"INSERT INTO authors (name, image_id) VALUES ('{name}', {image_id})", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();
            int id = (int)cmd.LastInsertedId;
            Get(id);
            return id;
        }

        public static void Update(int id, string name, int image_id)
        {
            MySqlCommand cmd = new MySqlCommand($"UPDATE authors SET name = '{name}', image_id='{image_id}' WHERE id='{id}'", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();
        }

    }
}