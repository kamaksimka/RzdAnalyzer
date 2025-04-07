namespace RZD.Api
{
    public class JsonResultResponse
    {
        public JsonResultResponse(bool success, object? result = null)
        {
            this.Success = success;
            this.Result = result;
        }

        public bool Success { get; }
        public object? Result { get; }
    }
}
