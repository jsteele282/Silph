using Silph.Core.Objects;

namespace Silph.Core.Repositories
{
    public class Messages
    {
        public static Messages Default { get; } = new();

        protected virtual MessageCodes Codes { get; } = MessageCodes.Default;

        public virtual Message Success { get; protected set; }
        public virtual Message ProjectConfigurationRead {  get; protected set; }
        public virtual Message ProjectConfigurationWritten { get; protected set; }

        public virtual Message RequiredValueMissing { get; protected set; }
        public virtual Message FileNotFound { get; protected set; }
        public virtual Message FileAlreadyExists { get; protected set; }
        public virtual Message FileWriteFailed { get; protected set; }
        public virtual Message ConfigurationInvalid { get; protected set; }

        public Messages(MessageCodes? codes = null) 
        { 
            //Codes = codes ?? new MessageCodes();

            Success = new Message(Codes.Success, "Operation completed successfully.", MessageType.Success);
            ProjectConfigurationRead = new Message(Codes.ProjectConfigurationRead, "Project configuration read successfully.", MessageType.Success);
            ProjectConfigurationWritten = new Message(Codes.ProjectConfigurationWritten, "Project configuration written successfully.", MessageType.Success);
            RequiredValueMissing = new Message(Codes.RequiredValueMissing, "A required value is missing.", MessageType.Error);
            FileNotFound = new Message(Codes.FileNotFound, "The specified file was not found.", MessageType.Error);
            FileAlreadyExists = new Message(Codes.FileAlreadyExists, "The specified file already exists.", MessageType.Error);
            FileWriteFailed = new Message(Codes.FileWriteFailed, "Failed to write to the specified file.", MessageType.Error);
            ConfigurationInvalid = new Message(Codes.ConfigurationInvalid, "The configuration is invalid.", MessageType.Error);
        }

        public Message Debug(string message, LogScope logScope, string? title = null)
        {
            return logScope switch
            {
                LogScope.Structured => new Message(Codes.StructuredDebug, message, MessageType.Debug, LogScope.Structured, title),
                LogScope.Bootstrap => new Message(Codes.BootstrapDebug, message, MessageType.Debug, LogScope.Bootstrap, title),
                _ => new Message(Codes.InformationDebug, message, MessageType.Debug, LogScope.None, title)
            };
        }
    }
}


//namespace Silph.Core.Messaging;

//public class MessageCodes
//{
//    // General / Success / Info: 1000-1999
//    public virtual MessageCode Success { get; } = new(1000, nameof(Success));
//    public virtual MessageCode OperationCompleted { get; } = new(1001, nameof(OperationCompleted));
//    public virtual MessageCode OperationCancelled { get; } = new(1002, nameof(OperationCancelled));
//    public virtual MessageCode NoActionTaken { get; } = new(1003, nameof(NoActionTaken));
//    public virtual MessageCode AlreadyExists { get; } = new(1004, nameof(AlreadyExists));
//    public virtual MessageCode AlreadyComplete { get; } = new(1005, nameof(AlreadyComplete));

//    // Validation: 2000-2999
//    public virtual MessageCode ValidationFailed { get; } = new(2000, nameof(ValidationFailed));
//    public virtual MessageCode RequiredValueMissing { get; } = new(2001, nameof(RequiredValueMissing));
//    public virtual MessageCode InvalidValue { get; } = new(2002, nameof(InvalidValue));
//    public virtual MessageCode InvalidFormat { get; } = new(2003, nameof(InvalidFormat));
//    public virtual MessageCode ValueOutOfRange { get; } = new(2004, nameof(ValueOutOfRange));
//    public virtual MessageCode ValueTooShort { get; } = new(2005, nameof(ValueTooShort));
//    public virtual MessageCode ValueTooLong { get; } = new(2006, nameof(ValueTooLong));
//    public virtual MessageCode DuplicateValue { get; } = new(2007, nameof(DuplicateValue));
//    public virtual MessageCode InvalidState { get; } = new(2008, nameof(InvalidState));

//    // Lookup / Not Found / State: 3000-3999
//    public virtual MessageCode NotFound { get; } = new(3000, nameof(NotFound));
//    public virtual MessageCode EntityNotFound { get; } = new(3001, nameof(EntityNotFound));
//    public virtual MessageCode RecordNotFound { get; } = new(3002, nameof(RecordNotFound));
//    public virtual MessageCode NoResultsFound { get; } = new(3003, nameof(NoResultsFound));
//    public virtual MessageCode MultipleResultsFound { get; } = new(3004, nameof(MultipleResultsFound));
//    public virtual MessageCode DependencyMissing { get; } = new(3005, nameof(DependencyMissing));
//    public virtual MessageCode DependencyConflict { get; } = new(3006, nameof(DependencyConflict));

//    // Permission / Access: 4000-4999
//    public virtual MessageCode AccessDenied { get; } = new(4000, nameof(AccessDenied));
//    public virtual MessageCode OperationNotAllowed { get; } = new(4001, nameof(OperationNotAllowed));
//    public virtual MessageCode ReadNotAllowed { get; } = new(4002, nameof(ReadNotAllowed));
//    public virtual MessageCode WriteNotAllowed { get; } = new(4003, nameof(WriteNotAllowed));
//    public virtual MessageCode DeleteNotAllowed { get; } = new(4004, nameof(DeleteNotAllowed));
//    public virtual MessageCode SchemaChangeNotAllowed { get; } = new(4005, nameof(SchemaChangeNotAllowed));

//    // Data / SQL / Persistence: 5000-5999
//    public virtual MessageCode DataOperationFailed { get; } = new(5000, nameof(DataOperationFailed));
//    public virtual MessageCode ConnectionFailed { get; } = new(5001, nameof(ConnectionFailed));
//    public virtual MessageCode QueryFailed { get; } = new(5002, nameof(QueryFailed));
//    public virtual MessageCode InsertFailed { get; } = new(5003, nameof(InsertFailed));
//    public virtual MessageCode UpdateFailed { get; } = new(5004, nameof(UpdateFailed));
//    public virtual MessageCode DeleteFailed { get; } = new(5005, nameof(DeleteFailed));
//    public virtual MessageCode TransactionFailed { get; } = new(5006, nameof(TransactionFailed));
//    public virtual MessageCode TableNotFound { get; } = new(5007, nameof(TableNotFound));
//    public virtual MessageCode ColumnNotFound { get; } = new(5008, nameof(ColumnNotFound));
//    public virtual MessageCode MappingFailed { get; } = new(5009, nameof(MappingFailed));

//    // File / Image / Path: 6000-6999
//    public virtual MessageCode FileNotFound { get; } = new(6000, nameof(FileNotFound));
//    public virtual MessageCode DirectoryNotFound { get; } = new(6001, nameof(DirectoryNotFound));
//    public virtual MessageCode PathInvalid { get; } = new(6002, nameof(PathInvalid));
//    public virtual MessageCode FileReadFailed { get; } = new(6003, nameof(FileReadFailed));
//    public virtual MessageCode FileWriteFailed { get; } = new(6004, nameof(FileWriteFailed));
//    public virtual MessageCode ImageLoadFailed { get; } = new(6005, nameof(ImageLoadFailed));
//    public virtual MessageCode ImageSaveFailed { get; } = new(6006, nameof(ImageSaveFailed));
//    public virtual MessageCode UnsupportedImageFormat { get; } = new(6007, nameof(UnsupportedImageFormat));

//    // Configuration / Startup / Environment: 7000-7999
//    public virtual MessageCode ConfigurationMissing { get; } = new(7000, nameof(ConfigurationMissing));
//    public virtual MessageCode ConfigurationInvalid { get; } = new(7001, nameof(ConfigurationInvalid));
//    public virtual MessageCode SettingMissing { get; } = new(7002, nameof(SettingMissing));
//    public virtual MessageCode SettingInvalid { get; } = new(7003, nameof(SettingInvalid));
//    public virtual MessageCode StartupFailed { get; } = new(7004, nameof(StartupFailed));
//    public virtual MessageCode ServiceUnavailable { get; } = new(7005, nameof(ServiceUnavailable));

//    // External / Service / Network: 8000-8999
//    public virtual MessageCode ExternalOperationFailed { get; } = new(8000, nameof(ExternalOperationFailed));
//    public virtual MessageCode ExternalServiceUnavailable { get; } = new(8001, nameof(ExternalServiceUnavailable));
//    public virtual MessageCode RequestFailed { get; } = new(8002, nameof(RequestFailed));
//    public virtual MessageCode ResponseInvalid { get; } = new(8003, nameof(ResponseInvalid));
//    public virtual MessageCode Timeout { get; } = new(8004, nameof(Timeout));

//    // Unexpected / System / Unknown: 9000-9999
//    public virtual MessageCode UnknownError { get; } = new(9000, nameof(UnknownError));
//    public virtual MessageCode UnexpectedError { get; } = new(9001, nameof(UnexpectedError));
//    public virtual MessageCode NotImplemented { get; } = new(9002, nameof(NotImplemented));
//    public virtual MessageCode UnsupportedOperation { get; } = new(9003, nameof(UnsupportedOperation));
//}