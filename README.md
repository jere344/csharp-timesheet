# csharp-timesheet

A timesheet and project management software written in C# wpf using material design.  
I do not recommend using it for production as no one is maintaining it, it was a 2-month school project. But you can use it however  you want.  
  
I did my best to make the graph and canvas system easily importable into a new project with similar feature.  
For that, check out 
- the ZoomableCanvas `app\wisecorp\Views\Components\zoomableCanvas`
- the Tree and ProjectTree in `app\wisecorp\Models\Graphs`  
- the ViewManageProject `app\wisecorp\Views\Manager\ViewManageProject`

> Not afiliated or related to wisecorp, I had no idea it was an actual company.

- [wisecorp-timesheet](#wisecorp-timesheet)
   * [Navigation system](#navigation-system)
   * [Account and login ](#account-and-login)
   * [theme and language](#theme-and-language)
   * [Complete log system](#complete-log-system)
   * [Project Management](#project-management)
   * [The time sheet](#the-time-sheet)
   * [More feature](#more-feature)

## Navigation system

- Only accessible page are shown in the left menu
<img src="https://github.com/user-attachments/assets/46800fde-4703-4971-a428-436178045815" width=50% height=50%>

## Account and login 
- With a secure "remember me" system storing a temporary and secured access key, and a "Forgot my password" system using emails.
- The sender email can be defined in appsettings.json
```json
"mailer": {
    "host": "smtp.gmail.com",
    "port": 587,
    "auth": {
      "user": "youremail@gmail.com",
      "pass": "exam plep assw ordd"
    }
  }
```
<img src="https://github.com/user-attachments/assets/c3ce8add-b80f-4c12-8248-ee0463507fde" width=50% height=50%>
<img src="https://github.com/user-attachments/assets/e1616700-c543-408b-9ea3-536744e810ac" width=50% height=50%>

- 3 accounts levels : Admin, Manager, User
- Can disable accounts
<img src="https://github.com/user-attachments/assets/5ea6bf9b-eb39-425c-8af9-c0bf78ffe9ba" width=50% height=50%>

- see profile
<img src="https://github.com/user-attachments/assets/06bc496f-ca68-49e2-b8cb-18c2ded158ef" width=50% height=50%>



## theme and language
<img src="https://github.com/user-attachments/assets/da6e5ce2-a66f-40ec-997e-0b15151f7bc9" width=50% height=50%>
<img src="https://github.com/user-attachments/assets/48527c52-4a36-4b20-8aeb-7edb46bae0ae" width=50% height=50%>

## Complete log system
- Everything is logged and can be accessed by the admin
<img src="https://github.com/user-attachments/assets/b5b2d6e0-7aba-402b-b184-af2097f3645a" width=50% height=50%>

- double click on a log to show more
<img src="https://github.com/user-attachments/assets/de86fbf9-d56e-4123-90e9-4ce649f586e1" width=50% height=50%>


## Project Management
- 2 ways to manage projects :
- In a tree-like structure 
<img src="https://github.com/user-attachments/assets/15a6615a-4691-49a0-b373-759bfc2d236f" width=50% height=50%>

- In a graph structure

<img src="https://github.com/user-attachments/assets/6b000c68-ab58-47d6-8a4a-b68be7d0f062" width=50% height=50%>

> The graph and navigable canvas was completely hand made on a simple canvas with buttons, path, coordinate.
- Can move a whole project branch and its subproject to another project 
<img src="https://github.com/user-attachments/assets/bf6e15c7-1cc4-4325-9d05-78697a7030c3" width=30% height=30%>
<img src="https://github.com/user-attachments/assets/877f3745-f68e-40d3-a7e8-5ee489b39a7b" width=30% height=30%>

> Here the project "test 25" was assigned as a sub project of "test" with its sub projects kept, editing the access permissions as well


- Click on a project to open and edit infos

<img src="https://github.com/user-attachments/assets/d6bf4da6-4edd-4458-a8b8-48d988c4d048" width=50% height=50%>

- Assign people to a project

<img src="https://github.com/user-attachments/assets/b70a8540-a19e-4c93-87a8-7b451428d867" width=50% height=50%>

> Assigning someone to a project and editing it can only be done by the admins or managers that are assigned to the project or created it  
> Assigning someone to a project give him access to all sub tasks of the project  
> We can show/hide disabled projects  
> We can assign a whole departement to a project  
> Able to dupplicate or disable a whole branch  
> We keep anb history of all worked hour, and a project can only be deleted if it has no worked hour. Project with hours worked on are only disabled and still shows up in employees times sheets  

## The time sheet
<img src="https://github.com/user-attachments/assets/bfe3e264-efea-4c62-9bce-26f49ced576c" width=50% height=50%>

- Allow adding hours and comments worked on each project on each day. The send button is replaced with a save after a change
<img src="https://github.com/user-attachments/assets/1a65410a-37bb-4a0e-b1d9-4a521d55d548" width=50% height=50%>

- Can export the time sheet as a xlsx or pdf file
<img src="https://github.com/user-attachments/assets/7d25a448-66db-4945-a24d-b7b15ab1143d" width=50% height=50%>

## More feature
Admins and manager can approve time sheets, or refuse with a motif allowing the employing to send another demand.  
The admin can create an account with a random password sent to the user by mail, or with a custom password. The user will be prompted to edit it's password on first login  
There are more features not included in this document. you can build it for yourself to explore features, simply edit the appsettings.json with a working database and build it.  


