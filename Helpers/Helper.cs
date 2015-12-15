using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;


public static class StringExtensions
{
    public static string FormatString(this DataRow dr, string fieldName)
    {
        return dr[fieldName] != null ? dr[fieldName].ToString() : string.Empty;
    }

    public static DateTime FormatDateTime(this DataRow dr, string fieldName)
    {
        return dr[fieldName] != DBNull.Value ? (DateTime)dr[fieldName] : DateTime.Now; //HUSK!! DateTime.MinValue = time 00:00:000
    }

    public static int FormatInt(this DataRow dr, string fieldName)
    {
        return dr[fieldName] != DBNull.Value ? (int)dr[fieldName] : 0;
    }

    public static decimal FormatDecimal(this DataRow dr, string fieldName)
    {
        return dr[fieldName] != null ? (decimal)dr[fieldName] : 0;
    }

    public static double FormatDouble(this DataRow dr, string fieldName)
    {
        return dr[fieldName] != null ? (double)dr[fieldName] : 0;
    }

    public static Guid FormatGuid(this DataRow dr, string fieldName)
    {
        return dr[fieldName] != null ? (Guid)dr[fieldName] : Guid.Empty;
    }

    public static bool FormatBool(this DataRow dr, string fieldName)
    {
        return dr[fieldName] != DBNull.Value ? (bool)dr[fieldName] : false;
    }


    public static string GetMonthName(this int monthNo)
    {
        DateTime dtGetDate = new DateTime(2000, monthNo, 1);

        string MonthName = dtGetDate.ToString("MMMM");
        MonthName = MonthName.Substring(0, 1).ToUpper() + MonthName.Substring(1).ToLower();

        return MonthName;
    }

    public static int CalculateAge(this DateTime birthdate)
    {
        int years = DateTime.Now.Year - birthdate.Year;
        if (birthdate.Year == 0001)
            return 0;
        if (DateTime.Now.Month < birthdate.Month || (DateTime.Now.Month == birthdate.Month && DateTime.Now.Day < birthdate.Day))
            years--;
        return years;
    }

    public static string ToFormatedDate(this DateTime date)
    {
        return date.ToLongDateString() + " - (" + date.ToShortTimeString() + ")";
    }
    public static string ToFormatedDate(this object date)
    {
        DateTime GetDate = Convert.ToDateTime(date);
        return GetDate.ToLongDateString() + " - (" + GetDate.ToShortTimeString() + ")";
    }

    public static string ToUrlName(this string urlName)
    {
        urlName = urlName.ToLower();
        urlName = urlName.Replace(" ", "-");
        urlName = urlName.Replace("c#", "c-sharp");
        urlName = urlName.Replace("æ", "ae");
        urlName = urlName.Replace("ø", "oe");
        urlName = urlName.Replace("å", "aa");
        urlName = urlName.Replace("'", "");
        urlName = urlName.Replace("/", "");
        urlName = urlName.Replace("&", "");
        urlName = urlName.Replace(";", "");
        urlName = urlName.Replace(":", "");
        urlName = urlName.Replace(",", "");
        urlName = urlName.Replace(".", "");
        urlName = urlName.Replace("+", "");
        urlName = urlName.Replace("=", "");
        urlName = urlName.Replace("(", "");
        urlName = urlName.Replace(")", "");
        urlName = urlName.Replace("%", "");
        urlName = urlName.Replace("#", "");
        urlName = urlName.Replace("!", "");
        urlName = urlName.Replace("---", "-");
        urlName = urlName.Replace("--", "-");

        return urlName;
    }

    public static string ToFileSize(this object source)
    {
        const int byteConversion = 1024;
        double bytes = Convert.ToDouble(source);
        if (bytes >= Math.Pow(byteConversion, 3)) //GB Range    
        {
            return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 3), 2), " GB");
        }
        else if (bytes >= Math.Pow(byteConversion, 2)) //MB Range    
        {
            return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 2), 2), " MB");
        }
        else if (bytes >= byteConversion) //KB Range    
        {
            return string.Concat(Math.Round(bytes / byteConversion, 2), " KB");
        }
        else //Bytes    
        {
            return string.Concat(bytes, " Bytes");
        }
    }
    
    public static string ToImageSize(this object filePath)
    {
        string ReturnVal = "";
        string FilePath = HttpContext.Current.Server.MapPath("/Content/Images") + "/" + filePath.ToString();

        if (new FileInfo(FilePath).Exists)
        {
            Image GetImage = Bitmap.FromFile(FilePath);
            ReturnVal =  GetImage.Width + "x" + GetImage.Height;
            GetImage.Dispose();
        }

        return ReturnVal;
    }

    public static string Cut(this object text, int cut = 50)
    {
        if (text.ToString().Length <= cut)
            return text.ToString();
        else
            return text.ToString().Remove(cut) + "...";
    }

    public static string CapFirst(this string text)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(text[0]) + text.Substring(1);
    }
}