using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWFA
{
     public partial class TaskViewSettings : Form
     {
          public TaskViewSettings(TaskViewSettingsValues values)
          {
               InitializeComponent();
               cbEnableParallelTasks.CheckedChanged += CbEnableParallelTasks_CheckedChanged;
               //clbSettings.ite
          }

          private void CbEnableParallelTasks_CheckedChanged(object sender, EventArgs e)
          {
               //throw new NotImplementedException();
          }

          public void SetEnableParallelTasks(bool value)
          {
               cbEnableParallelTasks.Checked = value;
          }

          public class TaskViewSettingsValues
          {
               
          }
     }
}
