using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PROGRAMMATION_SYST_ME.Ressources
{
    public static class LocalizedStrings
    {
        public static string WelcomeMessage => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "WelcomeMessage");
        public static string BackupInformation => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "BackupInformation");
        public static string Language => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "language");
        public static string Delete => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "Delete");
        public static string Create => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "create");
        public static string Update => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "update");
        public static string Execut => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "Execut");
        public static string Description => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "description");
        public static string ComplSave => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "ComplSave");
        public static string DifSave => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "DifSave");


    }
}
