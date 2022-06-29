using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServices.Services
{
    public class Meta
    {
        public float confidence { get; set; }
        public IList<string> gender { get; set; }
        public IList<string> age { get; set; }
        public IList<string> ethnicity { get; set; }
        public IList<string> eye_color { get; set; }
        public IList<string> hair_color { get; set; }
        public IList<string> hair_length { get; set; }
        public IList<string> emotion { get; set; }
    }

    public class Faces
    {
        public string id { get; set; }
        public string version { get; set; }
        public KeyValuePair<string, object>[] urls { get; set; }
        public Meta meta { get; set; }

    }

    public class GeneratePhotoResponse
    {
        public IList<Faces> faces { get; set; }
        public int total { get; set; }
        public Image Image { get; set; }
        public string Url { get; internal set; }
    }
}