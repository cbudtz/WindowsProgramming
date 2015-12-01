using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Area51.SoftwareModeler.Models
{
    
    public class Class : Shape
    {
        private string stereotype;
        private bool isAbstract;
        //private Point anchorPoint;
        private Visibility visibility;
        private List<Attribute> attributes;
        private List<Method> methods;



        public Class(String name, String stereotype, Boolean isAbstract, Point anchorPoint, Visibility visibility) : base()
        {
            this.name = name;
            this.stereotype = stereotype;
            this.isAbstract = isAbstract;
            this.X = anchorPoint.X;
            this.Y = anchorPoint.Y;
            //this.anchorPoint = anchorPoint;
            this.visibility = visibility;
            this.attributes = new List<Attribute>();
            this.methods = new List<Method>();

        }

        public Class() : base()
        {
        }

        public Class(int? id, String name, String stereotype, Boolean isAbstract, Point anchorPoint, Visibility visibility) : base(id)
        {
            this.name = name;
            this.stereotype = stereotype;
            this.isAbstract = isAbstract;
            this.X = anchorPoint.X;
            this.Y = anchorPoint.Y;
            //this.anchorPoint = anchorPoint;
            this.visibility = visibility;
            this.attributes = new List<Attribute>();
            this.methods = new List<Method>();


        }

        //Getters and setters
        public String Name { get { return name;} set { name = value; NotifyPropertyChanged(); } }
        public String StereoType { get { return stereotype; } set { stereotype = value; NotifyPropertyChanged();} }
        public Boolean IsAbstract { get { return isAbstract; } set { isAbstract = value; NotifyPropertyChanged();} }
        //public Point AnchorPoint { get { return anchorPoint; } set { anchorPoint = value; } }
        public Visibility Visibility { get { return visibility; } set { visibility = value; NotifyPropertyChanged();} }
        public List<Attribute> Attributes { get { return attributes; } set { attributes = value; NotifyPropertyChanged();} }
        public List<Method> Methods { get { return methods; } set { methods = value; NotifyPropertyChanged();} }



        public void addAttribute(string type, string name)
        {
            this.attributes.Add(new Attribute(type, name));
        }

        public void addAttribute(Visibility visibility, string type, string name)
        {
            this.attributes.Add(new Attribute(visibility, type, name));
        }

        public void addMethod(Visibility visibility, string name, string[] parameters)
        {
            Method m = new Method(visibility, name);
            string param = "";
            for (int i = 0; i < parameters.Length; i++)
                param += parameters + ", ";
            //m.addParameter(parameters[i]);
            param = param.Substring(0, param.Length - 1);
            m.Parameters = param;
            this.methods.Add(m);
        }

        public override string ToString()
        {
            return name;
        }
    }
}
