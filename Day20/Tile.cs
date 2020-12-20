namespace Day20
{
    internal record Tile(int Id, char[,] Values)
    {
        private readonly int _n = Values.GetLength(0); 
        
        // a|b -> b|a
        public Tile FlipVertically()
        {
            var flipped = Values.Clone() as char[,];

            for (var i = 0; i < _n; i++)
            {
                for (var j = 0; j < _n / 2; j++)
                {
                    flipped[i, j] = Values[i, _n - 1 - j];
                    flipped[i, _n - 1 - j] = Values[i, j];
                }
            }

            return new Tile(Id, flipped);
        }
            
        // a      b
        // -  ->  -
        // b      a
        public Tile FlipHorizontally()
        {
            var flipped = Values.Clone() as char[,];

            for (var i = 0; i < _n / 2; i++)
            {
                for (var j = 0; j < _n; j++)
                {
                    flipped[i, j] = Values[_n - 1 - i, j];
                    flipped[_n - 1 - i, j] = Values[i, j];
                }
            }

            return new Tile(Id, flipped);
        }

        
        // Checks if other tile can be placed on top of original tile
        public bool CanPlaceTop(Tile otherTile)
        {
            for (var i = 0; i < _n; i++)
            {
                if (Values[0, i] != otherTile.Values[_n - 1, i])
                    return false;
            }

            return true;
        }
        
        // Checks if other tile can be placed at bottom of original tile
        public bool CanPlaceBottom(Tile otherTile)
        {
            for (var i = 0; i < _n; i++)
            {
                if (Values[_n - 1, i] != otherTile.Values[0, i])
                    return false;
            }

            return true;
        }
        
        // Checks if other tile can be placed left of original tile
        public bool CanPlaceLeft(Tile otherTile)
        {
            for (var i = 0; i < _n; i++)
            {
                if (Values[i, 0] != otherTile.Values[i, _n - 1])
                    return false;
            }

            return true;
        }
        
        // Checks if other tile can be placed right of original tile
        public bool CanPlaceRight(Tile otherTile)
        {
            for (var i = 0; i < _n; i++)
            {
                if (Values[i, _n - 1] != otherTile.Values[i, 0])
                    return false;
            }

            return true;
        }

        // a b c      g d a
        // d e f  ->  h e b 
        // g h i      i f c
        public Tile RotateLeft()
        {
            var rotated = new char[_n, _n];

            for (var i = 0; i < _n; ++i)
            {
                for (var j = 0; j < _n; ++j)
                {
                    rotated[i, j] = Values[_n - j - 1, i];
                }
            }

            return new Tile(Id, rotated);
        }
    }
}