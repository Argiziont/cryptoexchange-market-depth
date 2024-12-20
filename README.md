"# cryptoexchange-market-depth" 
![image](https://github.com/user-attachments/assets/05a6d7fc-1dba-45cb-908b-4f5ff09509ce)

To run project
1. Go to `.\market-depth-api\cryptoexchange-market-depth\appsettings.json` and update connection string
2. Run `dotnet ef database update` or `Update-Database` to appy migration
3. Go back to root folder and run `cd market-depth-api\cryptoexchange-market-depth && dotnet run --urls "https://localhost:7275"`
4. In same folder run `cd market-depth-app && npm i && npm run dev`
