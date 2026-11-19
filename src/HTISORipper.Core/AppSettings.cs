using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace HTISORipper.Core
{
    /// <summary>User preferences. Stored per user in %APPDATA%\HTREASURE LTD\HT ISO Ripper\settings.xml.</summary>
    public sealed class AppSettings
    {
        public AppSettings()
        {
            DefaultOutputDirectory = PathUtil.DefaultOutputDirectory;
            VerificationMode = VerificationMode.FullSourceCompare;
            ReadSpeed = ReadSpeedPreference.Automatic;
            MaxReadRetries = 3;
            ErrorPolicy = ErrorPolicy.AbortOnUnreadableSector;
            EjectAfterSuccess = false;
            LogRetentionDays = 30;
            ReadBlockSectors = 32;
            Theme = AppTheme.System;
        }

        public string DefaultOutputDirectory { get; set; }
        public VerificationMode VerificationMode { get; set; }
        public ReadSpeedPreference ReadSpeed { get; set; }
        public int MaxReadRetries { get; set; }
        public ErrorPolicy ErrorPolicy { get; set; }
        public bool EjectAfterSuccess { get; set; }
        public int LogRetentionDays { get; set; }
        public int ReadBlockSectors { get; set; }
        public AppTheme Theme { get; set; }

        public void Normalise()
        {
            if (string.IsNullOrWhiteSpace(DefaultOutputDirectory) || PathUtil.ValidateDirectory(DefaultOutputDirectory) != null)
                DefaultOutputDirectory = PathUtil.DefaultOutputDirectory;
            if (MaxReadRetries < 0) MaxReadRetries = 0;
            if (MaxReadRetries > 20) MaxReadRetries = 20;
            if (LogRetentionDays < 1) LogRetentionDays = 1;
            if (LogRetentionDays > 3650) LogRetentionDays = 3650;
            if (ReadBlockSectors != 16 && ReadBlockSectors != 32 && ReadBlockSectors != 64) ReadBlockSectors = 32;
            if (!Enum.IsDefined(typeof(VerificationMode), VerificationMode)) VerificationMode = VerificationMode.FullSourceCompare;
            if (!Enum.IsDefined(typeof(ReadSpeedPreference), ReadSpeed)) ReadSpeed = ReadSpeedPreference.Automatic;
            if (!Enum.IsDefined(typeof(ErrorPolicy), ErrorPolicy)) ErrorPolicy = ErrorPolicy.AbortOnUnreadableSector;
            if (!Enum.IsDefined(typeof(AppTheme), Theme)) Theme = AppTheme.System;
        }
    }

    public sealed class SettingsStore
    {
        private readonly string _path;

        public SettingsStore(string path) { _path = path; }

        public static string DefaultPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "HTREASURE LTD", "HT ISO Ripper", "settings.xml");
            }
        }

        public string FilePath { get { return _path; } }

        /// <summary>Loads settings; returns defaults if the file is missing or corrupt.</summary>
        public AppSettings Load()
        {
            AppSettings settings = null;
            try
            {
                if (File.Exists(_path))
                {
                    var serializer = new XmlSerializer(typeof(AppSettings));
                    var xmlSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
                    using (var stream = File.OpenRead(_path))
                    using (var reader = XmlReader.Create(stream, xmlSettings))
                    {
                        settings = (AppSettings)serializer.Deserialize(reader);
                    }
                }
            }
            catch (InvalidOperationException) { settings = null; }
            catch (IOException) { settings = null; }
            catch (UnauthorizedAccessException) { settings = null; }
            catch (XmlException) { settings = null; }

            if (settings == null) settings = new AppSettings();
            settings.Normalise();
            return settings;
        }

        /// <summary>Saves atomically (write temp file, then replace).</summary>
        public void Save(AppSettings settings)
        {
            settings.Normalise();
            string dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            string temp = _path + ".tmp";
            var serializer = new XmlSerializer(typeof(AppSettings));
            using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(stream, settings);
                stream.Flush(true);
            }
            if (File.Exists(_path)) File.Replace(temp, _path, null);
            else File.Move(temp, _path);
        }
    }
}
