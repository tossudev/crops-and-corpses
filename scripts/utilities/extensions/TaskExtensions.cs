using System;
using System.Diagnostics;
using System.Threading.Tasks;


/// <summary>
/// Author Reima N.
/// Use without permission prohibited
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Suspends a process while the predicate function evaluates to true
    /// </summary>
    /// <param name="predicate"> function to be evaluated at each poll </param>
    /// <param name="pollFrequencyMS"> (optional) delay between polls in milliseconds </param>
    /// <example> await SuspendWhile(() => taskQueue.Count > 0) </example>
    /// <returns> Elapsed waiting time in milliseconds </returns>
        
    public static async Task<int> SuspendWhile(
        Func<bool> predicate, uint pollFrequencyMS = 5, bool waitIndefinitely = false, int timeOutS = 5)
    {
        int elapsedTimeMS = 0;
        int pollFrequency = (int)pollFrequencyMS;
            
        while (predicate.Invoke())
        {
            await Task.Delay(pollFrequency);
            elapsedTimeMS += pollFrequency;

            if (waitIndefinitely) continue;
            
            if (elapsedTimeMS / 1000 >= timeOutS) break;
        }
        
        return elapsedTimeMS;
    }
}