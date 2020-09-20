using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AbstraX
{
    public class NodeImageAttribute : Attribute
    {
        public string ImagePath { get; set; }

        public NodeImageAttribute(string imagePath)
        {
            this.ImagePath = imagePath;
        }
    }
}