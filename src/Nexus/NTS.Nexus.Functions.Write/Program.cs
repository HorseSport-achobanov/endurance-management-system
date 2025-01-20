using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

FunctionsDebugger.Enable();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .Build();
 
host.Run();
