using System;
using System.Collections.Generic;
using System.Windows;

namespace Area51.SoftwareModeler.Models
{
    public class Class
    {
        private string name;
        private string stereotype;
        private bool isAbstract;
        private Point anchorPoint;
        private Visibility visibility;
        private List<Attribute> attributes;
        private List<Method> methods;
        
        public Class(String name, String stereotype, Boolean isAbstract, Point anchorPoint, Visibility visibility)
        {
            this.name = name;
            this.stereotype = stereotype;
            this.isAbstract = isAbstract;
            this.anchorPoint = anchorPoint;
            this.visibility = visibility;

        }
        //Getters and setters
        public String Name  { get { return name; } set { name = value; } }
        public String StereoType { get { return stereotype; } set { stereotype = value; } }
        public Boolean IsAbstract { get { return isAbstract; } set { isAbstract = value; } }
        public Point AnchorPoint { get { return anchorPoint; } set { anchorPoint = value; } }
        public Visibility Visibility { get { return visibility; } set { visibility = value; } }


    }
}
