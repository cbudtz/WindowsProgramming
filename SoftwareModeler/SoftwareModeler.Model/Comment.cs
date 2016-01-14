using Area51.SoftwareModeler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Area51.SoftwareModeler.Models
{
    //[Obsolete("can be used later on to give comments a unique appearence. as of now they are shown as a class")]
    public class Comment : ClassView
    {
        public string CommentString { get; set; }

        public Comment(Point position, string commentString)
        {
            X = position.X;
            Y = position.Y;
            CommentString = commentString;
        }
    }
}
