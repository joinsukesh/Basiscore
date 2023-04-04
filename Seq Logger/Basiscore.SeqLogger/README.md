# Seq Logger for Sitecore
In this article, I have explained how to use the **Seq Logger** module and by that means, view the local instance’ Sitecore logs in **Seq**.


[Seq Logger](https://github.com/sukesh-y/Basiscore/blob/master/Seq%20Logger/Basiscore.SeqLogger/Services/SeqLogger.cs) is a simple log appender that I have created, 
which executes this logic -  after the Sitecore’s default _log.txt_ file is created, _Seq Logger_ will send the log data to the Seq API, which can be viewed real time in the _Seq_ tool.

## WHAT IS SEQ
[Seq](https://datalust.co/seq) is a centralized structured log server built for modern structured log data, and allows development teams to capture, search, and integrate structured log events.

<img src="https://user-images.githubusercontent.com/24619393/229768838-33be62c8-0726-4184-8a83-63ed6ece4c65.png" width="700" height="auto"/>
 

## STEP 1: INSTALL SEQ
1.	Download the latest version from the website - https://datalust.co/download

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769292-40742368-6470-4559-b478-16d7865b4925.png" />


2.	Run the executable and follow the instructions.

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769335-902289e8-e6d6-478e-877f-3ddfc3b5ca3d.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769399-fb43d017-3573-4f50-b35c-ffd0f1f070eb.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769433-3c41b136-5557-4571-bdd9-d4e0f90e39b8.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769460-62a2c92a-2c62-4d07-ac01-eafec68424b5.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769483-97dbd57d-d587-4d4c-b28d-573a97dba9a1.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769496-cadc897c-52fc-4a98-8cb2-f8fa870f8a22.png" />



3.	If you are prompted with the credentials screen, set your Username & Password.
4.	Once the installation is complete, browse this URL to view the portal – http://localhost:5341


## STEP 2: CONFIGURE SEQ
We’ll start with creating **Filters** & **Signals**. A _Signal_ is a collection of filters in Seq. You may find one or two default _Signals_ like this:

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769653-cf847c77-8aec-43dc-a1a3-54a0a34d7d0b.png" />


Here **@Level** is a default _Signal_. **Errors** & **Warnings** are its filters. If you click on **Errors**, it will display only the logs of _Level Type_ ‘ERROR’ from the existing list.
1.	Let us create a filter for _Debug_ logs. Select the **Errors** filter and click the pencil icon to edit.

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769924-a7e3a0d7-60b6-4ad5-986d-7f204b1f9382.png" />


2.	In the dropdown, select **Clone** to create a clone of this filter. 

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229769975-ba92b2f0-95cc-420c-af66-9eaec13a5c3f.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229770015-dd5fd363-cb82-4052-8532-9384b6e53d17.png" />


 

3.	Rename the Title to _Debug_. Change the text in **FILTERS** & **GROUP** section as shown.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229770119-c1fd8b97-7c67-4956-8a69-c8a33e291641.png" />


The query in the FILTERS section means, this filter should display logs of _@Level type_ ‘Debug’ and this filter should be inside the GROUP _@Level_.
4.	Click the Save icon to save this filter.

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229770240-401ae702-cc5b-4b2f-95ed-88e5921b0010.png" />


5.	Repeat the steps and create filters for Info (_@Level in [‘Info’] ci_) & Fatal (_@Level in [‘Fatal’] ci_).
6.	You may have multiple active Sitecore instances in your system. In such cases, it is good to have an application specific _Signal_, to filter the respective logs.

To create a new Signal, click on the + icon.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229770549-167837fa-cd13-4d6e-bb0d-6024051249aa.png" />


Edit the _Title_. Let’s say, your application’s name is ‘_Stratum_’. You can set the same name as the title. Click on the Save icon.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229770623-cf985a40-4fe9-4d94-9c13-473f13a80584.png" />


There are no filters for this _Signal_ yet. Let us create a filter which should display logs which have the ‘Application’ as ‘Stratum’. 
In the Search Box, type this query _Application = ‘Stratum’_ and click the **Add Filter** icon.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229770718-472b0731-8454-4c45-95a1-0d0b058fa529.png" />


This will add the filter to your _Signal_. Click on the _Save_ icon to save this _Signal_.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229770787-690ab6eb-1fc8-4a2c-b40d-86bff7ba6ece.png" />


In the query _Application = ‘Stratum’_, Application should be a property in the _Seq_ log & _Stratum_ should be its value.
We will configure this for our Sitecore application in a later step.

## STEP 3: DEPLOY THE MODULE FILES
1.	Download the module’s latest version from here - [https://github.com/sukesh-y/Downloads/tree/main/Basiscore/SeqLogger](https://github.com/sukesh-y/Downloads/tree/main/Basiscore/SeqLogger)
2.	Extract the zip and copy the files to the respective locations in your Sitecore instance’ webroot. 

The .zip has 2 files:
- **\App_config\Include\zzz.Basiscore\Basiscore.SeqLogger.config**  and
- **\bin\Basiscore.SeqLogger.dll**

The **Basiscore.SeqLogger.config** is the patch config where we can configure the log settings. The configurations are explained in a following section.

## STEP 4: VIEW LOGS IN SEQ
The _Seq_ portal can show only the logs created from now. 
It cannot show the past logs because it will not read from the _*.txt_ files. 

By default, _Seq Logger_ does not log the default system logs. 
However, to test if the module is working fine, let us change the setting to log the system files.
1.	Open the **\App_config\Include\zzz.Basiscore\Basiscore.SeqLogger.config** file and set **includesystemlogs** to _true_ and save the file.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229771324-e02c31a8-c0f7-4f5e-ad48-c03769849ca5.png" />


2.	Log in to your Sitecore CMS and browse any page like the Content Editor. Sitecore will create some logs and _Seq Logger_ should send them to the Seq API.
3.	Now refresh the _Seq_ page – https://localhost:5341/events 
You should be able to see the logs

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229771432-84acc254-f4e5-49f9-b280-e7e022722ca7.png" />

 
You can set the **includesystemlogs** setting back to _false_, if you do not wish to capture the system logs.

## THE PATCH CONFIG
These are the settings that you can configure for your Sitecore application in the Basiscore.SeqLogger.config

<table>
  <tr>
    <th>Setting</th>
    <th>Description</th>
  </tr>
  <tr>
    <td>applicationname</td>
    <td>The property ‘Application’ and the configured value (e.g., ‘Stratum’) will be sent to the Seq API and 
    it will be rendered like this:
      
 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229771813-41470712-d18c-47a2-83de-9144a71bf018.png" />
      
You can set your application name here. This will allow us to filter the logs by Application as discussed in _Step 2.6_ </td>
  </tr>
  <tr>
    <td>includesystemlogs</td>
    <td>Setting this to true, will capture the Sitecore’s default logs. By default, it is set to false. 
    This will capture only the application logs, i.e., logs that you are capturing in your code e.g., _Log.Error(“”, exception, this);_ </td>
  </tr>
  <tr>
    <td>excludeloggers</td>
    <td>	This setting is considered only when  _includesystemlogs_ is false. The logger names starting with any of the specified terms will be excluded from logging. This is to further prevent logging of unnecessary system logs.</td>
  </tr>
  <tr>
    <td>seqapiurl</td>
    <td>This is the Seq API’s end point to which the log data will be sent. Do not modify unless you have configured your Seq instance to a different URL.</td>
  </tr>
  <tr>
    <td>commonproperties</td>
    <td>Enter any common custom properties you wish to see for each log in Seq, in this pattern - _Key1,Value1|Key2,Value2_
      
e.g.: _Environment,Local|Source,Web_ 
      <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772198-f24dd4d9-659d-4812-b016-74f581ec6824.png" />
    </td>
  </tr>
  <tr>
    <td>level</td>
    <td>The default is “DEBUG”. This means, the logs of level DEBUG & above will be captured.</td>
  </tr>
  </table>


## HOW TO LOG IN CODE

### Basic Logging:
Use Sitecore’s default log methods which accept the “object owner” argument.

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772313-84911746-f6de-4dac-be09-02aa35f29662.png" />


This would render logs like this:

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772344-f436d78e-1507-4be3-9e0f-a51c09be80bc.png" />


### Log Additional Properties:

**#1 Pass them as a JSON string of _List<KeyValuePair<string, string>>_**

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772513-b5d0dc0d-b2ef-4f03-91a8-d25ea37500e8.png" />


Which will render like this:

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772526-17f26693-bf23-44e6-a68b-b03ec957f80d.png" />


**#2 Pass an object as JSON string**

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772586-0524036f-ee8c-4718-aafc-d006bdbd4eff.png" />


Which will render like this:

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772612-4d2a5f84-8571-47e8-ae35-5b2188e7775d.png" />


## INGEST LOGS FROM FILE
You can also ingest logs from a file. Here is an example.
I have copied a _log.{date}.{time}.txt_ file from my Sitecore instance, to _D:\_ and renamed it to _log.txt_.

The contents of this file are like this:

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772683-9ac856f9-7f5f-471f-8883-cf79a87fba28.png" />


Open the Windows command prompt and change the directory to the Seq installation folder and execute this command

> _seqcli ingest -i D:\log.txt -x "{Thread:nat} {@t} {@l} {@m:*}{:n}{@x:*}"_

This will ingest the file content as logs into _Seq_, like this:

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229772807-5dabdfb2-03e7-4981-b4d4-ec3232b026ca.png" />


The value after **-i** is the input file path.

The value after **-x** is the extraction pattern.

For example, if this is the entry in the log file:

> _79084 11:28:40 WARN  Runner started: DatFilesLoaderAsyncRunner_

The first part is the Thread id which is an integer.

The second part is the time stamp.

The third, is the log level.

The fourth, is the message.

So, the entry will be extracted using this pattern - 
> _{Thread:nat} {@t} {@l} {@m:*}{:n}{@x:*}_

Here ‘Thread’ is the name I have given, to capture the Thread Id and **nat** is _Seq_’s default keyword which will accept an integer.

The rest are _Seq_’s default keywords.

**@t** is Seq’s default keyword for accepting timestamp.

**@l** is Seq’s default keyword for accepting log level.

**@m** is Seq’s default keyword to accept message. This will take the text until it meets a new line, which is specified by **:n**.

And anything after the new line, I have considered it to be an exception. So **@x**.

You can refer to these articles for more information about ingestion from file and the extraction patterns.

[https://docs.datalust.co/docs/importing-log-files](https://docs.datalust.co/docs/importing-log-files)

[https://docs.datalust.co/docs/command-line-client#extraction-patterns](https://docs.datalust.co/docs/command-line-client#extraction-patterns)

## DELETE LOGS IN SEQ

Go to **Settings** > **Retention** and click on **MANUALLY DELETE EVENTS**.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773313-f800678b-7dc7-4e9c-8923-9b75ba6eb5ff.png" />


You can filter the existing entries and then click on the **Delete** icon. This will only delete the current filtered logs. If there are no filters selected, then it will delete all the logs.

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773352-7c3e7f68-45a9-4cb0-b7ee-786b7f121283.png" />


You can also add a policy to delete logs after a certain duration.

Go to **Settings** > **Retention**. Click on **ADD POLICY**.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773414-60fa9470-238e-41c0-8f2f-a6207794fb4c.png" />


## TROUBLESHOOTING

### Ingestion from file
While ingesting logs from a file, if it’s not working as expected, check the logs here for any helpful information – **Settings** > **Diagnostics** > **INGESTION LOG**

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773508-e0840a4b-6282-47ea-b6e8-a52c81a232c1.png" />


### Loading Seq tool

If you restart your machine, the Seq service may not start automatically and the Seq page will not load.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773624-14b04ab2-f01f-4cfc-8326-0c024267fe7c.png" />


In such case, open the Seq tool and do the setup again. That should work.

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773694-4bbab810-8478-40df-b7b8-d218d62f88e2.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773711-687efe7e-f244-4c64-a7c2-3147846fe034.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773742-f9735f96-291c-4e14-9350-429b833d5fe1.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773770-6d462612-5230-410a-9a63-617a900a067e.png" />


### Reset user credentials

If in any case, you see a prompt to enter credentials and you forgot them, they can be reset.

 <img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773826-0e5a21fd-9cba-4eb0-9354-7517d2671053.png" />

Execute this command in Windows Command Prompt

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773860-fd8c51ed-e2bc-4b5a-990e-c58de6979475.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773882-169e1b9c-9e4b-4d0f-bf5d-877a1baa5d12.png" />

<img width="700" height="auto" src="https://user-images.githubusercontent.com/24619393/229773904-d3aa50d4-de26-416e-92ef-f9a28e2e3898.png" />


Once that is done, you will be asked to reset your password

 
## 📝REFERENCES

Seq Documentation - [https://docs.datalust.co/docs](https://docs.datalust.co/docs)

Seq Github - [https://github.com/datalust](https://github.com/datalust)

