using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace AnimationCompressor
{
    // Not acturally bone
    
    public class Bone
    {
        public Bone Parent { get; private set; }

        public string Path { get; private set; }

        public AnimationCurve Curve { get; private set; }

        public float Sample(float time)
        {
            
            var value = 0f;

            if(Parent != null)
                value += Parent.Sample(time);

            return value;
        }
    }
}
