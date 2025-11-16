using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Ephemera.NBagOfTricks;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    /// <summary>
    /// xxxxx
    /// </summary>
    public class PatchConverter : Int64Converter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
        {
            return "TODO1";
            //return MidiDefs.GetInstrumentName((int)value!);

            //if (value is int && destinationType == typeof(string))
            //{
            //    return MidiDefs.GetInstrumentName((int)value);
            //}
            //return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
        {
            string txt = value.ToString();
            return 9999; // TODO1
            //return MidiDefs.GetInstrumentNumber(txt);

            //if (values[0] == txt)
            //{
            //    return false;
            //}
            //if (values[1] == txt)
            //{
            //    return true;
            //}
            //return base.ConvertFrom(context, culture, value);
        }
    }

    ///// <summary>
    ///// Converter for selecting property value from defined/known string lists.
    ///// </summary>
    //public class FixedListTypeConverter : TypeConverter
    //{
    //    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    //    {
    //        return true;
    //    }

    //    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    //    {
    //        return true;
    //    }

    //    /// Get the list using the property name as key.
    //    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    //    {
    //        List<string> rec = null;

    //        switch (context.PropertyDescriptor.Name)
    //        {
    //            case "PatchX":
    //                rec = new List<string>();
    //                for (int i = 0; i <= MidiDefs.MAX_MIDI; i++)
    //                {
    //                    rec.Add(MidiDefs.GetInstrumentName(i));
    //                }
    //                break;

    //            //case "AnalyzerType":
    //            //    rec = new List<string>();
    //            //    Array itypes = Enum.GetValues(typeof(Instrument.InstrumentType));
    //            //    foreach(object val in itypes)
    //            //    {
    //            //        if(val.ToString() != "None")
    //            //        {
    //            //            rec.Add(val.ToString());
    //            //        }
    //            //    }
    //            //    break;

    //            //case "DbServer":
    //            //    rec = Globals.Instance.ServerNames.Where(n => n.Contains("ECON")).ToList();
    //            //    break;

    //            //case "ADDServer":
    //            //    rec = Globals.Instance.ServerNames.Where(n => n.Contains("ADD")).ToList();
    //            //    break;

    //            //case "InstrumentDesc":
    //            //    rec = Globals.Instance.UserSettings.FavoriteInstruments;
    //            //    break;

    //            //case "TimeTemplateId":
    //            //    rec = Globals.Instance.TimeTemplates.GetAllIds();
    //            //    break;

    //            default:
    //                break;
    //        }

    //        StandardValuesCollection coll = new StandardValuesCollection(rec);

    //        return coll;
    //    }
    //}





    ///// <summary>
    ///// Replacement for the standard True/False.
    ///// </summary>
    //public class YesNoConverter : BooleanConverter
    //{
    //    /// <summary>
    //    /// What to put in the selector.
    //    /// </summary>
    //    private string[] values = new string[] { "No", "Yes" }; // { "", "X" };

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="context"></param>
    //    /// <param name="culture"></param>
    //    /// <param name="value"></param>
    //    /// <param name="destinationType"></param>
    //    /// <returns></returns>
    //    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    //    {
    //        if (value is bool && destinationType == typeof(string))
    //        {
    //            return values[(bool)value ? 1 : 0]; //You might need System::Convert::ToBoolean(value) instead of the cast to bool.
    //        }

    //        return base.ConvertTo(context, culture, value, destinationType);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="context"></param>
    //    /// <param name="culture"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    //    {
    //        string txt = value as string;

    //        if (values[0] == txt)
    //        {
    //            return false;
    //        }

    //        if (values[1] == txt)
    //        {
    //            return true;
    //        }

    //        return base.ConvertFrom(context, culture, value);
    //    }
    //}

    ///// <summary>
    ///// DateTime converter.
    ///// </summary>
    //public class DateTimeConverterEx : DateTimeConverter
    //{
    //    /// <summary>
    //    /// Overrides the ConvertTo method of TypeConverter.
    //    /// </summary>
    //    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    //    {
    //        object ret = null;

    //        if (value != null && value is DateTime)
    //        {
    //            DateTime v = (DateTime)value;

    //            if (destinationType == typeof(string))
    //            {
    //                ret = v.ToString();// Definitions.DATE_TIME_FORMAT);
    //            }
    //            else
    //            {
    //                ret = base.ConvertTo(context, culture, value, destinationType);
    //            }
    //        }

    //        return ret;
    //    }

    //    /// <summary>
    //    /// Overrides the ConvertFrom method of TypeConverter.
    //    /// </summary>
    //    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    //    {
    //        object ret = null;

    //        if (value is string)
    //        {
    //            try
    //            {
    //                //DateTime dt = DateTime.ParseExact(value.ToString(), Definitions.DATE_TIME_FORMAT_MSEC, null);
    //                DateTime dt = DateTime.Parse(value.ToString());
    //                ret = dt;
    //            }
    //            catch (Exception)
    //            {
    //                ret = DateTime.Now;
    //            }
    //        }
    //        else
    //        {
    //            ret = base.ConvertFrom(context, culture, value);
    //        }

    //        return ret;
    //    }
    //}
}