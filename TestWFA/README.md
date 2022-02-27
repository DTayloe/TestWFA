# TestWFA

## Purpose of TestWFA

- This poorly named project provides a quick way to track tasks in a hierarchy, including timing information as well as notes. This project was put together quickly, so that some useful functionality could be implemented in my work for project tracking. Do not expect this program to not explode spontaneously or lose your work, which may or may not always be a bad thing!

## Features

Each task has these abilities:
  - Notes
  - Folder from the file system to be associated with the task
    - Switches to or opens the assigned folder in Windows Explorer, handy for common locations that are a pain to get to
  - Timer
    - Editable history of timing events that save back to the model
    - Indicator of any running tasks in program treeview as well as Windows Taskbar (if "Combine taskbar buttons" is set to "Never" in Windows Taskbar settings). I prefer the exploded view.
    - Selectable option to view total time either only in the current task or current task + all subtasks so that total time towards a task can be computed
  - Additional functionality for each task can be implemented by adding a tab and it's features or explosions
  - Each task can contain multiple subtasks, each of which can contain everything in this list, ad infinitum
- Saves to an XML file for storage, retrieval, and explosions
- Nice dynamic window resizing
- Explosions (joking, this is actually stable... mostly >;)

## Known Bugs and Issues

- When saving the XML file, the name of the task is not escaped, so it might spontaneously explode since any invalid XML characters will likely crash the save mechanism and the entire save file will be lost. This is an easy fix that will be fixed in the future; I am aware of it but for my use case this fix was low priority. I like explosions anyway. This is also being used to test ways of providing a safer mechanism of saving using a temp file. Notes can contain anything since they are escaped.

## Background

- Many components are experimental MVC design approaches, and can be improved.
- There are a few buttons that do nothing, just there for experimental purposes. Feel free to click them to your heart's content, they won't explode often.

## Future

- The next stage of this project will use different technology, due to the inability to effectively override the appearance of many components in a Windows Forms application.
- This version only saves to disk - the next version will make use of an API to access a database so that many users and clients (implemented in different technologies such as a web version built using PHP/Laravel, WPF, React Native, maybe Flutter) can access the same synchronized data on any platform, without any explosions.