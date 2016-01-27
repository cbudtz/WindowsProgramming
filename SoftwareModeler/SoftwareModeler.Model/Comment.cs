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
        public string Content { get; set; }

        public Comment(Point position, string content) : this(null, position, content) {}

        public Comment(int? id, Point position, string content)
        {
            this.id = id?? nextId++;
            X = position.X;
            Y = position.Y;
            Content = content;
        }
    }
}
