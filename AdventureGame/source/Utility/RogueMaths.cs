namespace AdventureGame
{
    internal class RogueMaths
    {
        public static int Wrap(int input, int min, int max)
        {
            while (input > max)
            {
                // subtract one more than the difference between max and min
                input -= max - (min + 1);
            }
            while (input < min)
            {
                // add one more than the difference between max and min

                input += max - (min + 1);
            }

            return input;
        }

    }
}
