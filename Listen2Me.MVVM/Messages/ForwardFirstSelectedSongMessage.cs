using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.Messages;

public class ForwardFirstSelectedSongMessage
{
    public Song Song { get; set; }
    
    public ForwardFirstSelectedSongMessage(Song message)
    {
        Song = message;
    }
}