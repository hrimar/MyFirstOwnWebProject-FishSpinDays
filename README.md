# MyFirstOwnWebProject-FishSpinDays
FishSpinDays is my first own web project with roles - a blog for fishing stories and events.
The project is created with ASP.NET MVC Core and Entity Framework Core. AutoMapper is used for automatic mapping of objects and xUnit and Moq are used for unit testing. 
The project is published on Azure (https://fishspindays.azurewebsites.net).

The application is separated into areas with multiple layouts and accessible by users with different access level:
- All users could read all the available information, but they haven't permission to write anything.
- Only loged in users could read publications in sections choosed by dropdown list; to like a publications, but not their own publications; could make comments and like or dislike comments.
- The admin of the application could create new main sections and subsections in publications of different types; could write publications and edit the existed ones; could delete the comments; could see the list of all users with his publications and make Ban of user.
