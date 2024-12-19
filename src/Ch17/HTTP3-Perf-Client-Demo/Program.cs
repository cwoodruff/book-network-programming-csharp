using System.Diagnostics;
using System.Net;

async Task MeasurePerformance(Version httpVersion, int requestCount)
{
    using var client = new HttpClient()
    {
        DefaultRequestVersion = httpVersion,
        DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
    };
    
    var tasks = new Task<HttpResponseMessage>[requestCount];
    var stopwatch = Stopwatch.StartNew();

    for (int i = 0; i < requestCount; i++)
    {
        tasks[i] = client.GetAsync("https://localhost:5001/process");
    }

    await Task.WhenAll(tasks);
    stopwatch.Stop();

    var totalDuration = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"{httpVersion}: Processed {requestCount} requests in {totalDuration}ms");
}

// Measure HTTP/2 performance
await MeasurePerformance(HttpVersion.Version20, 50);

// Measure HTTP/3 performance
await MeasurePerformance(HttpVersion.Version30, 50);