MyNotes App
--------------------------------------

This application shall allow the user to add some notes and have a collection of their previous notes stored online.

The intent is to make use of various AWS services to achieve this.

Goals / High level requirements / technical details:
Facebook based login
Simple ui that allows for some text and image to be added as notes
Text stored with metadata in dynamo db
Image stored in s3 bucket
Dynamo db row points to the image
Image resolution downgraded to 512 x 512 by a lambda function or local client
Api gateway to provide a rest call to post notes?
Api gateway api to get all notes?
A web page that can return all the notes in a simple html format. Can use lambda here to create html tags around the notes and image
Ability to delete or edit notes and image

Wpf ui in desktop client to add, edit and delete notes
Notes can be sorted in any order
Can pin or star notes

