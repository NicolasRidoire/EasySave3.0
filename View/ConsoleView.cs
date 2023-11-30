using PROGRAMMATION_SYST_ME.Model;
using PROGRAMMATION_SYST_ME.ViewModel;
using System;
using System.Threading.Channels;
public enum errorCode
{
    SUCCESS = 0, // Allow code to continue
    NORMAL_EXIT = 1, // Normal exit code
    INPUT_ERROR = 2, // User entered wrong input
    SOURCE_ERROR = 3, // Source File/Folder is inaccessible to the user
    BUSINESS_SOFT_LAUNCHED = 4 // Error if business software is launched while user starts backups
};
namespace PROGRAMMATION_SYST_ME.View
{
    /// <summary>
    /// Console view for the user (interface)
    /// </summary>
    class ConsoleView
    {
        private readonly UserInteractionViewModel userInteract = new UserInteractionViewModel();
        public errorCode error { get; set; }
        /// <summary>
        /// Method that allows for initial selection between executing backup jobs or editing backup jobs
        /// </summary>
        public void InitialChoice() 
        {
            Console.Clear();
            foreach (BackupJobDataModel job in userInteract.BackupJobsData)
            {
                Console.WriteLine(job.Id + 1 + " -> " + job.Name);
            }
            Console.WriteLine("Choose between C (Change backup jobs) or E (Execute backup jobs) or L (Logs extension) or Q (Quit): ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "C":
                    UpdateChoice();
                    break;
                case "E":
                    ExecuteChoice(); 
                    break;
                case "Q":
                    error = errorCode.NORMAL_EXIT;
                    break;
                case "L":
                    UpdateLogExtension();
                    break;

                default:
                    error = errorCode.INPUT_ERROR;
                    break;
            }
            if (error == errorCode.SUCCESS)
                InitialChoice();
            else 
                PrintError(error);
                InitialChoice();
        }
        /// <summary>
        /// Method that asks the user for the backup job(s) to execute, then executes the selected backup job(s)
        /// </summary>
        private void ExecuteChoice() 
        {
            Console.WriteLine("Select the backup jobs to execute (example : 1-3 or 1;3) or Q to Quit : ");
            var selection = Console.ReadLine();
            error = userInteract.ExecuteJob(selection);
        }
        /// <summary>
        /// Method that asks the user if he wants 
        /// </summary>
        private void UpdateChoice()
        {
            Console.WriteLine("Choose between : \n" +
                "C -> Create a backup job\n" +
                "M -> Modify a backup job\n" +
                "D -> Delete a backup job\n" +
                "Q -> Quit");
            switch (Console.ReadLine()) 
            {
                case "C":
                    CreateJob();
                    break;
                case "M":
                    ModifyJob();
                    break;
                case "D":
                    DeleteJob();
                    break;
                case "Q":
                    break;
                default:
                    error = errorCode.INPUT_ERROR;
                    break;
            }
        }
        /// <summary>
        /// Create a new backup job
        /// </summary>
        private void CreateJob()
        {
            Console.WriteLine("Enter the name of the backup job you want to create :");
            var name = Console.ReadLine();
            Console.WriteLine("Enter the source of " +  name + " :");
            var source = Console.ReadLine();
            Console.WriteLine("Enter the destination of " + name + " :");
            var dest = Console.ReadLine();
            Console.WriteLine("Enter the type of " + name + " (0 for full backup or 1 for differential backup) :");
            int type;
            try // Convertion from string to int is risky
            {
                type = int.Parse(Console.ReadLine());
            }
            catch (System.FormatException)
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            int id = userInteract.CreateJob(name, source, dest, type);
            ShowParam(id);
            Console.WriteLine("Confirm : (M to Modify or anything else to confirm)");
            if (Console.ReadLine() == "M")
            {
                Console.Clear();
                ShowParam(id);
                UpdateChoice();
            }
        }

        public void DeleteJob()
        {
            Console.WriteLine("Select the backup job to delete : ");
            int jobChoice;
            try // Convertion from string to int is risky
            {
                jobChoice = int.Parse(Console.ReadLine()) - 1;
            }
            catch (System.FormatException)
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            Console.WriteLine("Confirm that you want to delete this backup job :");
            ShowParam(jobChoice);
            Console.WriteLine("\nEnter C to Confirm");
            var change = Console.ReadLine();
            switch (change)
            {
                case "C":
                    error = userInteract.DeleteJob(jobChoice);
                    break;
                default:
                    return;
            }
        }
        /// <summary>
        /// Modify a pre-existring backup job
        /// </summary>
        private void ModifyJob() 
        {
            Console.WriteLine("Select the backup job to modify : ");
            int jobChoice;
            try // Convertion from string to int is risky
            {
                jobChoice = int.Parse(Console.ReadLine()) - 1;
            } catch(System.FormatException)
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            if (!(jobChoice >= 0 && jobChoice < userInteract.BackupJobsData.Count))
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            Console.WriteLine("Select what you want to change : ");
            ShowParam(jobChoice);
            Console.WriteLine("Q -> Quit");
            var change = Console.ReadLine();

            // Utilisation d'une méthode pour la lisibilité de la condition
            if (!IsValidInputChange(change))
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            switch (change)
            {
                case "Q":
                    return;
                case "T":
                    Console.Write("New value: ");
                    Console.WriteLine(" (0 for full backup or 1 for differential backup)");
                    break;
                
            }

            var newValue = Console.ReadLine();
            if (!IsValidNewValue(newValue, change))
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            
            error = userInteract.UpdateJob(jobChoice, change, newValue);
            ShowParam(jobChoice);
            Console.WriteLine("Confirm : (M to Modify or anything else to confirm)");
            if (Console.ReadLine() == "M")
            {
                Console.Clear();
                ShowParam(jobChoice);
                UpdateChoice();
            }
        }
        
        /// <summary>
        /// method that allows the user to select the extension of the log file
        /// </summary>
        private void UpdateLogExtension()
        {
            Console.WriteLine("Select the extension of the log file (xml or json) : ");
            var extension = Console.ReadLine();
            if (extension == "xml" || extension == "json")
            {
                userInteract.ChangeExtensionLog(extension);
                Console.WriteLine("Extension changed to : " + extension);
            }
            else
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            Console.WriteLine("press any key to continue");
            Console.ReadKey();
        }
        /// <summary>
        /// Method that shows the current backup job's properties
        /// </summary>
        /// <param name="jobChoice">Index of the backup job list</param>
        private void ShowParam(int jobChoice)
        {
            Console.WriteLine($"N -> Name : {userInteract.BackupJobsData[jobChoice].Name}");
            Console.WriteLine($"S -> Source path : {userInteract.BackupJobsData[jobChoice].Source}");
            Console.WriteLine($"D -> Destination path : {userInteract.BackupJobsData[jobChoice].Destination}");
            Console.WriteLine("T -> Type : {0}", userInteract.BackupJobsData[jobChoice].Type == 0 ? "Full backup" : "Differential backup");
        }
        /// <summary>
        /// Private method to handle errors
        /// </summary>
        /// <param name="errorCode"></param>
        private void PrintError(errorCode error)
        {
            switch (error)
            {
                case errorCode.NORMAL_EXIT: 
                    Console.WriteLine("Successful exit");
                    Environment.Exit(0);
                    break;
                case errorCode.INPUT_ERROR: FormatError("Invalid input"); break;
                case errorCode.SOURCE_ERROR: FormatError("Source directory not found"); break;
                case errorCode.BUSINESS_SOFT_LAUNCHED: FormatError("Business software is started. Please close it"); break;
            }
        }
        /// <summary>
        /// Formats the error and displays it
        /// </summary>
        /// <param name="msg"></param>
        private void FormatError(string msg)
        {
            Console.WriteLine($"Error {error} : {msg}");
            Console.ReadKey();
        }
        /// <summary>
        /// Unreadable verification for user updateChoice input
        /// </summary>
        /// <param name="change">user input</param>
        /// <returns>true if Input is valid, false if not</returns>
        private bool IsValidInputChange(string change) => (change == "N" || change == "S" || change == "D" || change == "T" || change == "Q");
        /// <summary>
        /// Unreadable verification for user newValue input
        /// </summary>
        /// <param name="change">user input</param>
        /// <returns>true if Input is valid, false if not</returns>
        private bool IsValidNewValue(string newValue, string change) => newValue == "" || (change == "T" && !(newValue == "0" || newValue == "1"));
    }
}
