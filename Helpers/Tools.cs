using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Net.Mail;

/// <summary>
/// Summary description for Tools
/// </summary>
public class Tools
{
	public Tools()
	{
		
	}
    
    public static Control FindControlRecursive(Control Root, string Id)
    {

        if (Root.ID == Id)

            return Root;

        foreach (Control Ctl in Root.Controls)
        {
            Control FoundCtl = FindControlRecursive(Ctl, Id);
            if (FoundCtl != null)
                return FoundCtl;
        }
        return null;
    }

    public static string removeDec(object prodSize)
    {
        string size = prodSize.ToString();
        string last = size.Substring(size.LastIndexOf(',') + 1);
        int pos = size.LastIndexOf(",");

        if (last == "00")
        {
            size = size.Remove(pos);
        }
        else if (size.EndsWith("0"))
        {
            size = size.Substring(0, size.Length - 1);
        }

        return size;
    }

    public static void SendMail(string from, string to, string subject, string body, string smtp)
    {
        MailMessage mailMessage = new MailMessage(from, to, subject, body);
        mailMessage.Body = ConvertToHTML(body);
        mailMessage.IsBodyHtml = true;
        SmtpClient client = new SmtpClient(smtp);
        client.Port = 25;
        client.Send(mailMessage);
    }

    public static string CropText(string input, int MaxLenght, bool DoDots)
    {

        string result = input;
        int InputLength = input.Length;

        string lastChar = result.Substring(result.Length - 1);

        if (MaxLenght < InputLength)
        {
            result = input.Substring(0, MaxLenght);

            if (lastChar == " ")
            {
                result = result.Substring(0, result.Length - 1);
            }

            if (DoDots)
            {
                result += "...";
            }
        }

        return result;
    }

    public static string CharacterToolInput(string input)
    {

        string result = input.Replace("\r\n", "<br />"); //.Replace("[b]", "<b>").Replace("[/b]", "</b>").Replace("[k]", "<i>").Replace("[/k]", "</i>").Replace("æ", "&aelig;").Replace("ø", "&oslash;").Replace("å", "&aring;").Replace("Æ", "&AElig;").Replace("Ø", "&Oslash;").Replace("Å", "&Aring;");
        return result;
    }

    public static string CharactorToolInputForHeaders(string input)
    {

        string result = input.Replace("\r\n", "<br />").Replace("[b]", "<b>").Replace("[/b]", "</b>").Replace("[k]", "<i>").Replace("[/k]", "</i>").Replace("æ", "&aelig;").Replace("ø", "&oslash;").Replace("å", "&aring;").Replace("Æ", "&AElig;").Replace("Ø", "&Oslash;").Replace("Å", "&Aring;");
        return result;
    }

    public static string CharacterToolOutput(string output)
    {

        string result = output.Replace("\r\n", "<br />");//.Replace("&aelig;", "æ").Replace("&oslash;", "ø").Replace("&aring;", "å").Replace("&AElig;", "Æ").Replace("&Oslash;", "Ø").Replace("&Aring;", "Å");
        return result;
    }

    //Denne bruges når data indsættes i database
    public static string CharactorToolOutputForHeaders(string output)
    {

        string result = output.Replace("<br />", Environment.NewLine); //.Replace("&aelig;", "æ").Replace("&oslash;", "ø").Replace("&aring;", "å").Replace("&AElig;", "Æ").Replace("&Oslash;", "Ø").Replace("&Aring;", "Å");
        return result;
    }

    //Bruges når data hentes fra database
    public static string ConvertToHTML(string input)
    {
        return input.Replace(Environment.NewLine, "<br />");
    }

}
