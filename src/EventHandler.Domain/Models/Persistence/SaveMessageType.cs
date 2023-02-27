namespace EventHandler.Domain.Models.Persistence
{
    public enum SaveMessageType
    {
        None,
        BufferOverflow,
        HandleImmediately,
        EndOfFileCharacter
    }
}
