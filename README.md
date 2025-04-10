# CosmosOdyssey

The "Cosmos Odyssey" web app provides an intuitive platform for customers to explore the best travel deals within our solar system. It fetches pricing data from an API and allows users to select routes between different planets. The system displays available routes and prices based on the customer's preferences. Once the customer has reviewed the options and selected a route, they can proceed to make a reservation under their name.



Following tools are required:

https://dotnet.microsoft.com/en-us/download

https://learn.microsoft.com/en-us/ef/core/cli/dotnet

If you're setting up the database for the first time or need to apply any pending migrations, run the following commands to create and update your database schema:

dotnet ef migrations add --project DAL --startup-project WebApp --context AppDbContext InitialCreate

dotnet ef database --project DAL --startup-project WebApp update

To run the code:

dotnet run --project WebApp

In home page customer can choose origin and destination planets, filter routes based on company name and sort routes by price, duration or distance.
