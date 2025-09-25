# MyFirstOwnWebProject-FishSpinDays
FishSpinDays is my first own web project with roles - a blog for fishing stories and events.

The application is separated into multiple Layers (Data, Services, Web) with Areas that are accessible by users with different access levels:
- All users could read all the available information, but they haven't permission to write anything.
- Only logged in users could write publications in different fishing sections chosen by dropdown list (sea fishing, freshwater fishing, rods and reels, lures, handmade lures, fishing schools and etc.); could like publications, but not their own publications; could make some comments and like or dislike comments; could use our real-time communication chat channel and use the API of the application.
- The admin of the application could create new main sections and subsections in publications of different types; could write publications and edit the existed ones; could delete the comments; could see the list of all users with his publications and make Ban of user.

Tech stack: .NET 8, ASP.NET Core, EF Core, API, SignalR, SQL Server, Azure.
