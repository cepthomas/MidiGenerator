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
using NAudio.Midi;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using Ephemera.MidiLib;


namespace MidiGenerator
{
    ///// <summary>Select a patch from list.</summary>
    //public class PatchSelectorTypeEditor : UITypeEditor
    //{
    //    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    //    {
    //        IWindowsFormsEditorService? _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

    //        // Dig out from context.
    //        string[] vals = xxxnew string[MidiDefs.MAX_MIDI];

    //        if (context is not null && context.Instance is not null)
    //        {
    //            Type t = context!.Instance!.GetType();
    //            PropertyInfo? prop = t.GetProperty("PresetFile"); //CurrentPresets
    //            var vv = prop.GetValue(context.Instance, null);
    //            string pfile = (string)vv;
    //            vals = Presets.Load(pfile);
    //        }

    //        // Fill the selector.
    //        string sel = value!.ToString(); // default
    //        var lb = new ListBox
    //        {
    //            Width = 250,
    //            SelectionMode = SelectionMode.One
    //        };
    //        lb.Click += (_, __) => _service!.CloseDropDown();
    //        vals.ForEach(v => lb.Items.Add(v));
    //        _service!.DropDownControl(lb);

    //        return lb.SelectedItem is null ? value : lb.SelectedItem;
    //    }

    //    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
    //    {
    //        return UITypeEditorEditStyle.DropDown;
    //    }
    //} 

    /// <summary>Select a patch from list.</summary>
    public class PatchTypeEditor : UITypeEditor
    {
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            IWindowsFormsEditorService? _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            // Dig out from context.
            string[] vals;// = new string[MidiDefs.MAX_MIDI];

            Type t = context!.Instance!.GetType();
            PropertyInfo? prop = t.GetProperty("Instruments");
            vals = (string[])prop.GetValue(context.Instance, null);

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
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    /// <summary>Select a channel from list.</summary>
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

            Enumerable.Range(1, MidiDefs.NUM_CHANNELS).ForEach(v => lb.Items.Add(v.ToString()));

            _service!.DropDownControl(lb);

            return lb.SelectedItem is null ? value : int.Parse((string)lb.SelectedItem);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    /// <summary>Select a device from list.</summary>
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
}