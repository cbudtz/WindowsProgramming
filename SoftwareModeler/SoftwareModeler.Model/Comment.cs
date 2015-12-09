using Area51.SoftwareModeler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Models
{
    [Obsolete("can be used later on to give comments a unique appearence. as of now they are shown as a class")]
    class Comment : Shape
    {
        private string commentString { get; set; }

        public Comment(string commentString)
        {
            this.commentString = commentString;
        }
    }
}
