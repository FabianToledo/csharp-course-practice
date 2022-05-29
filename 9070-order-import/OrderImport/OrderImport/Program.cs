using OrderImport.Model;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Linq;
using OrderImport.DTOs;
using AutoMapper;
using OrderImport;

string operation;
string customersFilename;
string ordersFilename;

if (args.Length >= 2 && args[0] == "--")
{
    operation = args[1];
}
else
{
#if !DEBUG
    return;
#else
    //operation = "import";
    //operation = "clean";
    operation = "full";
    //operation = "check";
#endif
}

if ((operation == "import" || operation == "full") &&
    args.Length == 4)
{
    customersFilename = args[2];
    ordersFilename = args[3];
}
else
{
#if !DEBUG
    return;
#else
    customersFilename = "customers.txt";
    ordersFilename = "orders.txt";
#endif
}

if (!File.Exists(customersFilename) || !File.Exists(ordersFilename))
{
    Console.WriteLine("File does not exist");
    return;
}

// Create DB Context Factory
var factory = new OrdersContextFactory();
// Create an operations object
Operations operate = new(factory);

switch (operation)
{
    case "import":
    case "full":

        // Auto-mapper configuration
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DtosProfile>());
        var mapper = config.CreateMapper();

        // Import files to Dtos
        var importedCustomers = await ReadCsv<CustomerDto>(customersFilename);
        var importedOrders = await ReadCsv<OrderDto>(ordersFilename);

        // Map into customers
        List<Customer> customers = mapper.Map<List<CustomerDto>, List<Customer>>(importedCustomers);

        // Map into customers orders
        foreach (Customer customer in customers)
        {
            IEnumerable<Order> ordersForCustomer = mapper.Map<IEnumerable<OrderDto>, IEnumerable<Order>>(importedOrders.Where(o => o.CustomerName == customer.Name));
            customer.Orders = new();
            customer.Orders.AddRange(ordersForCustomer);
        } 

        if (operation == "full")
            await operate.Clean();

        await operate.Import(customers);

        if (operation == "full")
            foreach (var excededCustomer in await operate.Check())
                Console.WriteLine(excededCustomer);


        break;

    case "clean":
        await operate.Clean();
        break;

    case "check":
        foreach (var excededCustomer in await operate.Check())
            Console.WriteLine(excededCustomer);
        break;
}


//foreach (var customer in customers)
//{
//    Console.WriteLine($"{customer.Id} - {customer.Name} - {customer.CreditLimit}");
//    foreach (var order in customer.Orders ?? new())
//    {
//        Console.WriteLine($"\t{order.Id} - {order.CustomerId} - {order.OrderDate} - {order.OrderValue}");
//    }
//    Console.WriteLine();
//}

//foreach (var customer in importedCustomers)
//{
//    Console.WriteLine($"{customer.CustomerName} - {customer.CreditLimit}");
//}

//foreach (var order in importedOrders)
//{
//    Console.WriteLine($"{order.CustomerName} - {order.OrderDate} - {order.OrderValue}");
//}



async Task<List<T>> ReadCsv<T>(string filename)
{
    var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t"
    };
    using var reader = new StreamReader(filename);
    using var csv = new CsvReader(reader, csvConfig);

    return await csv.GetRecordsAsync<T>().ToListAsync();
}