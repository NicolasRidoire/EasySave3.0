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
        public static string Message => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "Message");
        public static string BackupInformation => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "BackupInformation");
        public static string SaveInfo1 => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "SaveInfo1");
        public static string SaveInfo2 => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "SaveInfo2");
        public static string SaveInfo3 => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "SaveInfo3");
        public static string Language => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "language");
        public static string Delete => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "Delete");
        public static string Create => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "create");
        public static string Update => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "update");
        public static string Execut => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "Execut");
        public static string Description => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "description");
        public static string ComplSave => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "ComplSave");
        public static string DifSave => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "DifSave");
        public static string LabName => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "LabName");
        public static string LabId => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "LabId");
        public static string LabSource => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "LabSource");
        public static string LabDest => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "LabDest");
        public static string LabSaveT => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "LabSaveT");
        public static string ButCancel => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "ButCancel");
        public static string ButValidate => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "ButValidate");
        public static string BackupEnd => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "BackupEnd");
        public static string BackupError => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "BackupError");
        public static string Crypt => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "Crypt");
        public static string NameUsed => LanguageManager.GetLocalizedString(Thread.CurrentThread.CurrentUICulture.Name, "NameUsed");



    }
}
