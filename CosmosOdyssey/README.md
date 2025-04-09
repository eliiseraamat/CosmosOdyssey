dotnet tool install --global dotnet-ef

dotnet ef migrations add --project DAL --startup-project WebApp --context AppDbContext InitialCreate
dotnet ef migrations --project DAL --startup-project WebApp remove

dotnet ef database --project DAL --startup-project WebApp update
dotnet ef database --project DAL --startup-project WebApp drop