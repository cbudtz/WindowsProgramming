using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using Area51.SoftwareModeler.Helpers;

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

            Console.WriteLine("Convert:"+value.ToString());
            return HelperFunctions.GetDescription((Visibility)value);
            //return HelperFunctions.GetEnumFromDescription(value.ToString(), Visibility.Default);
            //switch (value.ToString().ToLower())
            //{
            //    case "yes":
            //        return true;
            //    case "no":
            //        return false;

            //    default:
            //        return Binding.DoNothing;
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("ConvertBack:" + value.ToString());
            //return HelperFunctions.GetDescription((Visibility) value);
            return HelperFunctions.GetEnumFromDescription(value.ToString(), Visibility.Default);

            //if (value is bool)
            //{
            //    if ((bool)value == true)
            //        return "yes";
            //    else
            //        return "no";
            //}
            //return "no";
        }
    }




}
namespace Area51.SoftwareModeler.Helpers
{
    public static class HelperFunctions
    {
        public static string GetDescription<T>(this T enumerationValue)
        where T : struct
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
            String[] myArray = new String[4];
            myArray[0] = "+";
            myArray[1] = "";
            myArray[2] = "#";
            myArray[3] = "-";

            return myArray;
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
