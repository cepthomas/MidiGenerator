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
    public class PatchSelectorTypeEditor : ListSelectorTypeEditor
    {
        protected override List<string>? ProvideValues(ITypeDescriptorContext context)
        {
            // Dig out from context.
            List<string>? vals = null;

            Type t = context.Instance.GetType();
            PropertyInfo prop = t.GetProperty("CurrentPresets");
            if (prop != null)
            {
                var names = prop.GetValue(context.Instance, null);
                if (names != null && names is List<string>)
                {
                    vals = names as List<string>;
                }
            }

            return vals;
        }
    }

    public class ChannelSelectorTypeEditor : ListSelectorTypeEditor
    {
        protected override List<string>? ProvideValues(ITypeDescriptorContext context)
        {
            List<string> vals = [];
            Range[1..Defs.NUM_CHANNELS].ForEach(v => vals.Add(v));
            return vals;
        }
    }

    public class DevicesTypeEditor : ListSelectorTypeEditor
    {
        protected override List<string>? ProvideValues(ITypeDescriptorContext context)
        {
            List<string> vals = [];
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                vals.Add(MidiOut.DeviceInfo(i).ProductName));
            }
            return vals;
        }
    }

    /// <summary>
    /// Generic property editor for selection from a list.
    /// </summary>
    public abstract class ListSelectorTypeEditor : UITypeEditor
    {
        /// <summary>This is provided by the derived class.</summary>
        protected abstract List<string>? ProvideValues(ITypeDescriptorContext context);

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
            lb.Click += (_, __) => _service.CloseDropDown();
            var vals = ProvideValues(context);
            vals.ForEach(v => lb.Items.Add(v));
            _service.DropDownControl(lb);

            return lb.SelectedItem is null ? value : lb.SelectedItem.ToString();
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}