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
                    Console.WriteLine("GOT");
                    return _unsavedChanges;
               }
               set
               {
                    Console.WriteLine($"SET to {value}");
                    _unsavedChanges = value;
                    _view.UnsavedChanges(value);
               }
          }

          public void UpdateViewCurrentFilePath(string currentFilePath)
          {
               _view.UpdateViewCurrentFilePath(currentFilePath);
          }

          private enum NodeTypes
          {
               tasks,
               task,
               task_name,
               task_tasks,
               task_folder
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
                    xtw.WriteStartElement("task");

                    xtw.WriteStartElement("task_name");
                    xtw.WriteString(task.Name);
                    xtw.WriteEndElement();

                    xtw.WriteStartElement("task_folder");
                    xtw.WriteString(task.Folder);
                    xtw.WriteEndElement();

                    //xtw.WriteStartElement("task_series");
                    //xtw.WriteString(task.TaskSeriesItem);
                    //xtw.WriteEndElement();

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
               ClearData();
               
               if (File.Exists(file))
               {
                    _model.CurrentFilePath = file;

                    Console.WriteLine(file);
                    XmlReaderSettings xrs = new XmlReaderSettings();
                    xrs.IgnoreWhitespace = true;
                    XmlReader xr = XmlReader.Create(file, xrs);

                    //TaskItem currentTask = null;
                    NodeTypes currentNodeType = NodeTypes.tasks;
                    Stack<TaskItem> currentTask = new Stack<TaskItem>();
                    string indent = "";
                    bool emptyFlag = false;
                    while (xr.Read())
                    {
                         emptyFlag = false;
                         //Console.WriteLine(xr.NodeType);
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
                                             case nameof(NodeTypes.task_tasks):
                                                  {
                                                       Console.WriteLine();
                                                       indent += "\t";

                                                       //_currentTask.SubTasks.Add(new TaskItem());
                                                       //_currentTask = _currentTask.SubTasks[_currentTask.SubTasks.Count - 1];
                                                       currentNodeType = NodeTypes.task_tasks;
                                                       if (emptyFlag)
                                                       {

                                                       }
                                                       else
                                                       {
                                                            //other tasks to add to the current task
                                                            //currentTask.Peek().SubTasks
                                                       }
                                                  }
                                                  break;
                                             case nameof(NodeTypes.task_folder):
                                                  {
                                                       currentNodeType = NodeTypes.task_folder;
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

                                   Console.Write((xr.Name == "task" || xr.Name == "task_tasks" ? (emptyFlag ? "" : indent) : "") + "Name: " + xr.Name + ", ");
                                   //Console.WriteLine(""+xr.);
                                   //Console.WriteLine("" + xr.);
                                   //xr.ReadSubtree();
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
                                                       currentTask.Peek().Name = text;
                                                       //Console.WriteLine($"HELLO [{text}][{xr.Value}][{xr.ReadContentAsString()}][{currentTask.Peek().Name}]");
                                                  }
                                                  break;
                                             case NodeTypes.task_tasks:
                                                  {

                                                  }
                                                  break;
                                             case NodeTypes.task_folder:
                                                  {
                                                       currentTask.Peek().Folder = text;
                                                       //Console.WriteLine($"HELLO [{text}][{xr.Value}][{xr.ReadContentAsString()}][{currentTask.Peek().Name}]");
                                                  }
                                                  break;
                                             default:
                                                  break;
                                        }
                                   }
                                   break;
                              case XmlNodeType.CDATA:
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

                                   //     _view.CreateEmptyTask(task);
                                   //     _model.Tasks.Add(task);
                                   if (currentTask.Count != 0)
                                   {
                                        currentTask.Pop();//XXX just discard this???
                                   }
                                   //refresh the view data???

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

          public void UpdateViewTaskFolder(int taskItemID)
          {
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

          private void ClearData()
          {
               _model.ClearData();
               _view.ClearData();
               UnsavedChanges = false;
          }

          public void ErrorScan()
          {
               _model.ErrorScan();
               _view.ErrorScan();
          }
     }
}
