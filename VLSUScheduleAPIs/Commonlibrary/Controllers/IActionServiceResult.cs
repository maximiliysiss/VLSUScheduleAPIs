namespace Commonlibrary.Controllers
{
    public enum ResultCode
    {
        Success,
        Error
    }

    public interface IActionServiceResult
    {
        ResultCode ResultCode { get; set; }
    }

    public class PrimitiveServiceResult : IActionServiceResult
    {
        public ResultCode ResultCode { get; set; } = ResultCode.Success;
    }
}