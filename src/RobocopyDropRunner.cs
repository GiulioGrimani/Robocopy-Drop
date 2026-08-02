using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using Microsoft.Win32;

namespace RobocopyDrop
{

    internal static class UiText
    {
        private const string UserRegistryPath = "HKEY_CURRENT_USER\\Software\\RobocopyDrop";
        private const string MachineRegistryPath = "HKEY_LOCAL_MACHINE\\Software\\RobocopyDrop";
        private static string language;

        private static readonly Dictionary<string, string> English = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Impostazioni Robocopy Drop", "Robocopy Drop Settings" },
            { "Prestazioni avanzate", "Advanced performance" },
            { "Numero di thread usati da Robocopy per le copie multithread.", "Number of threads used by Robocopy for multithreaded copies." },
            { "Thread Robocopy:", "Robocopy threads:" },
            { "Automatico (adattivo)", "Automatic (adaptive)" },
            { "Automatico: 32 thread sui dischi locali; meno thread su USB, supporti rimovibili e rete.", "Automatic: 32 threads on local drives; fewer threads on USB, removable media, and network destinations." },
            { "32 thread - predefinito locale", "32 threads - local default" },
            { "Lingua interfaccia:", "Interface language:" },
            { "Italiano", "Italian" },
            { "Inglese", "English" },
            { "La nuova lingua sara usata dalla prossima finestra di Robocopy Drop.", "The new language will be used by the next Robocopy Drop window." },
            { "Thread effettivi: determinati all'avvio della copia", "Effective threads: determined when the copy starts" },
            { "Thread effettivi: ", "Effective threads: " },
            { "Annulla", "Cancel" },
            { "Salva", "Save" },
            { "Salvataggio non riuscito", "Save failed" },
            { "Sorgente", "Source" },
            { "Formato della richiesta non valido.", "Invalid request format." },
            { "La richiesta non contiene sorgenti sufficienti.", "The request does not contain enough source items." },
            { "Sorgente non trovata: ", "Source not found: " },
            { "Sorgente e destinazione coincidono: ", "Source and destination are the same: " },
            { "Non e possibile copiare una cartella dentro se stessa: ", "A folder cannot be copied into itself: " },
            { "Piu sorgenti produrrebbero lo stesso elemento di destinazione: ", "Multiple sources would create the same destination item: " },
            { "Una cartella sorgente collide con un file nella destinazione: ", "A source folder conflicts with a destination file: " },
            { "Un file sorgente collide con una cartella nella destinazione: ", "A source file conflicts with a destination folder: " },
            { "Analisi completata", "Analysis completed" },
            { "Cartella non leggibile: ", "Folder cannot be read: " },
            { "Collegamento o junction ignorato: ", "Link or junction skipped: " },
            { "File non trovato: ", "File not found: " },
            { "Impossibile leggere il file: ", "Unable to read file: " },
            { "Due sorgenti produrrebbero lo stesso file di destinazione: ", "Two sources would create the same destination file: " },
            { "Conflitto durante la copia", "Copy conflict" },
            { "Nella destinazione esiste gia un file con lo stesso nome", "A file with the same name already exists in the destination" },
            { "Conflitto ", "Conflict " },
            { " di ", " of " },
            { "File da copiare", "File to copy" },
            { "File nella destinazione", "File in destination" },
            { "Confronta SHA-256", "Compare SHA-256" },
            { "Applica questa scelta a tutti i conflitti rimanenti", "Apply this choice to all remaining conflicts" },
            { "Sostituisci", "Replace" },
            { "Ignora", "Skip" },
            { "Conserva entrambi", "Keep both" },
            { "Annulla copia", "Cancel copy" },
            { "Dimensione: ", "Size: " },
            { "Modificato: ", "Modified: " },
            { "Calcolo degli hash in corso...", "Calculating hashes..." },
            { "Gli hash SHA-256 coincidono: il contenuto e identico.", "The SHA-256 hashes match: the contents are identical." },
            { "Gli hash SHA-256 sono diversi.", "The SHA-256 hashes are different." },
            { "Confronto non riuscito: ", "Comparison failed: " },
            { "Destinazione: ", "Destination: " },
            { "File pianificati: ", "Planned files: " },
            { "Byte pianificati: ", "Planned bytes: " },
            { "Profilo destinazione: ", "Destination profile: " },
            { "Thread Robocopy: ", "Robocopy threads: " },
            { "Operazione annullata dall'utente.", "Operation cancelled by the user." },
            { "ERRORE: ", "ERROR: " },
            { "Robocopy ha restituito il codice ", "Robocopy returned exit code " },
            { " per ", " for " },
            { "Modalita precisa per conflitti: ", "Precise conflict mode: " },
            { " per un gruppo di file.", " for a group of files." },
            { "STDERR: ", "STDERR: " },
            { "Impossibile avviare robocopy.exe.", "Unable to start robocopy.exe." },
            { "Copia in corso", "Copy in progress" },
            { "Codice di uscita Robocopy: ", "Robocopy exit code: " },
            { "Gia completato nel tentativo precedente: ", "Already completed in the previous attempt: " },
            { "Conserva entrambi: ", "Keep both: " },
            { "Copia non riuscita: ", "Copy failed: " },
            { "AVVISO metadati: ", "METADATA WARNING: " },
            { "Copia con Robocopy", "Copy with Robocopy" },
            { "Preparazione della copia...", "Preparing copy..." },
            { "Impostazioni...", "Settings..." },
            { "Analisi delle sorgenti e della destinazione", "Analyzing sources and destination" },
            { "Piu dettagli", "More details" },
            { "Meno dettagli", "Fewer details" },
            { "Apri destinazione", "Open destination" },
            { "Copia riepilogo", "Copy summary" },
            { "Salva report...", "Save report..." },
            { "Verifica SHA-256", "Verify SHA-256" },
            { "Riprova", "Retry" },
            { "Conserva un report per tutte le copie", "Keep a report for every copy" },
            { "ERRORE PREPARAZIONE: ", "PREPARATION ERROR: " },
            { "Preparazione non riuscita", "Preparation failed" },
            { "AVVISO: ", "WARNING: " },
            { "Copia non avviata", "Copy not started" },
            { "Analisi dei conflitti...", "Analyzing conflicts..." },
            { "Spazio libero insufficiente", "Insufficient free space" },
            { "Servono circa ", "About " },
            { ", ma sono disponibili ", " is required, but only " },
            { "Copia di ", "Copying " },
            { " file da ", " files from " },
            { " sorgente", " source" },
            { " sorgenti", " sources" },
            { " a ", " to " },
            { "Velocita: calcolo...", "Speed: calculating..." },
            { "Tempo rimanente: calcolo...", "Time remaining: calculating..." },
            { "Avvio di Robocopy...", "Starting Robocopy..." },
            { " file da copiare (", " files to copy (" },
            { "Modalita thread selezionata: ", "Selected thread mode: " },
            { "Velocita: ", "Speed: " },
            { "Tempo rimanente: ", "Time remaining: " },
            { "Rimanenti: ", "Remaining: " },
            { " file (", " files (" },
            { "Annullata", "Cancelled" },
            { "Con errori", "With errors" },
            { "Durata: ", "Duration: " },
            { "Operazione completata", "Operation completed" },
            { "Operazione annullata", "Operation cancelled" },
            { "Operazione completata con problemi", "Operation completed with issues" },
            { "Copia completata", "Copy completed" },
            { "Copia annullata", "Copy cancelled" },
            { "Copia completata con problemi", "Copy completed with issues" },
            { "Chiudi", "Close" },
            { "Report salvato automaticamente: ", "Report saved automatically: " },
            { "Nessun file e stato copiato.", "No files were copied." },
            { "Consulta Piu dettagli per informazioni tecniche.", "See More details for technical information." },
            { "Errore", "Error" },
            { "Copiati: ", "Copied: " },
            { " | Gia aggiornati: ", " | Already up to date: " },
            { " | Sostituiti: ", " | Replaced: " },
            { " | Ignorati: ", " | Skipped: " },
            { " | Conservati entrambi: ", " | Kept both: " },
            { "ROBOCOPY DROP - RIEPILOGO", "ROBOCOPY DROP - SUMMARY" },
            { "Esito: ", "Result: " },
            { "Completata", "Completed" },
            { "Completata con problemi", "Completed with issues" },
            { "Avvio: ", "Started: " },
            { "Fine: ", "Finished: " },
            { "File analizzati: ", "Files analyzed: " },
            { "File da copiare: ", "Files to copy: " },
            { "Dati pianificati: ", "Planned data: " },
            { "Impostazione thread usata: ", "Thread setting used: " },
            { "Gia aggiornati: ", "Already up to date: " },
            { "Sostituiti: ", "Replaced: " },
            { "Ignorati dall'utente: ", "Skipped by user: " },
            { "Conservati entrambi: ", "Kept both: " },
            { "Avvisi: ", "Warnings: " },
            { "Errori preparazione: ", "Preparation errors: " },
            { "File completati: ", "Files completed: " },
            { "Dati completati: ", "Data completed: " },
            { "Operazioni Robocopy fallite: ", "Failed Robocopy operations: " },
            { "Copie native fallite: ", "Failed native copies: " },
            { "Errore: ", "Error: " },
            { "Report automatico: ", "Automatic report: " },
            { "DETTAGLI TECNICI", "TECHNICAL DETAILS" },
            { "[Dettagli troncati per limite di memoria]", "[Details truncated due to memory limit]" },
            { "Impostazione thread aggiornata: ", "Thread setting updated: " },
            { "L'impostazione e stata salvata. Sara usata dalla prossima copia o dal prossimo tentativo.", "The setting has been saved. It will be used for the next copy or retry." },
            { "Impostazioni salvate", "Settings saved" },
            { "Vuoi annullare l'operazione? I file gia completati resteranno nella destinazione.", "Cancel the operation? Files already completed will remain in the destination." },
            { "Annulla operazione", "Cancel operation" },
            { "Annullamento in corso...", "Cancelling..." },
            { "La copia o la verifica e ancora in corso. Vuoi annullarla e chiudere?", "A copy or verification is still running. Cancel it and close?" },
            { "Operazione in corso", "Operation in progress" },
            { "===== NUOVO TENTATIVO =====", "===== NEW ATTEMPT =====" },
            { "Riepilogo copiato negli appunti.", "Summary copied to the clipboard." },
            { "Salva report Robocopy Drop", "Save Robocopy Drop report" },
            { "File di testo (*.txt)|*.txt|Tutti i file (*.*)|*.*", "Text files (*.txt)|*.txt|All files (*.*)|*.*" },
            { "La verifica SHA-256 rileggera integralmente sorgente e destinazione e puo richiedere molto tempo. Continuare?", "SHA-256 verification will fully reread the source and destination and may take a long time. Continue?" },
            { "Verifica SHA-256 in corso", "SHA-256 verification in progress" },
            { "File verificati: ", "Files verified: " },
            { "Calcolo hash...", "Calculating hash..." },
            { "Verifica SHA-256 annullata", "SHA-256 verification cancelled" },
            { "Verifica SHA-256 completata", "SHA-256 verification completed" },
            { "Tutti i file verificati sono identici byte per byte.", "All verified files are byte-for-byte identical." },
            { "Nessuna differenza rilevata.", "No differences detected." },
            { "Verifica SHA-256: tutti i file coincidono.", "SHA-256 verification: all files match." },
            { "Verifica SHA-256 completata con differenze", "SHA-256 verification completed with differences" },
            { " differenze o errori rilevati.", " differences or errors detected." },
            { "Consulta Piu dettagli.", "See More details." },
            { "Differenze", "Differences" },
            { "HASH: ", "HASH: " },
            { "Richiesta di copia non valida.", "Invalid copy request." },
            { "Impossibile aprire la cartella dei report: ", "Unable to open the reports folder: " },
            { "Guida non trovata: ", "Guide not found: " },
            { "Impossibile aprire la guida: ", "Unable to open the guide: " },
            { "Vuoi disinstallare Robocopy Drop?", "Uninstall Robocopy Drop?" },
            { "Robocopy Drop - Disinstalla", "Robocopy Drop - Uninstall" },
            { "Impossibile avviare la disinstallazione: ", "Unable to start uninstall: " },
            { "Profilo manuale", "Manual profile" },
            { "Profilo locale veloce", "Fast local drive profile" },
            { "Profilo USB/rimovibile - file piccoli", "USB/removable profile - small files" },
            { "Profilo USB/rimovibile - file grandi", "USB/removable profile - large files" },
            { "Profilo rete", "Network profile" },
            { "Profilo supporto ottico", "Optical media profile" },
            { "Profilo prudente", "Conservative profile" },
            { " - Copia", " - Copy" },
            { " (errore ", " (error " },
            { "Aggiornamenti", "Updates" },
            { "Controlla automaticamente gli aggiornamenti", "Automatically check for updates" },
            { "Versione installata: ", "Installed version: " },
            { "Controlla ora", "Check now" },
            { "Aggiorna ora", "Update now" },
            { "Ultima verifica: mai", "Last check: never" },
            { "Controllo aggiornamenti in corso...", "Checking for updates..." },
            { "Repository GitHub non configurato.", "GitHub repository is not configured." },
            { "La risposta GitHub non contiene una release stabile valida.", "The GitHub response does not contain a valid stable release." },
            { "La versione della release GitHub non e riconoscibile.", "The GitHub release version could not be recognized." },
            { "La release non contiene l'MSI previsto per la lingua installata: ", "The release does not contain the expected MSI for the installed language: " },
            { "L'URL dell'asset GitHub non e valido.", "The GitHub asset URL is invalid." },
            { "Nessun aggiornamento disponibile.", "No updates are available." },
            { "Nessuna release pubblicata su GitHub.", "No GitHub release has been published yet." },
            { "Versione disponibile: ", "Available version: " },
            { "Verifica non riuscita: ", "Check failed: " },
            { "E disponibile Robocopy Drop ", "Robocopy Drop " },
            { "Scaricare, verificare e avviare automaticamente l'aggiornamento?", "Download, verify, and start the update automatically?" },
            { "Aggiornamento disponibile", "Update available" },
            { "GitHub non ha fornito il digest SHA-256 dell'asset. Aggiornamento automatico bloccato.", "GitHub did not provide the asset SHA-256 digest. Automatic update blocked." },
            { "Il digest SHA-256 dell'MSI scaricato non corrisponde a GitHub.", "The downloaded MSI SHA-256 digest does not match GitHub." },
            { "L'MSI non ha una firma Authenticode valida. Aggiornamento automatico bloccato.", "The MSI does not have a valid Authenticode signature. Automatic update blocked." },
            { "La firma Authenticode dell'MSI non e attendibile. Aggiornamento automatico bloccato.", "The MSI Authenticode signature is not trusted. Automatic update blocked." },
            { "Il firmatario dell'MSI non corrisponde a quello autorizzato dalla configurazione.", "The MSI signer does not match the signer authorized by the configuration." },
            { "Aggiornamento Robocopy Drop", "Robocopy Drop Update" },
            { "Download e verifica della versione ", "Downloading and verifying version " },
            { "Connessione a GitHub...", "Connecting to GitHub..." },
            { "Il download e stato reindirizzato verso un host non autorizzato.", "The download was redirected to an unauthorized host." },
            { "La catena di reindirizzamento del download GitHub non e valida.", "The GitHub download redirect chain is invalid." },
            { "La dimensione dell'MSI scaricato non corrisponde alla release GitHub.", "The downloaded MSI size does not match the GitHub release." },
            { "Verifica SHA-256 e firma digitale...", "Verifying SHA-256 and digital signature..." },
            { "Aggiornamento annullato.", "Update cancelled." },
            { "Aggiornamento non riuscito: ", "Update failed: " },
            { "Aggiornamento non riuscito", "Update failed" },
            { "Il digest SHA-256 coincide, ma il pacchetto non e firmato digitalmente. Non e possibile verificare l'identita dell'autore. Continuare comunque?", "The SHA-256 digest matches, but the package is not digitally signed. The author's identity cannot be verified. Continue anyway?" },
            { "Pacchetto non firmato", "Unsigned package" },
            { "Installazione non avviata.", "Installation was not started." },
            { "Avvio di Windows Installer...", "Starting Windows Installer..." },
            { "Impossibile avviare Windows Installer: ", "Unable to start Windows Installer: " },
            { "Download in corso: ", "Downloading: " },
            { "Completa o annulla l'operazione corrente prima di aggiornare Robocopy Drop.", "Complete or cancel the current operation before updating Robocopy Drop." },
            { "Aggiornamento rinviato", "Update postponed" }
        };

        public static string Language
        {
            get
            {
                if (string.IsNullOrEmpty(language)) Initialize();
                return language;
            }
        }

        public static bool IsEnglish { get { return string.Equals(Language, "en", StringComparison.OrdinalIgnoreCase); } }

        public static void Initialize()
        {
            if (!string.IsNullOrEmpty(language)) return;
            string value = ReadLanguage(UserRegistryPath, "UILanguage");
            if (string.IsNullOrEmpty(value)) value = ReadLanguage(UserRegistryPath, "DefaultLanguage");
            if (string.IsNullOrEmpty(value)) value = ReadLanguage(MachineRegistryPath, "DefaultLanguage");
            if (string.IsNullOrEmpty(value))
                value = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("it", StringComparison.OrdinalIgnoreCase) ? "it" : "en";
            language = NormalizeLanguage(value);
        }

        private static string ReadLanguage(string path, string name)
        {
            try
            {
                object value = Registry.GetValue(path, name, null);
                return value == null ? null : Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            catch { return null; }
        }

        private static string NormalizeLanguage(string value)
        {
            return !string.IsNullOrEmpty(value) && value.StartsWith("it", StringComparison.OrdinalIgnoreCase) ? "it" : "en";
        }

        public static void SaveLanguage(string value)
        {
            language = NormalizeLanguage(value);
            Registry.SetValue(UserRegistryPath, "UILanguage", language, RegistryValueKind.String);
        }

        public static string T(string italian)
        {
            if (!IsEnglish || string.IsNullOrEmpty(italian)) return italian;
            string translated;
            return English.TryGetValue(italian, out translated) ? translated : italian;
        }

        public static void Apply(Control root)
        {
            if (root == null) return;
            root.Text = T(root.Text);
            foreach (Control child in root.Controls) Apply(child);
        }
    }

    internal sealed class LanguageOption
    {
        public string Value;
        public string Label;
        public LanguageOption(string value, string label) { Value = value; Label = label; }
        public override string ToString() { return Label; }
    }

    internal enum ConflictAction
    {
        None,
        Replace,
        Skip,
        KeepBoth
    }

    internal enum FileDisposition
    {
        NewFile,
        Identical,
        Conflict
    }

    internal sealed class FileItem
    {
        public string SourcePath;
        public string DestinationPath;
        public string EffectiveDestinationPath;
        public long Size;
        public DateTime SourceWriteUtc;
        public DateTime DestinationWriteUtc;
        public long DestinationSize;
        public FileDisposition Disposition;
        public ConflictAction Action;
        public int TopLevelIndex;
    }

    internal sealed class DirectoryItem
    {
        public string SourcePath;
        public string DestinationPath;
        public DateTime SourceWriteUtc;
        public int TopLevelIndex;
    }

    internal sealed class TopLevelItem
    {
        public string SourcePath;
        public string DestinationPath;
        public bool IsDirectory;
        public int Index;
    }

    internal sealed class CopyPlan
    {
        public string DestinationRoot;
        public readonly List<TopLevelItem> TopLevels = new List<TopLevelItem>();
        public readonly List<FileItem> Files = new List<FileItem>();
        public readonly List<DirectoryItem> Directories = new List<DirectoryItem>();
        public readonly List<string> Warnings = new List<string>();
        public readonly List<string> Errors = new List<string>();

        public IEnumerable<FileItem> Conflicts
        {
            get { return Files.Where(f => f.Disposition == FileDisposition.Conflict); }
        }

        public long BytesToCopy
        {
            get
            {
                return Files.Where(ShouldCopy).Sum(f => f.Size);
            }
        }

        public int FilesToCopy
        {
            get { return Files.Count(ShouldCopy); }
        }

        public int IdenticalCount
        {
            get { return Files.Count(f => f.Disposition == FileDisposition.Identical); }
        }

        public int ReplacedCount
        {
            get { return Files.Count(f => f.Disposition == FileDisposition.Conflict && f.Action == ConflictAction.Replace); }
        }

        public int SkippedCount
        {
            get { return Files.Count(f => f.Disposition == FileDisposition.Conflict && f.Action == ConflictAction.Skip); }
        }

        public int KeptBothCount
        {
            get { return Files.Count(f => f.Disposition == FileDisposition.Conflict && f.Action == ConflictAction.KeepBoth); }
        }

        public static bool ShouldCopy(FileItem item)
        {
            if (item.Disposition == FileDisposition.NewFile) return true;
            if (item.Disposition == FileDisposition.Identical) return false;
            return item.Action == ConflictAction.Replace || item.Action == ConflictAction.KeepBoth;
        }
    }

    internal sealed class ProgressSnapshot
    {
        public string Phase;
        public string CurrentItem;
        public long CompletedBytes;
        public long TotalBytes;
        public int CompletedFiles;
        public int TotalFiles;
        public double BytesPerSecond;
        public TimeSpan? Eta;
    }

    internal sealed class CopyResult
    {
        public bool Cancelled;
        public bool Success;
        public int RobocopyFailures;
        public int NativeCopyFailures;
        public int CompletedFiles;
        public long CompletedBytes;
        public readonly List<string> Errors = new List<string>();
    }

    internal static class NativeMethods
    {
        public const int SW_HIDE = 0;
        public const int SW_RESTORE = 9;
        public const uint COPY_FILE_FAIL_IF_EXISTS = 0x00000001;
        public const uint COPY_FILE_RESTARTABLE = 0x00000002;
        public const uint PROGRESS_CONTINUE = 0;
        public const uint PROGRESS_CANCEL = 1;
        public const uint TBPF_NOPROGRESS = 0x0;
        public const uint TBPF_INDETERMINATE = 0x1;
        public const uint TBPF_NORMAL = 0x2;
        public const uint TBPF_ERROR = 0x4;
        public const uint TBPF_PAUSED = 0x8;

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        public delegate uint CopyProgressRoutine(
            long TotalFileSize,
            long TotalBytesTransferred,
            long StreamSize,
            long StreamBytesTransferred,
            uint StreamNumber,
            uint CallbackReason,
            IntPtr SourceFile,
            IntPtr DestinationFile,
            IntPtr Data);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessIoCounters(IntPtr processHandle, out IO_COUNTERS counters);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CopyFileEx(
            string existingFileName,
            string newFileName,
            CopyProgressRoutine progressRoutine,
            IntPtr data,
            ref int cancel,
            uint copyFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CommandLineToArgvW(string commandLine, out int argc);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LocalFree(IntPtr handle);
    }

    [ComImport]
    [Guid("EA1AFB91-9E28-4B86-90E9-9E9F8A5EEA84")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITaskbarList3
    {
        void HrInit();
        void AddTab(IntPtr hwnd);
        void DeleteTab(IntPtr hwnd);
        void ActivateTab(IntPtr hwnd);
        void SetActiveAlt(IntPtr hwnd);
        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fullscreen);
        void SetProgressValue(IntPtr hwnd, ulong completed, ulong total);
        void SetProgressState(IntPtr hwnd, uint flags);
    }

    [ComImport]
    [Guid("56FDF344-FD6D-11D0-958A-006097C9A090")]
    internal class TaskbarList
    {
    }

    internal static class ThemeHelper
    {
        public static bool IsDarkMode()
        {
            try
            {
                object value = Registry.GetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                    "AppsUseLightTheme", 1);
                return Convert.ToInt32(value, CultureInfo.InvariantCulture) == 0;
            }
            catch
            {
                return false;
            }
        }

        public static void Apply(Form form)
        {
            if (!IsDarkMode()) return;
            Color background = Color.FromArgb(32, 32, 32);
            Color field = Color.FromArgb(24, 24, 24);
            Color button = Color.FromArgb(52, 52, 52);
            Color text = Color.FromArgb(242, 242, 242);
            Color muted = Color.FromArgb(185, 185, 185);
            form.BackColor = background;
            form.ForeColor = text;
            ApplyToControls(form.Controls, background, field, button, text, muted);
            form.HandleCreated += delegate
            {
                try
                {
                    int enabled = 1;
                    NativeMethods.DwmSetWindowAttribute(form.Handle, 20, ref enabled, sizeof(int));
                }
                catch
                {
                }
            };
        }

        private static void ApplyToControls(Control.ControlCollection controls, Color background, Color field, Color button, Color text, Color muted)
        {
            foreach (Control control in controls)
            {
                Label label = control as Label;
                TextBox box = control as TextBox;
                Button action = control as Button;
                GroupBox group = control as GroupBox;
                CheckBox check = control as CheckBox;
                ComboBox combo = control as ComboBox;

                if (box != null)
                {
                    box.BackColor = field;
                    box.ForeColor = text;
                }
                else if (combo != null)
                {
                    combo.BackColor = field;
                    combo.ForeColor = text;
                    combo.FlatStyle = FlatStyle.Flat;
                }
                else if (action != null)
                {
                    action.UseVisualStyleBackColor = false;
                    action.BackColor = button;
                    action.ForeColor = text;
                    action.FlatStyle = FlatStyle.Flat;
                    action.FlatAppearance.BorderColor = Color.FromArgb(85, 85, 85);
                }
                else if (label != null)
                {
                    bool wasMuted = label.ForeColor == SystemColors.GrayText;
                    label.BackColor = background;
                    label.ForeColor = wasMuted ? muted : text;
                }
                else if (group != null || check != null)
                {
                    control.BackColor = background;
                    control.ForeColor = text;
                }

                if (control.HasChildren) ApplyToControls(control.Controls, background, field, button, text, muted);
            }
        }
    }

    internal sealed class ThreadOption
    {
        public int Value;
        public string Label;

        public ThreadOption(int value, string label)
        {
            Value = value;
            Label = label;
        }

        public override string ToString()
        {
            return Label;
        }
    }

    internal sealed class DestinationMediaProfile
    {
        public DriveType DriveType;
        public bool IsUsb;
        public string FileSystem;
    }

    internal static class DestinationMediaDetector
    {
        public static DestinationMediaProfile Detect(string destinationRoot)
        {
            DestinationMediaProfile profile = new DestinationMediaProfile();
            profile.DriveType = DriveType.Unknown;
            profile.IsUsb = false;
            profile.FileSystem = string.Empty;

            string root = null;
            try
            {
                root = Path.GetPathRoot(destinationRoot);
                if (!string.IsNullOrEmpty(root))
                {
                    DriveInfo drive = new DriveInfo(root);
                    profile.DriveType = drive.DriveType;
                    if (drive.IsReady) profile.FileSystem = drive.DriveFormat ?? string.Empty;
                }
            }
            catch
            {
                profile.DriveType = DriveType.Unknown;
            }

            // DriveInfo reports some external USB HDDs/SSDs as Fixed. WMI lets
            // us identify the underlying physical bus without requiring admin.
            if (string.IsNullOrEmpty(root) || root.StartsWith("\\\\", StringComparison.Ordinal))
                return profile;

            string logicalDeviceId = root.TrimEnd('\\');
            if (logicalDeviceId.Length != 2 || logicalDeviceId[1] != ':')
                return profile;

            try
            {
                string escaped = logicalDeviceId.Replace("'", "''");
                string partitionQuery = "ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + escaped + "'} " +
                    "WHERE AssocClass=Win32_LogicalDiskToPartition";

                using (ManagementObjectSearcher partitionSearcher = new ManagementObjectSearcher(partitionQuery))
                using (ManagementObjectCollection partitions = partitionSearcher.Get())
                {
                    foreach (ManagementObject partition in partitions)
                    {
                        object diskIndexValue = partition["DiskIndex"];
                        if (diskIndexValue == null) continue;
                        uint diskIndex = Convert.ToUInt32(diskIndexValue, CultureInfo.InvariantCulture);
                        string diskQuery = "SELECT InterfaceType, PNPDeviceID, MediaType, Model FROM Win32_DiskDrive WHERE Index=" +
                            diskIndex.ToString(CultureInfo.InvariantCulture);

                        using (ManagementObjectSearcher diskSearcher = new ManagementObjectSearcher(diskQuery))
                        using (ManagementObjectCollection disks = diskSearcher.Get())
                        {
                            foreach (ManagementObject disk in disks)
                            {
                                string interfaceType = Convert.ToString(disk["InterfaceType"], CultureInfo.InvariantCulture) ?? string.Empty;
                                string pnp = Convert.ToString(disk["PNPDeviceID"], CultureInfo.InvariantCulture) ?? string.Empty;
                                string model = Convert.ToString(disk["Model"], CultureInfo.InvariantCulture) ?? string.Empty;
                                string media = Convert.ToString(disk["MediaType"], CultureInfo.InvariantCulture) ?? string.Empty;
                                string combined = interfaceType + " " + pnp + " " + model + " " + media;
                                if (interfaceType.Equals("USB", StringComparison.OrdinalIgnoreCase) ||
                                    combined.IndexOf("USBSTOR", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    combined.IndexOf(" USB ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    combined.StartsWith("USB ", StringComparison.OrdinalIgnoreCase))
                                {
                                    profile.IsUsb = true;
                                    return profile;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Detection is best-effort. DriveType remains a safe fallback.
            }

            return profile;
        }
    }

    internal static class AppSettings
    {
        private const string RegistryPath = "HKEY_CURRENT_USER\\Software\\RobocopyDrop";
        private static readonly int[] AllowedThreadModes = new int[] { 0, 1, 4, 8, 16, 32, 64 };

        public static int LoadThreadMode()
        {
            try
            {
                object versionValue = Registry.GetValue(RegistryPath, "ThreadModeVersion", 0);
                int settingVersion = Convert.ToInt32(versionValue, CultureInfo.InvariantCulture);
                object value = Registry.GetValue(RegistryPath, "RobocopyThreads", 0);
                int parsed = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                if (!AllowedThreadModes.Contains(parsed)) parsed = 0;

                // In v1.3.4 the default was stored as an explicit 32. From
                // v1.3.5 onward the default is adaptive: still 32 on fast local
                // disks, but lower on USB/removable and network destinations.
                if (settingVersion < 2)
                {
                    if (parsed == 32) parsed = 0;
                    Registry.SetValue(RegistryPath, "RobocopyThreads", parsed, RegistryValueKind.DWord);
                    Registry.SetValue(RegistryPath, "ThreadModeVersion", 2, RegistryValueKind.DWord);
                }
                return parsed;
            }
            catch
            {
                return 0;
            }
        }

        public static void SaveThreadMode(int value)
        {
            if (!AllowedThreadModes.Contains(value)) value = 0;
            Registry.SetValue(RegistryPath, "RobocopyThreads", value, RegistryValueKind.DWord);
            Registry.SetValue(RegistryPath, "ThreadModeVersion", 2, RegistryValueKind.DWord);
        }

        public static int ResolveThreadCount(int mode)
        {
            return mode == 0 ? 32 : mode;
        }

        public static int ResolveThreadCount(int mode, CopyPlan plan, out string profileDescription)
        {
            if (mode != 0)
            {
                profileDescription = UiText.T("Profilo manuale");
                return Math.Max(1, Math.Min(64, mode));
            }

            DestinationMediaProfile media = DestinationMediaDetector.Detect(plan == null ? null : plan.DestinationRoot);
            DriveType driveType = media.DriveType;

            long bytes = plan == null ? 0L : plan.BytesToCopy;
            int files = plan == null ? 0 : plan.FilesToCopy;
            long average = files <= 0 ? 0L : bytes / Math.Max(1, files);

            if (driveType == DriveType.Removable || media.IsUsb)
            {
                if (average > 4L * 1024L * 1024L)
                {
                    profileDescription = UiText.T("Profilo USB/rimovibile - file grandi");
                    return 8;
                }
                profileDescription = UiText.T("Profilo USB/rimovibile - file piccoli");
                return 4;
            }

            switch (driveType)
            {
                case DriveType.Network:
                    profileDescription = UiText.T("Profilo rete");
                    return 8;

                case DriveType.CDRom:
                    profileDescription = UiText.T("Profilo supporto ottico");
                    return 1;

                case DriveType.Fixed:
                    profileDescription = UiText.T("Profilo locale veloce");
                    return 32;

                default:
                    profileDescription = UiText.T("Profilo prudente");
                    return 8;
            }
        }

        public static string DescribeThreadMode(int mode)
        {
            return mode == 0 ? UiText.T("Automatico (adattivo)") : mode.ToString(CultureInfo.CurrentCulture);
        }

        public static ThreadOption[] GetThreadOptions()
        {
            return new ThreadOption[]
            {
                new ThreadOption(0, UiText.T("Automatico (adattivo)")),
                new ThreadOption(1, "1 thread"),
                new ThreadOption(4, "4 thread"),
                new ThreadOption(8, "8 thread"),
                new ThreadOption(16, "16 thread"),
                new ThreadOption(32, UiText.T("32 thread - predefinito locale")),
                new ThreadOption(64, "64 thread")
            };
        }
    }

    internal sealed class GitHubReleaseAsset
    {
        public string name { get; set; }
        public string browser_download_url { get; set; }
        public string digest { get; set; }
        public long size { get; set; }
    }

    internal sealed class GitHubReleaseResponse
    {
        public string tag_name { get; set; }
        public string html_url { get; set; }
        public bool draft { get; set; }
        public bool prerelease { get; set; }
        public GitHubReleaseAsset[] assets { get; set; }
    }

    internal sealed class UpdateConfiguration
    {
        public string Owner;
        public string Repository;
        public string ApiVersion;
        public bool RequireSignedUpdates;
        public string[] AllowedSignerThumbprints;

        public bool IsConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Owner) &&
                       !string.IsNullOrWhiteSpace(Repository) &&
                       !Owner.StartsWith("__", StringComparison.Ordinal) &&
                       !Repository.StartsWith("__", StringComparison.Ordinal);
            }
        }

        public static UpdateConfiguration Load()
        {
            UpdateConfiguration configuration = new UpdateConfiguration();
            configuration.Owner = Read("GitHubOwner");
            configuration.Repository = Read("GitHubRepository");
            configuration.ApiVersion = Read("GitHubApiVersion");
            if (string.IsNullOrWhiteSpace(configuration.ApiVersion)) configuration.ApiVersion = "2026-03-10";

            bool requireSigned;
            configuration.RequireSignedUpdates = bool.TryParse(Read("RequireSignedUpdates"), out requireSigned) && requireSigned;
            string pins = Read("AllowedSignerThumbprints");
            configuration.AllowedSignerThumbprints = string.IsNullOrWhiteSpace(pins)
                ? new string[0]
                : pins.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(NormalizeThumbprint)
                      .Where(delegate(string value) { return value.Length > 0; })
                      .Distinct(StringComparer.OrdinalIgnoreCase)
                      .ToArray();
            return configuration;
        }

        private static string Read(string key)
        {
            try { return ConfigurationManager.AppSettings[key] ?? string.Empty; }
            catch { return string.Empty; }
        }

        public static string NormalizeThumbprint(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            StringBuilder normalized = new StringBuilder();
            foreach (char character in value)
            {
                if (Uri.IsHexDigit(character)) normalized.Append(char.ToUpperInvariant(character));
            }
            return normalized.ToString();
        }
    }

    internal sealed class UpdateCheckResult
    {
        public bool IsConfigured;
        public bool NoPublishedRelease;
        public bool IsUpdateAvailable;
        public Version CurrentVersion;
        public Version LatestVersion;
        public string ReleasePageUrl;
        public string AssetName;
        public string AssetUrl;
        public string AssetDigest;
        public long AssetSize;
        public string ErrorMessage;
    }

    internal sealed class AuthenticodeVerificationResult
    {
        public bool IsSigned;
        public bool IsTrusted;
        public int StatusCode;
        public string SignerThumbprint;
        public string SignerSubject;
    }

    internal static class AuthenticodeVerifier
    {
        private const uint WtdUiNone = 2;
        private const uint WtdRevokeNone = 0;
        private const uint WtdChoiceFile = 1;
        private const uint WtdStateActionVerify = 1;
        private const uint WtdStateActionClose = 2;
        private const uint WtdProvFlags = 0;
        private const int TrustENoSignature = unchecked((int)0x800B0100);
        private const int TrustESubjectFormUnknown = unchecked((int)0x800B0003);

        private static readonly Guid WintrustActionGenericVerifyV2 =
            new Guid("00AAC56B-CD44-11D0-8CC2-00C04FC295EE");

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WintrustFileInfo
        {
            public uint cbStruct;
            public IntPtr pcwszFilePath;
            public IntPtr hFile;
            public IntPtr pgKnownSubject;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WintrustData
        {
            public uint cbStruct;
            public IntPtr pPolicyCallbackData;
            public IntPtr pSIPClientData;
            public uint dwUIChoice;
            public uint fdwRevocationChecks;
            public uint dwUnionChoice;
            public IntPtr pFile;
            public uint dwStateAction;
            public IntPtr hWVTStateData;
            public IntPtr pwszURLReference;
            public uint dwProvFlags;
            public uint dwUIContext;
        }

        [DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern int WinVerifyTrust(IntPtr hwnd, ref Guid actionId, IntPtr trustData);

        public static AuthenticodeVerificationResult Verify(string path)
        {
            AuthenticodeVerificationResult result = new AuthenticodeVerificationResult();
            IntPtr filePathPointer = IntPtr.Zero;
            IntPtr fileInfoPointer = IntPtr.Zero;
            IntPtr trustDataPointer = IntPtr.Zero;
            try
            {
                filePathPointer = Marshal.StringToCoTaskMemUni(path);
                WintrustFileInfo fileInfo = new WintrustFileInfo();
                fileInfo.cbStruct = (uint)Marshal.SizeOf(typeof(WintrustFileInfo));
                fileInfo.pcwszFilePath = filePathPointer;
                fileInfo.hFile = IntPtr.Zero;
                fileInfo.pgKnownSubject = IntPtr.Zero;

                fileInfoPointer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WintrustFileInfo)));
                Marshal.StructureToPtr(fileInfo, fileInfoPointer, false);

                WintrustData trustData = new WintrustData();
                trustData.cbStruct = (uint)Marshal.SizeOf(typeof(WintrustData));
                trustData.pPolicyCallbackData = IntPtr.Zero;
                trustData.pSIPClientData = IntPtr.Zero;
                trustData.dwUIChoice = WtdUiNone;
                trustData.fdwRevocationChecks = WtdRevokeNone;
                trustData.dwUnionChoice = WtdChoiceFile;
                trustData.pFile = fileInfoPointer;
                trustData.dwStateAction = WtdStateActionVerify;
                trustData.hWVTStateData = IntPtr.Zero;
                trustData.pwszURLReference = IntPtr.Zero;
                trustData.dwProvFlags = WtdProvFlags;
                trustData.dwUIContext = 0;

                trustDataPointer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WintrustData)));
                Marshal.StructureToPtr(trustData, trustDataPointer, false);
                Guid action = WintrustActionGenericVerifyV2;
                int status = WinVerifyTrust(new IntPtr(-1), ref action, trustDataPointer);
                result.StatusCode = status;
                result.IsTrusted = status == 0;
                result.IsSigned = status != TrustENoSignature && status != TrustESubjectFormUnknown;

                WintrustData closeData = (WintrustData)Marshal.PtrToStructure(trustDataPointer, typeof(WintrustData));
                closeData.dwStateAction = WtdStateActionClose;
                Marshal.StructureToPtr(closeData, trustDataPointer, false);
                WinVerifyTrust(new IntPtr(-1), ref action, trustDataPointer);

                if (result.IsTrusted)
                {
                    try
                    {
                        X509Certificate certificate = X509Certificate.CreateFromSignedFile(path);
                        using (X509Certificate2 certificate2 = new X509Certificate2(certificate))
                        {
                            result.SignerThumbprint = UpdateConfiguration.NormalizeThumbprint(certificate2.Thumbprint);
                            result.SignerSubject = certificate2.Subject;
                        }
                    }
                    catch
                    {
                        result.SignerThumbprint = string.Empty;
                        result.SignerSubject = string.Empty;
                    }
                }
            }
            finally
            {
                if (trustDataPointer != IntPtr.Zero) Marshal.FreeCoTaskMem(trustDataPointer);
                if (fileInfoPointer != IntPtr.Zero) Marshal.FreeCoTaskMem(fileInfoPointer);
                if (filePathPointer != IntPtr.Zero) Marshal.FreeCoTaskMem(filePathPointer);
            }
            return result;
        }
    }

    internal sealed class PackageVerificationResult
    {
        public bool IsUnsigned;
        public string SignerSubject;
    }

    internal static class UpdateManager
    {
        private const string RegistryPath = "HKEY_CURRENT_USER\\Software\\RobocopyDrop";
        private static readonly TimeSpan AutomaticCheckInterval = TimeSpan.FromHours(24);

        public static Version CurrentVersion
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return version ?? new Version(0, 0, 0, 0);
            }
        }

        public static string CurrentVersionText
        {
            get
            {
                Version version = CurrentVersion;
                return version.Major.ToString(CultureInfo.InvariantCulture) + "." +
                       version.Minor.ToString(CultureInfo.InvariantCulture) + "." +
                       Math.Max(0, version.Build).ToString(CultureInfo.InvariantCulture);
            }
        }

        public static bool IsConfigured
        {
            get { return UpdateConfiguration.Load().IsConfigured; }
        }

        public static bool LoadAutomaticCheckEnabled()
        {
            try
            {
                object value = Registry.GetValue(RegistryPath, "AutomaticUpdateChecks", 1);
                return Convert.ToInt32(value, CultureInfo.InvariantCulture) != 0;
            }
            catch { return true; }
        }

        public static void SaveAutomaticCheckEnabled(bool enabled)
        {
            Registry.SetValue(RegistryPath, "AutomaticUpdateChecks", enabled ? 1 : 0, RegistryValueKind.DWord);
        }

        public static bool IsAutomaticCheckDue()
        {
            if (!LoadAutomaticCheckEnabled() || !IsConfigured) return false;
            try
            {
                object value = Registry.GetValue(RegistryPath, "LastUpdateCheckUtc", null);
                if (value == null) return true;
                DateTime parsed;
                if (!DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out parsed)) return true;
                return DateTime.UtcNow - parsed.ToUniversalTime() >= AutomaticCheckInterval;
            }
            catch { return true; }
        }

        public static void BeginCheck(Control dispatcher, bool force, Action<UpdateCheckResult> completed)
        {
            if (dispatcher == null || completed == null) return;
            if (!force && !IsAutomaticCheckDue()) return;

            Thread thread = new Thread(delegate()
            {
                UpdateCheckResult result = CheckNow();
                if (dispatcher.IsDisposed || !dispatcher.IsHandleCreated) return;
                try
                {
                    dispatcher.BeginInvoke((MethodInvoker)delegate { completed(result); });
                }
                catch
                {
                }
            });
            thread.IsBackground = true;
            thread.Name = "Robocopy Drop update check";
            thread.Start();
        }

        public static UpdateCheckResult CheckNow()
        {
            UpdateCheckResult result = new UpdateCheckResult();
            result.CurrentVersion = CurrentVersion;
            UpdateConfiguration configuration = UpdateConfiguration.Load();
            result.IsConfigured = configuration.IsConfigured;
            if (!configuration.IsConfigured)
            {
                result.ErrorMessage = UiText.T("Repository GitHub non configurato.");
                return result;
            }

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string endpoint = "https://api.github.com/repos/" + Uri.EscapeDataString(configuration.Owner) + "/" +
                                  Uri.EscapeDataString(configuration.Repository) + "/releases?per_page=20";
                using (TimeoutWebClient client = new TimeoutWebClient(15000))
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.Accept] = "application/vnd.github+json";
                    client.Headers[HttpRequestHeader.UserAgent] = "RobocopyDrop/" + CurrentVersionText;
                    client.Headers["X-GitHub-Api-Version"] = configuration.ApiVersion;
                    string json = client.DownloadString(endpoint);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = 4 * 1024 * 1024;
                    GitHubReleaseResponse[] releases = serializer.Deserialize<GitHubReleaseResponse[]>(json);
                    GitHubReleaseResponse release = releases == null ? null : releases.FirstOrDefault(
                        delegate(GitHubReleaseResponse candidate)
                        {
                            return candidate != null && !candidate.draft && !candidate.prerelease;
                        });
                    if (release == null)
                    {
                        result.NoPublishedRelease = true;
                        return result;
                    }

                    Version latest = ParseVersion(release.tag_name);
                    if (latest == null)
                        throw new InvalidDataException(UiText.T("La versione della release GitHub non e riconoscibile."));

                    result.LatestVersion = latest;
                    result.ReleasePageUrl = release.html_url;
                    result.IsUpdateAvailable = latest > result.CurrentVersion;
                    if (!result.IsUpdateAvailable) return result;

                    string language = UiText.IsEnglish ? "en" : "it";
                    string expectedName = "RobocopyDrop-" + VersionText(latest) + "-" + language + "-x64.msi";
                    GitHubReleaseAsset asset = release.assets == null ? null : release.assets.FirstOrDefault(
                        delegate(GitHubReleaseAsset candidate)
                        {
                            return candidate != null && string.Equals(candidate.name, expectedName, StringComparison.OrdinalIgnoreCase);
                        });
                    if (asset == null)
                        throw new FileNotFoundException(UiText.T("La release non contiene l'MSI previsto per la lingua installata: ") + expectedName);

                    ValidateReleaseAssetUrl(asset.browser_download_url);
                    result.AssetName = asset.name;
                    result.AssetUrl = asset.browser_download_url;
                    result.AssetDigest = asset.digest;
                    result.AssetSize = asset.size;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                try
                {
                    Registry.SetValue(RegistryPath, "LastUpdateCheckUtc", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture), RegistryValueKind.String);
                }
                catch
                {
                }
            }
        }

        public static void ConfirmAndInstall(IWin32Window owner, UpdateCheckResult update)
        {
            if (update == null || !update.IsUpdateAvailable) return;
            DialogResult answer = MessageBox.Show(owner,
                UiText.T("E disponibile Robocopy Drop ") + VersionText(update.LatestVersion) + ".\r\n\r\n" +
                UiText.T("Scaricare, verificare e avviare automaticamente l'aggiornamento?"),
                UiText.T("Aggiornamento disponibile"), MessageBoxButtons.YesNo, MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
            if (answer != DialogResult.Yes) return;

            using (UpdateDownloadForm form = new UpdateDownloadForm(update))
            {
                form.ShowDialog(owner);
                if (form.InstallerStarted) Application.Exit();
            }
        }

        public static PackageVerificationResult VerifyDownloadedPackage(string path, UpdateCheckResult update)
        {
            if (string.IsNullOrWhiteSpace(update.AssetDigest) ||
                !update.AssetDigest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException(UiText.T("GitHub non ha fornito il digest SHA-256 dell'asset. Aggiornamento automatico bloccato."));

            string expectedHash = UpdateConfiguration.NormalizeThumbprint(update.AssetDigest.Substring("sha256:".Length));
            string actualHash;
            using (SHA256 algorithm = SHA256.Create())
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024, FileOptions.SequentialScan))
            {
                actualHash = BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty);
            }
            if (!string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
                throw new CryptographicException(UiText.T("Il digest SHA-256 dell'MSI scaricato non corrisponde a GitHub."));

            UpdateConfiguration configuration = UpdateConfiguration.Load();
            AuthenticodeVerificationResult signature = AuthenticodeVerifier.Verify(path);
            bool pinsConfigured = configuration.AllowedSignerThumbprints != null && configuration.AllowedSignerThumbprints.Length > 0;
            if (!signature.IsSigned)
            {
                if (configuration.RequireSignedUpdates || pinsConfigured)
                    throw new CryptographicException(UiText.T("L'MSI non ha una firma Authenticode valida. Aggiornamento automatico bloccato."));
                PackageVerificationResult unsignedResult = new PackageVerificationResult();
                unsignedResult.IsUnsigned = true;
                return unsignedResult;
            }

            if (!signature.IsTrusted)
                throw new CryptographicException(UiText.T("La firma Authenticode dell'MSI non e attendibile. Aggiornamento automatico bloccato.") +
                    " (0x" + signature.StatusCode.ToString("X8", CultureInfo.InvariantCulture) + ")");

            if (pinsConfigured && !configuration.AllowedSignerThumbprints.Contains(signature.SignerThumbprint, StringComparer.OrdinalIgnoreCase))
                throw new CryptographicException(UiText.T("Il firmatario dell'MSI non corrisponde a quello autorizzato dalla configurazione."));

            PackageVerificationResult signedResult = new PackageVerificationResult();
            signedResult.IsUnsigned = false;
            signedResult.SignerSubject = signature.SignerSubject;
            return signedResult;
        }

        public static void StartInstaller(string msiPath)
        {
            ProcessStartInfo information = new ProcessStartInfo();
            information.FileName = Path.Combine(Environment.SystemDirectory, "msiexec.exe");
            information.Arguments = "/i " + PathHelpers.QuoteArgument(msiPath) + " /passive /norestart";
            information.UseShellExecute = true;
            Process.Start(information);
        }

        public static bool RunSelfTest()
        {
            Version version = ParseVersion("v1.6.7");
            if (version == null || version.Major != 1 || version.Minor != 6 || version.Build != 7) return false;
            if (ParseVersion("not-a-version") != null) return false;
            if (UpdateConfiguration.NormalizeThumbprint("aa-bb cc") != "AABBCC") return false;
            return true;
        }

        public static string VersionText(Version version)
        {
            if (version == null) return string.Empty;
            return version.Major.ToString(CultureInfo.InvariantCulture) + "." +
                   version.Minor.ToString(CultureInfo.InvariantCulture) + "." +
                   Math.Max(0, version.Build).ToString(CultureInfo.InvariantCulture);
        }

        private static Version ParseVersion(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            Match match = Regex.Match(value, @"(?<!\d)(\d+)\.(\d+)\.(\d+)(?:\.(\d+))?(?!\d)", RegexOptions.CultureInvariant);
            if (!match.Success) return null;
            int major, minor, build, revision = 0;
            if (!int.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out major) ||
                !int.TryParse(match.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out minor) ||
                !int.TryParse(match.Groups[3].Value, NumberStyles.None, CultureInfo.InvariantCulture, out build)) return null;
            if (match.Groups[4].Success && !int.TryParse(match.Groups[4].Value, NumberStyles.None, CultureInfo.InvariantCulture, out revision)) return null;
            return new Version(major, minor, build, revision);
        }

        private static void ValidateReleaseAssetUrl(string value)
        {
            Uri uri;
            if (!Uri.TryCreate(value, UriKind.Absolute, out uri) ||
                !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException(UiText.T("L'URL dell'asset GitHub non e valido."));
        }

        public static bool IsAllowedDownloadHost(Uri uri)
        {
            if (uri == null || !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)) return false;
            string host = uri.Host;
            return string.Equals(host, "github.com", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(host, "objects.githubusercontent.com", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(host, "release-assets.githubusercontent.com", StringComparison.OrdinalIgnoreCase) ||
                   host.EndsWith(".githubusercontent.com", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal sealed class TimeoutWebClient : WebClient
    {
        private readonly int timeoutMilliseconds;
        public TimeoutWebClient(int timeoutMilliseconds) { this.timeoutMilliseconds = timeoutMilliseconds; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request != null) request.Timeout = timeoutMilliseconds;
            HttpWebRequest http = request as HttpWebRequest;
            if (http != null) http.ReadWriteTimeout = timeoutMilliseconds;
            return request;
        }
    }

    internal sealed class UpdateDownloadForm : Form
    {
        private readonly UpdateCheckResult update;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Button cancelButton;
        private volatile bool cancellationRequested;
        private bool workerFinished;

        public bool InstallerStarted { get; private set; }

        public UpdateDownloadForm(UpdateCheckResult update)
        {
            this.update = update;
            BuildUi();
            Shown += delegate { StartDownload(); };
            FormClosing += UpdateDownloadFormClosing;
        }

        private void BuildUi()
        {
            Text = UiText.T("Aggiornamento Robocopy Drop");
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = true;
            ClientSize = new Size(520, 180);
            Font = SystemFonts.MessageBoxFont;
            AutoScaleMode = AutoScaleMode.Dpi;
            Icon = SystemIcons.Information;

            Label title = new Label();
            title.Text = UiText.T("Download e verifica della versione ") + UpdateManager.VersionText(update.LatestVersion);
            title.Font = new Font(Font.FontFamily, 12.0f, FontStyle.Regular);
            title.Location = new Point(22, 20);
            title.Size = new Size(475, 28);
            Controls.Add(title);

            statusLabel = new Label();
            statusLabel.Text = UiText.T("Connessione a GitHub...");
            statusLabel.AutoEllipsis = true;
            statusLabel.Location = new Point(22, 58);
            statusLabel.Size = new Size(475, 40);
            Controls.Add(statusLabel);

            progressBar = new ProgressBar();
            progressBar.Location = new Point(22, 104);
            progressBar.Size = new Size(475, 22);
            progressBar.Style = ProgressBarStyle.Continuous;
            Controls.Add(progressBar);

            cancelButton = new Button();
            cancelButton.Text = UiText.T("Annulla");
            cancelButton.Location = new Point(407, 140);
            cancelButton.Size = new Size(90, 30);
            cancelButton.Click += delegate { RequestCancellation(); };
            Controls.Add(cancelButton);
            CancelButton = cancelButton;

            UiText.Apply(this);
            ThemeHelper.Apply(this);
        }

        private void StartDownload()
        {
            Thread worker = new Thread(DownloadWorker);
            worker.IsBackground = true;
            worker.Name = "Robocopy Drop update download";
            worker.Start();
        }

        private void DownloadWorker()
        {
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RobocopyDrop", "Updates", UpdateManager.VersionText(update.LatestVersion));
            string finalPath = Path.Combine(directory, update.AssetName);
            string temporaryPath = finalPath + ".download";
            try
            {
                Directory.CreateDirectory(directory);
                TryDelete(temporaryPath);

                using (HttpWebResponse response = OpenAllowedDownloadResponse(update.AssetUrl))
                {
                    long total = response.ContentLength > 0 ? response.ContentLength : update.AssetSize;
                    using (Stream input = response.GetResponseStream())
                    using (FileStream output = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None, 128 * 1024, FileOptions.SequentialScan))
                    {
                        byte[] buffer = new byte[128 * 1024];
                        long completed = 0;
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (cancellationRequested) throw new OperationCanceledException();
                            output.Write(buffer, 0, read);
                            completed += read;
                            ReportProgress(completed, total);
                        }
                    }
                }

                if (cancellationRequested) throw new OperationCanceledException();
                FileInfo downloaded = new FileInfo(temporaryPath);
                if (update.AssetSize > 0 && downloaded.Length != update.AssetSize)
                    throw new InvalidDataException(UiText.T("La dimensione dell'MSI scaricato non corrisponde alla release GitHub."));

                BeginInvoke((MethodInvoker)delegate { statusLabel.Text = UiText.T("Verifica SHA-256 e firma digitale..."); });
                PackageVerificationResult verification = UpdateManager.VerifyDownloadedPackage(temporaryPath, update);
                TryDelete(finalPath);
                File.Move(temporaryPath, finalPath);
                BeginInvoke((MethodInvoker)delegate { CompleteVerification(finalPath, verification); });
            }
            catch (OperationCanceledException)
            {
                TryDelete(temporaryPath);
                BeginInvoke((MethodInvoker)delegate
                {
                    workerFinished = true;
                    statusLabel.Text = UiText.T("Aggiornamento annullato.");
                    Close();
                });
            }
            catch (Exception ex)
            {
                TryDelete(temporaryPath);
                BeginInvoke((MethodInvoker)delegate
                {
                    workerFinished = true;
                    cancelButton.Text = UiText.T("Chiudi");
                    cancelButton.Enabled = true;
                    statusLabel.Text = UiText.T("Aggiornamento non riuscito: ") + ex.Message;
                    progressBar.Value = 0;
                    MessageBox.Show(this, ex.Message, UiText.T("Aggiornamento non riuscito"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        private static HttpWebResponse OpenAllowedDownloadResponse(string initialUrl)
        {
            Uri current;
            if (!Uri.TryCreate(initialUrl, UriKind.Absolute, out current) || !UpdateManager.IsAllowedDownloadHost(current))
                throw new InvalidDataException(UiText.T("L'URL dell'asset GitHub non e valido."));

            const int maximumRedirects = 6;
            for (int redirect = 0; redirect <= maximumRedirects; redirect++)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(current);
                request.UserAgent = "RobocopyDrop/" + UpdateManager.CurrentVersionText;
                request.Accept = "application/octet-stream";
                request.AllowAutoRedirect = false;
                request.Timeout = 30000;
                request.ReadWriteTimeout = 30000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                int statusCode = (int)response.StatusCode;
                bool isRedirect = statusCode == 301 || statusCode == 302 || statusCode == 303 ||
                                  statusCode == 307 || statusCode == 308;
                if (!isRedirect)
                {
                    if (!UpdateManager.IsAllowedDownloadHost(response.ResponseUri))
                    {
                        response.Close();
                        throw new InvalidDataException(UiText.T("Il download e stato reindirizzato verso un host non autorizzato."));
                    }
                    return response;
                }

                string location = response.Headers[HttpResponseHeader.Location];
                response.Close();
                if (redirect == maximumRedirects || string.IsNullOrWhiteSpace(location))
                    throw new InvalidDataException(UiText.T("La catena di reindirizzamento del download GitHub non e valida."));

                Uri next;
                if (!Uri.TryCreate(current, location, out next) || !UpdateManager.IsAllowedDownloadHost(next))
                    throw new InvalidDataException(UiText.T("Il download e stato reindirizzato verso un host non autorizzato."));
                current = next;
            }

            throw new InvalidDataException(UiText.T("La catena di reindirizzamento del download GitHub non e valida."));
        }

        private void CompleteVerification(string path, PackageVerificationResult verification)
        {
            workerFinished = true;
            if (verification.IsUnsigned)
            {
                DialogResult unsignedAnswer = MessageBox.Show(this,
                    UiText.T("Il digest SHA-256 coincide, ma il pacchetto non e firmato digitalmente. Non e possibile verificare l'identita dell'autore. Continuare comunque?"),
                    UiText.T("Pacchetto non firmato"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (unsignedAnswer != DialogResult.Yes)
                {
                    statusLabel.Text = UiText.T("Installazione non avviata.");
                    cancelButton.Text = UiText.T("Chiudi");
                    cancelButton.Enabled = true;
                    return;
                }
            }

            try
            {
                statusLabel.Text = UiText.T("Avvio di Windows Installer...");
                cancelButton.Enabled = false;
                UpdateManager.StartInstaller(path);
                InstallerStarted = true;
                Close();
            }
            catch (Exception ex)
            {
                InstallerStarted = false;
                cancelButton.Text = UiText.T("Chiudi");
                cancelButton.Enabled = true;
                statusLabel.Text = UiText.T("Impossibile avviare Windows Installer: ") + ex.Message;
                MessageBox.Show(this, ex.Message, UiText.T("Aggiornamento non riuscito"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReportProgress(long completed, long total)
        {
            if (IsDisposed) return;
            try
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    int percent = total <= 0 ? 0 : (int)Math.Min(100, completed * 100L / total);
                    progressBar.Value = Math.Max(0, Math.Min(100, percent));
                    statusLabel.Text = UiText.T("Download in corso: ") + percent.ToString(CultureInfo.CurrentCulture) + "%";
                });
            }
            catch
            {
            }
        }

        private void RequestCancellation()
        {
            if (workerFinished)
            {
                Close();
                return;
            }
            cancellationRequested = true;
            cancelButton.Enabled = false;
            statusLabel.Text = UiText.T("Annullamento in corso...");
        }

        private void UpdateDownloadFormClosing(object sender, FormClosingEventArgs e)
        {
            if (workerFinished || InstallerStarted) return;
            e.Cancel = true;
            RequestCancellation();
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch { }
        }
    }


    internal sealed class SettingsForm : Form
    {
        private readonly bool forceUpdateCheck;
        private ComboBox threadsCombo;
        private ComboBox languageCombo;
        private Label resolvedLabel;
        private CheckBox automaticUpdatesCheckBox;
        private Label updateStatusLabel;
        private Button checkUpdatesButton;
        private Button installUpdateButton;
        private UpdateCheckResult pendingUpdate;
        private bool updateCheckInProgress;

        public int SelectedThreadMode { get; private set; }

        public SettingsForm() : this(false)
        {
        }

        public SettingsForm(bool forceUpdateCheck)
        {
            this.forceUpdateCheck = forceUpdateCheck;
            BuildUi();
            Shown += SettingsFormShown;
        }

        private void BuildUi()
        {
            Text = UiText.T("Impostazioni Robocopy Drop");
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = true;
            ClientSize = new Size(540, 520);
            Font = SystemFonts.MessageBoxFont;
            AutoScaleMode = AutoScaleMode.Dpi;
            Icon = SystemIcons.Information;

            Label title = new Label();
            title.Text = UiText.T("Prestazioni avanzate");
            title.Font = new Font(Font.FontFamily, 13.0f, FontStyle.Regular);
            title.Location = new Point(22, 20);
            title.Size = new Size(490, 28);
            Controls.Add(title);

            Label description = new Label();
            description.Text = UiText.T("Numero di thread usati da Robocopy per le copie multithread.");
            description.ForeColor = SystemColors.GrayText;
            description.Location = new Point(22, 55);
            description.Size = new Size(490, 22);
            Controls.Add(description);

            Label label = new Label();
            label.Text = UiText.T("Thread Robocopy:");
            label.Location = new Point(22, 94);
            label.Size = new Size(145, 24);
            Controls.Add(label);

            threadsCombo = new ComboBox();
            threadsCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            threadsCombo.Location = new Point(172, 90);
            threadsCombo.Size = new Size(320, 28);
            ThreadOption[] options = AppSettings.GetThreadOptions();
            threadsCombo.Items.AddRange(options);
            int current = AppSettings.LoadThreadMode();
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Value == current)
                {
                    threadsCombo.SelectedIndex = i;
                    break;
                }
            }
            if (threadsCombo.SelectedIndex < 0) threadsCombo.SelectedIndex = 0;
            threadsCombo.SelectedIndexChanged += delegate { UpdateResolvedLabel(); };
            Controls.Add(threadsCombo);

            resolvedLabel = new Label();
            resolvedLabel.ForeColor = SystemColors.GrayText;
            resolvedLabel.Location = new Point(172, 124);
            resolvedLabel.Size = new Size(330, 22);
            Controls.Add(resolvedLabel);
            UpdateResolvedLabel();

            Label note = new Label();
            note.Text = UiText.T("Automatico: 32 thread sui dischi locali; meno thread su USB, supporti rimovibili e rete.");
            note.ForeColor = SystemColors.GrayText;
            note.Location = new Point(22, 153);
            note.Size = new Size(490, 42);
            Controls.Add(note);

            Label languageLabel = new Label();
            languageLabel.Text = UiText.T("Lingua interfaccia:");
            languageLabel.Location = new Point(22, 218);
            languageLabel.Size = new Size(145, 24);
            Controls.Add(languageLabel);

            languageCombo = new ComboBox();
            languageCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            languageCombo.Location = new Point(172, 214);
            languageCombo.Size = new Size(220, 28);
            LanguageOption[] languages = new LanguageOption[]
            {
                new LanguageOption("it", UiText.T("Italiano")),
                new LanguageOption("en", UiText.T("Inglese"))
            };
            languageCombo.Items.AddRange(languages);
            for (int i = 0; i < languages.Length; i++)
            {
                if (string.Equals(languages[i].Value, UiText.Language, StringComparison.OrdinalIgnoreCase))
                {
                    languageCombo.SelectedIndex = i;
                    break;
                }
            }
            if (languageCombo.SelectedIndex < 0) languageCombo.SelectedIndex = 0;
            Controls.Add(languageCombo);

            Label languageNote = new Label();
            languageNote.Text = UiText.T("La nuova lingua sara usata dalla prossima finestra di Robocopy Drop.");
            languageNote.ForeColor = SystemColors.GrayText;
            languageNote.Location = new Point(172, 248);
            languageNote.Size = new Size(330, 42);
            Controls.Add(languageNote);

            Label separator = new Label();
            separator.BorderStyle = BorderStyle.Fixed3D;
            separator.Location = new Point(22, 296);
            separator.Size = new Size(490, 2);
            Controls.Add(separator);

            Label updateTitle = new Label();
            updateTitle.Text = UiText.T("Aggiornamenti");
            updateTitle.Font = new Font(Font.FontFamily, 12.0f, FontStyle.Regular);
            updateTitle.Location = new Point(22, 312);
            updateTitle.Size = new Size(490, 26);
            Controls.Add(updateTitle);

            Label installedVersion = new Label();
            installedVersion.Text = UiText.T("Versione installata: ") + UpdateManager.CurrentVersionText;
            installedVersion.Location = new Point(22, 344);
            installedVersion.Size = new Size(490, 22);
            Controls.Add(installedVersion);

            automaticUpdatesCheckBox = new CheckBox();
            automaticUpdatesCheckBox.Text = UiText.T("Controlla automaticamente gli aggiornamenti");
            automaticUpdatesCheckBox.AutoSize = true;
            automaticUpdatesCheckBox.Location = new Point(22, 373);
            automaticUpdatesCheckBox.Checked = UpdateManager.LoadAutomaticCheckEnabled();
            automaticUpdatesCheckBox.Enabled = UpdateManager.IsConfigured;
            Controls.Add(automaticUpdatesCheckBox);

            updateStatusLabel = new Label();
            updateStatusLabel.Text = UpdateManager.IsConfigured
                ? UiText.T("Ultima verifica: mai")
                : UiText.T("Repository GitHub non configurato.");
            updateStatusLabel.ForeColor = SystemColors.GrayText;
            updateStatusLabel.AutoEllipsis = true;
            updateStatusLabel.Location = new Point(22, 402);
            updateStatusLabel.Size = new Size(490, 42);
            Controls.Add(updateStatusLabel);

            checkUpdatesButton = new Button();
            checkUpdatesButton.Text = UiText.T("Controlla ora");
            checkUpdatesButton.Location = new Point(22, 448);
            checkUpdatesButton.Size = new Size(120, 32);
            checkUpdatesButton.Enabled = UpdateManager.IsConfigured;
            checkUpdatesButton.Click += delegate { BeginUpdateCheck(true); };
            Controls.Add(checkUpdatesButton);

            installUpdateButton = new Button();
            installUpdateButton.Text = UiText.T("Aggiorna ora");
            installUpdateButton.Location = new Point(152, 448);
            installUpdateButton.Size = new Size(120, 32);
            installUpdateButton.Enabled = false;
            installUpdateButton.Click += UpdateNowClicked;
            Controls.Add(installUpdateButton);

            Button cancel = new Button();
            cancel.Text = UiText.T("Annulla");
            cancel.Location = new Point(330, 472);
            cancel.Size = new Size(82, 32);
            cancel.Click += CancelClicked;
            Controls.Add(cancel);

            Button save = new Button();
            save.Text = UiText.T("Salva");
            save.Location = new Point(420, 472);
            save.Size = new Size(92, 32);
            save.Click += SaveClicked;
            Controls.Add(save);

            AcceptButton = save;
            CancelButton = cancel;
            UiText.Apply(this);
            ThemeHelper.Apply(this);
        }

        private void SettingsFormShown(object sender, EventArgs e)
        {
            if (forceUpdateCheck || (automaticUpdatesCheckBox.Checked && UpdateManager.IsAutomaticCheckDue()))
                BeginUpdateCheck(forceUpdateCheck);
        }

        private void BeginUpdateCheck(bool force)
        {
            if (updateCheckInProgress) return;
            if (!UpdateManager.IsConfigured)
            {
                updateStatusLabel.Text = UiText.T("Repository GitHub non configurato.");
                return;
            }
            updateCheckInProgress = true;
            checkUpdatesButton.Enabled = false;
            installUpdateButton.Enabled = false;
            updateStatusLabel.Text = UiText.T("Controllo aggiornamenti in corso...");
            UpdateManager.BeginCheck(this, force, ApplyUpdateResult);
        }

        private void ApplyUpdateResult(UpdateCheckResult result)
        {
            updateCheckInProgress = false;
            checkUpdatesButton.Enabled = UpdateManager.IsConfigured;
            pendingUpdate = result != null && result.IsUpdateAvailable ? result : null;
            installUpdateButton.Enabled = pendingUpdate != null;
            if (result == null)
            {
                updateStatusLabel.Text = UiText.T("Verifica non riuscita: ") + UiText.T("Errore");
                return;
            }
            if (!result.IsConfigured)
            {
                updateStatusLabel.Text = UiText.T("Repository GitHub non configurato.");
                return;
            }
            if (result.NoPublishedRelease)
            {
                updateStatusLabel.Text = UiText.T("Nessuna release pubblicata su GitHub.");
                return;
            }
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                updateStatusLabel.Text = UiText.T("Verifica non riuscita: ") + result.ErrorMessage;
                return;
            }
            updateStatusLabel.Text = result.IsUpdateAvailable
                ? UiText.T("Versione disponibile: ") + UpdateManager.VersionText(result.LatestVersion)
                : UiText.T("Nessun aggiornamento disponibile.");
        }

        public void ActivateFromExternalRequest(bool checkUpdates)
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            if (!Visible) Show();
            Activate();
            BringToFront();
            if (IsHandleCreated)
            {
                NativeMethods.ShowWindow(Handle, NativeMethods.SW_RESTORE);
                NativeMethods.SetForegroundWindow(Handle);
            }
            if (checkUpdates) BeginUpdateCheck(true);
        }

        private void UpdateNowClicked(object sender, EventArgs e)
        {
            UpdateManager.ConfirmAndInstall(this, pendingUpdate);
        }

        private void UpdateResolvedLabel()
        {
            ThreadOption option = threadsCombo == null ? null : threadsCombo.SelectedItem as ThreadOption;
            int mode = option == null ? 0 : option.Value;
            resolvedLabel.Text = mode == 0
                ? UiText.T("Thread effettivi: determinati all'avvio della copia")
                : UiText.T("Thread effettivi: ") + AppSettings.ResolveThreadCount(mode).ToString(CultureInfo.CurrentCulture);
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            ThreadOption option = threadsCombo.SelectedItem as ThreadOption;
            LanguageOption languageOption = languageCombo.SelectedItem as LanguageOption;
            SelectedThreadMode = option == null ? 0 : option.Value;
            try
            {
                AppSettings.SaveThreadMode(SelectedThreadMode);
                UiText.SaveLanguage(languageOption == null ? UiText.Language : languageOption.Value);
                UpdateManager.SaveAutomaticCheckEnabled(automaticUpdatesCheckBox.Checked);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, UiText.T("Salvataggio non riuscito"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


    internal static class PathHelpers
    {
        public static string Normalize(string path)
        {
            string full = Path.GetFullPath(path);
            string root = Path.GetPathRoot(full);
            int minimum = string.IsNullOrEmpty(root) ? 0 : root.Length;
            while (full.Length > minimum && (full.EndsWith("\\", StringComparison.Ordinal) || full.EndsWith("/", StringComparison.Ordinal)))
                full = full.Substring(0, full.Length - 1);
            return full;
        }

        public static string LeafName(string path)
        {
            string normalized = Normalize(path);
            string name = Path.GetFileName(normalized);
            if (!string.IsNullOrEmpty(name)) return name;
            string root = Path.GetPathRoot(normalized);
            if (string.IsNullOrEmpty(root)) return UiText.T("Sorgente");
            string cleaned = root.Trim('\\', '/').Replace(':', '_').Replace('\\', '_').Replace('/', '_');
            return string.IsNullOrEmpty(cleaned) ? UiText.T("Sorgente") : cleaned;
        }

        public static bool EqualsPath(string first, string second)
        {
            return string.Equals(Normalize(first), Normalize(second), StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsChildPath(string candidate, string parent)
        {
            string c = Normalize(candidate) + Path.DirectorySeparatorChar;
            string p = Normalize(parent) + Path.DirectorySeparatorChar;
            return c.StartsWith(p, StringComparison.OrdinalIgnoreCase);
        }

        public static string QuoteArgument(string value)
        {
            if (value == null) return "\"\"";
            StringBuilder result = new StringBuilder();
            result.Append('"');
            int backslashes = 0;
            foreach (char ch in value)
            {
                if (ch == '\\')
                {
                    backslashes++;
                }
                else if (ch == '"')
                {
                    result.Append('\\', backslashes * 2 + 1);
                    result.Append('"');
                    backslashes = 0;
                }
                else
                {
                    result.Append('\\', backslashes);
                    backslashes = 0;
                    result.Append(ch);
                }
            }
            result.Append('\\', backslashes * 2);
            result.Append('"');
            return result.ToString();
        }

        public static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            double value = Math.Max(0, bytes);
            int index = 0;
            while (value >= 1024 && index < suffixes.Length - 1)
            {
                value /= 1024;
                index++;
            }
            if (index == 0) return ((long)value).ToString("N0", CultureInfo.CurrentCulture) + " " + suffixes[index];
            return value.ToString(value >= 100 ? "N0" : value >= 10 ? "N1" : "N2", CultureInfo.CurrentCulture) + " " + suffixes[index];
        }

        public static string FormatDuration(TimeSpan value)
        {
            if (value.TotalHours >= 1) return string.Format(CultureInfo.CurrentCulture, "{0} h {1} min", (int)value.TotalHours, value.Minutes);
            if (value.TotalMinutes >= 1) return string.Format(CultureInfo.CurrentCulture, "{0} min {1} s", value.Minutes, value.Seconds);
            return string.Format(CultureInfo.CurrentCulture, "{0} s", Math.Max(0, value.Seconds));
        }

        public static string MakeKeepBothPath(string originalDestination)
        {
            string directory = Path.GetDirectoryName(originalDestination);
            string extension = Path.GetExtension(originalDestination);
            string baseName = Path.GetFileNameWithoutExtension(originalDestination);
            string candidate = Path.Combine(directory, baseName + UiText.T(" - Copia") + extension);
            int number = 2;
            while (File.Exists(candidate) || Directory.Exists(candidate))
            {
                candidate = Path.Combine(directory, baseName + UiText.T(" - Copia") + " (" + number.ToString(CultureInfo.InvariantCulture) + ")" + extension);
                number++;
            }
            return candidate;
        }
    }

    internal static class RequestReader
    {
        public static List<string> Read(string requestPath)
        {
            byte[] bytes = File.ReadAllBytes(requestPath);
            if (bytes.Length < 2 || bytes[0] != 0xFF || bytes[1] != 0xFE)
                throw new InvalidDataException(UiText.T("Formato della richiesta non valido."));
            string content = Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
            return content.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }

    internal static class PlanBuilder
    {
        public static CopyPlan Build(List<string> requestLines, Action<int, string> progress, CancellationToken token)
        {
            if (requestLines == null || requestLines.Count < 2)
                throw new InvalidDataException(UiText.T("La richiesta non contiene sorgenti sufficienti."));

            CopyPlan plan = new CopyPlan();
            plan.DestinationRoot = PathHelpers.Normalize(requestLines[0]);
            Directory.CreateDirectory(plan.DestinationRoot);
            TestDestinationWritable(plan.DestinationRoot);

            Dictionary<string, FileItem> destinationMap = new Dictionary<string, FileItem>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> topDestinations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int totalTop = requestLines.Count - 1;

            for (int index = 0; index < totalTop; index++)
            {
                token.ThrowIfCancellationRequested();
                string source = PathHelpers.Normalize(requestLines[index + 1]);
                progress((int)((index * 100L) / Math.Max(1, totalTop)), source);

                if (!File.Exists(source) && !Directory.Exists(source))
                {
                    plan.Errors.Add(UiText.T("Sorgente non trovata: ") + source);
                    continue;
                }

                string destination = Path.Combine(plan.DestinationRoot, PathHelpers.LeafName(source));
                if (PathHelpers.EqualsPath(source, destination))
                {
                    plan.Errors.Add(UiText.T("Sorgente e destinazione coincidono: ") + source);
                    continue;
                }
                if (Directory.Exists(source) && PathHelpers.IsChildPath(destination, source))
                {
                    plan.Errors.Add(UiText.T("Non e possibile copiare una cartella dentro se stessa: ") + source);
                    continue;
                }

                bool sourceIsDirectory = Directory.Exists(source);
                if (!topDestinations.Add(destination))
                {
                    plan.Errors.Add(UiText.T("Piu sorgenti produrrebbero lo stesso elemento di destinazione: ") + destination);
                    continue;
                }
                if (sourceIsDirectory && File.Exists(destination))
                {
                    plan.Errors.Add(UiText.T("Una cartella sorgente collide con un file nella destinazione: ") + destination);
                    continue;
                }
                if (!sourceIsDirectory && Directory.Exists(destination))
                {
                    plan.Errors.Add(UiText.T("Un file sorgente collide con una cartella nella destinazione: ") + destination);
                    continue;
                }

                TopLevelItem top = new TopLevelItem();
                top.SourcePath = source;
                top.DestinationPath = destination;
                top.IsDirectory = sourceIsDirectory;
                top.Index = plan.TopLevels.Count;
                plan.TopLevels.Add(top);

                if (top.IsDirectory)
                    ScanDirectory(plan, top, destinationMap, progress, token);
                else
                    AddFile(plan, top, source, destination, destinationMap);
            }

            progress(100, UiText.T("Analisi completata"));
            return plan;
        }

        private static void TestDestinationWritable(string destination)
        {
            string test = Path.Combine(destination, ".robocopydrop-write-test-" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                using (FileStream stream = new FileStream(test, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.WriteByte(0);
                }
            }
            finally
            {
                try { if (File.Exists(test)) File.Delete(test); } catch { }
            }
        }

        private static void ScanDirectory(CopyPlan plan, TopLevelItem top, Dictionary<string, FileItem> destinationMap,
            Action<int, string> progress, CancellationToken token)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(top.SourcePath);
            int scanned = 0;

            while (pending.Count > 0)
            {
                token.ThrowIfCancellationRequested();
                string current = pending.Pop();
                string relative = current.Length == top.SourcePath.Length ? string.Empty : current.Substring(top.SourcePath.Length).TrimStart('\\', '/');
                string destinationDirectory = string.IsNullOrEmpty(relative) ? top.DestinationPath : Path.Combine(top.DestinationPath, relative);

                if (File.Exists(destinationDirectory))
                {
                    plan.Errors.Add(UiText.T("Una cartella sorgente collide con un file nella destinazione: ") + destinationDirectory);
                    continue;
                }

                DirectoryInfo sourceDirectory = new DirectoryInfo(current);
                DirectoryItem directoryItem = new DirectoryItem();
                directoryItem.SourcePath = current;
                directoryItem.DestinationPath = destinationDirectory;
                directoryItem.SourceWriteUtc = sourceDirectory.LastWriteTimeUtc;
                directoryItem.TopLevelIndex = top.Index;
                plan.Directories.Add(directoryItem);

                FileSystemInfo[] entries;
                try
                {
                    entries = sourceDirectory.GetFileSystemInfos();
                }
                catch (Exception ex)
                {
                    plan.Warnings.Add(UiText.T("Cartella non leggibile: ") + current + " (" + ex.Message + ")");
                    continue;
                }

                foreach (FileSystemInfo entry in entries)
                {
                    token.ThrowIfCancellationRequested();
                    DirectoryInfo childDirectory = entry as DirectoryInfo;
                    if (childDirectory != null)
                    {
                        if ((childDirectory.Attributes & FileAttributes.ReparsePoint) != 0)
                        {
                            plan.Warnings.Add(UiText.T("Collegamento o junction ignorato: ") + childDirectory.FullName);
                            continue;
                        }
                        pending.Push(childDirectory.FullName);
                    }
                    else
                    {
                        string fileRelative = entry.FullName.Substring(top.SourcePath.Length).TrimStart('\\', '/');
                        string destinationFile = Path.Combine(top.DestinationPath, fileRelative);
                        AddFile(plan, top, entry.FullName, destinationFile, destinationMap);
                        scanned++;
                        if ((scanned % 250) == 0) progress(-1, entry.FullName);
                    }
                }
            }
        }

        private static void AddFile(CopyPlan plan, TopLevelItem top, string source, string destination,
            Dictionary<string, FileItem> destinationMap)
        {
            FileInfo sourceInfo;
            try
            {
                sourceInfo = new FileInfo(source);
                if (!sourceInfo.Exists)
                {
                    plan.Errors.Add(UiText.T("File non trovato: ") + source);
                    return;
                }
            }
            catch (Exception ex)
            {
                plan.Errors.Add(UiText.T("Impossibile leggere il file: ") + source + " (" + ex.Message + ")");
                return;
            }

            if (Directory.Exists(destination))
            {
                plan.Errors.Add(UiText.T("Un file sorgente collide con una cartella nella destinazione: ") + destination);
                return;
            }

            if (destinationMap.ContainsKey(destination))
            {
                plan.Errors.Add(UiText.T("Due sorgenti produrrebbero lo stesso file di destinazione: ") + destination);
                return;
            }

            FileItem item = new FileItem();
            item.SourcePath = source;
            item.DestinationPath = destination;
            item.EffectiveDestinationPath = destination;
            item.Size = sourceInfo.Length;
            item.SourceWriteUtc = sourceInfo.LastWriteTimeUtc;
            item.TopLevelIndex = top.Index;
            item.Action = ConflictAction.None;

            FileInfo destinationInfo = new FileInfo(destination);
            if (!destinationInfo.Exists)
            {
                item.Disposition = FileDisposition.NewFile;
            }
            else
            {
                item.DestinationSize = destinationInfo.Length;
                item.DestinationWriteUtc = destinationInfo.LastWriteTimeUtc;
                if (item.Size == destinationInfo.Length && item.SourceWriteUtc == destinationInfo.LastWriteTimeUtc)
                {
                    item.Disposition = FileDisposition.Identical;
                    item.Action = ConflictAction.Skip;
                }
                else
                {
                    item.Disposition = FileDisposition.Conflict;
                }
            }

            destinationMap.Add(destination, item);
            plan.Files.Add(item);
        }

    }

    internal sealed class ConflictForm : Form
    {
        private readonly FileItem item;
        private readonly int index;
        private readonly int total;
        private Label hashResult;
        private CheckBox applyAll;
        public ConflictAction SelectedAction { get; private set; }
        public bool ApplyToAll { get { return applyAll.Checked; } }

        public ConflictForm(FileItem item, int index, int total)
        {
            this.item = item;
            this.index = index;
            this.total = total;
            SelectedAction = ConflictAction.None;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = UiText.T("Conflitto durante la copia");
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = SystemIcons.Warning;
            ClientSize = new Size(720, 450);
            Font = SystemFonts.MessageBoxFont;

            Label heading = new Label();
            heading.Text = UiText.T("Nella destinazione esiste gia un file con lo stesso nome");
            heading.Font = new Font(Font, FontStyle.Bold);
            heading.AutoSize = true;
            heading.Location = new Point(24, 22);
            Controls.Add(heading);

            Label counter = new Label();
            counter.Text = UiText.T("Conflitto ") + index.ToString(CultureInfo.CurrentCulture) + UiText.T(" di ") + total.ToString(CultureInfo.CurrentCulture);
            counter.AutoSize = true;
            counter.ForeColor = SystemColors.GrayText;
            counter.Location = new Point(24, 52);
            Controls.Add(counter);

            GroupBox sourceBox = CreateFileBox(UiText.T("File da copiare"), item.SourcePath, item.Size, item.SourceWriteUtc, 24, 82);
            GroupBox destinationBox = CreateFileBox(UiText.T("File nella destinazione"), item.DestinationPath, item.DestinationSize, item.DestinationWriteUtc, 366, 82);
            Controls.Add(sourceBox);
            Controls.Add(destinationBox);

            hashResult = new Label();
            hashResult.Text = "";
            hashResult.AutoEllipsis = true;
            hashResult.Location = new Point(24, 262);
            hashResult.Size = new Size(662, 36);
            Controls.Add(hashResult);

            Button compare = new Button();
            compare.Text = UiText.T("Confronta SHA-256");
            compare.Location = new Point(24, 304);
            compare.Size = new Size(150, 34);
            compare.Click += delegate { CompareHashes(compare); };
            Controls.Add(compare);

            applyAll = new CheckBox();
            applyAll.Text = UiText.T("Applica questa scelta a tutti i conflitti rimanenti");
            applyAll.AutoSize = true;
            applyAll.Location = new Point(194, 312);
            Controls.Add(applyAll);

            Button replace = CreateChoiceButton(UiText.T("Sostituisci"), 24, ConflictAction.Replace);
            Button skip = CreateChoiceButton(UiText.T("Ignora"), 194, ConflictAction.Skip);
            Button keep = CreateChoiceButton(UiText.T("Conserva entrambi"), 364, ConflictAction.KeepBoth);
            Button cancel = new Button();
            cancel.Text = UiText.T("Annulla copia");
            cancel.Location = new Point(534, 370);
            cancel.Size = new Size(152, 38);
            cancel.DialogResult = DialogResult.Cancel;
            cancel.Click += delegate { SelectedAction = ConflictAction.None; };

            Controls.Add(replace);
            Controls.Add(skip);
            Controls.Add(keep);
            Controls.Add(cancel);
            CancelButton = cancel;
            ActiveControl = skip;
            UiText.Apply(this);
            ThemeHelper.Apply(this);
        }

        private GroupBox CreateFileBox(string title, string path, long size, DateTime writeUtc, int x, int y)
        {
            GroupBox box = new GroupBox();
            box.Text = title;
            box.Location = new Point(x, y);
            box.Size = new Size(320, 160);

            TextBox pathBox = new TextBox();
            pathBox.Text = path;
            pathBox.ReadOnly = true;
            pathBox.BorderStyle = BorderStyle.None;
            pathBox.BackColor = SystemColors.Control;
            pathBox.Location = new Point(14, 28);
            pathBox.Size = new Size(290, 42);
            pathBox.Multiline = true;
            box.Controls.Add(pathBox);

            Label sizeLabel = new Label();
            sizeLabel.Text = UiText.T("Dimensione: ") + PathHelpers.FormatBytes(size);
            sizeLabel.AutoSize = true;
            sizeLabel.Location = new Point(14, 86);
            box.Controls.Add(sizeLabel);

            Label dateLabel = new Label();
            dateLabel.Text = UiText.T("Modificato: ") + writeUtc.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
            dateLabel.AutoSize = true;
            dateLabel.Location = new Point(14, 116);
            box.Controls.Add(dateLabel);
            return box;
        }

        private Button CreateChoiceButton(string text, int x, ConflictAction action)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, 370);
            button.Size = new Size(152, 38);
            button.Click += delegate
            {
                SelectedAction = action;
                DialogResult = DialogResult.OK;
                Close();
            };
            return button;
        }

        private void CompareHashes(Button compareButton)
        {
            try
            {
                UseWaitCursor = true;
                compareButton.Enabled = false;
                hashResult.Text = UiText.T("Calcolo degli hash in corso...");
                hashResult.Refresh();
                string first = HashFile(item.SourcePath);
                string second = HashFile(item.DestinationPath);
                hashResult.Text = string.Equals(first, second, StringComparison.OrdinalIgnoreCase)
                    ? UiText.T("Gli hash SHA-256 coincidono: il contenuto e identico.")
                    : UiText.T("Gli hash SHA-256 sono diversi.");
            }
            catch (Exception ex)
            {
                hashResult.Text = UiText.T("Confronto non riuscito: ") + ex.Message;
            }
            finally
            {
                compareButton.Enabled = true;
                UseWaitCursor = false;
            }
        }

        private static string HashFile(string path)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty);
            }
        }
    }

    internal sealed class CopyEngine
    {
        private readonly CopyPlan plan;
        private readonly Action<ProgressSnapshot> progress;
        private readonly Action<string> detail;
        private readonly CancellationToken token;
        private readonly int robocopyThreads;
        private Process currentProcess;
        private long completedBytes;
        private int completedFiles;
        private readonly Stopwatch totalWatch = Stopwatch.StartNew();
        private long lastReportedBytes;
        private long lastReportTicks;
        private double smoothedSpeed;

        public CopyEngine(CopyPlan plan, Action<ProgressSnapshot> progress, Action<string> detail, CancellationToken token, int robocopyThreads)
        {
            this.plan = plan;
            this.progress = progress;
            this.detail = detail;
            this.token = token;
            this.robocopyThreads = Math.Max(1, Math.Min(64, robocopyThreads));
        }

        public void CancelCurrentProcess()
        {
            Process process = currentProcess;
            if (process == null) return;
            try
            {
                if (!process.HasExited) process.Kill();
            }
            catch
            {
            }
        }

        public CopyResult Execute()
        {
            CopyResult result = new CopyResult();
            try
            {
                detail(UiText.T("Destinazione: ") + plan.DestinationRoot);
                detail(UiText.T("File pianificati: ") + plan.FilesToCopy.ToString(CultureInfo.CurrentCulture));
                detail(UiText.T("Byte pianificati: ") + plan.BytesToCopy.ToString(CultureInfo.CurrentCulture));
                detail(UiText.T("Thread Robocopy: ") + robocopyThreads.ToString(CultureInfo.CurrentCulture));

                foreach (TopLevelItem top in plan.TopLevels)
                {
                    token.ThrowIfCancellationRequested();
                    if (top.IsDirectory)
                    {
                        bool precision = plan.Files.Any(f => f.TopLevelIndex == top.Index &&
                            f.Disposition == FileDisposition.Conflict &&
                            (f.Action == ConflictAction.Skip || f.Action == ConflictAction.KeepBoth));
                        if (precision) ExecutePrecisionDirectory(top, result);
                        else ExecuteFastDirectory(top, result);
                    }
                    else
                    {
                        ExecuteTopLevelFile(top, result);
                    }
                }

                CopyDirectoryMetadata();
                result.Success = result.RobocopyFailures == 0 && result.NativeCopyFailures == 0 && result.Errors.Count == 0;
            }
            catch (OperationCanceledException)
            {
                result.Cancelled = true;
                result.Success = false;
                CancelCurrentProcess();
                detail(UiText.T("Operazione annullata dall'utente."));
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(ex.Message);
                detail(UiText.T("ERRORE: ") + ex.ToString());
                CancelCurrentProcess();
            }
            finally
            {
                result.CompletedBytes = completedBytes;
                result.CompletedFiles = completedFiles;
            }
            return result;
        }

        private void ExecuteFastDirectory(TopLevelItem top, CopyResult result)
        {
            List<FileItem> planned = plan.Files.Where(f => f.TopLevelIndex == top.Index && CopyPlan.ShouldCopy(f)).ToList();
            long expected = planned.Sum(f => f.Size);
            int files = planned.Count;
            Directory.CreateDirectory(top.DestinationPath);
            string args = PathHelpers.QuoteArgument(top.SourcePath) + " " + PathHelpers.QuoteArgument(top.DestinationPath) +
                " /E /XJ /COPY:DAT /DCOPY:DAT /MT:" + robocopyThreads.ToString(CultureInfo.InvariantCulture) + " /R:1 /W:1 /BYTES /FP /NP /NFL /NDL";
            int exit = RunRobocopy(args, expected, files, top.SourcePath);
            if (exit >= 8)
            {
                result.RobocopyFailures++;
                result.Errors.Add(UiText.T("Robocopy ha restituito il codice ") + exit.ToString(CultureInfo.InvariantCulture) + UiText.T(" per ") + top.SourcePath);
            }
        }

        private void ExecutePrecisionDirectory(TopLevelItem top, CopyResult result)
        {
            detail(UiText.T("Modalita precisa per conflitti: ") + top.SourcePath);
            foreach (DirectoryItem directory in plan.Directories.Where(d => d.TopLevelIndex == top.Index).OrderBy(d => d.DestinationPath.Length))
            {
                token.ThrowIfCancellationRequested();
                Directory.CreateDirectory(directory.DestinationPath);
            }

            List<FileItem> standard = plan.Files.Where(f => f.TopLevelIndex == top.Index &&
                (f.Disposition == FileDisposition.NewFile ||
                 (f.Disposition == FileDisposition.Conflict && f.Action == ConflictAction.Replace))).ToList();

            IEnumerable<IGrouping<string, FileItem>> groups = standard.GroupBy(
                f => Path.GetDirectoryName(f.SourcePath) + "\n" + Path.GetDirectoryName(f.DestinationPath),
                StringComparer.OrdinalIgnoreCase);

            foreach (IGrouping<string, FileItem> group in groups)
            {
                token.ThrowIfCancellationRequested();
                List<FileItem> batch = new List<FileItem>();
                int commandLength = 0;
                foreach (FileItem item in group)
                {
                    int next = item.SourcePath.Length + 8;
                    if (batch.Count > 0 && commandLength + next > 26000)
                    {
                        RunFileBatch(batch, result);
                        batch.Clear();
                        commandLength = 0;
                    }
                    batch.Add(item);
                    commandLength += next;
                }
                if (batch.Count > 0) RunFileBatch(batch, result);
            }

            foreach (FileItem item in plan.Files.Where(f => f.TopLevelIndex == top.Index &&
                f.Disposition == FileDisposition.Conflict && f.Action == ConflictAction.KeepBoth))
            {
                token.ThrowIfCancellationRequested();
                CopyKeepBoth(item, result);
            }
        }

        private void ExecuteTopLevelFile(TopLevelItem top, CopyResult result)
        {
            FileItem item = plan.Files.FirstOrDefault(f => f.TopLevelIndex == top.Index);
            if (item == null || !CopyPlan.ShouldCopy(item)) return;
            if (item.Action == ConflictAction.KeepBoth)
            {
                CopyKeepBoth(item, result);
                return;
            }
            RunFileBatch(new List<FileItem> { item }, result);
        }

        private void RunFileBatch(List<FileItem> files, CopyResult result)
        {
            if (files.Count == 0) return;
            string sourceParent = Path.GetDirectoryName(files[0].SourcePath);
            string destinationParent = Path.GetDirectoryName(files[0].DestinationPath);
            Directory.CreateDirectory(destinationParent);
            StringBuilder args = new StringBuilder();
            args.Append(PathHelpers.QuoteArgument(sourceParent));
            args.Append(' ');
            args.Append(PathHelpers.QuoteArgument(destinationParent));
            foreach (FileItem item in files)
            {
                args.Append(' ');
                args.Append(PathHelpers.QuoteArgument(Path.GetFileName(item.SourcePath)));
            }
            args.Append(" /COPY:DAT /MT:");
            args.Append(robocopyThreads.ToString(CultureInfo.InvariantCulture));
            args.Append(" /R:1 /W:1 /BYTES /FP /NP /NFL /NDL");
            long expected = files.Sum(f => f.Size);
            int exit = RunRobocopy(args.ToString(), expected, files.Count, files[0].SourcePath);
            if (exit >= 8)
            {
                result.RobocopyFailures++;
                result.Errors.Add(UiText.T("Robocopy ha restituito il codice ") + exit.ToString(CultureInfo.InvariantCulture) + UiText.T(" per un gruppo di file."));
            }
        }

        private static Encoding GetRobocopyConsoleEncoding()
        {
            // Robocopy writes redirected console output using the Windows OEM
            // code page. Reading that byte stream as UTF-16 produces the CJK-like
            // mojibake previously visible in the expandable details panel.
            try
            {
                int codePage = CultureInfo.CurrentCulture.TextInfo.OEMCodePage;
                return Encoding.GetEncoding(codePage);
            }
            catch
            {
                return Encoding.Default;
            }
        }

        private int RunRobocopy(string arguments, long expectedBytes, int expectedFiles, string displayItem)
        {
            token.ThrowIfCancellationRequested();
            detail("");
            detail("robocopy.exe " + arguments);

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "robocopy.exe";
            start.Arguments = arguments;
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            Encoding robocopyEncoding = GetRobocopyConsoleEncoding();
            start.StandardOutputEncoding = robocopyEncoding;
            start.StandardErrorEncoding = robocopyEncoding;

            Process process = new Process();
            process.StartInfo = start;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args)
            {
                if (args.Data != null)
                {
                    // Keep Robocopy's compact header, summary and errors in the
                    // expandable panel. Progress is sampled from process I/O
                    // counters every 250 ms instead of updating the GUI once per
                    // file, which is prohibitively expensive on large repositories.
                    detail(args.Data);
                }
            };
            process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args)
            {
                if (args.Data != null) detail(UiText.T("STDERR: ") + args.Data);
            };

            if (!process.Start()) throw new InvalidOperationException(UiText.T("Impossibile avviare robocopy.exe."));
            currentProcess = process;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            ulong initialWrites = 0;
            NativeMethods.IO_COUNTERS initialCounters;
            if (NativeMethods.GetProcessIoCounters(process.Handle, out initialCounters)) initialWrites = initialCounters.WriteTransferCount;
            Stopwatch operationWatch = Stopwatch.StartNew();

            while (!process.WaitForExit(250))
            {
                if (token.IsCancellationRequested)
                {
                    try { process.Kill(); } catch { }
                    token.ThrowIfCancellationRequested();
                }
                NativeMethods.IO_COUNTERS counters;
                long operationBytes = 0;
                if (NativeMethods.GetProcessIoCounters(process.Handle, out counters))
                {
                    ulong writes = counters.WriteTransferCount >= initialWrites ? counters.WriteTransferCount - initialWrites : 0;
                    operationBytes = (long)Math.Min((ulong)Math.Max(0, expectedBytes), writes);
                }
                UpdateSpeedAndReport(UiText.T("Copia in corso"), displayItem, completedBytes + operationBytes, plan.BytesToCopy, completedFiles, plan.FilesToCopy);
            }

            process.WaitForExit();
            int exitCode = process.ExitCode;
            currentProcess = null;
            detail(UiText.T("Codice di uscita Robocopy: ") + exitCode.ToString(CultureInfo.InvariantCulture));

            if (exitCode < 8)
            {
                completedBytes += expectedBytes;
                completedFiles += expectedFiles;
            }
            UpdateSpeedAndReport(UiText.T("Copia in corso"), displayItem, completedBytes, plan.BytesToCopy, completedFiles, plan.FilesToCopy);
            process.Dispose();
            return exitCode;
        }

        private void CopyKeepBoth(FileItem item, CopyResult result)
        {
            string destination = item.EffectiveDestinationPath;
            if (string.IsNullOrEmpty(destination) || PathHelpers.EqualsPath(destination, item.DestinationPath))
            {
                destination = PathHelpers.MakeKeepBothPath(item.DestinationPath);
                item.EffectiveDestinationPath = destination;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            if (File.Exists(destination))
            {
                FileInfo existing = new FileInfo(destination);
                if (existing.Length == item.Size && existing.LastWriteTimeUtc == item.SourceWriteUtc)
                {
                    detail(UiText.T("Gia completato nel tentativo precedente: ") + destination);
                    completedBytes += item.Size;
                    completedFiles++;
                    return;
                }
            }
            detail(UiText.T("Conserva entrambi: ") + item.SourcePath + " -> " + destination);

            long baseBytes = completedBytes;
            int cancel = 0;
            NativeMethods.CopyProgressRoutine callback = delegate(long totalFileSize, long transferred, long streamSize,
                long streamTransferred, uint streamNumber, uint reason, IntPtr source, IntPtr target, IntPtr data)
            {
                if (token.IsCancellationRequested)
                {
                    cancel = 1;
                    return NativeMethods.PROGRESS_CANCEL;
                }
                UpdateSpeedAndReport(UiText.T("Copia in corso"), item.SourcePath, baseBytes + transferred,
                    plan.BytesToCopy, completedFiles, plan.FilesToCopy);
                return NativeMethods.PROGRESS_CONTINUE;
            };

            bool ok = NativeMethods.CopyFileEx(item.SourcePath, destination, callback, IntPtr.Zero, ref cancel, NativeMethods.COPY_FILE_RESTARTABLE);
            GC.KeepAlive(callback);
            if (!ok)
            {
                int error = Marshal.GetLastWin32Error();
                if (token.IsCancellationRequested || error == 1235)
                    throw new OperationCanceledException();
                result.NativeCopyFailures++;
                result.Errors.Add(UiText.T("Copia non riuscita: ") + item.SourcePath + UiText.T(" (errore ") + error.ToString(CultureInfo.InvariantCulture) + ")");
                detail("ERRORE CopyFileEx " + error.ToString(CultureInfo.InvariantCulture));
                return;
            }
            try
            {
                File.SetLastWriteTimeUtc(destination, item.SourceWriteUtc);
                File.SetAttributes(destination, File.GetAttributes(item.SourcePath));
            }
            catch (Exception metadataError)
            {
                detail(UiText.T("AVVISO metadati: ") + metadataError.Message);
            }
            completedBytes += item.Size;
            completedFiles++;
            UpdateSpeedAndReport(UiText.T("Copia in corso"), item.SourcePath, completedBytes, plan.BytesToCopy, completedFiles, plan.FilesToCopy);
        }

        private void CopyDirectoryMetadata()
        {
            foreach (DirectoryItem directory in plan.Directories.OrderByDescending(d => d.DestinationPath.Length))
            {
                try
                {
                    if (Directory.Exists(directory.DestinationPath))
                        Directory.SetLastWriteTimeUtc(directory.DestinationPath, directory.SourceWriteUtc);
                }
                catch
                {
                }
            }
        }

        private void UpdateSpeedAndReport(string phase, string currentItem, long currentBytes, long totalBytes, int currentFiles, int totalFiles)
        {
            long nowTicks = totalWatch.ElapsedTicks;
            if (lastReportTicks == 0)
            {
                lastReportTicks = nowTicks;
                lastReportedBytes = currentBytes;
            }
            double elapsed = (nowTicks - lastReportTicks) / (double)Stopwatch.Frequency;
            if (elapsed >= 0.45)
            {
                long delta = Math.Max(0, currentBytes - lastReportedBytes);
                double instant = delta / elapsed;
                smoothedSpeed = smoothedSpeed <= 0 ? instant : smoothedSpeed * 0.72 + instant * 0.28;
                lastReportTicks = nowTicks;
                lastReportedBytes = currentBytes;
            }
            ReportProgress(phase, currentItem, currentBytes, totalBytes, currentFiles, totalFiles, smoothedSpeed);
        }

        private void ReportProgress(string phase, string currentItem, long currentBytes, long totalBytes, int currentFiles, int totalFiles, double speed)
        {
            ProgressSnapshot snapshot = new ProgressSnapshot();
            snapshot.Phase = phase;
            snapshot.CurrentItem = currentItem;
            snapshot.CompletedBytes = Math.Min(Math.Max(0, currentBytes), Math.Max(0, totalBytes));
            snapshot.TotalBytes = totalBytes;
            snapshot.CompletedFiles = currentFiles;
            snapshot.TotalFiles = totalFiles;
            snapshot.BytesPerSecond = speed;
            long remaining = Math.Max(0, totalBytes - snapshot.CompletedBytes);
            if (speed > 1) snapshot.Eta = TimeSpan.FromSeconds(remaining / speed);
            progress(snapshot);
        }
    }

    internal sealed class MainForm : Form
    {
        private readonly string requestPath;
        private CancellationTokenSource cancellation;
        private CopyPlan plan;
        private CopyEngine engine;
        private CopyResult result;
        private DateTime startedAt;
        private DateTime finishedAt;
        private readonly object detailLock = new object();
        private readonly StringBuilder allDetails = new StringBuilder();
        private readonly Queue<string> pendingDetails = new Queue<string>();
        private bool detailsTruncated;
        private bool expanded;
        private bool running;
        private bool closeAfterCancel;
        private string automaticReportPath;
        private ITaskbarList3 taskbar;
        private NotifyIcon notifyIcon;
        private int selectedThreadMode;
        private int activeThreadMode;
        private int activeRobocopyThreads;
        private string activeProfileDescription = string.Empty;
        private UpdateCheckResult pendingUpdate;

        private Label titleLabel;
        private Label routeLabel;
        private ProgressBar progressBar;
        private Label percentLabel;
        private Label speedLabel;
        private Label etaLabel;
        private Label currentLabel;
        private Label remainingLabel;
        private Button detailsButton;
        private Button cancelCloseButton;
        private Button openDestinationButton;
        private Button copySummaryButton;
        private Button saveReportButton;
        private Button verifyButton;
        private Button retryButton;
        private Button settingsButton;
        private LinkLabel updateLink;
        private TextBox detailsBox;
        private CheckBox keepReportsCheckBox;
        private System.Windows.Forms.Timer detailTimer;

        public MainForm(string requestPath)
        {
            this.requestPath = requestPath;
            selectedThreadMode = AppSettings.LoadThreadMode();
            activeThreadMode = selectedThreadMode;
            activeRobocopyThreads = AppSettings.ResolveThreadCount(activeThreadMode);
            BuildUi();
            Load += MainFormLoad;
            Shown += delegate { BeginAutomaticUpdateCheck(); };
            FormClosing += MainFormClosing;
        }

        private void BuildUi()
        {
            Text = UiText.T("Copia con Robocopy");
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(720, 360);
            ClientSize = new Size(760, 390);
            Font = SystemFonts.MessageBoxFont;
            AutoScaleMode = AutoScaleMode.Dpi;
            Icon = SystemIcons.Information;

            titleLabel = new Label();
            titleLabel.Text = UiText.T("Preparazione della copia...");
            titleLabel.Font = new Font(Font.FontFamily, 13.5f, FontStyle.Regular);
            titleLabel.AutoEllipsis = true;
            titleLabel.Location = new Point(24, 22);
            titleLabel.Size = new Size(570, 30);
            Controls.Add(titleLabel);

            settingsButton = new Button();
            settingsButton.Text = UiText.T("Impostazioni...");
            settingsButton.Location = new Point(620, 18);
            settingsButton.Size = new Size(114, 32);
            settingsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            settingsButton.Click += delegate { OpenSettings(); };
            Controls.Add(settingsButton);

            routeLabel = new Label();
            routeLabel.Text = UiText.T("Analisi delle sorgenti e della destinazione");
            routeLabel.AutoEllipsis = true;
            routeLabel.ForeColor = SystemColors.GrayText;
            routeLabel.Location = new Point(24, 58);
            routeLabel.Size = new Size(710, 22);
            Controls.Add(routeLabel);

            progressBar = new ProgressBar();
            progressBar.Location = new Point(24, 94);
            progressBar.Size = new Size(710, 24);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 28;
            Controls.Add(progressBar);

            percentLabel = NewMetricLabel("", 24, 132, 100);
            speedLabel = NewMetricLabel("", 260, 132, 190);
            etaLabel = NewMetricLabel("", 520, 132, 214);

            currentLabel = new Label();
            currentLabel.Text = "";
            currentLabel.AutoEllipsis = true;
            currentLabel.Location = new Point(24, 172);
            currentLabel.Size = new Size(710, 23);
            Controls.Add(currentLabel);

            remainingLabel = new Label();
            remainingLabel.Text = "";
            remainingLabel.AutoEllipsis = true;
            remainingLabel.ForeColor = SystemColors.GrayText;
            remainingLabel.Location = new Point(24, 202);
            remainingLabel.Size = new Size(710, 23);
            Controls.Add(remainingLabel);

            updateLink = new LinkLabel();
            updateLink.Text = string.Empty;
            updateLink.AutoEllipsis = true;
            updateLink.Location = new Point(24, 224);
            updateLink.Size = new Size(560, 22);
            updateLink.Visible = false;
            updateLink.TabStop = true;
            updateLink.LinkClicked += UpdateLinkClicked;
            Controls.Add(updateLink);

            detailsButton = new Button();
            detailsButton.Text = UiText.T("Piu dettagli");
            detailsButton.Location = new Point(24, 250);
            detailsButton.Size = new Size(120, 34);
            detailsButton.Click += delegate { ToggleDetails(); };
            Controls.Add(detailsButton);

            openDestinationButton = NewBottomButton(UiText.T("Apri destinazione"), 162, 142);
            openDestinationButton.Visible = false;
            openDestinationButton.Click += delegate { OpenDestination(); };

            copySummaryButton = NewBottomButton(UiText.T("Copia riepilogo"), 316, 130);
            copySummaryButton.Visible = false;
            copySummaryButton.Click += delegate { CopySummary(); };

            saveReportButton = NewBottomButton(UiText.T("Salva report..."), 458, 126);
            saveReportButton.Visible = false;
            saveReportButton.Click += delegate { SaveReportInteractively(); };

            verifyButton = NewBottomButton(UiText.T("Verifica SHA-256"), 596, 138);
            verifyButton.Visible = false;
            verifyButton.Click += delegate { StartVerification(); };

            retryButton = NewBottomButton(UiText.T("Riprova"), 596, 138);
            retryButton.Visible = false;
            retryButton.Click += delegate { RetryCopy(); };

            cancelCloseButton = new Button();
            cancelCloseButton.Text = UiText.T("Annulla");
            cancelCloseButton.Location = new Point(614, 310);
            cancelCloseButton.Size = new Size(120, 38);
            cancelCloseButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            cancelCloseButton.Click += CancelCloseClicked;
            Controls.Add(cancelCloseButton);

            detailsBox = new TextBox();
            detailsBox.Multiline = true;
            detailsBox.ReadOnly = true;
            detailsBox.ScrollBars = ScrollBars.Both;
            detailsBox.WordWrap = false;
            detailsBox.Font = new Font("Consolas", 9.0f);
            detailsBox.Location = new Point(24, 306);
            detailsBox.Size = new Size(710, 245);
            detailsBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            detailsBox.Visible = false;
            Controls.Add(detailsBox);

            keepReportsCheckBox = new CheckBox();
            keepReportsCheckBox.Text = UiText.T("Conserva un report per tutte le copie");
            keepReportsCheckBox.AutoSize = true;
            keepReportsCheckBox.Location = new Point(24, 570);
            keepReportsCheckBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            keepReportsCheckBox.Visible = false;
            keepReportsCheckBox.Checked = LoadKeepReportsSetting();
            keepReportsCheckBox.CheckedChanged += delegate { SaveKeepReportsSetting(keepReportsCheckBox.Checked); };
            Controls.Add(keepReportsCheckBox);

            detailTimer = new System.Windows.Forms.Timer();
            detailTimer.Interval = 250;
            detailTimer.Tick += delegate { FlushPendingDetails(); };
            detailTimer.Start();

            try
            {
                taskbar = (ITaskbarList3)new TaskbarList();
                taskbar.HrInit();
            }
            catch
            {
                taskbar = null;
            }

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Information;
            notifyIcon.Text = "Robocopy Drop";
            notifyIcon.Visible = false;
            UiText.Apply(this);
            ThemeHelper.Apply(this);
        }

        private Label NewMetricLabel(string text, int x, int y, int width)
        {
            Label label = new Label();
            label.Text = text;
            label.AutoEllipsis = true;
            label.Location = new Point(x, y);
            label.Size = new Size(width, 22);
            Controls.Add(label);
            return label;
        }

        private Button NewBottomButton(string text, int x, int width)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, 250);
            button.Size = new Size(width, 34);
            Controls.Add(button);
            return button;
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            cancellation = new CancellationTokenSource();
            running = true;
            RefreshUpdateLink();
            startedAt = DateTime.Now;
            SetTaskbarState(NativeMethods.TBPF_INDETERMINATE);
            Thread worker = new Thread(BuildPlanWorker);
            worker.IsBackground = true;
            worker.Name = "RobocopyDrop plan builder";
            worker.Start();
        }

        private void BuildPlanWorker()
        {
            try
            {
                List<string> lines = RequestReader.Read(requestPath);
                try { File.Delete(requestPath); } catch { }
                plan = PlanBuilder.Build(lines, ScanProgress, cancellation.Token);
                BeginInvoke((MethodInvoker)PlanReady);
            }
            catch (OperationCanceledException)
            {
                BeginInvoke((MethodInvoker)delegate { FinishCancelledBeforeCopy(); });
            }
            catch (Exception ex)
            {
                AppendDetail(UiText.T("ERRORE PREPARAZIONE: ") + ex.ToString());
                BeginInvoke((MethodInvoker)delegate { FinishFatal(UiText.T("Preparazione non riuscita"), ex.Message); });
            }
        }

        private void ScanProgress(int percent, string current)
        {
            if (IsDisposed) return;
            BeginInvoke((MethodInvoker)delegate
            {
                currentLabel.Text = current;
                if (percent >= 0)
                {
                    progressBar.Style = ProgressBarStyle.Continuous;
                    progressBar.MarqueeAnimationSpeed = 0;
                    progressBar.Value = Math.Max(0, Math.Min(100, percent));
                    percentLabel.Text = percent.ToString(CultureInfo.CurrentCulture) + "%";
                }
            });
        }

        private void PlanReady()
        {
            if (plan == null) return;
            foreach (string warning in plan.Warnings) AppendDetail(UiText.T("AVVISO: ") + warning);
            foreach (string error in plan.Errors) AppendDetail(UiText.T("ERRORE: ") + error);

            if (plan.Errors.Count > 0)
            {
                FinishFatal(UiText.T("Copia non avviata"), string.Join(Environment.NewLine, plan.Errors.Take(5).ToArray()));
                return;
            }

            routeLabel.Text = BuildRouteText();
            titleLabel.Text = UiText.T("Analisi dei conflitti...");
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 28;
            percentLabel.Text = "";

            List<FileItem> conflicts = plan.Conflicts.ToList();
            ConflictAction applyAllAction = ConflictAction.None;
            for (int i = 0; i < conflicts.Count; i++)
            {
                if (applyAllAction != ConflictAction.None)
                {
                    conflicts[i].Action = applyAllAction;
                    if (applyAllAction == ConflictAction.KeepBoth)
                        conflicts[i].EffectiveDestinationPath = PathHelpers.MakeKeepBothPath(conflicts[i].DestinationPath);
                    continue;
                }

                using (ConflictForm dialog = new ConflictForm(conflicts[i], i + 1, conflicts.Count))
                {
                    DialogResult choice = dialog.ShowDialog(this);
                    if (choice != DialogResult.OK || dialog.SelectedAction == ConflictAction.None)
                    {
                        FinishCancelledBeforeCopy();
                        return;
                    }
                    conflicts[i].Action = dialog.SelectedAction;
                    if (dialog.SelectedAction == ConflictAction.KeepBoth)
                        conflicts[i].EffectiveDestinationPath = PathHelpers.MakeKeepBothPath(conflicts[i].DestinationPath);
                    if (dialog.ApplyToAll) applyAllAction = dialog.SelectedAction;
                }
            }

            string spaceError = ValidateResolvedFreeSpace();
            if (!string.IsNullOrEmpty(spaceError))
            {
                AppendDetail(UiText.T("ERRORE: ") + spaceError);
                FinishFatal(UiText.T("Spazio libero insufficiente"), spaceError);
                return;
            }

            StartCopyWorker();
        }

        private string ValidateResolvedFreeSpace()
        {
            try
            {
                string root = Path.GetPathRoot(plan.DestinationRoot);
                if (string.IsNullOrEmpty(root)) return null;
                DriveInfo drive = new DriveInfo(root);
                if (!drive.IsReady) return null;
                long needed = 0;
                foreach (FileItem item in plan.Files)
                {
                    if (item.Disposition == FileDisposition.NewFile) needed += item.Size;
                    else if (item.Disposition == FileDisposition.Conflict && item.Action == ConflictAction.KeepBoth) needed += item.Size;
                    else if (item.Disposition == FileDisposition.Conflict && item.Action == ConflictAction.Replace)
                    {
                        long delta = item.Size - item.DestinationSize;
                        if (delta > 0) needed += delta;
                    }
                }
                if (needed > drive.AvailableFreeSpace)
                    return UiText.T("Servono circa ") + PathHelpers.FormatBytes(needed) + UiText.T(", ma sono disponibili ") + PathHelpers.FormatBytes(drive.AvailableFreeSpace) + ".";
            }
            catch
            {
            }
            return null;
        }

        private string BuildRouteText()
        {
            int sources = plan.TopLevels.Count;
            return UiText.T("Copia di ") + plan.Files.Count.ToString("N0", CultureInfo.CurrentCulture) + UiText.T(" file da ") +
                sources.ToString(CultureInfo.CurrentCulture) + (sources == 1 ? UiText.T(" sorgente") : UiText.T(" sorgenti")) +
                UiText.T(" a ") + plan.DestinationRoot;
        }

        private void StartCopyWorker()
        {
            titleLabel.Text = UiText.T("Copia in corso");
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.MarqueeAnimationSpeed = 0;
            progressBar.Value = 0;
            percentLabel.Text = "0%";
            speedLabel.Text = UiText.T("Velocita: calcolo...");
            etaLabel.Text = UiText.T("Tempo rimanente: calcolo...");
            currentLabel.Text = UiText.T("Avvio di Robocopy...");
            remainingLabel.Text = plan.FilesToCopy.ToString("N0", CultureInfo.CurrentCulture) + UiText.T(" file da copiare (") +
                PathHelpers.FormatBytes(plan.BytesToCopy) + ")";
            SetTaskbarState(NativeMethods.TBPF_NORMAL);
            openDestinationButton.Visible = false;
            copySummaryButton.Visible = false;
            saveReportButton.Visible = false;
            verifyButton.Visible = false;
            retryButton.Visible = false;
            cancelCloseButton.Enabled = true;

            selectedThreadMode = AppSettings.LoadThreadMode();
            activeThreadMode = selectedThreadMode;
            activeRobocopyThreads = AppSettings.ResolveThreadCount(activeThreadMode, plan, out activeProfileDescription);
            AppendDetail(UiText.T("Modalita thread selezionata: ") + AppSettings.DescribeThreadMode(activeThreadMode));
            AppendDetail(UiText.T("Profilo destinazione: ") + activeProfileDescription);
            engine = new CopyEngine(plan, UpdateProgress, AppendDetail, cancellation.Token, activeRobocopyThreads);
            Thread worker = new Thread(CopyWorker);
            worker.IsBackground = true;
            worker.Name = "RobocopyDrop copy engine";
            worker.Start();
        }

        private void CopyWorker()
        {
            result = engine.Execute();
            finishedAt = DateTime.Now;
            BeginInvoke((MethodInvoker)FinishCopy);
        }

        private void UpdateProgress(ProgressSnapshot snapshot)
        {
            if (IsDisposed) return;
            BeginInvoke((MethodInvoker)delegate
            {
                long total = Math.Max(1, snapshot.TotalBytes);
                int percent = snapshot.TotalBytes <= 0 ? 100 : (int)Math.Min(100, snapshot.CompletedBytes * 100L / total);
                progressBar.Value = Math.Max(0, Math.Min(100, percent));
                percentLabel.Text = percent.ToString(CultureInfo.CurrentCulture) + "%";
                speedLabel.Text = snapshot.BytesPerSecond > 1 ? UiText.T("Velocita: ") + PathHelpers.FormatBytes((long)snapshot.BytesPerSecond) + "/s" : UiText.T("Velocita: calcolo...");
                etaLabel.Text = snapshot.Eta.HasValue ? UiText.T("Tempo rimanente: ") + PathHelpers.FormatDuration(snapshot.Eta.Value) : UiText.T("Tempo rimanente: calcolo...");
                currentLabel.Text = snapshot.CurrentItem ?? snapshot.Phase;
                int remainingFiles = Math.Max(0, snapshot.TotalFiles - snapshot.CompletedFiles);
                long remainingBytes = Math.Max(0, snapshot.TotalBytes - snapshot.CompletedBytes);
                remainingLabel.Text = UiText.T("Rimanenti: ") + remainingFiles.ToString("N0", CultureInfo.CurrentCulture) + UiText.T(" file (") + PathHelpers.FormatBytes(remainingBytes) + ")";
                SetTaskbarProgress((ulong)Math.Max(0, snapshot.CompletedBytes), (ulong)Math.Max(1, snapshot.TotalBytes));
            });
        }

        private void FinishCopy()
        {
            running = false;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 100;
            percentLabel.Text = result.Success ? "100%" : result.Cancelled ? UiText.T("Annullata") : UiText.T("Con errori");
            speedLabel.Text = UiText.T("Durata: ") + PathHelpers.FormatDuration(finishedAt - startedAt);
            etaLabel.Text = "";
            currentLabel.Text = result.Success ? UiText.T("Operazione completata") : result.Cancelled ? UiText.T("Operazione annullata") : UiText.T("Operazione completata con problemi");
            remainingLabel.Text = BuildCompactSummary();
            titleLabel.Text = result.Success ? UiText.T("Copia completata") : result.Cancelled ? UiText.T("Copia annullata") : UiText.T("Copia completata con problemi");
            cancelCloseButton.Text = UiText.T("Chiudi");
            cancelCloseButton.Enabled = true;
            openDestinationButton.Visible = true;
            copySummaryButton.Visible = true;
            saveReportButton.Visible = true;
            verifyButton.Visible = result.Success && plan.FilesToCopy > 0;
            retryButton.Visible = !result.Success && plan != null;

            if (result.Success)
            {
                SetTaskbarState(NativeMethods.TBPF_NOPROGRESS);
                if (keepReportsCheckBox.Checked) automaticReportPath = SaveAutomaticReport("successo");
            }
            else
            {
                SetTaskbarState(result.Cancelled ? NativeMethods.TBPF_PAUSED : NativeMethods.TBPF_ERROR);
                automaticReportPath = SaveAutomaticReport(result.Cancelled ? "annullata" : "errore");
                if (!string.IsNullOrEmpty(automaticReportPath)) AppendDetail(UiText.T("Report salvato automaticamente: ") + automaticReportPath);
            }
            RefreshUpdateLink();
            NotifyIfMinimized();
            if (closeAfterCancel) Close();
        }

        private void FinishCancelledBeforeCopy()
        {
            running = false;
            finishedAt = DateTime.Now;
            titleLabel.Text = UiText.T("Copia annullata");
            currentLabel.Text = UiText.T("Nessun file e stato copiato.");
            remainingLabel.Text = "";
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            percentLabel.Text = UiText.T("Annullata");
            speedLabel.Text = "";
            etaLabel.Text = "";
            cancelCloseButton.Text = UiText.T("Chiudi");
            cancelCloseButton.Enabled = true;
            SetTaskbarState(NativeMethods.TBPF_PAUSED);
            automaticReportPath = SaveAutomaticReport("annullata-prima-della-copia");
            RefreshUpdateLink();
            if (closeAfterCancel) Close();
        }

        private void FinishFatal(string title, string message)
        {
            running = false;
            finishedAt = DateTime.Now;
            titleLabel.Text = title;
            currentLabel.Text = message;
            remainingLabel.Text = UiText.T("Consulta Piu dettagli per informazioni tecniche.");
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            percentLabel.Text = UiText.T("Errore");
            speedLabel.Text = "";
            etaLabel.Text = "";
            cancelCloseButton.Text = UiText.T("Chiudi");
            cancelCloseButton.Enabled = true;
            saveReportButton.Visible = true;
            copySummaryButton.Visible = true;
            SetTaskbarState(NativeMethods.TBPF_ERROR);
            automaticReportPath = SaveAutomaticReport("errore-preparazione");
            RefreshUpdateLink();
        }

        private string BuildCompactSummary()
        {
            if (plan == null) return string.Empty;
            return UiText.T("Copiati: ") + (result == null ? 0 : result.CompletedFiles).ToString("N0", CultureInfo.CurrentCulture) +
                UiText.T(" | Gia aggiornati: ") + plan.IdenticalCount.ToString("N0", CultureInfo.CurrentCulture) +
                UiText.T(" | Sostituiti: ") + plan.ReplacedCount.ToString("N0", CultureInfo.CurrentCulture) +
                UiText.T(" | Ignorati: ") + plan.SkippedCount.ToString("N0", CultureInfo.CurrentCulture) +
                UiText.T(" | Conservati entrambi: ") + plan.KeptBothCount.ToString("N0", CultureInfo.CurrentCulture);
        }

        private string BuildSummary()
        {
            StringBuilder summary = new StringBuilder();
            summary.AppendLine(UiText.T("ROBOCOPY DROP - RIEPILOGO"));
            summary.AppendLine("==========================");
            summary.AppendLine(UiText.T("Esito: ") + (result == null ? titleLabel.Text : result.Success ? UiText.T("Completata") : result.Cancelled ? UiText.T("Annullata") : UiText.T("Completata con problemi")));
            summary.AppendLine(UiText.T("Avvio: ") + startedAt.ToString("G", CultureInfo.CurrentCulture));
            if (finishedAt != DateTime.MinValue) summary.AppendLine(UiText.T("Fine: ") + finishedAt.ToString("G", CultureInfo.CurrentCulture));
            if (finishedAt != DateTime.MinValue) summary.AppendLine(UiText.T("Durata: ") + PathHelpers.FormatDuration(finishedAt - startedAt));
            if (plan != null)
            {
                summary.AppendLine(UiText.T("Destinazione: ") + plan.DestinationRoot);
                summary.AppendLine(UiText.T("File analizzati: ") + plan.Files.Count.ToString("N0", CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("File da copiare: ") + plan.FilesToCopy.ToString("N0", CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("Dati pianificati: ") + PathHelpers.FormatBytes(plan.BytesToCopy));
                summary.AppendLine(UiText.T("Thread Robocopy: ") + activeRobocopyThreads.ToString(CultureInfo.CurrentCulture));
                if (!string.IsNullOrEmpty(activeProfileDescription)) summary.AppendLine(UiText.T("Profilo destinazione: ") + activeProfileDescription);
                summary.AppendLine(UiText.T("Impostazione thread usata: ") + AppSettings.DescribeThreadMode(activeThreadMode));
                summary.AppendLine(UiText.T("Gia aggiornati: ") + plan.IdenticalCount.ToString("N0", CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("Sostituiti: ") + plan.ReplacedCount.ToString("N0", CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("Ignorati dall'utente: ") + plan.SkippedCount.ToString("N0", CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("Conservati entrambi: ") + plan.KeptBothCount.ToString("N0", CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("Avvisi: ") + plan.Warnings.Count.ToString("N0", CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("Errori preparazione: ") + plan.Errors.Count.ToString("N0", CultureInfo.CurrentCulture));
            }
            if (result != null)
            {
                summary.AppendLine(UiText.T("File completati: ") + result.CompletedFiles.ToString("N0", CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("Dati completati: ") + PathHelpers.FormatBytes(result.CompletedBytes));
                summary.AppendLine(UiText.T("Operazioni Robocopy fallite: ") + result.RobocopyFailures.ToString(CultureInfo.CurrentCulture));
                summary.AppendLine(UiText.T("Copie native fallite: ") + result.NativeCopyFailures.ToString(CultureInfo.CurrentCulture));
                foreach (string error in result.Errors) summary.AppendLine(UiText.T("Errore: ") + error);
            }
            if (!string.IsNullOrEmpty(automaticReportPath)) summary.AppendLine(UiText.T("Report automatico: ") + automaticReportPath);
            return summary.ToString();
        }

        private string BuildFullReport()
        {
            StringBuilder report = new StringBuilder();
            report.Append(BuildSummary());
            report.AppendLine();
            report.AppendLine(UiText.T("DETTAGLI TECNICI"));
            report.AppendLine("================");
            lock (detailLock)
            {
                report.Append(allDetails.ToString());
                if (detailsTruncated) report.AppendLine(UiText.T("[Dettagli troncati per limite di memoria]"));
            }
            return report.ToString();
        }

        private void AppendDetail(string line)
        {
            if (line == null) return;
            lock (detailLock)
            {
                const int maxChars = 32 * 1024 * 1024;
                if (allDetails.Length + line.Length + 2 <= maxChars)
                    allDetails.AppendLine(line);
                else
                    detailsTruncated = true;
                pendingDetails.Enqueue(line);
                while (pendingDetails.Count > 5000) pendingDetails.Dequeue();
            }
        }

        private void FlushPendingDetails()
        {
            if (!detailsBox.Visible) return;
            StringBuilder batch = new StringBuilder();
            lock (detailLock)
            {
                int count = 0;
                while (pendingDetails.Count > 0 && count < 250)
                {
                    batch.AppendLine(pendingDetails.Dequeue());
                    count++;
                }
            }
            if (batch.Length == 0) return;
            if (detailsBox.TextLength > 2000000)
            {
                detailsBox.Select(0, 500000);
                detailsBox.SelectedText = string.Empty;
            }
            detailsBox.AppendText(batch.ToString());
        }

        private void BeginAutomaticUpdateCheck()
        {
            if (!UpdateManager.IsAutomaticCheckDue()) return;
            UpdateManager.BeginCheck(this, false, delegate(UpdateCheckResult checkResult)
            {
                pendingUpdate = checkResult != null && checkResult.IsUpdateAvailable ? checkResult : null;
                RefreshUpdateLink();
            });
        }

        private void RefreshUpdateLink()
        {
            if (updateLink == null) return;
            bool show = !running && pendingUpdate != null && pendingUpdate.IsUpdateAvailable;
            updateLink.Visible = show;
            updateLink.Enabled = show;
            updateLink.Text = show
                ? UiText.T("Versione disponibile: ") + UpdateManager.VersionText(pendingUpdate.LatestVersion) +
                  " - " + UiText.T("Aggiorna ora")
                : string.Empty;
        }

        private void UpdateLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (running)
            {
                MessageBox.Show(this,
                    UiText.T("Completa o annulla l'operazione corrente prima di aggiornare Robocopy Drop."),
                    UiText.T("Aggiornamento rinviato"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            UpdateManager.ConfirmAndInstall(this, pendingUpdate);
        }

        private void OpenSettings()
        {
            bool copyAlreadyRunning = running && engine != null;
            DialogResult settingsResult = Program.ShowSettingsSingleInstance(this, false);
            if (settingsResult != DialogResult.OK) return;

            selectedThreadMode = AppSettings.LoadThreadMode();
            BeginAutomaticUpdateCheck();
            string resolvedProfile = string.Empty;
            int resolved = plan == null
                ? AppSettings.ResolveThreadCount(selectedThreadMode)
                : AppSettings.ResolveThreadCount(selectedThreadMode, plan, out resolvedProfile);
            if (plan == null) resolvedProfile = string.Empty;
            if (!copyAlreadyRunning && engine == null)
            {
                activeThreadMode = selectedThreadMode;
                activeRobocopyThreads = resolved;
                activeProfileDescription = resolvedProfile;
                AppendDetail(UiText.T("Impostazione thread aggiornata: ") + AppSettings.DescribeThreadMode(selectedThreadMode));
            }
            else
            {
                MessageBox.Show(this,
                    UiText.T("L'impostazione e stata salvata. Sara usata dalla prossima copia o dal prossimo tentativo."),
                    UiText.T("Impostazioni salvate"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ToggleDetails()
        {
            expanded = !expanded;
            detailsBox.Visible = expanded;
            keepReportsCheckBox.Visible = expanded;
            detailsButton.Text = expanded ? UiText.T("Meno dettagli") : UiText.T("Piu dettagli");
            ClientSize = expanded ? new Size(760, 640) : new Size(760, 390);
            if (expanded)
            {
                detailsBox.SetBounds(24, 306, 710, 236);
                keepReportsCheckBox.Location = new Point(24, 560);
                updateLink.Location = new Point(24, 584);
                cancelCloseButton.Location = new Point(614, 574);
                lock (detailLock)
                {
                    detailsBox.Text = allDetails.ToString();
                    pendingDetails.Clear();
                }
                detailsBox.SelectionStart = detailsBox.TextLength;
                detailsBox.ScrollToCaret();
            }
            else
            {
                updateLink.Location = new Point(24, 224);
                cancelCloseButton.Location = new Point(614, 310);
            }
            RefreshUpdateLink();
        }

        private void CancelCloseClicked(object sender, EventArgs e)
        {
            if (!running)
            {
                Close();
                return;
            }
            DialogResult answer = MessageBox.Show(this,
                UiText.T("Vuoi annullare l'operazione? I file gia completati resteranno nella destinazione."),
                UiText.T("Annulla operazione"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (answer != DialogResult.Yes) return;
            cancellation.Cancel();
            if (engine != null) engine.CancelCurrentProcess();
            cancelCloseButton.Enabled = false;
            currentLabel.Text = UiText.T("Annullamento in corso...");
        }

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (running)
            {
                DialogResult answer = MessageBox.Show(this,
                    UiText.T("La copia o la verifica e ancora in corso. Vuoi annullarla e chiudere?"),
                    UiText.T("Operazione in corso"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (answer != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                e.Cancel = true;
                closeAfterCancel = true;
                cancellation.Cancel();
                if (engine != null) engine.CancelCurrentProcess();
                cancelCloseButton.Enabled = false;
                currentLabel.Text = UiText.T("Annullamento in corso...");
                return;
            }
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            SetTaskbarState(NativeMethods.TBPF_NOPROGRESS);
        }

        private void RetryCopy()
        {
            if (running || plan == null) return;
            AppendDetail("");
            AppendDetail(UiText.T("===== NUOVO TENTATIVO ====="));
            cancellation = new CancellationTokenSource();
            running = true;
            RefreshUpdateLink();
            closeAfterCancel = false;
            startedAt = DateTime.Now;
            finishedAt = DateTime.MinValue;
            result = null;
            StartCopyWorker();
        }

        private void OpenDestination()
        {
            if (plan == null || string.IsNullOrEmpty(plan.DestinationRoot)) return;
            try { Process.Start("explorer.exe", PathHelpers.QuoteArgument(plan.DestinationRoot)); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Robocopy Drop", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void CopySummary()
        {
            try
            {
                Clipboard.SetText(BuildSummary());
                MessageBox.Show(this, UiText.T("Riepilogo copiato negli appunti."), "Robocopy Drop", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Robocopy Drop", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveReportInteractively()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = UiText.T("Salva report Robocopy Drop");
            dialog.Filter = UiText.T("File di testo (*.txt)|*.txt|Tutti i file (*.*)|*.*");
            dialog.FileName = "RobocopyDrop-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture) + ".txt";
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                File.WriteAllText(dialog.FileName, BuildFullReport(), new UTF8Encoding(true));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, UiText.T("Salvataggio non riuscito"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SaveAutomaticReport(string suffix)
        {
            try
            {
                string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RobocopyDrop", "Logs");
                Directory.CreateDirectory(directory);
                string path = Path.Combine(directory, "RobocopyDrop-" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture) + "-" + suffix + ".txt");
                File.WriteAllText(path, BuildFullReport(), new UTF8Encoding(true));
                return path;
            }
            catch
            {
                return null;
            }
        }

        private void StartVerification()
        {
            if (running || plan == null) return;
            DialogResult answer = MessageBox.Show(this,
                UiText.T("La verifica SHA-256 rileggera integralmente sorgente e destinazione e puo richiedere molto tempo. Continuare?"),
                UiText.T("Verifica SHA-256"), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (answer != DialogResult.Yes) return;

            running = true;
            RefreshUpdateLink();
            cancellation = new CancellationTokenSource();
            verifyButton.Enabled = false;
            cancelCloseButton.Text = UiText.T("Annulla");
            titleLabel.Text = UiText.T("Verifica SHA-256 in corso");
            progressBar.Value = 0;
            SetTaskbarState(NativeMethods.TBPF_NORMAL);
            Thread worker = new Thread(VerifyWorker);
            worker.IsBackground = true;
            worker.Start();
        }

        private void VerifyWorker()
        {
            List<FileItem> items = plan.Files.Where(CopyPlan.ShouldCopy).ToList();
            long total = items.Sum(f => f.Size) * 2L;
            long done = 0;
            int checkedFiles = 0;
            List<string> mismatches = new List<string>();
            try
            {
                foreach (FileItem item in items)
                {
                    cancellation.Token.ThrowIfCancellationRequested();
                    string destination = string.IsNullOrEmpty(item.EffectiveDestinationPath) ? item.DestinationPath : item.EffectiveDestinationPath;
                    string sourceHash = ComputeHashWithProgress(item.SourcePath, done, total, item.SourcePath);
                    done += item.Size;
                    string destinationHash = ComputeHashWithProgress(destination, done, total, destination);
                    done += item.Size;
                    if (!string.Equals(sourceHash, destinationHash, StringComparison.OrdinalIgnoreCase))
                        mismatches.Add(item.SourcePath + " -> " + destination);
                    checkedFiles++;
                    VerificationProgress(done, total, checkedFiles, items.Count, item.SourcePath);
                }
                BeginInvoke((MethodInvoker)delegate { FinishVerification(mismatches, false); });
            }
            catch (OperationCanceledException)
            {
                BeginInvoke((MethodInvoker)delegate { FinishVerification(mismatches, true); });
            }
            catch (Exception ex)
            {
                mismatches.Add(UiText.T("ERRORE: ") + ex.Message);
                BeginInvoke((MethodInvoker)delegate { FinishVerification(mismatches, false); });
            }
        }

        private string ComputeHashWithProgress(string path, long baseBytes, long totalBytes, string current)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024, FileOptions.SequentialScan))
            {
                byte[] buffer = new byte[1024 * 1024];
                int read;
                long fileDone = 0;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cancellation.Token.ThrowIfCancellationRequested();
                    sha.TransformBlock(buffer, 0, read, buffer, 0);
                    fileDone += read;
                    VerificationProgress(baseBytes + fileDone, totalBytes, 0, plan.FilesToCopy, current);
                }
                sha.TransformFinalBlock(new byte[0], 0, 0);
                return BitConverter.ToString(sha.Hash).Replace("-", string.Empty);
            }
        }

        private void VerificationProgress(long done, long total, int filesDone, int totalFiles, string current)
        {
            if (IsDisposed) return;
            BeginInvoke((MethodInvoker)delegate
            {
                int percent = total <= 0 ? 100 : (int)Math.Min(100, done * 100L / total);
                progressBar.Value = Math.Max(0, Math.Min(100, percent));
                percentLabel.Text = percent.ToString(CultureInfo.CurrentCulture) + "%";
                currentLabel.Text = current;
                remainingLabel.Text = filesDone > 0 ? UiText.T("File verificati: ") + filesDone.ToString("N0", CultureInfo.CurrentCulture) + UiText.T(" di ") + totalFiles.ToString("N0", CultureInfo.CurrentCulture) : UiText.T("Calcolo hash...");
                SetTaskbarProgress((ulong)Math.Max(0, done), (ulong)Math.Max(1, total));
            });
        }

        private void FinishVerification(List<string> mismatches, bool cancelled)
        {
            running = false;
            cancelCloseButton.Text = UiText.T("Chiudi");
            cancelCloseButton.Enabled = true;
            verifyButton.Enabled = true;
            RefreshUpdateLink();
            if (cancelled)
            {
                titleLabel.Text = UiText.T("Verifica SHA-256 annullata");
                percentLabel.Text = UiText.T("Annullata");
                SetTaskbarState(NativeMethods.TBPF_PAUSED);
                if (closeAfterCancel) Close();
                return;
            }
            if (mismatches.Count == 0)
            {
                titleLabel.Text = UiText.T("Verifica SHA-256 completata");
                currentLabel.Text = UiText.T("Tutti i file verificati sono identici byte per byte.");
                remainingLabel.Text = UiText.T("Nessuna differenza rilevata.");
                progressBar.Value = 100;
                percentLabel.Text = "100%";
                SetTaskbarState(NativeMethods.TBPF_NOPROGRESS);
                AppendDetail(UiText.T("Verifica SHA-256: tutti i file coincidono."));
            }
            else
            {
                titleLabel.Text = UiText.T("Verifica SHA-256 completata con differenze");
                currentLabel.Text = mismatches.Count.ToString(CultureInfo.CurrentCulture) + UiText.T(" differenze o errori rilevati.");
                remainingLabel.Text = UiText.T("Consulta Piu dettagli.");
                percentLabel.Text = UiText.T("Differenze");
                SetTaskbarState(NativeMethods.TBPF_ERROR);
                foreach (string mismatch in mismatches) AppendDetail(UiText.T("HASH: ") + mismatch);
                automaticReportPath = SaveAutomaticReport("hash-differenze");
            }
            if (closeAfterCancel) Close();
        }

        private bool LoadKeepReportsSetting()
        {
            try
            {
                object value = Registry.GetValue("HKEY_CURRENT_USER\\Software\\RobocopyDrop", "KeepAllReports", 0);
                return Convert.ToInt32(value, CultureInfo.InvariantCulture) != 0;
            }
            catch { return false; }
        }

        private void SaveKeepReportsSetting(bool enabled)
        {
            try
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\RobocopyDrop", "KeepAllReports", enabled ? 1 : 0, RegistryValueKind.DWord);
            }
            catch
            {
            }
        }

        private void SetTaskbarState(uint state)
        {
            if (taskbar == null || !IsHandleCreated) return;
            try { taskbar.SetProgressState(Handle, state); } catch { }
        }

        private void SetTaskbarProgress(ulong completed, ulong total)
        {
            if (taskbar == null || !IsHandleCreated) return;
            try { taskbar.SetProgressValue(Handle, completed, total); } catch { }
        }

        private void NotifyIfMinimized()
        {
            if (WindowState != FormWindowState.Minimized) return;
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipTitle = titleLabel.Text;
            notifyIcon.BalloonTipText = currentLabel.Text;
            notifyIcon.ShowBalloonTip(5000);
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 6000;
            timer.Tick += delegate
            {
                notifyIcon.Visible = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
    }

    internal static class Program
    {
        private const string SettingsMutexName = "Local\\RobocopyDrop.Settings";
        private const string SettingsActivateEventName = "Local\\RobocopyDrop.Settings.Activate";
        private const string SettingsUpdateEventName = "Local\\RobocopyDrop.Settings.CheckUpdates";

        private static void ActivateExistingSettingsWindowFallback()
        {
            try
            {
                Process current = Process.GetCurrentProcess();
                string processName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                for (int attempt = 0; attempt < 20; attempt++)
                {
                    foreach (Process process in Process.GetProcessesByName(processName))
                    {
                        try
                        {
                            if (process.Id == current.Id) continue;
                            process.Refresh();
                            IntPtr handle = process.MainWindowHandle;
                            if (handle == IntPtr.Zero) continue;
                            NativeMethods.ShowWindow(handle, NativeMethods.SW_RESTORE);
                            NativeMethods.SetForegroundWindow(handle);
                            return;
                        }
                        catch
                        {
                        }
                        finally
                        {
                            process.Dispose();
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch
            {
            }
        }

        private static bool SignalNamedSettingsEvent(string eventName)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                try
                {
                    using (EventWaitHandle signalEvent = EventWaitHandle.OpenExisting(eventName))
                    {
                        signalEvent.Set();
                        return true;
                    }
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    Thread.Sleep(100);
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private static void SignalExistingSettingsInstance(bool forceUpdateCheck)
        {
            bool activationSignaled = SignalNamedSettingsEvent(SettingsActivateEventName);
            if (forceUpdateCheck)
                SignalNamedSettingsEvent(SettingsUpdateEventName);
            if (!activationSignaled)
                ActivateExistingSettingsWindowFallback();
        }

        private static Thread StartSettingsSignalThread(
            SettingsForm form,
            EventWaitHandle activateEvent,
            EventWaitHandle updateEvent)
        {
            Thread signalThread = new Thread(delegate()
            {
                WaitHandle[] signals = new WaitHandle[] { activateEvent, updateEvent };
                while (!form.IsDisposed)
                {
                    int signalIndex;
                    try
                    {
                        signalIndex = WaitHandle.WaitAny(signals, 500);
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }

                    if (signalIndex == WaitHandle.WaitTimeout) continue;
                    bool checkUpdates = signalIndex == 1;

                    for (int attempt = 0; attempt < 50 && !form.IsDisposed; attempt++)
                    {
                        if (!form.IsHandleCreated)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        try
                        {
                            bool requestedCheck = checkUpdates;
                            form.BeginInvoke((MethodInvoker)delegate
                            {
                                form.ActivateFromExternalRequest(requestedCheck);
                            });
                        }
                        catch
                        {
                        }
                        break;
                    }
                }
            });
            signalThread.IsBackground = true;
            signalThread.Name = "Robocopy Drop settings activation";
            signalThread.Start();
            return signalThread;
        }

        internal static DialogResult ShowSettingsSingleInstance(
            IWin32Window owner,
            bool forceUpdateCheck)
        {
            bool createdNew = false;
            Mutex settingsMutex = null;
            try
            {
                settingsMutex = new Mutex(true, SettingsMutexName, out createdNew);
                if (!createdNew)
                {
                    SignalExistingSettingsInstance(forceUpdateCheck);
                    return DialogResult.None;
                }

                using (EventWaitHandle activateEvent = new EventWaitHandle(
                    false, EventResetMode.AutoReset, SettingsActivateEventName))
                using (EventWaitHandle updateEvent = new EventWaitHandle(
                    false, EventResetMode.AutoReset, SettingsUpdateEventName))
                using (SettingsForm form = new SettingsForm(forceUpdateCheck))
                {
                    StartSettingsSignalThread(form, activateEvent, updateEvent);
                    if (owner == null)
                    {
                        Application.Run(form);
                        return form.DialogResult;
                    }
                    return form.ShowDialog(owner);
                }
            }
            finally
            {
                if (settingsMutex != null)
                {
                    if (createdNew)
                    {
                        try { settingsMutex.ReleaseMutex(); }
                        catch { }
                    }
                    settingsMutex.Dispose();
                }
            }
        }

        private static int RunSettings(bool forceUpdateCheck)
        {
            ShowSettingsSingleInstance(null, forceUpdateCheck);
            return 0;
        }

        private static int OpenReportsFolder()
        {
            try
            {
                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RobocopyDrop", "Logs");
                Directory.CreateDirectory(path);
                Process.Start(new ProcessStartInfo("explorer.exe", "\"" + path + "\"")
                {
                    UseShellExecute = true
                });
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(UiText.T("Impossibile aprire la cartella dei report: ") + ex.Message,
                    "Robocopy Drop", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 20;
            }
        }

        private static int OpenGuide()
        {
            try
            {
                string fileName = UiText.IsEnglish ? "GUIDE-EN.pdf" : "GUIDA-IT.pdf";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (!File.Exists(path))
                    throw new FileNotFoundException(UiText.T("Guida non trovata: ") + path);
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(UiText.T("Impossibile aprire la guida: ") + ex.Message,
                    "Robocopy Drop", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 21;
            }
        }

        private static int RunUninstall(string productCode)
        {
            Guid parsedProductCode;
            if (!Guid.TryParse(productCode, out parsedProductCode))
            {
                MessageBox.Show(UiText.T("Impossibile avviare la disinstallazione: ") +
                    UiText.T("Richiesta di copia non valida."),
                    "Robocopy Drop", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 22;
            }

            DialogResult answer = MessageBox.Show(
                UiText.T("Vuoi disinstallare Robocopy Drop?"),
                UiText.T("Robocopy Drop - Disinstalla"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes) return 0;

            try
            {
                Thread.Sleep(250);

                ProcessStartInfo information = new ProcessStartInfo();
                information.FileName = Path.Combine(Environment.SystemDirectory, "msiexec.exe");
                information.Arguments = "/x " + parsedProductCode.ToString("B") +
                    " /passive /norestart";
                information.UseShellExecute = true;
                Process.Start(information);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(UiText.T("Impossibile avviare la disinstallazione: ") + ex.Message,
                    "Robocopy Drop", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 23;
            }
        }

        [STAThread]
        private static int Main(string[] args)
        {
            UiText.Initialize();
            try { NativeMethods.SetProcessDPIAware(); } catch { }
            IntPtr console = NativeMethods.GetConsoleWindow();
            if (console != IntPtr.Zero) NativeMethods.ShowWindow(console, NativeMethods.SW_HIDE);

            if (args.Length == 1 && string.Equals(args[0], "--self-test", StringComparison.OrdinalIgnoreCase))
            {
                string robocopy = Path.Combine(Environment.SystemDirectory, "robocopy.exe");
                if (!File.Exists(robocopy)) return 10;
                return UpdateManager.RunSelfTest() ? 0 : 11;
            }

            if (args.Length == 1 &&
                (string.Equals(args[0], "--settings", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(args[0], "--check-updates", StringComparison.OrdinalIgnoreCase)))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                bool forceUpdateCheck = string.Equals(args[0], "--check-updates", StringComparison.OrdinalIgnoreCase);
                return RunSettings(forceUpdateCheck);
            }

            if (args.Length == 1 && string.Equals(args[0], "--open-reports", StringComparison.OrdinalIgnoreCase))
                return OpenReportsFolder();

            if (args.Length == 1 && string.Equals(args[0], "--open-guide", StringComparison.OrdinalIgnoreCase))
                return OpenGuide();

            if (args.Length == 2 &&
                string.Equals(args[0], "--uninstall", StringComparison.OrdinalIgnoreCase))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                return RunUninstall(args[1]);
            }

            if (args.Length != 1 || !File.Exists(args[0]))
            {
                MessageBox.Show(UiText.T("Richiesta di copia non valida."), "Robocopy Drop", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 2;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args[0]));
            return 0;
        }
    }
}
