using System;

namespace Services.Game
{
    public class ShotFired : IEvent<Game>, IValidate<Game>
    {
        public Guid PlayerId { get; set; }
        public Coordinate Coordinate { get; set; }
        
        public bool Validate(Game state)
        {
            if (state.Status != Game.GameCreated && state.Status != Game.GameStarted)
            {
                this.ErrorMessages.Add($"{state.Status} : Unable to Fire");
            }

            if (state.ShipsB.Count <= 0 || state.ShipsA.Count <= 0)
            {
                this.ErrorMessages.Add("No ships placed");
            }

            return this.ErrorMessages.Count == 0;
        }
    }
}