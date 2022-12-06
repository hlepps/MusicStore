using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.DB
{
    public class DBSong : DBLibraryObject
    {
        public string name;
        public List<DBAuthor> authors;
        public DBImage image;
    }
}
