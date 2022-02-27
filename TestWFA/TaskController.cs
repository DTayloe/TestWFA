using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TestWFA
{
     public class TaskController
     {
          private TaskView _view;
          private TaskModel _model;

          private bool _unsavedChanges = false;
          public bool UnsavedChanges
          {
               get
               {
                    return _unsavedChanges;
               }
               set
               {
                    Console.WriteLine($"[INFO] TaskController.UnsavedChanges: SET to {value}");
                    _unsavedChanges = value;
                    _view.UnsavedChanges(value);
               }
          }

          //public TaskViewSettingsView.TaskViewSettingsValues TaskViewSettingsValue { get; set; }

          public void UpdateViewCurrentFilePath(string currentFilePath)
          {
               _view.UpdateViewCurrentFilePath(currentFilePath);
          }

          private enum NodeTypes
          {
               tasks,
               task,
               task_name,
               task_folder,
               task_note,
               task_series,
               task_event,
               task_event_starting,
               task_event_ending
          }

          public TaskController(TaskView view, TaskModel model)
          {
               _model = model;
               _view = view;
               _model.SetController(this);
               _view.SetController(this);

               LoadModelToView();
          }
          
          /// <summary>
          /// True for continue / success, false for cancel / failure
          /// </summary>
          /// <param name="askUserIfChangesToBeSaved">Gives user option to cancel the save</param>
          /// <returns></returns>
          public bool SaveXmlFileToDisk(bool askUserIfChangesToBeSaved)
          {
               if (UnsavedChanges)
               {
                    DialogResult result = DialogResult.Yes;
                    if (askUserIfChangesToBeSaved)
                    {
                         result = MessageBox.Show("Do you wish to save your changes?", "Unsaved changes", MessageBoxButtons.YesNoCancel);
                    }

                    if (result == DialogResult.Yes)
                    {// where do we want to save those changes?
                         if (_model.CurrentFilePath == null)
                         {// we have never saved
                              SaveFileDialog saveDialog = new SaveFileDialog();
                              saveDialog.Filter = "XML files (*.xml)|*.xml";
                              saveDialog.DefaultExt = "xml";
                              saveDialog.AddExtension = true;
                              if (saveDialog.ShowDialog() == DialogResult.OK)
                              {
                                   _model.CurrentFilePath = saveDialog.FileName;
                              }
                              else
                              {
                                   Console.WriteLine("[INFO] TaskController.SaveXmlFileToDisk: not saved; user cancelled save");
                                   return false;
                              }
                         }
                    }
                    else if (result == DialogResult.No)
                    {
                         Console.WriteLine("[INFO] TaskController.SaveXmlFileToDisk: not saved; user did not want changes saved");
                         return true;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                         Console.WriteLine("[INFO] TaskController.SaveXmlFileToDisk: not saved; user did not want changes saved - cancelled");
                         return false;
                    }
               }
               else
               {
                    Console.WriteLine("[INFO] TaskController.SaveXmlFileToDisk: not saved; no opened files or changes");
                    return true;
               }
               
               // begin the save
               XmlTextWriter xtw = new XmlTextWriter(_model.CurrentFilePath, Encoding.UTF8);
               xtw.Formatting = Formatting.Indented;
               xtw.WriteStartDocument();


               xtw.WriteStartElement("tasks");

               RecurseXmlTasks(_model.Tasks, xtw);

               xtw.WriteEndElement();


               xtw.WriteEndDocument();
               xtw.Close();

               UnsavedChanges = false;

               return true;
          }

          private void RecurseXmlTasks(TaskItem tasks, XmlTextWriter xtw)
          {
               for (int i = 0; i < tasks.Count; i++)
               {
                    TaskItem task = tasks[i];
                    xtw.WriteStartElement(NodeTypes.task.ToString());

                    xtw.WriteStartElement(NodeTypes.task_name.ToString());
                    //xtw.WriteString(System.Security.SecurityElement.Escape(task.Name));
                    xtw.WriteString(task.Name); // XXX unescaped version 
                    xtw.WriteEndElement();

                    xtw.WriteStartElement(NodeTypes.task_folder.ToString());
                    //xtw.WriteString(System.Security.SecurityElement.Escape(task.Folder));
                    xtw.WriteString(task.Folder);// XXX unescaped version 
                    xtw.WriteEndElement();

                    xtw.WriteStartElement(NodeTypes.task_series.ToString());
                    task.TaskSeriesItem.WriteXmlToSave(xtw);
                    xtw.WriteEndElement();

                    xtw.WriteStartElement(NodeTypes.task_note.ToString());
                    xtw.WriteCData(System.Security.SecurityElement.Escape(task.Note));
                    xtw.WriteEndElement();

                    Console.WriteLine(task.Name);
                    RecurseXmlTasks(task, xtw);

                    xtw.WriteEndElement();
               }
          }

          public void LoadXmlFileToModel(string file)
          {
               if (!SaveXmlFileToDisk(askUserIfChangesToBeSaved:true))
               {// false has been returned indicating failure
                    // canceled
                    return;
               }

               // clear data
               ClearAllData();
               
               if (File.Exists(file))
               {
                    _model.CurrentFilePath = file;

                    Console.WriteLine(file);
                    XmlReaderSettings xrs = new XmlReaderSettings();
                    xrs.IgnoreWhitespace = true;
                    XmlReader xr = XmlReader.Create(file, xrs);
                    
                    NodeTypes currentNodeType = NodeTypes.tasks;
                    Stack<TaskItem> currentTask = new Stack<TaskItem>();
                    string indent = "";
                    bool emptyFlag = false;
                    while (xr.Read())
                    {
                         emptyFlag = false;
                         switch (xr.NodeType)
                         {
                              case XmlNodeType.None:
                                   break;
                              case XmlNodeType.Element:
                                   if (xr.IsEmptyElement)
                                   {
                                        Console.Write("empty --> ");
                                        emptyFlag = true;
                                   }
                                   else
                                   {
                                        // save the type of node (and prepare to load a task, etc) so we know what type later on when inside the text node (between the xml beginning and ending tags)
                                        switch (xr.Name)
                                        {
                                             case nameof(NodeTypes.tasks):
                                                  {
                                                       Console.WriteLine();
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task):
                                                  {
                                                       Console.WriteLine();
                                                       indent += "\t";

                                                       currentNodeType = NodeTypes.task;
                                                       TaskItem t = new TaskItem();

                                                       TaskItem parentTask = null;
                                                       if (currentTask.Count != 0)
                                                       {
                                                            parentTask = currentTask.Peek();
                                                       }
                                                       _model.AddSubTask(parentTask, t);            // add to the parent the new task in the model. This saves the reference to the object so that when popped off the stack, we'll still have it
                                                       currentTask.Push(t);                         // push on the stack the new task so we can fill in other data, and later come back to it's parent task
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task_name):
                                                  {
                                                       currentNodeType = NodeTypes.task_name;
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task_folder):
                                                  {
                                                       currentNodeType = NodeTypes.task_folder;
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task_note):
                                                  {
                                                       currentNodeType = NodeTypes.task_note;
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task_series):
                                                  {
                                                       currentNodeType = NodeTypes.task_series;
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task_event):
                                                  {
                                                       currentNodeType = NodeTypes.task_event;
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task_event_starting):
                                                  {
                                                       currentNodeType = NodeTypes.task_event_starting;
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task_event_ending):
                                                  {
                                                       currentNodeType = NodeTypes.task_event_ending;
                                                  }
                                                  break;
                                             case "":
                                                  {

                                                  }
                                                  break;
                                             default:
                                                  break;
                                        }
                                   }

                                   Console.Write((xr.Name == "task" ? (emptyFlag ? "" : indent) : "") + "Name: " + xr.Name + ", ");
                                   break;
                              case XmlNodeType.Attribute:
                                   break;
                              case XmlNodeType.Text:
                                   {
                                        string text = xr.ReadContentAsString(); // MUST STORE THIS IN A VARIABLE... IT IS CONSUMED WHEN READ
                                        Console.Write("Text: " + text + ", ");
                                        switch (currentNodeType)
                                        {
                                             case NodeTypes.tasks:
                                                  {

                                                  }
                                                  break;
                                             case NodeTypes.task:
                                                  {

                                                  }
                                                  break;
                                             case NodeTypes.task_name:
                                                  {
                                                       //currentTask.Peek().Name = System.Net.WebUtility.HtmlDecode(text);  // XXX escaped version
                                                       currentTask.Peek().Name = text;
                                                       //Console.WriteLine($"HELLO [{text}][{xr.Value}][{xr.ReadContentAsString()}][{currentTask.Peek().Name}]");
                                                  }
                                                  break;
                                             case NodeTypes.task_folder:
                                                  {
                                                       //currentTask.Peek().Folder = System.Net.WebUtility.HtmlDecode(text);  // XXX escaped version
                                                       currentTask.Peek().Folder = text;
                                                       //Console.WriteLine($"HELLO [{text}][{xr.Value}][{xr.ReadContentAsString()}][{currentTask.Peek().Name}]");
                                                  }
                                                  break;
                                             case NodeTypes.task_series:
                                                  {

                                                  }
                                                  break;
                                             case NodeTypes.task_event:
                                                  {

                                                  }
                                                  break;
                                             case NodeTypes.task_event_starting:
                                                  {
                                                       long longTime = 0;
                                                       if (long.TryParse(text, out longTime))
                                                       {
                                                            currentTask.Peek().TaskSeriesItem.AddTaskEvent(new TaskEvent(new DateTime(longTime)));
                                                       }
                                                       else
                                                       {
                                                            Console.WriteLine($"[ERROR] TaskController.LoadXmlFileToModel: taskeventstarting [{text}] from file could not be parsed");
                                                       }
                                                  }
                                                  break;
                                             case NodeTypes.task_event_ending:
                                                  {
                                                       long longTime = 0;
                                                       if (long.TryParse(text, out longTime))
                                                       {
                                                            currentTask.Peek().TaskSeriesItem.Current.EndingTime = new DateTime(longTime);
                                                       }
                                                       else
                                                       {
                                                            Console.WriteLine($"[ERROR] TaskController.LoadXmlFileToModel: taskeventending [{text}] from file could not be parsed");
                                                       }
                                                  }
                                                  break;
                                             default:
                                                  break;
                                        }
                                   }
                                   break;
                              case XmlNodeType.CDATA:
                                   {
                                        string text = xr.ReadContentAsString();

                                        switch (currentNodeType)
                                        {
                                             case NodeTypes.task_note:
                                                  {
                                                       Console.WriteLine($"FOUND CDATA RCAS [{text}]");
                                                       currentTask.Peek().Note = System.Net.WebUtility.HtmlDecode(text);
                                                  }
                                                  break;
                                             default:
                                                  break;
                                        }
                                   }
                                   break;
                              case XmlNodeType.EntityReference:
                                   break;
                              case XmlNodeType.Entity:
                                   break;
                              case XmlNodeType.ProcessingInstruction:
                                   break;
                              case XmlNodeType.Comment:
                                   break;
                              case XmlNodeType.Document:
                                   break;
                              case XmlNodeType.DocumentType:
                                   break;
                              case XmlNodeType.DocumentFragment:
                                   break;
                              case XmlNodeType.Notation:
                                   break;
                              case XmlNodeType.Whitespace:
                                   break;
                              case XmlNodeType.SignificantWhitespace:
                                   break;
                              case XmlNodeType.EndElement:
                                   Console.Write("\n" + indent + "End");
                                   indent = (indent.Length - 1) > 0 ? indent.Remove(indent.Length - 1) : "";

                                   switch (xr.Name)
                                   {
                                        case nameof(NodeTypes.task):
                                             currentTask.Pop();
                                             break;
                                        default:
                                             break;
                                   }

                                   break;
                              case XmlNodeType.EndEntity:
                                   break;
                              case XmlNodeType.XmlDeclaration:
                                   break;
                              default:
                                   break;
                         }
                    }

                    xr.Close();
                    Console.WriteLine();

                    LoadModelToView();

                    UnsavedChanges = false;

                    ErrorScan();
               }
          }

          public List<TaskItem> GetRunningTasks()
          {
               List<TaskItem> runningTasks = new List<TaskItem>();
               GetRunningTasks(runningTasks, _model.Tasks);

               return runningTasks;
          }

          private void GetRunningTasks(List<TaskItem> runningTasks, TaskItem task)
          {
               if (task.TaskSeriesItem.State == TaskEventState.TaskEventRunning)
               {
                    runningTasks.Add(task);
               }

               foreach (TaskItem item in task.SubTasks)
               {
                    GetRunningTasks(runningTasks, item);
               }
          }

          public void UpdateModelTaskNote(int taskItemID, string newNote)
          {
               TaskItem task = FindTaskItemByID(taskItemID);
               if (task != null)
               {
                    Console.WriteLine("TEXT CHANGED [" + newNote + "]");
                    task.Note = newNote;
               }
          }

          public void UpdateViewTaskFolder(int taskItemID)
          {
               /* 
               EEE compare to UpdateModelTaskNote... use _model to find task item or just model in this class? 
               Depends on if you even want/need that method in this class... is it just bad design to have it available 
               to the view through the controller, so we should just use the _model way?
               */
               TaskItem t = _model.FindTaskItem(taskItemID);
               if(t != null)
               {
                    _view.UpdateViewTaskFolder(taskItemID, t.Folder);
               }
               else
               {
                    Console.WriteLine($"[ERROR] TaskController.UpdateViewTaskFolder: task with id [{taskItemID}] not found");
               }
          }

          public void UpdateModelTaskFolder(int taskItemID, string folder)
          {
               _model.UpdateModelTaskFolder(taskItemID, folder);
               UnsavedChanges = true;
               UpdateViewTaskFolder(taskItemID);
          }

          /// <summary>
          /// Compare this to RemoveNode... which way is better, 
          /// having methods in model or controller?
          /// </summary>
          /// <param name="taskItemID"></param>
          /// <param name="newTaskName"></param>
          public void UpdateModelTaskName(int taskItemID, string newTaskName)
          {
               _model.FindTaskItem(taskItemID).Name = newTaskName;
               UnsavedChanges = true;
          }
          
          public void RemoveNode(int taskItemID)
          {
               _model.RemoveTask(taskItemID);
               _view.RemoveTask(taskItemID);
               _view.UpdateViewTaskPanel();
               UnsavedChanges = true;

               ErrorScan();
          }

          public TaskItem FindTaskItemByID(int taskItemID)
          {
               return _model.FindTaskItem(taskItemID);
          }
          
          public void LoadModelToView()
          {
               LoadTaskTreeToView(_model.Tasks);
          }

          private void LoadTaskTreeToView(TaskItem cursorTask)
          {
               for (int i = 0; i < cursorTask.Count; i++)
               {
                    _view.AddSubTask(cursorTask, cursorTask[i]);
                    LoadTaskTreeToView(cursorTask[i]);
               }

               ErrorScan();
          }

          public void AddSubTask(TaskItem parentTask, TaskItem newTask)
          {
               if(_model.FindTaskItem(newTask.ID) != null || _view.FindTreeNodeByTask(newTask) != null)
               {
                    throw new Exception("[CRITICAL] There was already an existing node with that ID");
               }

               _model.AddSubTask(parentTask, newTask);
               _view.AddSubTask(parentTask, newTask);

               UnsavedChanges = true;
               
               ErrorScan();
          }

          private void ClearAllData()
          {
               _model.ClearAllModelData();
               _view.ClearAllViewData();
               UnsavedChanges = false;
          }

          public void ErrorScan()
          {
               _model.ErrorScan();
               _view.ErrorScan();
          }

          public void ResetTaskEventHistory(int taskItemID, bool resetChildren)
          {
               TaskItem task = _model.FindTaskItem(taskItemID);
               if (task != null)
               {
                    task.TaskSeriesItem.Reset();
                    _view.UpdateViewTaskPanel();
                    _view.UpdateTreeNodeRunningState(task, _view.FindTreeNodeByTask(task));
                    if (resetChildren)
                    {
                         for (int i = 0; i < task.SubTasks.Count; i++)
                         {
                              ResetTaskEventHistory(task.SubTasks[i].ID, resetChildren);
                         }
                    }
               }
          }
     }
}
