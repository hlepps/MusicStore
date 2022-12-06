using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.DB
{
    public class DBAlbum : DBLibraryObject
    {
        public string name;
        public DBAuthor author;
        public List<DBSong> songs;
        public DBImage image;
    }
}
