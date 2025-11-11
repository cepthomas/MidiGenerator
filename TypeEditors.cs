using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using NAudio.Midi;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;


namespace MidiGenerator
{


    /// <summary>
    /// Plug in to property grid.
    /// </summary>
    public class DevicesTypeEditor : UITypeEditor // TODO1
    {
        /// <summary>
        /// Do the work.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            IWindowsFormsEditorService? editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            if (editorService is not null && context is not null && value is not null && value is string)
            {
               //DevicesEditor ed = new();

               //switch (context.PropertyDescriptor!.Name)
               //{
               //    case "InputDevices":
               //        ed.Text = "Edit Input Devices";
               //        ed.Devices = MidiSettings.LibSettings.InputDevices;
               //        break;

               //    case "OutputDevices":
               //        ed.Text = "Edit Output Devices";
               //        ed.Devices = MidiSettings.LibSettings.OutputDevices;
               //        break;

               //    default:
               //        throw new InvalidOperationException("This should never happen!");
               //}

               //editorService.ShowDialog(ed);

               //value = ed.Devices;
            }

            return value!;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }



    /// <summary>
    /// Property editor for a single column name. The source is typically a custom collection editor supplied elsewhere.
    /// TODO-F This is a bit klunky because it is difficult to locate the property grid editor the collection editor uses.
    /// </summary>
    public class ListSelectorTypeEditor : UITypeEditor
    {
        /// <summary>
        /// System provided editor hosting.
        /// </summary>
       // private IWindowsFormsEditorService _service = null;

        /// <summary>
        /// This gets set by the client.
        /// </summary>
        public static List<string> Values = ["aaa", "bbb", "ccc"];// new List<string>();

        /// <summary>
        /// The user wants to edit something.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            // If you need to access something about the context of the property (the parent object etc), that is what the 
            // ITypeDescriptorContext (in EditValue) provides; it tells you the PropertyDescriptor and Instance (the MyType) that is involved.

            IWindowsFormsEditorService _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            string selval = value == null ? "Definitions.UNKNOWN_STRING" : value.ToString(); // the editor contents

            // Determine explicit or implicit mode.
            List<string> vals = null;

            if (Values != null && Values.Count > 0)
            {
                vals = Values;
            }
            else
            {
                //// Dig out from context.
                //Type t = context.Instance.GetType();
                //PropertyInfo prop = t.GetProperty("ColumnNames");
                //if (prop != null)
                //{
                //    var names = prop.GetValue(context.Instance, null);
                //    if(names != null && names is List<string>)
                //    {
                //        vals = names as List<string>;
                //    }
                //}
            }

            if (vals != null)
            {
                ListBox lb = new ListBox();
                lb.Width = 250;
                lb.SelectionMode = SelectionMode.One;
                lb.Click += (object sender, EventArgs e) => _service.CloseDropDown();

                // Fill the list box.
                foreach (string s in vals)
                {
                    int i = lb.Items.Add(s);
                }

                _service.DropDownControl(lb);

                if (lb.SelectedItem != null)
                {
                    selval = lb.SelectedItem.ToString();
                }
            }

            return selval;
        }


        // private void ListBox_Click(object sender, EventArgs e)
        // {
        //     if (_service != null)
        //     {
        //         _service.CloseDropDown();
        //     }
        // }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}