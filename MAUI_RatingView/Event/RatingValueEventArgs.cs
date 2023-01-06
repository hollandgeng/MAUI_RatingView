using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUI_RatingView.Event
{
    public class RatingValueEventArgs : EventArgs
    {
        public int Value { get; init; }

        public RatingValueEventArgs(int value) 
        {
            Value = value;
        }
    }
}
