using Redux;

namespace services.player.Events
{
    public class GameStarted: IEvent<services.Game>
    {
        public override bool IsValid(services.Game state)
        {
            return true;
        }
    }
}