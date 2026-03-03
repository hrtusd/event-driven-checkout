var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddRabbitMQ("messaging");
var sql = builder.AddSqlServer("sql-server");
var redis = builder.AddRedis("cache");

var orderDb = sql.AddDatabase("OrderDb");
var logisticsDb = sql.AddDatabase("LogisticsDb");

var basket = builder.AddProject<Projects.EventDrivenCheckout_Basket>("basket")
    .WithReference(redis);

var order = builder.AddProject<Projects.EventDrivenCheckout_Order>("order")
    .WithReference(rabbit)
    .WithReference(orderDb);

var logistics = builder.AddProject<Projects.EventDrivenCheckout_Logistics>("logistics")
    .WithReference(rabbit)
    .WithReference(logisticsDb);

builder.Build().Run();