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

            MySqlCommand cmd = new MySqlCommand($"SELECT name, image_id FROM authors WHERE id={id}", DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();

            DBAuthor temp = new DBAuthor();
            temp.name = (string)rdr[0];
            temp.image = DBImagesSaved.Get((int)rdr[1]);

            
            dictionary.Add(id, temp);

            DBConn.instance.conn.Close();
            return dictionary[id];
        }

    }
}