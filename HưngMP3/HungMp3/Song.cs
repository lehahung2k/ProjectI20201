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
        private string songUrl;
        private string lyrics;
        private string downloadUrl;
        private string savePath;
        private string imageUrl;
        private double duration;
        private double position;

        public string SongName { get => songName; set => songName = value; }
        public string SongUrl { get => songUrl; set => songUrl = value; }
        public int STT { get => sTT; set => sTT = value; }
        public string Lyrics { get => lyrics; set => lyrics = value; }
        public string DownloadUrl { get => downloadUrl; set => downloadUrl = value; }
        public string SavePath { get => savePath; set => savePath = value; }
        public string ImageUrl { get => imageUrl; set => imageUrl = value; }
        public double Duration { get => duration; set => duration = value; }
        public double Position { get => position; set => position = value; }
    }
}
