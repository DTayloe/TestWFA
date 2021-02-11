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
                    _currentFilePath = value;
                    _controller.UpdateViewCurrentFilePath(_currentFilePath);
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

          public void ClearData()
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
     }
}
