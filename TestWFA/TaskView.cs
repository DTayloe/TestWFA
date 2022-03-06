using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;

namespace TestWFA
{
     public partial class TaskView : Form
     {
          private TaskController _controller = null;

          public const string TIMER_CURRENT_EVENT = "Current Event";
          public const string TIMER_THIS_TASK = "This Task";
          public const string TIMER_TOTAL = "This Task + Subtasks";

          public TaskView()
          {
               InitializeComponent();
               menuStrip1.Renderer = new MyRenderer();

               this.Text = "Stopped";

               // current running directory of app
               //https://stackoverflow.com/a/930832/3752444
               Console.WriteLine(Directory.GetParent(Assembly.GetExecutingAssembly().Location));
               Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);

               openFileDialog1.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
               openFileDialog1.Filter = "XML files (*.xml)|*.xml";

               timerRefresh.Tick += new EventHandler(UpdateViewTaskTimer);

               // initialize columns
               {
                    DataGridViewTextBoxColumn dgtc = new DataGridViewTextBoxColumn();
                    dgtc.DataPropertyName = "StartingTime";
                    dgtc.Name = "Starting Time";
                    dgvTaskEventHistory.Columns.Add(dgtc);
               }
               {
                    DataGridViewTextBoxColumn dgtc = new DataGridViewTextBoxColumn();
                    dgtc.DataPropertyName = "EndingTime";
                    dgtc.Name = "Ending Time";
                    dgvTaskEventHistory.Columns.Add(dgtc);
               }
               {
                    DataGridViewTextBoxColumn dgtc = new DataGridViewTextBoxColumn();
                    dgtc.DataPropertyName = "ElapsedDisplay";
                    dgtc.Name = "Elapsed";
                    dgvTaskEventHistory.Columns.Add(dgtc);
               }
               {
                    DataGridViewTextBoxColumn dgtc = new DataGridViewTextBoxColumn();
                    dgtc.DataPropertyName = "State";
                    dgtc.Name = "State";
                    dgvTaskEventHistory.Columns.Add(dgtc);
               }

               cbTaskTimeMode.Items.Add(TIMER_CURRENT_EVENT);
               cbTaskTimeMode.Items.Add(TIMER_THIS_TASK);
               cbTaskTimeMode.Items.Add(TIMER_TOTAL);
               cbTaskTimeMode.SelectedIndex = 1;
               cbTaskTimeMode.SelectedValueChanged += cbTaskTimeMode_SelectedValueChanged;

               //treeViewTasks.contex

               ClearAllViewData();

               //treeViewTasks.HideSelection = true;
          }

          public void UnsavedChanges(bool unsavedChangesExist)
          {
               toolStripMenuItemSave.Enabled = unsavedChangesExist;
          }

          public void SetController(TaskController controller)
          {
               _controller = controller;
          }

          private void btnCollapseAll_Click(object sender, EventArgs e)
          {
               treeViewTasks.CollapseAll();
               UpdateViewTaskPanel();
          }

          private void btnExpandAll_Click(object sender, EventArgs e)
          {
               treeViewTasks.ExpandAll();
          }

          private void btnNewTask_Click(object sender, EventArgs e)
          {
               SelectAndEdit(InsertSubtaskOnNode(GetParentTreeNodeFromSelection()));

          }

          private void btnNewTaskSub_Click(object sender, EventArgs e)
          {
               SelectAndEdit(InsertSubtaskOnNode(treeViewTasks.SelectedNode));
          }

          /// <summary>
          /// Accepts the current state of the task
          /// </summary>
          /// <param name="state"></param>
          public void UpdateBtnStartStopState(TaskEventState state)
          {
               //TaskSeries.PrintState(state);
               switch (state)
               {
                    case TaskEventState.TaskEventNew:
                    case TaskEventState.TaskEventComplete:
                         btnTaskStartStop.Text = "Start Task";
                         btnTaskStartStop.BackColor = SystemColors.Control;
                         btnTaskStartStop.ForeColor = SystemColors.ControlText;
                         break;
                    case TaskEventState.TaskEventRunning:
                         // the item is now running
                         btnTaskStartStop.Text = "Stop Task";
                         btnTaskStartStop.BackColor = Color.DarkRed;
                         btnTaskStartStop.ForeColor = Color.White;
                         break;
                    default:
                         break;
               }
          }

          //public static void UpdateTreeNodeRunningState()

          public void UpdateTreeNodeRunningState(TaskItem task, TreeNode node)
          {
               switch (task.TaskSeriesItem.State)
               {
                    case TaskEventState.TaskEventNew:
                    case TaskEventState.TaskEventComplete:
                         node.BackColor = SystemColors.Window;
                         node.ForeColor = SystemColors.WindowText;
                         break;
                    case TaskEventState.TaskEventRunning:
                         node.BackColor = Color.DarkRed;
                         node.ForeColor = Color.White;
                         break;
                    default:
                         break;
               }
          }

          public void SetLblDigit(LinkLabel label, int digit)
          {
               label.Text = Utility.DisplayDigitWithZero(digit);
          }

          public string TextBoxTask
          {
               get
               {
                    return txtTask.Text;
               }

               set
               {
                    txtTask.Text = value;
               }
          }

          private void SetTaskTimer(TaskItem t)
          {
               TimeSpan timeToDisplay = TimeSpan.Zero;
               switch (cbTaskTimeMode.SelectedItem)
               {
                    case TIMER_CURRENT_EVENT:
                         //Console.WriteLine("TIMER_CURRENT_EVENT");
                         timeToDisplay = t.TaskSeriesItem.TaskEvents.Last().Elapsed;
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

               SetLblDigit(lblDigitDays, timeToDisplay.Days);
               SetLblDigit(lblDigitHours, timeToDisplay.Hours);
               SetLblDigit(lblDigitMinutes, timeToDisplay.Minutes);
               SetLblDigit(lblDigitSeconds, timeToDisplay.Seconds);
               UpdateBtnStartStopState(t.TaskSeriesItem.State);
          }

          private void btnTaskStartStop_Click(object sender, EventArgs e)
          {
               TaskItem task = _controller.FindTaskItemByID(GetIDFromSelection());
               if (task != null)
               {
                    // XXX I may not even need this switch! Could I just pass in the state to the methods that need it...?
                    switch (task.TaskSeriesItem.State)
                    {
                         case TaskEventState.TaskEventNew:
                         case TaskEventState.TaskEventComplete:
                              Console.WriteLine("Starting...");
                              // item is currently new

                              // XXX stop others, will change with settings in future
                              if (true)
                              {
                                   foreach (TaskItem item in _controller.GetRunningTasks())
                                   {
                                        if (item.TaskSeriesItem.State == TaskEventState.TaskEventRunning)
                                        {
                                             item.TaskSeriesItem.Stop();
                                             UpdateTreeNodeRunningState(item, FindTreeNodeByTask(item));
                                             Console.WriteLine($"Stopped [{item.Name},{item.ID}]");
                                        }
                                   }
                              }

                              UpdateBtnStartStopState(task.TaskSeriesItem.Start());
                              UpdateTreeNodeRunningState(task, treeViewTasks.SelectedNode);
                              //UpdateViewTaskbarTitle(task);
                              break;
                         case TaskEventState.TaskEventRunning:
                              Console.WriteLine("Stopping...");
                              // item is currently running
                              UpdateBtnStartStopState(task.TaskSeriesItem.Stop());
                              UpdateTreeNodeRunningState(task, treeViewTasks.SelectedNode);
                              //UpdateViewTaskbarTitle(task);
                              break;
                    }

                    UpdateViewTaskbarTitle();

                    _controller.UnsavedChanges = true;

                    UpdateViewTaskPanel();
               }
          }

          private void cbTaskTimeMode_SelectedValueChanged(object sender, EventArgs e)
          {
               TaskItem task = _controller.FindTaskItemByID(GetIDFromSelection());
               if (task != null)
               {
                    UpdateViewTaskTimer(null, null);
               }
          }

          private void btnFocusRunning_Click(object sender, EventArgs e)
          {
               List<TaskItem> running = _controller.GetRunningTasks();
               if (running.Count > 0)
               {
                    treeViewTasks.CollapseAll();

                    TreeNode t = FindTreeNodeByTask(running[0]);
                    treeViewTasks.SelectedNode = t;
                    // XXX doesn't expand all running tasks, can easily do later if needed with below code
                    //TreeNode tParent = t.Parent;
                    //while (tParent != null)
                    //{
                    //     Console.WriteLine($"Parent: {tParent.Text}");
                    //     tParent.Expand();
                    //     tParent = tParent.Parent;
                    //}
               }
          }

          private void btnChooseFolder_Click(object sender, EventArgs e)
          {
               if (treeViewTasks.SelectedNode != null)
               {
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.IsFolderPicker = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                         string folder = dialog.FileName;
                         // XXX the exist check should go into code where opening it
                         if (Directory.Exists(folder))
                         {
                              Console.WriteLine(folder);

                              _controller.UpdateModelTaskFolder(GetIDFromSelection(), folder);

                              //UpdateFolderData(folder);
                              //SetDataTaskFolder(folder);
                              //UpdateViewTaskFolder();
                         }
                    }
               }
          }

          private void btnOpenFolder_Click(object sender, EventArgs e)
          {
               if (treeViewTasks.SelectedNode != null)
               {
                    TaskItem task = _controller.FindTaskItemByID(GetIDFromSelection());
                    if (task != null)
                    {
                         string folder = task.Folder;
                         if (Directory.Exists(folder))
                         {
                              if (true)
                              {
                                   // open a new folder location or existing
                                   Process.Start(folder);
                              }
                              else
                              {
                                   // open a new folder location
                                   Process.Start("explorer.exe", folder);
                              }
                         }
                    }
               }
          }

          /* 
          EEE this and next method show 2 different ways for updating the model... 
          - one where we retrieve a TaskItem and modify here in this class
               - the view now has to be aware of how the data is stored... is this bad?
          - one where we send the data to the model
               - extra methods are needed to get the data there (one in controller, one in model)
               - what if there are a crazy number of data items to be modified, and we need a parameter for each of them?
                    - maybe using the command pattern is the solution here
          - one that is a hybrid by making a method in the controller that does the retrieving like in the first way
               - extra method needed, when could just use the existing methods for retrieving
          */
          private void rtbTask_TextChanged(object sender, EventArgs e)
          {
               if (treeViewTasks.SelectedNode != null)
               {// XXX every single action / keystoke sends the entire contents of rtb to the model... this might not be super great
                    TaskItem task = _controller.FindTaskItemByID(GetIDFromSelection());
                    if (task != null)
                    {
                         TextBoxBase rtb = (TextBoxBase)sender;

                         Console.WriteLine("TEXT CHANGED [" + rtb.Text + "]");

                         if (task.Note != rtb.Text)
                         {// there are only changes needing saved if the rtb text does not match the model. Otherwise selecting a different task will trigger the save button
                              task.Note = rtb.Text;
                              _controller.UnsavedChanges = true;
                         }
                    }
               }
          }

          /*EEE*/
          //private void rtbTask_TextChanged(object sender, EventArgs e)
          //{
          //     if (treeViewTasks.SelectedNode != null)
          //     {// XXX every single action / keystoke sends the entire contents of rtb to the model... this might not be super great
          //          RichTextBox rtb = (RichTextBox)sender;
          //          Console.WriteLine("TEXT CHANGED [" + rtb.Text + "]");
          //          _controller.UpdateModelTaskNote(GetIDFromSelection(), rtb.Text);
          //     }
          //}

          private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
          {
               if (openFileDialog1.ShowDialog() == DialogResult.OK)
               {
                    _controller.LoadXmlFileToModel(openFileDialog1.FileName);
               }
          }

          private void toolStripMenuItemSave_Click(object sender, EventArgs e)
          {
               _controller.SaveXmlFileToDisk(askUserIfChangesToBeSaved: false);
          }

          private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
          {
               Console.WriteLine("settings!");
               //TaskViewSettings tvs = new TaskViewSettings();

               //tvs.ShowDialog();
          }

          private void resetTaskEventHistoryToolStripMenuItem_Click(object sender, EventArgs e)
          {
               Console.WriteLine("Resetting history");
               _controller.ResetTaskEventHistory(GetIDFromSelection(), false);
          }

          private void resetTaskEventHistoryAndChildrenToolStripMenuItem_Click(object sender, EventArgs e)
          {
               Console.WriteLine("Resetting history and children");
               _controller.ResetTaskEventHistory(GetIDFromSelection(), true);
          }

          private void treeViewTasks_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
          {
               if (e.Button == MouseButtons.Right && e.Node.IsSelected)
               {
                    Console.WriteLine("RIGHT CLICK only on selected MAN " + e.Node.Tag);
                    cmsTreeView.Show(treeViewTasks, new Point(e.X, e.Y));
               }
          }

          /// <summary>
          /// This event listener is fired after the editing has completed.
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
          private void treeViewTasks_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
          {
               if (e.Label != null)
               {
                    if (e.Label.Trim().Length > 0)
                    {
                         e.Node.EndEdit(false);

                         // need to use this since the treeview is only changed after the method ends. This adds to the current thread a task to complete.
                         this.BeginInvoke((MethodInvoker)delegate
                         {

                              // XXX change below to more general update method that updates everything... maybe even in the controller... then could fire that off and update model
                              _controller.UpdateModelTaskName((int)e.Node.Tag, e.Label.Trim());
                              UpdateViewTaskPanel();
                         });
                    }
                    else
                    {
                         e.CancelEdit = true;
                         Console.WriteLine("[ERROR] TaskView.treeViewTasks_AfterLabelEdit: label was empty");
                         e.Node.BeginEdit();
                    }
               }
               else
               {
                    e.CancelEdit = true;
                    Console.WriteLine("[INFO] TaskView.treeViewTasks_AfterLabelEdit: label was null, clicked off/no change after edit mode");
                    e.Node.EndEdit(false);
               }
          }

          private void treeViewTasks_MouseDown(object sender, MouseEventArgs e)
          {
               Console.WriteLine("TEST");
          }

          private void treeViewTasks_KeyDown(object sender, KeyEventArgs e)
          {
               Console.WriteLine("treeViewTasks_KeyDown");
               //Console.WriteLine(Utility.DanPropertyList(e));
          }

          private void treeViewTasks_KeyPress(object sender, KeyPressEventArgs e)
          {
               Console.WriteLine("treeViewTasks_KeyPress");
               //Console.WriteLine(Utility.DanPropertyList(e));

          }

          private void treeViewTasks_KeyUp(object sender, KeyEventArgs e)
          {
               Console.WriteLine("treeViewTasks_KeyUp");
               //Console.WriteLine(Utility.DanPropertyList(e));
               if (e.KeyCode == Keys.Delete)
               {
                    _controller.RemoveNode(GetIDFromSelection());
               }
          }

          private void treeViewTasks_AfterSelect(object sender, TreeViewEventArgs e)
          {
               UpdateViewTaskPanel();
          }

          public void UpdateViewTaskPanel()
          {
               UpdateViewCheckEnabledRightPanel();
               UpdateViewGroupBoxSelectedTaskText();
               _controller.UpdateViewTaskFolder(GetIDFromSelection());
               UpdateViewTaskTimer(null, null);
               timerRefresh.Start();
               UpdateViewTaskNote();
               UpdateViewTaskHistory();
               UpdateViewTaskbarTitle();
          }

          private void UpdateViewCheckEnabledRightPanel()
          {
               if (treeViewTasks.SelectedNode == null)
               {
                    groupBoxSelectedTask.Enabled = false;
                    ClearPaneSelectedTask();
               }
               else
               {
                    groupBoxSelectedTask.Enabled = true;
               }
          }

          private void UpdateViewTaskbarTitle()
          {
               List<TaskItem> runningTasks = _controller.GetRunningTasks();
               int count = runningTasks.Count;
               Console.WriteLine($"Running count: {count}");

               if (count == 0)
               {
                    this.Text = "Stopped";
               }
               else if (count == 1)
               {
                    this.Text = "\u25B6" + " [" + runningTasks[0].Name + "]";
                    //if (task != null)
                    //{
                    //     switch (task.TaskSeriesItem.State)
                    //     {
                    //          case TaskEventState.TaskEventNew:
                    //          case TaskEventState.TaskEventComplete:
                    //               this.Text = "Stopped";
                    //               break;
                    //          case TaskEventState.TaskEventRunning:
                    //               this.Text = "\u25B6" + " [" + task.Name + "]";
                    //               break;
                    //          default:
                    //               break;
                    //     }
                    //}
                    //else
                    //{
                    //     this.Text = "Stopped";
                    //}
               }
               else
               {// multiple running tasks
                    this.Text = "\u25B6" + " [" + count + " running]";
               }


          }

          public void UpdateViewCurrentFilePath(string currentFilePath)
          {
               toolStripMenuItemFilePath.Text = currentFilePath;
          }

          private void UpdateViewGroupBoxSelectedTaskText()
          {
               if (treeViewTasks.SelectedNode != null)
               {
                    groupBoxSelectedTask.Text = treeViewTasks.SelectedNode.FullPath;
               }
          }

          /// <summary>
          /// Update view to reflect what is selected in the treeview
          /// Should this method:
          /// - check that the selected ID is correct, update view
          /// - find treenode with tag in list, update view
          /// </summary>
          /// <param name="taskItemID"></param>
          /// <param name="folder"></param>
          public void UpdateViewTaskFolder(int taskItemID, string folder)
          {
               if (taskItemID == GetIDFromSelection())
               {
                    txtFolderPath.Text = folder;
               }
               else
               {
                    Console.WriteLine("[ERROR] TaskView.UpdateTaskFolder: ID did not match selected node");
               }
          }

          public void UpdateViewTaskTimer(Object myObject, EventArgs myEventArgs)
          {
               TaskItem task = _controller.FindTaskItemByID(GetIDFromSelection());
               if (task != null)
               {
                    SetTaskTimer(task);
               }
          }

          public void UpdateViewTaskHistory()
          {
               TaskItem task = _controller.FindTaskItemByID(GetIDFromSelection());
               if (task != null)
               {
                    // populate datasource
                    BindingSource bs = new BindingSource();

                    foreach (TaskEvent item in task.TaskSeriesItem.TaskEvents)
                    {
                         // XXX eventually once bugs worked out, only completed or errored tasks should be allowed here???
                         bs.Add(item);
                    }

                    // initialize DGV
                    dgvTaskEventHistory.AutoGenerateColumns = false;
                    dgvTaskEventHistory.DataSource = bs;
               }
          }

          public void UpdateViewTaskNote()
          {
               TaskItem task = _controller.FindTaskItemByID(GetIDFromSelection());
               if (task != null)
               {
                    TextBoxTask = task.Note;
               }
          }

          #region MVC methods

          public TaskItem GetTaskItemFromID(int taskItemID)
          {
               return _controller.FindTaskItemByID(taskItemID);
          }

          public int GetIDFromSelection()
          {
               int result = -1;
               if (treeViewTasks.SelectedNode != null)
               {
                    result = (int)treeViewTasks.SelectedNode.Tag;
               }

               return result;
          }

          private TreeNode InsertSubtaskOnNode(TreeNode insertionParent)
          {
               TaskItem result = null;

               if (insertionParent != null)
               {
                    int taskItemID = (int)(insertionParent.Tag);
                    result = GetTaskItemFromID(taskItemID);
               }

               TaskItem newTask = new TaskItem("New Task");
               _controller.AddSubTask(result, newTask);

               return FindTreeNodeByTask(newTask);     // XXX inefficient... could not return treenode because of controller/MVC, how to resolve?
          }

          public TreeNode FindTreeNodeByTask(TaskItem task)
          {
               if (task != null)
               {
                    return FindTreeNodeByTaskID(treeViewTasks.Nodes, task.ID);
               }
               else
               {
                    return null;
               }
          }

          private TreeNode FindTreeNodeByTaskID(TreeNodeCollection testNodes, int treeViewTaskItemID)
          {
               TreeNode result = null;
               for (int i = 0; i < testNodes.Count; i++)
               {
                    int testID = (int)testNodes[i].Tag;
                    if (testID == treeViewTaskItemID)
                    {
                         result = testNodes[i];
                         break;
                    }
                    else
                    {
                         result = FindTreeNodeByTaskID(testNodes[i].Nodes, treeViewTaskItemID);
                         if (result != null)
                         {
                              // we found something!
                              break;
                         }
                    }
               }

               return result;
          }

          public TreeNode GetParentTreeNodeFromSelection()
          {
               TreeNode result = null;
               if (treeViewTasks.SelectedNode != null)
               {
                    if (treeViewTasks.SelectedNode.Parent != null)
                    {
                         result = treeViewTasks.SelectedNode.Parent;
                    }
                    else
                    {
                         // the result is null since there was no parent
                    }
               }
               else
               {
                    // the result is null since there is no selected node
               }

               return result;
          }

          public void ClearAllViewData()
          {
               treeViewTasks.Nodes.Clear();
               ClearPaneSelectedTask();
               groupBoxSelectedTask.Enabled = false;
          }

          public void ClearPaneSelectedTask()
          {
               groupBoxSelectedTask.Text = "SELECTED TASK NAME HERE";
               SetTaskTimer(new TaskItem());
               TextBoxTask = "";
               txtFolderPath.Text = "";
               ClearViewTaskHistory();
          }

          public void ClearViewTaskHistory()
          {
               // populate datasource
               BindingSource bs = new BindingSource();

               // initialize DGV
               dgvTaskEventHistory.AutoGenerateColumns = false;
               dgvTaskEventHistory.DataSource = bs;
          }

          public void RemoveTask(int taskItemID)
          {
               TreeNode result = FindTreeNodeByTaskID(treeViewTasks.Nodes, taskItemID);
               if (result != null)
               {
                    result.Remove();
               }
               else
               {
                    Console.WriteLine($"[ERROR] TaskView.RemoveTask: could not delete taskID [{taskItemID}]");
               }
          }

          public void ErrorScan()
          {
               SortedSet<int> testSetView = new SortedSet<int>();
               ErrorScan(testSetView, treeViewTasks.Nodes);
          }

          private void ErrorScan(SortedSet<int> testSet, TreeNodeCollection nodes)
          {
               foreach (TreeNode item in nodes)
               {
                    int before = testSet.Count;
                    testSet.Add((int)item.Tag);

                    if (testSet.Count != before + 1)
                    {
                         Console.WriteLine($"[CRITICAL] TaskView.ErrorScan: Duplicate tag [{(int)item.Tag}][{item.Text}]");
                    }

                    ErrorScan(testSet, item.Nodes);
               }
          }

          public void SelectAndEdit(TreeNode editNode)
          {
               treeViewTasks.SelectedNode = editNode;
               if (!treeViewTasks.SelectedNode.IsEditing)
               {
                    treeViewTasks.SelectedNode.BeginEdit();
               }
          }

          /// <summary>
          /// Add to a parent task a subtask 
          /// </summary>
          /// <param name="parentTask">Use null if there are no parents, it will then be added to the root tasks.</param>
          /// <param name="newTask">A new task to add to the parent.</param>
          public void AddSubTask(TaskItem parentTask, TaskItem newTask)
          {
               TreeNode foundParent = FindTreeNodeByTask(parentTask);
               Console.WriteLine("Par: " + parentTask + "\tChi: " + newTask);
               TreeNodeCollection subtaskList = foundParent != null ? foundParent.Nodes : treeViewTasks.Nodes;
               TreeNode newNode = subtaskList.Add(newTask.Name);
               newNode.Tag = newTask.ID;
               UpdateTreeNodeRunningState(newTask, newNode);

               // cannot add paramerter... i guess i need to have an editing mode that can tell when the new task button has been clicked
               //if (selectAndEditAfterAdd)
               //{

               //}


               //if (parentTask != null)
               //{
               //     // find the parent task in the tree
               //     TreeNode result = FindTreeNodeByTask(parentTask);

               //     if (result != null)
               //     {// it exists in the existing view
               //          TreeNode addedNode = result.Nodes.Add(newTask.Name);
               //          addedNode.Tag = newTask.ID;
               //     }
               //     else
               //     {// if it doesn't exist in the existing view, add it
               //          //Console.WriteLine($"[ERROR] TaskView.AddSubTask: Could not find task with ID [{parentTask.ID}]");
               //     }
               //}
               //else
               //{
               //     // there were no matching tasks, put it in the root tasks
               //     TreeNode addedNode = treeViewTasks.Nodes.Add(newTask.Name);
               //     addedNode.Tag = newTask.ID;
               //}
          }


          #endregion

          /// <summary>
          /// Fixes the menuitems staying selected after mouse exits the menuStrip
          /// https://stackoverflow.com/a/47241407/3752444
          /// </summary>
          private class MyRenderer : ToolStripProfessionalRenderer
          {
               public MyRenderer() : base(new MyColorTable()) { }
               protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
               {
                    base.OnRenderMenuItemBackground(e);
                    if (e.Item.Enabled)
                    {
                         using (Pen p = new Pen(((MyColorTable)ColorTable).MenuItemEnabledAlert))
                         {
                              Rectangle r = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                              e.Graphics.DrawRectangle(p, r);
                         }
                    }
               }

               private class MyColorTable : ProfessionalColorTable
               {
                    public override Color MenuItemBorder { get { return Color.Transparent; } }
                    public Color MenuItemEnabledBorder { get { return base.MenuItemBorder; } }
                    public Color MenuItemEnabledAlert { get { return Color.Red; } }
               }
          }

          private void TaskView_FormClosing(object sender, FormClosingEventArgs e)
          {
               e.Cancel = !_controller.SaveXmlFileToDisk(askUserIfChangesToBeSaved: true);
          }

          private void txtTask_KeyDown(object sender, KeyEventArgs e)
          {
               if (e.Control && e.KeyCode == Keys.V)
               {
                    ((RichTextBox)sender).Paste(DataFormats.GetFormat("Text"));
                    e.Handled = true;
               }
          }
     }
}
