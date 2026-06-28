namespace Silph.Core.Repositories
{
    public class MessageCodes
    {
        public static MessageCodes Default { get; } = new();

        public int StructuredDebug => 0;
        public int BootstrapDebug => 1;
        public int InformationDebug => 2;

        // Validation/Info: 100-199
        public int AlreadyExists => 100;
        public int AlreadyComplete => 101;
        public int DataNotFound => 102;

        // Success: 200-299
        public int Success => 200;
        public int OperationCompleted => 201;
        public int OperationAccepted => 202;
        public int OperationCancelled => 203;
        public int ProjectConfigurationRead => 210;
        public int ProjectConfigurationWritten => 211;


        // Warning: 300-399
        public int Warning => 300;


        // General Error: 400-499
        public int DataExpected => 400;
        public int RecordNotFound => 401;
        public int RequiredValueMissing => 402;
        public int InvalidValue => 404;
        public int ConfigurationInvalid => 405;
        public int FileNotFound => 410;
        public int FileAlreadyExists => 411;
        public int FileWriteFailed => 412;


        // Server Error: 500-599

        // TODO: A tool (console tool?) is needed to ensure unique message codes between inherited project and Silph
    }
}
