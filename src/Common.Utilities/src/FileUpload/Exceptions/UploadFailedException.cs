namespace Common.Utilities.FileUpload;

public class UploadFailedException : Exception
{
    public UploadFailedException(string message) 
        : base(message)
    {
    }

    public UploadFailedException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
