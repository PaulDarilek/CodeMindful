Configuration Management Database (CMDB)

(CMDB) is a repository of information related to all the components of an information system. 
It includes details about the configuration items (CIs) in the system, their attributes, and relationships between them. 
The CMDB is used to track the state of each CI and its relationships with other CIs, which helps in managing changes, incidents, and problems in the IT environment.

Configuration Item (CI) -- components of your IT infrastructure to deliver business services
Assets are corporate resources that you need to manage from an inventory, financial, and contractual perspective. 

An Item can be both an Asset and a CI

Asset -- LinksTo --> (Asset | CI)
CI -- LinksTo --> (CI | Asset)

Source Code Configuration
- Git Repository (Or TFS)
- Solution
- Project (Solution Dependant)
- NuGet Packages (Project Dependant)

Builds and Releases
- Build (Depend on Solution File)
- Release
  - Server(s), Path, Website/Service/Console App
  - Dependant Services (Database, Web-Api, FileSystem)
  - Identity of Application
  - Security (Users,Groups, Roles)


The following are some of the common kinds of objects in an AD network:

- User: A user object represents a user account of an individual who needs access to resources in an AD network. 
  The user account has a user name and is authenticated using a password to prevent unauthorized individuals from accessing the network’s resources. 
  Active Directory has two types of user accounts namely:
  - Administrator account: a full-fledged permanent account that has higher privileges for administrative purposes
  - Guest account: a temporary account that has limited access to resources and limited permissions
- Computer: A computer object represents a workstation or a server computer in the AD network.
- Contact: A contact object contains contact information of people who are associated with but not a part of the organization. For example, vendors, service technicians, etc.
- Group: A group object is a container object that contains users, computers, and other groups. Groups are used to manage AD permissions where all the objects within a group will inherit the permissions assigned to the group.
- Organizational Unit (OU): An organizational unit is also a container object that can contain users, computers, groups, or shared folders. OUs are used for organizational purposes, manage resources within an organization, and delegate control among objects within the OU.

Shared folder: A shared folder object is a pointer for a specific shared folder that points towards where the folder in question is located. The pointer does not contain any data from the folder.