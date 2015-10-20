using Area51.SoftwareModeler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Model
{
    class Comment : Shape
    {
        private string commentString { get; set; }

        public Comment(string commentString)
        {
            this.commentString = commentString;
        }
    }
}
