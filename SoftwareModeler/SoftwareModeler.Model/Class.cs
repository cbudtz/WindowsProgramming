using System;
using System.Collections.Generic;
using System.Linq;
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

        private int width = 600; //default
        private int height = 600; //default
        
        public Class(String name, String stereotype, Boolean isAbstract, Point anchorPoint, Visibility visibility)
        {
            this.name = name;
            this.stereotype = stereotype;
            this.isAbstract = isAbstract;
            this.anchorPoint = anchorPoint;
            this.visibility = visibility;
            this.attributes = new List<Attribute>();
            this.methods = new List<Method>();

        }
        //Getters and setters
        public String Name  { get { return name; } set { name = value; } }
        public String StereoType { get { return stereotype; } set { stereotype = value; } }
        public Boolean IsAbstract { get { return isAbstract; } set { isAbstract = value; } }
        public Point AnchorPoint { get { return anchorPoint; } set { anchorPoint = value; } }
        public Visibility Visibility { get { return visibility; } set { visibility = value; } }
        public List<Attribute> Attributes { get { return attributes; } set { attributes = value; } }
        public List<Method> Methods { get { return methods; } set { methods = value; } }



        public void addAttribute(string type, string name)
        {
            this.attributes.Add(new Attribute(type, name));
        }

        public void addMethod(Visibility visibility, string name, string[] parameters)
        {
            Method m = new Method(visibility, name);
            for (int i = 0; i < parameters.Length; i++)
                m.addParameter(parameters[i]);
            
            this.methods.Add(m);
        }

        public double X { get { return anchorPoint.X; } set { anchorPoint.X = value; } }
        public double Y { get { return anchorPoint.Y; } set { anchorPoint.Y = value; } }
    }
}
