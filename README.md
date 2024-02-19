# HackerNews Tech Test

A simple REST API built with .NET 8 that gets the top _n_ "best" stories from the Hacker News API and returns some metadata about those stories.

## Running the application

`git clone https://github.com/pete-coleman/hacker-news-tech-test.git`

`cd .\hacker-news-tech-test\NewsAggregator\NewsAggregator`

`dotnet run`

You can then access the application:

-   using swagger at https://localhost:7210/swagger.
-   using a browser/postman/curl etc. at https://localhost:7210/api/hackernews/stories/best?count=5.
-   using visual studio you can debug the application or use the `NewsAggregator.http` file to issue the GET request.

## Next Steps

This is intentionally a very basic implementation, it contains rudimentary caching, exception handling and logging with some simple configuration. It meets the requirements given without being overdeveloped, changes in scope for this "project" would have minimal impact at this stage. That being said there are some areas that could be addressed:

-   There are currently no unit tests or automated tests of any kind. The logic present is minimal but some integration tests would be very useful.
-   There is no authentication whatsoever, it isn't a requirement but it should certainly be a consideration in the future.
-   Moving to a distributed cache would mean our caching can be persisted even if the API goes down or is restarted for whatever reason.

Improvements to the exception handling, logging and configuration would be expected as the application develops.
