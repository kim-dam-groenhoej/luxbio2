using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for VAL
/// </summary>
public class VAL
{
    public static bool IsEmail(string input)
    {
        Regex emailregex = new Regex("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*"); // Simpel validering af Email


        if (!emailregex.IsMatch(input))
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public static bool IsPhone(string input)
    {
        Regex phoneregex = new Regex("^([0-9]{8})+$"); //bruges til validering af danske Telefon nummere.

        if (!phoneregex.IsMatch(input))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool IsNumber(string input)
    {
        Regex numberregex = new Regex("^([0-9]{})+$"); //bruges til validering af nummere.

        if (numberregex.IsMatch(input))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsInt(string input)
    {
        try
        {
            int i = int.Parse(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsText(string input)
    {
        Regex textregex = new Regex("^([a-z]{})+$"); //bruges til validering af Text.

        if (textregex.IsMatch(input))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsTextbox(string txtBox)
    {

        if (!String.IsNullOrEmpty(txtBox)) // Kontrollere om en tekstboks er tom.
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsEmailValid(string input) // input fortæller at teksten skrives af bruger.
    {

        if (input.EndsWith("college.dk") || input.EndsWith("tcaa.dk") || input.EndsWith("ccaa.dk")) // kontrollere om en email ender på college.dk, tcaa.dk eller ccaa.dk og retunere verdien sandt eller falsk.
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
