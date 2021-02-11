using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWFA
{
     public class TaskItem : IList<TaskItem>
     {
          private static int ID_COUNT = 0;

          private TaskItem _parent = null;
          public TaskItem Parent
          {
               get;
               set;
          }

          /// <summary>
          /// -1 is uninitialized
          /// -2 is the root node
          /// </summary>
          private int _ID = -1;
          public int ID
          {
               get
               {
                    if (_ID == -1)
                    {
                         _ID = ID_COUNT++;
                    }
                    
                    return _ID;
               }
          }

          private IList<TaskItem> _subTasks = null;
          public IList<TaskItem> SubTasks
          {
               get
               {
                    if (_subTasks == null)
                    {
                         _subTasks = new List<TaskItem>();
                    }

                    return _subTasks;
               }
          }

          public string Name { get; set; }
          
          public string Folder { get; set; }

          private TaskSeries _taskSeriesItem = null;
          public TaskSeries TaskSeriesItem
          {
               get
               {
                    if (_taskSeriesItem == null)
                    {
                         _taskSeriesItem = new TaskSeries();
                    }

                    return _taskSeriesItem;
               }
          }

          private string _note = null;
          public string Note
          {
               get
               {
                    if (_note == null)
                    {
                         _note = "";
                    }

                    return _note;
               }
               set
               {
                    _note = value;
               }
          }



          public int Count => SubTasks.Count;

          public bool IsReadOnly => SubTasks.IsReadOnly;


          public TimeSpan ElapsedTotal
          {
               get
               {
                    TimeSpan total = TimeSpan.Zero;
                    foreach (TaskItem item in SubTasks)
                    {
                         total += item.ElapsedTotal;
                    }

                    return total + TaskSeriesItem.Elapsed;
               }
          }

          public TaskItem this[int index] { get => SubTasks[index]; set => SubTasks[index] = value; }

          public TaskItem()
          {
               
          }

          public TaskItem(string name)
          {
               Name = name;
          }

          public TaskItem(string name, string folder)
          {
               Name = name;
               Folder = folder;
          }

          public TaskItem FindTaskItem(int taskItemID)
          {
               TaskItem result = null;
               for (int i = 0; i < SubTasks.Count; i++)
               {
                    if (SubTasks[i].ID == taskItemID)
                    {
                         result = SubTasks[i];
                         break;
                    }
                    else
                    {
                         result = SubTasks[i].FindTaskItem(taskItemID);
                         if (result != null)
                         {
                              break;
                         }
                    }
               }

               return result;
          }

          public int IndexOf(TaskItem item)
          {
               return SubTasks.IndexOf(item);
          }

          public void Insert(int index, TaskItem item)
          {
               SubTasks.Insert(index, item);
               SubTasks[index]._parent = this;
          }

          public void RemoveAt(int index)
          {
               SubTasks.RemoveAt(index);
          }

          public void Add(TaskItem item)
          {
               SubTasks.Add(item);
               SubTasks[SubTasks.Count - 1]._parent = this;
          }

          public void Clear()
          {
               SubTasks.Clear();
          }

          public bool Contains(TaskItem item)
          {
               return SubTasks.Contains(item);
          }

          public void CopyTo(TaskItem[] array, int arrayIndex)
          {
               SubTasks.CopyTo(array, arrayIndex);
          }

          public bool Remove(TaskItem item)
          {
               bool success = false;
               for (int i = 0; i < SubTasks.Count; i++)
               {
                    if (SubTasks[i].ID == item.ID)
                    {
                         SubTasks.RemoveAt(i);
                         success = true;
                         break;
                    }
                    else
                    {
                         success = SubTasks[i].Remove(item);
                         if (success)
                         {
                              // we found and deleted it!
                              break;
                         }
                    }
               }

               return success;
          }

          public IEnumerator<TaskItem> GetEnumerator()
          {
               return SubTasks.GetEnumerator();
          }

          IEnumerator IEnumerable.GetEnumerator()
          {
               return SubTasks.GetEnumerator();
          }

          public override string ToString()
          {
               return $"ID[{ID}]\tNAME[{Name}]\tSUB[{Count}]";
          }
     }
}
