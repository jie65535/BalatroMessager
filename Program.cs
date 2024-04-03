using System;
using System.Threading;
using System.Threading.Tasks;

namespace BalatroMessager
{
    internal class Program
    {
        private const int DebugMessagePort = 12345;

        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            try
            {
                var logCat = new LogCat(DebugMessagePort);
                await logCat.Listen(Cts.Token);
            }
            catch (TaskCanceledException)
            {
                // Ignored.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Cts.Cancel();
        }
    }
}
