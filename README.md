Speeching Client
================

**Speeching summary:**

>This application has been made as a tool for those who wish to get feedback about their speech. By completing the various available practice tasks on a regular basis you should be able to track your progress towards the goal of having clearer speech.

>While using the application you can complete various simple scenarios, assessments and tasks. These activities have been designed to help assess your speech clarity and record your vocal responses. Upon completion of an activity, you can choose to upload these recordings. This is where the fun begins!

![Speeching structure](https://raw.githubusercontent.com/GSDan/Speeching_Client/master/Droid_PeopleWithParkinsons/Resources/drawable/diagram.png)

>If you choose to upload your recordings, they will be distibuted anonymously to a 'crowd' of users. These users will not know who you are, nor will you know who they are. They'll be asked to give feedback on various aspects of your speech, such as clarity, pacing and pitch. We then find the average for these statistics and return them to you!

>Soon after submitting your results for an activity, your main feed will start to fill up with 'cards' like the one above. Each card will contain a measurement of a different area of speech. Use these measurements and their accompanying tips to help improve your speech!

**Speeching Client**

This repository contains the mobile client, written in C# using Xamarin.
The application is reliant on the serverside implementation being online in order to pull content.

*Currently only Android is supported, however all network related code should be able to be referenced by implementations on other Xamarin supported platforms*

**Referenced by the cross-platform project, SpeechingCommon:**

- Newtonsoft.Json
- mscorlib
- RestSharp.Portable
- SharpZipLib

**Referenced by the Android project:**

- SpeechingCommon
- Xamarin.Android.Support.v4
- Xamarin.Android.Support.v7.AppCompat
- GooglePlayServicesLib
- mscorlib
- Newtonsoft.Json
- PagerSlidingTabStrip
- RadialProgress.Android
- Restsharp.MonoDroid
- SharpZipLib