using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TaskFrackerControlLibrary.Design
{
    [ToolboxItemFilter("TaskTimeControlLibrary.TestA", ToolboxItemFilterType.Require)]
    [ToolboxItemFilter("TaskTimeControlLibrary.TestB", ToolboxItemFilterType.Require)]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class TaskTimeControlRootDesigner: DocumentDesigner
    {

    }
}
