using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWFA
{
     static class Program
     {
          /// <summary>
          /// The main entry point for the application.
          /// </summary>
          [STAThread]
          static void Main()
          {
               Application.EnableVisualStyles();
               Application.SetCompatibleTextRenderingDefault(false);

               TaskView _view = new TaskView();
               _view.Visible = false;

               TaskModel _model = new TaskModel();
               
               TaskController _controller = new TaskController(_view, _model);

               Application.Run(_view);
          }
     }
}
