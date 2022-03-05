using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestWFA
{
     public class TaskModel
     {
          private TaskController _controller = null;

          public TaskItem Tasks = null;

          private string _currentFilePath = null;
          public string CurrentFilePath
          {
               get
               {
                    return _currentFilePath;
               }
               set
               {
                    if (value != null && value != _currentFilePath)
                    {
                         WriteStartFilePathToRegistry(value);
                    }
                    _controller.UpdateViewCurrentFilePath(value);
                    _currentFilePath = value;
               }
          }
          
          public TaskModel()
          {
               Tasks = new TaskItem();
          }

          public void SetController(TaskController controller)
          {
               _controller = controller;
          }

          /// <summary>
          /// Add to a parent task a subtask 
          /// </summary>
          /// <param name="parentTask">Use null if there are no parents, it will then be added to the root tasks.</param>
          /// <param name="newTask">A new task to add to the parent.</param>
          public void AddSubTask(TaskItem parentTask, TaskItem newTask)
          {
               if (parentTask != null)
               {
                    // we have a task that needs a subtask
                    parentTask.Add(newTask);
               }
               else
               {
                    // there were no matching tasks, put it in the root tasks
                    Tasks.Add(newTask);
               }
          }

          /// <summary>
          /// Add to a parent task a subtask
          /// </summary>
          /// <param name="parentTaskID">Use -1 if there are no parents, it will then be added to the root tasks.</param>
          /// <param name="newTask">A new task to add to the parent.</param>
          public void AddSubTask(int parentTaskID, TaskItem newTask)
          {
               AddSubTask(FindTaskItem(parentTaskID), newTask);
          }

          public TaskItem FindTaskItem(int taskItemID)
          {
               return Tasks.FindTaskItem(taskItemID);
          }
          
          public void RemoveTask(int taskItemID)
          {
               if (!Tasks.Remove(FindTaskItem(taskItemID)))
               {
                    Console.WriteLine($"[ERROR] TaskModel.RemoveTask: Could not delete task {taskItemID}");
               }
          }

          public void ErrorScan()
          {
               SortedSet<int> testSetModel = new SortedSet<int>();
               ErrorScan(testSetModel, Tasks.SubTasks);
          }

          private void ErrorScan(SortedSet<int> testSet, IList<TaskItem> subTasks)
          {
               foreach (TaskItem item in subTasks)
               {
                    int before = testSet.Count;
                    testSet.Add(item.ID);

                    if (testSet.Count != before + 1)
                    {
                         Console.WriteLine($"[CRITICAL] TaskModel.ErrorScan: Duplicate ID [{item.ID}][{item.Name}]");
                    }

                    ErrorScan(testSet, item.SubTasks);
               }
          }

          public void ClearAllModelData()
          {
               Tasks = new TaskItem();
               CurrentFilePath = null;
          }

          public void UpdateModelTaskFolder(int taskItemID, string folder)
          {
               TaskItem result = FindTaskItem(taskItemID);
               if(result != null)
               {
                    result.Folder = folder;
               }
               else
               {
                    Console.WriteLine($"[ERROR] TaskModel.UpdateTaskFolder: Could not find task item with ID [{taskItemID}]");
               }
          }

          private void WriteStartFilePathToRegistry(string path)
          {
               string strKeyMain = "TaskFracker";
               string strKeySub = "StartFile";

               // Opens or creates a RegistryKey
               RegistryKey startFileKey = Registry.CurrentUser.CreateSubKey(strKeyMain, true);
               try
               {
                    if (startFileKey != null)
                    {
                         Console.WriteLine($"Finding prevously saved key's value...");
                         object startFilePath = startFileKey.GetValue(strKeySub);
                         if (startFilePath != null)
                         {
                              Console.WriteLine($"Value of key found: [{startFilePath}]");

                              Console.WriteLine($"Changing value to: [{path}]");
                         }
                         else
                         {
                              Console.WriteLine($"Key's value hasn't been created yet, creating: [{path}]");
                         }

                         startFileKey.SetValue(strKeySub, path);

                         startFileKey.Close();
                         startFileKey = null;
                    }
                    else
                    {
                         Console.WriteLine($"[ERROR] Something went wrong trying to write the registry key: [{strKeyMain}]");
                    }
               }
               catch (Exception)
               {
                    Console.WriteLine($"[CATCH] Something went wrong trying to write the registry key: [{strKeyMain}]");
               }
               finally
               {
                    if (startFileKey != null)
                    {
                         startFileKey.Close();
                         startFileKey = null;
                    }
               }
          }

          public string ReadStartFilePathFromRegistry()
          {
               string strKeyMain = "TaskFracker";
               string strKeySub = "StartFile";

               // Opens or creates a RegistryKey
               RegistryKey startFileKey = Registry.CurrentUser.OpenSubKey(strKeyMain);
               try
               {
                    if (startFileKey != null)
                    {
                         Console.WriteLine($"Finding prevously saved key's value...");
                         object startFilePath = startFileKey.GetValue(strKeySub);
                         if (startFilePath != null)
                         {
                              Console.WriteLine($"Value of key found: [{startFilePath}]");

                              startFileKey.Close();
                              startFileKey = null;

                              return (string)startFilePath;
                         }
                         else
                         {
                              Console.WriteLine("Key's value not found");

                              startFileKey.Close();
                              startFileKey = null;
                         }
                    }
                    else
                    {
                         Console.WriteLine($"[ERROR] Something went wrong trying to read the registry key: [{strKeyMain}]");
                    }
               }
               catch (Exception)
               {
                    Console.WriteLine($"[CATCH] Something went wrong trying to read the registry key: [{strKeyMain}]");
               }
               finally
               {
                    if (startFileKey != null)
                    {
                         startFileKey.Close();
                         startFileKey = null;
                    }
               }

               return null;
          }
     }
}
