var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddRabbitMQ("messaging").WithManagementPlugin();
var sql = builder.AddSqlServer("sql-server");
var redis = builder.AddRedis("cache");

var orderDb = sql.AddDatabase("OrderDb");

var basket = builder.AddProject<Projects.EventDrivenCheckout_Basket>("basket")
    .WithReference(rabbit)
    .WithReference(redis);

var order = builder.AddProject<Projects.EventDrivenCheckout_Order>("order")
    .WithReference(rabbit)
    .WithReference(orderDb);

var logistics = builder.AddProject<Projects.EventDrivenCheckout_Logistics>("logistics")
    .WithReference(rabbit);

builder.Build().Run();