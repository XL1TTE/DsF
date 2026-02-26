namespace Common.Utilities.FileUpload;

public class NotAnImageException : Exception
{
    public NotAnImageException(string contentType) 
        : base($"The file is not a valid image. Content type: {contentType}")
    {
    }
}
