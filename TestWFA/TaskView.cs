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

namespace TestWFA
{
     public partial class TaskView : Form
     {
          private TaskController _controller = null;
          
          public TaskView()
          {
               InitializeComponent();
               menuStrip1.Renderer = new MyRenderer();

               // current running directory of app
               //https://stackoverflow.com/a/930832/3752444
               Console.WriteLine(Directory.GetParent(Assembly.GetExecutingAssembly().Location));
               Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);

               openFileDialog1.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
               openFileDialog1.Filter = "XML files (*.xml)|*.xml";

               timerRefresh.Tick += new EventHandler(UpdateViewTaskTimer);
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
          public void SetBtnStartStopState(TaskEventState state)
          {
               TaskSeries.PrintState(state);
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

          public void SetLblDigit(LinkLabel label, int digit)
          {
               label.Text = digit < 10 ? "0" + digit.ToString() : digit.ToString();
          }

          private void btnTaskStartStop_Click(object sender, EventArgs e)
          {
               TaskItem t = _controller.FindTaskItemByID(GetIDFromSelection());
               if (t != null)
               {
                    switch (t.TaskSeriesItem.State)
                    {
                         case TaskEventState.TaskEventNew:
                         case TaskEventState.TaskEventComplete:
                              Console.WriteLine("Starting...");
                              // item is currently new
                              SetBtnStartStopState(t.TaskSeriesItem.Start());
                              break;
                         case TaskEventState.TaskEventRunning:
                              Console.WriteLine("Stopping...");
                              // item is currently running
                              SetBtnStartStopState(t.TaskSeriesItem.Stop());
                              break;
                    }

                    _controller.UnsavedChanges = true;
               }
          }

          private void btnFocusRunning_Click(object sender, EventArgs e)
          {
               if (treeViewTasks.SelectedNode != null)
               {
                    //treeViewTasks.GetNodeAt
               }
          }

          private void btnChooseFolder_Click(object sender, EventArgs e)
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

          private void btnOpenFolder_Click(object sender, EventArgs e)
          {
               if (treeViewTasks.SelectedNode != null)
               {
                    string folder = _controller.FindTaskItemByID(GetIDFromSelection()).Folder;
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

          private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
          {
               if (openFileDialog1.ShowDialog() == DialogResult.OK)
               {
                    _controller.LoadXmlFileToModel(openFileDialog1.FileName);
               }
          }

          private void toolStripMenuItemSave_Click(object sender, EventArgs e)
          {
               _controller.SaveXmlFileToDisk(askUserIfChangesToBeSaved:false);
          }

          /// <summary>
          /// This event listener is fired after the editing has completed.
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
          private void treeViewTasks_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
          {
               if(e.Label != null)
               {
                    if(e.Label.Trim().Length > 0)
                    {
                         e.Node.EndEdit(false);

                         // need to use this since the treeview is only changed after the method ends. This adds to the current thread a task to complete.
                         this.BeginInvoke((MethodInvoker)delegate {

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
                    //e.Node.BeginEdit();
                    e.Node.EndEdit(false);
               }
          }

          private void treeViewTasks_MouseDown(object sender, MouseEventArgs e)
          {

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
                    //treeViewTasks.SelectedNode.Remove();
                    
                    _controller.RemoveNode(GetIDFromSelection());
               }
          }

          private void treeViewTasks_AfterSelect(object sender, TreeViewEventArgs e)
          {
               UpdateViewTaskPanel();
          }

          //need to think how ysing this... will erase anything youve typed in rtf box
          private void UpdateViewTaskPanel()
          {

               //SAVE DATA furst!!!

               UpdateViewGroupBoxSelectedTaskText();
               _controller.UpdateViewTaskFolder(GetIDFromSelection());
               UpdateViewTaskTimer(null, null);
               timerRefresh.Start();
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
               TaskItem t = _controller.FindTaskItemByID(GetIDFromSelection());
               if (t != null)
               {
                    /* XXX this is wrong... need 2 different elapsed timers, one for task item, then one for the only 
                     * running task item. also will need to ensure there is only one running task
                    */               
                    SetLblDigit(lblDigitDays, t.TaskSeriesItem.Elapsed.Days);
                    SetLblDigit(lblDigitHours, t.TaskSeriesItem.Elapsed.Hours);
                    SetLblDigit(lblDigitMinutes, t.TaskSeriesItem.Elapsed.Minutes);
                    SetLblDigit(lblDigitSeconds, t.TaskSeriesItem.Elapsed.Seconds);
                    SetBtnStartStopState(t.TaskSeriesItem.State);
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

          public void ClearData()
          {
               treeViewTasks.Nodes.Clear();
          }

          public void RemoveTask(int taskItemID)
          {
               TreeNode result = FindTreeNodeByTaskID(treeViewTasks.Nodes, taskItemID);
               if (result!=null)
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
               Console.WriteLine("Par: "+parentTask+"\tChi: "+newTask);
               TreeNodeCollection subtaskList = foundParent != null ? foundParent.Nodes : treeViewTasks.Nodes;
               TreeNode newNode = subtaskList.Add(newTask.Name);
               newNode.Tag = newTask.ID;

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
                    if(e.Item.Enabled && e.Item.Selected)
                    {
                         using (Pen p = new Pen(((MyColorTable)ColorTable).MenuItemEnabledBorder))
                         {
                              Rectangle r = new Rectangle(0, 0, e.Item.Width-1, e.Item.Height-1);
                              e.Graphics.DrawRectangle(p, r);
                         }
                    }
               }

               private class MyColorTable : ProfessionalColorTable
               {
                    public override Color MenuItemBorder { get { return Color.Transparent; } }
                    public Color MenuItemEnabledBorder { get { return base.MenuItemBorder; } }
               }
          }

          private void TaskView_FormClosing(object sender, FormClosingEventArgs e)
          {
               e.Cancel = !_controller.SaveXmlFileToDisk(askUserIfChangesToBeSaved:true);
          }
     }
}
