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


namespace MidiGenerator
{
    /// <summary>Select a patch from list.</summary>
    public class PatchTypeEditor : UITypeEditor
    {
        /// <inheritdoc />
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            IWindowsFormsEditorService? _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            Channel chan = context.Instance as Channel;
            var vals = chan.Instruments;

            //Type t = context!.Instance!.GetType();
            //var fld = t.GetField("_instruments", BindingFlags.NonPublic | BindingFlags.Instance);
            //var vals = (Dictionary<int, string>)fld.GetValue(context.Instance)!;

            // Fill the selector.
            int sel = (int)value!; // default
            var lb = new ListBox
            {
                Width = 150,
                SelectionMode = SelectionMode.One
            };
            lb.Click += (_, __) => _service!.CloseDropDown();
            vals.ForEach(v => lb.Items.Add(v));
            _service!.DropDownControl(lb);

            return lb.SelectedItem is null ? value : lb.SelectedIndex;
        }

        /// <inheritdoc />
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    /// <summary>Select a channel from list.</summary>
    public class ChannelSelectorTypeEditor : UITypeEditor
    {
        /// <inheritdoc />
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            IWindowsFormsEditorService? _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            // Fill the selector.
            var lb = new ListBox
            {
                Width = 50,
                SelectionMode = SelectionMode.One
            };
            lb.Click += (_, __) => _service!.CloseDropDown();

            Enumerable.Range(1, MidiDefs.NUM_CHANNELS).ForEach(v => lb.Items.Add(v.ToString()));

            _service!.DropDownControl(lb);

            return lb.SelectedItem is null ? value : int.Parse((string)lb.SelectedItem);
        }

        /// <inheritdoc />
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    /// <summary>Select a device from list.</summary>
    public class DeviceTypeEditor : UITypeEditor
    {
        /// <inheritdoc />
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

            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                lb.Items.Add(MidiOut.DeviceInfo(i).ProductName);
            }

            _service!.DropDownControl(lb);

            return lb.SelectedItem is null ? value : lb.SelectedItem.ToString();
        }

        /// <inheritdoc />
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    /// <summary>
    /// Convert between int and string versions.
    /// </summary>
    public class PatchConverter : Int64Converter
    {
        /// <inheritdoc />
        public override object? ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
        {
            Channel chan = context.Instance as Channel;
            return chan.Instruments[(int)value];

            //return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <inheritdoc />
        public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
        {
            Channel chan = context.Instance as Channel;
            var res = chan.Instruments.FirstOrDefault(ch => ch.Value == (string)value);
            return res.Value;

            //return base.ConvertFrom(context, culture, value);
        }
    }
}