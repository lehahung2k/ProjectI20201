using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungMp3
{
    public class Song
    {
        private int sTT;
        private string songName;
        private string singerName;
        private string songUrl;

        public string SongName { get => songName; set => songName = value; }
        public string SingerName { get => singerName; set => singerName = value; }
        
        public string SongUrl { get => songUrl; set => songUrl = value; }
        public int STT { get => sTT; set => sTT = value; }
    }
}
