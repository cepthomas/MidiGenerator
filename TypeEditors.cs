
#define _NNNNN

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Reflection;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;

using NAudio.Midi; // TODO1 hide?

using MidiLibNew;


namespace MidiGenerator
{

#if _NNNNN
    public class PatchSelectorTypeEditor : UITypeEditor
    {
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            IWindowsFormsEditorService? _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            // Dig out from context.
            string[] vals = new string[Defs.MAX_MIDI];

            if (context is not null && context.Instance is not null)
            {
                Type t = context!.Instance!.GetType();
                PropertyInfo? prop = t.GetProperty("PresetFile"); //CurrentPresets
                var vv = prop.GetValue(context.Instance, null);
                string pfile = (string)vv;

                //context.Instance


                //var info = GetType().GetProperty("PresetFile");
                //var pfile = (string)info.GetValue(this, null);
                vals = Presets.Load(pfile);
            }

            // Fill the selector.
            int sel = (int)value!; // default
            var lb = new ListBox
            {
                Width = 250,
                SelectionMode = SelectionMode.One
            };
            lb.Click += (_, __) => _service!.CloseDropDown();
            vals.ForEach(v => lb.Items.Add(v));
            _service!.DropDownControl(lb);

            return lb.SelectedItem is null ? value : lb.SelectedIndex;
            //return lb.SelectedItem is null ? value : (int)lb.SelectedItem;


            if (vals is not null)
            {
            }
            else
            {
            }

            if (lb.SelectedItem is not null)
            {
                sel = (int)lb.SelectedItem;
            }


            //        // Fill the selector.
            //        var lb = new ListBox
            //        {
            //            Width = 250,
            //            SelectionMode = SelectionMode.One
            //        };
            //        lb.Click += (_, __) => _service!.CloseDropDown();
            //        var vals = ProvideValues(context);
            //        vals.ForEach(v => lb.Items.Add(v));
            //        _service!.DropDownControl(lb);

            //        return lb.SelectedItem is null ? value : lb.SelectedItem.ToString();



            //return sel;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    public class ChannelSelectorTypeEditor : UITypeEditor
    {
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            IWindowsFormsEditorService? _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            // Fill the selector.
            var lb = new ListBox
            {
                Width = 100,
                SelectionMode = SelectionMode.One
            };
            lb.Click += (_, __) => _service!.CloseDropDown();

            Enumerable.Range(1, Defs.NUM_CHANNELS).ForEach(v => lb.Items.Add(v.ToString()));

            _service!.DropDownControl(lb);

            return lb.SelectedItem is null ? value : int.Parse((string)lb.SelectedItem);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    public class DevicesTypeEditor : UITypeEditor
    {
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            IWindowsFormsEditorService? _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            // Fill the selector.
            var lb = new ListBox
            {
                Width = 150,
                SelectionMode = SelectionMode.One
            };
            lb.Click += (_, __) => _service!.CloseDropDown();

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                lb.Items.Add(MidiOut.DeviceInfo(i).ProductName);
            }

            _service!.DropDownControl(lb);

            return lb.SelectedItem is null ? value : lb.SelectedItem.ToString();
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

#else


    public class PatchSelectorTypeEditor : ListSelectorTypeEditor
    {
        protected override List<string> ProvideValues(ITypeDescriptorContext? context)
        {
            // Dig out from context.
            List<string>? vals = null;

            if (context is not null && context.Instance is not null)
            {
                Type t = context!.Instance!.GetType();
                PropertyInfo? prop = t.GetProperty("CurrentPresets");
                if (prop != null)
                {
                    var names = prop.GetValue(context.Instance, null);
                    if (names != null && names is List<string>)
                    {
                        vals = names as List<string>;
                    }
                }
            }

            return vals ?? ["No presets specified"];
        }
    }

    public class ChannelSelectorTypeEditor : ListSelectorTypeEditor
    {
        protected override List<string> ProvideValues(ITypeDescriptorContext? context)
        {
            List<string> vals = [];
            Enumerable.Range(1, Defs.NUM_CHANNELS).ForEach(v => vals.Add(v.ToString()));
            return vals;
        }
    }

    public class DevicesTypeEditor : ListSelectorTypeEditor
    {
        protected override List<string> ProvideValues(ITypeDescriptorContext? context)
        {
            List<string> vals = [];
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                vals.Add(MidiOut.DeviceInfo(i).ProductName);
            }
            return vals;
        }
    }

    /// <summary>
    /// Generic property editor for selection from a list. TODO1 put in nbui.
    /// </summary>
    public abstract class ListSelectorTypeEditor : UITypeEditor
    {
        /// <summary>This is provided by the derived class.</summary>
        protected abstract List<string> ProvideValues(ITypeDescriptorContext? context);

        /// <summary>The user wants to edit something.</summary>
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            IWindowsFormsEditorService? _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            // Fill the selector.
            var lb = new ListBox
            {
                Width = 250,
                SelectionMode = SelectionMode.One
            };
            lb.Click += (_, __) => _service!.CloseDropDown();
            var vals = ProvideValues(context);
            vals.ForEach(v => lb.Items.Add(v));
            _service!.DropDownControl(lb);

            return lb.SelectedItem is null ? value : lb.SelectedItem.ToString();
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }


#endif

}