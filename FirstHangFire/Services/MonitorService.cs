using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Resources;
using Hangfire.Server;
using static System.TimeSpan;

namespace FirstHangFire.Services;

public class MonitorService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await AddJobHangFire();
    }

    private async Task AddJobHangFire()
    {
        //Agendado
        BackgroundJob.Schedule(() => Print("Agendamento", null), FromSeconds(5));
        
        //Em fila
        var jobId = BackgroundJob.Enqueue("test", () => Print("Test in Queue", null));
        
        //Roda um processo a partir de um Id Pai
        BackgroundJob.ContinueJobWith(jobId, () => Print($"Rodou após terminar o job id {jobId}", null));
        
        //Recorrente
        RecurringJob.AddOrUpdate("RecurringJob", () => PrintRecurringJob("Recurring", null), MinuteInterval(5));
    }

    public void Print(String message, PerformContext? context)
    {
        context.WriteLine(message);
    }
    
    public void PrintRecurringJob(String message, PerformContext? context)
    {
        context.WriteLine("Inicio do Processo");
        
        Thread.Sleep(FromMinutes(5));
        context.WriteLine("Processo quase terminando");
        
        Thread.Sleep(FromSeconds(5));
        
        context.WriteLine("Processo Finalizado");
        context.WriteLine(message);
    }

    public static string MinuteInterval(int interval)
    {
        return $"*/{interval} * * * *";
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}