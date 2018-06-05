using UnityEngine;
using System.Collections;


namespace Eyesblack.Optimizations {

    public static class GraphicsDetails {
        public static float[,] ObjectViewDistance = new float[4, 4] {
            {400, 300, 200, 80},
            {300, 200, 100, 60},
            {200, 100, 80, 40},
            {100, 80, 40, 20}
        };
        public static int ObjectDetailLevel { get; set; }

 
    }

}
