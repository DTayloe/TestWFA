using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestWFA
{
     public class TaskSeries
     {
          private static int ID_COUNT = 0;

          /// <summary>
          /// -1 is uninitialized
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

          private IList<TaskEvent> _taskEvents = null;
          public IList<TaskEvent> TaskEvents
          {
               get
               {
                    if (_taskEvents == null)
                    {
                         _taskEvents = new List<TaskEvent>();
                    }

                    return _taskEvents;
               }
          }

          public TaskSeries()
          {

          }

          public void AddTaskEvent(TaskEvent taskEvent)
          {
               TaskEvents.Add(taskEvent);
          }

          public void RemoveTaskEvent(int taskEventID)
          {
               throw new NotImplementedException();
          }
          
          public TaskEvent Current
          {
               get
               {
                    if (TaskEvents.Count == 0)
                    {
                         TaskEvents.Add(new TaskEvent());
                    }

                    return TaskEvents[TaskEvents.Count - 1];
               }
               set
               {
                    TaskEvents[TaskEvents.Count - 1] = value;
               }
          }
          
          public TaskEventState State
          {
               get
               {
                    return Current.State;
               }
          }

          public static void PrintState(TaskEventState state)
          {
               switch (state)
               {
                    case TaskEventState.TaskEventNew:
                         Console.WriteLine("new!");
                         break;
                    case TaskEventState.TaskEventRunning:
                         Console.WriteLine("started!");
                         break;
                    case TaskEventState.TaskEventComplete:
                         Console.WriteLine("complete!");
                         break;
               }
          }

          public TimeSpan Elapsed
          {
               get
               {
                    TimeSpan total = TimeSpan.Zero;
                    int runningCountErrorCheck = 0;

                    //Console.WriteLine();
                    foreach (TaskEvent item in TaskEvents)
                    {
                         total += item.Elapsed;
                         if (item.State == TaskEventState.TaskEventRunning)
                         {
                              runningCountErrorCheck++;
                         }
                         //Console.WriteLine($"{item}");
                    }
                    //Console.WriteLine();

                    if (runningCountErrorCheck > 1)
                    {
                         Console.WriteLine("[ERROR] TaskSeries.Elapsed: There were more than one TaskEvent in a running state, setting the previous tasks to zero time elapsed");

                         // go all the way, except for the ending one
                         for (int i = 0; i < TaskEvents.Count - 1; i++)
                         {
                              if (TaskEvents[i].State == TaskEventState.TaskEventRunning)
                              {// set so that there is no elapsed time for this event
                                   TaskEvents[i].EndingTime = TaskEvents[i].StartingTime;
                              }
                         }
                    }

                    return total;
               }
          }

          /// <summary>
          /// Returns state after starting.
          /// </summary>
          /// <returns></returns>
          public TaskEventState Start()
          {
               if (!Current.Start())
               {// if we cannot start, that means it is running or completed
                    switch (Current.State)
                    {
                         case TaskEventState.TaskEventNew:
                              // illegal, should have been able to start from the Start method
                              Console.WriteLine("[ERROR] illegal, should have been able to start from the Start method");
                              break;
                         case TaskEventState.TaskEventRunning:
                              // illegal
                              Console.WriteLine("[ERROR] illegal, should not try starting a task that was already running");
                              break;
                         case TaskEventState.TaskEventComplete:
                              // make new, and start it
                              TaskEvents.Add(new TaskEvent());
                              Current.Start();
                              break;
                    }
               }

               return Current.State;
          }

          public TaskEventState Stop()
          {
               if (!Current.Stop())
               {// if we cannot stop, that means it is new or complete
                    switch (Current.State)
                    {
                         case TaskEventState.TaskEventNew:
                              // illegal, should not stop a new task
                              Console.WriteLine("[ERROR] illegal, should not stop a new task");
                              break;
                         case TaskEventState.TaskEventRunning:
                              Console.WriteLine("[ERROR] illegal, should have been able to stop a running task");
                              break;
                         case TaskEventState.TaskEventComplete:
                              // make new
                              Console.WriteLine("[ERROR] illegal, should not try stopping a task that was already complete");
                              break;
                    }
               }

               return Current.State;
          }

          public void WriteXmlToSave(XmlTextWriter xtw)
          {
               foreach (TaskEvent item in TaskEvents)
               {
                    xtw.WriteStartElement("task_event");

                    xtw.WriteStartElement("task_event_starting");
                    xtw.WriteString(""+item.StartingTime.Ticks);
                    xtw.WriteEndElement();

                    xtw.WriteStartElement("task_event_ending");
                    xtw.WriteString("" + item.EndingTime.Ticks);
                    xtw.WriteEndElement();

                    xtw.WriteEndElement();
               }
          }

          public void Reset()
          {
               _taskEvents = null;
          }
     }

     public enum TaskEventState
     {
          TaskEventNew,
          TaskEventRunning,
          TaskEventComplete
     }

     public class TaskEvent
     {
          private TaskEventState _state = TaskEventState.TaskEventNew;
          public TaskEventState State
          {
               get
               {
                    return _state;
               }
          }

          private DateTime _startingTime = DateTime.MinValue;
          public DateTime StartingTime
          {
               get
               {
                    return _startingTime;
               }
               set
               {
                    if (true)
                    {

                    }

                    if (value != DateTime.MinValue)
                    {
                         _state = TaskEventState.TaskEventRunning;
                    }

                    _startingTime = value;
               }
          }

          private DateTime _endingTime = DateTime.MinValue;
          public DateTime EndingTime
          {
               get
               {
                    return _endingTime;
               }

               set
               {
                    /*
                     allow changing this state only when the value coming in is not the min value... 
                     when saving to xml endingtime is min value if it was running when the app was quit, 
                     so loading from a save file will change the state to completed because of the zero.
                     */
                    if (value != DateTime.MinValue)
                    {
                         _state = TaskEventState.TaskEventComplete;
                    }

                    _endingTime = value;
               }
          }

          public TimeSpan Elapsed
          {
               get
               {
                    if (StartingTime != DateTime.MinValue)
                    {// we have started
                         if (EndingTime != DateTime.MinValue)
                         {// we have ended
                              //Case: start !MinValue, end !MinValue  Started and stopped      (complete)
                              return EndingTime - StartingTime;
                         }
                         else
                         {// we have not ended yet
                              //Case: start !MinValue, end MinValue   Started, still running   (started / running)
                              // XXX THIS IS WHERE THE BUG IS HAPPENING
                              return DateTime.Now - StartingTime;
                         }
                    }
                    else
                    {// we have not started
                         //Case: start MinValue, end MinValue         Not started              (never ran)
                         return TimeSpan.Zero;
                    }
               }
          }

          public string ElapsedDisplay
          {
               get
               {
                    return    $"{Utility.DisplayDigitWithZero(Elapsed.Days)}D " +
                              $"{Utility.DisplayDigitWithZero(Elapsed.Hours)}H " +
                              $"{Utility.DisplayDigitWithZero(Elapsed.Minutes)}M " +
                              $"{Utility.DisplayDigitWithZero(Elapsed.Seconds)}S";
               }
          }
          
          /// <summary>
          /// True for success, false for failure
          /// </summary>
          /// <returns></returns>
          public bool Start()
          {
               if (StartingTime != DateTime.MinValue)
               {// we have started
                    if (EndingTime != DateTime.MinValue)
                    {// we have ended
                         //Case: start !MinValue, end !MinValue  Started and stopped      (complete)
                         return false;
                    }
                    else
                    {// we have not ended yet
                         //Case: start !MinValue, end MinValue   Started, still running   (started / running)
                         return false;
                    }
               }
               else
               {// we have not started
                    //Case: start MinValue, end MinValue         Not started              (never ran)
                    StartingTime = DateTime.Now;
                    return true;
               }
          }

          public bool Stop()
          {
               if (StartingTime != DateTime.MinValue)
               {// we have started
                    if (EndingTime != DateTime.MinValue)
                    {// we have ended
                         //Case: start !MinValue, end !MinValue  Started and stopped      (complete)
                         return false;
                    }
                    else
                    {// we have not ended yet
                         //Case: start !MinValue, end MinValue   Started, still running   (started / running)
                         EndingTime = DateTime.Now;
                         return true;
                    }
               }
               else
               {// we have not started
                    //Case: start MinValue, end MinValue         Not started              (never ran)
                    return false;
               }
          }

          public TaskEvent()
          {

          }
          
          public TaskEvent(DateTime startingTime)
          {
               StartingTime = startingTime;
          }

          public TaskEvent(DateTime startingTime, DateTime endingTime)
          {
               StartingTime = startingTime;
               EndingTime = endingTime;
          }

          public override string ToString()
          {
               return $"[TaskEvent: Start({StartingTime}), End({EndingTime}), State({State})]";
          }
     }
}
