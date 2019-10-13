using System;
using System.Collections.Generic;
using Services.Ship;

namespace Services.Game
{
    public class BattleshipPlaced : IEvent<Game>, IValidate<Game>
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

        public bool Validate(Game state)
        {
            bool isValid = true;
            if (!IsWithinBoard(this.Battleship))
            {
                ErrorMessages.Add("Battleship outside board");
                isValid = false;
            }

            if (!IsSafeToPlaceOnBoard(this.Battleship,
                PlayerId == state.PlayerA ? state.ShipsA : state.ShipsB))
            {
                ErrorMessages.Add("Battleship overlaps");
                isValid = false;
            }

            if (state.Status != Game.GameCreated)
            {
                ErrorMessages.Add("Game is locked");
                isValid = false;
            }

            return isValid;
        }
    }
}