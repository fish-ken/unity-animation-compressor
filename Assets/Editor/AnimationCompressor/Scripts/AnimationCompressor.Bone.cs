using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace AnimationCompressor
{
    public class Bone
    {
        public Bone Parent { get; private set; }

        public string Path { get; private set; }

        public AnimationCurve Curve { get; private set; }

        public float Sample()
        {
            if(Parent)

            Parent.Sample();
        }
    }
}
