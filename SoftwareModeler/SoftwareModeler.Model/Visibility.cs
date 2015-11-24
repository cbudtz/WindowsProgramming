using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using Area51.SoftwareModeler.Helpers;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Models

{
    public enum Visibility
    {
        [Description("+")]
        Public,
        [Description("")]
        Default,
        [Description("#")]
        Protected,
        [Description("-")]
        Private 
    }
    
    public enum testMethod
    {
        testCommit, pleasework
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
        {

            //Console.WriteLine("Convert:"+value.ToString());
            return HelperFunctions.GetDescription((Visibility)value);

        }

        public object ConvertBack(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
        {
            //Console.WriteLine("ConvertBack:" + value.ToString());
            return HelperFunctions.GetEnumFromDescription(value.ToString(), Visibility.Default);
        }
    }




}
namespace Area51.SoftwareModeler.Helpers
{
    public static class HelperFunctions
    {
        public static string GetDescription(Enum enumerationValue)
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

        }

        public static Array GetDescriptions(Type enumType)
        {
            //dirty fix
            String[] enumDescriptions = new String[4];
            enumDescriptions[0] = "+";
            enumDescriptions[1] = "";
            enumDescriptions[2] = "#";
            enumDescriptions[3] = "-";

            return enumDescriptions;

            //work in progress
            ////Enum[] enumValues = (Enum[])Array.CreateInstance(enumType, System.Enum.GetValues(enumType).Length);
            //String[] enumValues = new String[System.Enum.GetValues(enumType).Length];

            ////Array[] enumValues = (enumType.GetElementType()[])System.Enum.GetValues(enumType);
            //String[] enumDescriptions = new String[enumValues.Length];
            //for (int i=0;i<enumValues.Length;i++)
            //{
            //    enumDescriptions[i] = HelperFunctions.GetDescription(System.Enum.Parse(enumValues[i]));
            //}

            //String[] enumDescriptions = new String[System.Enum.GetValues(enumType).Length];

            //Type type = enumerationValue.GetType();
            //if (!type.IsEnum)
            //{
            //    throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            //}

            ////Tries to find a DescriptionAttribute for a potential friendly name
            ////for the enum
            //MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            //if (memberInfo != null && memberInfo.Length > 0)
            //{
            //    object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            //    if (attrs != null && attrs.Length > 0)
            //    {
            //        //Pull out the description value
            //        return ((DescriptionAttribute)attrs[0]).Description;
            //    }
            //}
            ////If we have no description attribute, just return the ToString of the enum
            //return enumerationValue.ToString();

        }



        public static string GetEnumFromDescription(string enumerationValue, Enum myEnum)
        {
            Type type = myEnum.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfos = type.GetMembers();

            foreach (MemberInfo memberInfo in memberInfos)
            {
                object[] attrs = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length>0 && ((DescriptionAttribute) attrs[0]).Description == enumerationValue.ToString())
                {
                    Console.WriteLine("member:"+ memberInfo.Name);
                    return memberInfo.Name;

                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

        }

    }
}
