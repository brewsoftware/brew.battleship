using System;
using System.Collections.Generic;

namespace services.player.Game
{
    public class BattleshipPlaced : IEvent<services.Game>
    {
        public Guid PlayerId { get; set; }
        public Battleship Battleship { get; set; }
        
        private bool IsWithinBoard(Battleship battleship)
        {
            foreach (var coords in battleship.GetCoords())
            {
                if (coords.X > 10 || coords.X < 0 || coords.Y > 10 || coords.Y < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsSafeToPlaceOnBoard(Battleship battleship, List<Battleship> battleships)
        {
            foreach (var bs in battleships)
            {
                foreach (var bsCoord in bs.GetCoords())
                {
                    if (battleship.X == bsCoord.X && battleship.Y == bsCoord.Y)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool IsValid(services.Game state)
        {
            return IsWithinBoard(this.Battleship) 
                   && IsSafeToPlaceOnBoard(this.Battleship,
                       PlayerId == state.PlayerA ? state.ShipsA : state.ShipsB)
                && state.Status == services.Game.GameCreated;
        }
    }
}