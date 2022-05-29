

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure(app =>
        {
            app.Run( async context =>
            {
                // Simulate a BAD access to (e.g.) a DB
                //Thread.Sleep(1000); // BAD because it blocks the execution
                //Task.Delay(1000).Wait(); // BAD because it blocks the execution 

                // Simulate a Well done access to (e.g.) a DB
                //await Task.Delay(2000); // GOOD because it does not block the execution 
                await Task.Delay(TimeSpan.FromSeconds(10)); // GOOD because it does not block the execution

                await context.Response.WriteAsync("Hello World!");
            });
        });
    })
    .Build()
    .Run();


