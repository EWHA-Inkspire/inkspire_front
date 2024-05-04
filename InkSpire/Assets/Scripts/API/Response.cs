public class Response<T>
{
    public bool success;
    public int code;
    public string message;
    public T data;

    public override string ToString()
    {
        return "success: " + success.ToString() + "\ncode: " + code.ToString() + "\nmessage: " + message + "\ndata: " + data;
    }
}