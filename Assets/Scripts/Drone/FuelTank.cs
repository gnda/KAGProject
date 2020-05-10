using UnityEngine;

namespace Drone
{
    public class FuelTank : MonoBehaviour
    {
        public int Fuel { get; set; } = 0;
        public int FuelTime { get; set; } = 0;

        public void ReductFuel(int value)
        {
            if(FuelTime == 10)
            {
                int soustraction = Fuel - value;
                if (soustraction > 0)
                {
                    Fuel = Fuel - value;
                }
                else
                {
                    Fuel = 0;
                    //game over
                }
            
                FuelTime = 0;
            }
            else
            {
                FuelTime++;
            }
        }
    }
}