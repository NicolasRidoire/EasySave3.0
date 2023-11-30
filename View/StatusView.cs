
using System;

namespace PROGRAMMATION_SYST_ME.View
{
    class StatusView
    {
        /// <summary>
        /// Show backup start status to the user
        /// </summary>
        /// <param name="jobName">Name of the backup</param>
        public void JobStart(string jobName)
        {
            Console.WriteLine($"Job {jobName} start");
        }
        /// <summary>
        /// Show backup end status to the user
        /// </summary>
        /// <param name="jobName">Name of the backup</param>
        public void JobStop(string jobName, long elapsedTime)
        {
           Console.WriteLine($"Job {jobName} ended in {elapsedTime} milliseconds");
        }
        /// <summary>
        /// Show backups ending (once every job is done) status to the user
        /// </summary>
        public void JobsComplete() 
        {
            Console.WriteLine("-> All save jobs are complete");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
