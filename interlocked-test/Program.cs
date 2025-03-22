//
// Sample file for the blog post 'Does Interlocked Solve a Real-Life Problem?'
// https://busygin.com/posts/does-interlocked-solve-real-life-problem/
//
using System.Collections.Concurrent;

{
    Console.WriteLine("Synchronous");

    var orderService = new BurgerOrderService();
    var bag = new ConcurrentBag<int>();

    for (var i = 0; i < 1000; i++)
    {
        bag.Add(orderService.GetOrderNumber());
    }

    PrintResults(bag);
}

{
    Console.WriteLine("Use increment operator");

    var orderService = new BurgerOrderService();
    var bag = new ConcurrentBag<int>();

    var tasks = new List<Task>();
    for (var i = 0; i < 1000; i++)
    {
        tasks.Add(GetOrderNumberAsync(orderService, bag));
    }
    await Task.WhenAll(tasks);

    PrintResults(bag);
}

{
    Console.WriteLine("Use Interlocked.Increment");

    var orderService = new BurgerOrderService();
    var bag = new ConcurrentBag<int>();

    var tasks = new List<Task>();
    for (var i = 0; i < 1000; i++)
    {
        tasks.Add(GetOrderNumberThreadSafeAsync(orderService, bag));
    }
    await Task.WhenAll(tasks);

    PrintResults(bag);
}

static async Task GetOrderNumberAsync(BurgerOrderService service, ConcurrentBag<int> bag)
{
    await Task.Delay(1);
    bag.Add(service.GetOrderNumber());
}

static async Task GetOrderNumberThreadSafeAsync(BurgerOrderService service, ConcurrentBag<int> bag)
{
    await Task.Delay(1);
    bag.Add(service.GetOrderNumberButThreadSafe());
}

static void PrintResults(ConcurrentBag<int> bag)
{
    Console.WriteLine("Total order numbers: {0}", bag.Count);
    Console.WriteLine("Unique order numbers: {0}", bag.Distinct().Count());
}

class BurgerOrderService
{
    private int orderNumber = 0;

    public int GetOrderNumber()
    {
        return ++orderNumber;
    }

    public int GetOrderNumberButThreadSafe()
    {
        return Interlocked.Increment(ref orderNumber);
    }
}

