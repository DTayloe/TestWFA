using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Globalization;
using TestWFA.TaskItem; // could not add this project as a reference because of circular dependency issues.

namespace TestWFA
{
    [Designer(typeof(TaskFrackerControlLibrary.Design.TaskTimeControlRootDesigner), typeof(IRootDesigner))]
    public partial class TaskTimeControl : UserControl
    {
        public const string TIMER_CURRENT_EVENT = "Current Event";
        public const string TIMER_THIS_TASK = "This Task";
        public const string TIMER_TOTAL = "This Task + Subtasks";

        public TaskTimeControl()
        {
            InitializeComponent();
        }

        public void Load()
        {
            cbTaskTimeMode.Items.Add(TIMER_CURRENT_EVENT);
            cbTaskTimeMode.Items.Add(TIMER_THIS_TASK);
            cbTaskTimeMode.Items.Add(TIMER_TOTAL);
            cbTaskTimeMode.SelectedIndex = 1;
            cbTaskTimeMode.SelectedValueChanged += cbTaskTimeMode_SelectedValueChanged;
        }

        public void SetTime(TaskItem t)
        {
            TimeSpan timeToDisplay = TimeSpan.Zero;
            switch (cbTaskTimeMode.SelectedItem)
            {
                case TIMER_CURRENT_EVENT:
                    //Console.WriteLine("TIMER_CURRENT_EVENT");
                    timeToDisplay = t.TaskSeriesItem.Current.Elapsed;
                    break;
                case TIMER_THIS_TASK:
                    //Console.WriteLine("TIMER_THIS_TASK");
                    timeToDisplay = t.TaskSeriesItem.Elapsed;
                    break;
                case TIMER_TOTAL:
                    //Console.WriteLine("TIMER_TOTAL");
                    timeToDisplay = t.ElapsedTotal;
                    break;
                default:
                    Console.WriteLine("[ERROR] SetTaskTimer: DEFAULT");
                    break;
            }

            DisplayTime(timeToDisplay);
        }

        private void SetLblDigit(LinkLabel label, int digit)
        {
            label.Text = Utility.DisplayDigitWithZero(digit);
        }

        private void DisplayTime(TimeSpan timeToDisplay)
        {
            SetLblDigit(lblDigitDays, timeToDisplay.Days);
            SetLblDigit(lblDigitHours, timeToDisplay.Hours);
            SetLblDigit(lblDigitMinutes, timeToDisplay.Minutes);
            SetLblDigit(lblDigitSeconds, timeToDisplay.Seconds);
        }
    }

    public class TaskTimeControl_TypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
