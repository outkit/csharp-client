namespace Outkit
{
    public enum OutkitStatus
    {
        Received,
        QueuedForRendering,
        Rendered,
        QueuedForDelivery,
        Delivered,
        RenderError,
        BackendError,
        InternalError
    }
}
