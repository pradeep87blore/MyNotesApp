MyNotes App
--------------------------------------

This application shall allow the user to add some notes and have a collection of their previous notes stored online.

The intent is to make use of various AWS services to achieve this.

Goals / High level requirements / technical details:
Facebook based login - TBD
Simple ui that allows for some text and image to be added as notes - Done
Text stored with metadata in dynamo db - Done
Image stored in s3 bucket - Done
Dynamo db row points to the image - Done
Image resolution downgraded to 512 x 512 by a lambda function or local client - TBD
Api gateway to provide a rest call to post notes? - TBD
Api gateway api to get all notes? - TBD
A web page that can return all the notes in a simple html format. Can use lambda here to create html tags around the notes and image - TBD
Ability to delete or edit notes and image - TBD
Wpf ui in desktop client to add, edit and delete notes - TBD
Notes can be sorted in any order - TBD
Can pin or star notes - TBD

# To run this:
1. First deploy the Cloudformation template (MyNotesAppTable.template) appropriately. This creates the DynamoDB tables
2. Next, run the project ResourceInitializer. This creates the required S3 tables, etc.
3. Next, launch the application by running the MainPage project.

# TODO:
1. Create the Cognito ID pool with CF or at least programatically
2. Thumbnail creator and displaying the image in the notes post app restart
3. Display user's Twitter profile image in the app

