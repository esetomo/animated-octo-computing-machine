using DxLibDLL;
using System;

namespace Lemonade
{
    public class MV1Animation
    {
        private readonly int modelHandle;
        private readonly int animationIndex;
        private int attachIndex = -1;
        private int currentFrame;
        private int totalFrame;

        enum States
        {
            Stop,
            FadeIn,
            Running,
            FadeOut,
        }
        private States state;
        private float blend;

        public MV1Animation(int modelHandle, int animationIndex)
        {
            this.modelHandle = modelHandle;
            this.animationIndex = animationIndex;
            this.state = States.Stop;
        }

        public void FadeIn()
        {
            Attach();

            state = States.FadeIn;
            blend = 0.0f;
        }

        public void FadeOut()
        {
            state = States.FadeOut;
            blend = 1.0f;
        }

        public void Attach()
        {
            attachIndex = DX.MV1AttachAnim(modelHandle, animationIndex);
            totalFrame = (int)DX.MV1GetAnimTotalTime(modelHandle, attachIndex);
            currentFrame = 0;
            state = States.Running;
            blend = 1.0f;
        }

        public void Detach()
        {
            DX.MV1DetachAnim(modelHandle, attachIndex);
            attachIndex = -1;
            state = States.Stop;
            blend = 0.0f;
        }

        public void NextFrame()
        {
            // easeInOutSine
            double b = Math.Sin((blend - 0.5) * Math.PI) * 0.5 + 0.5;
            DX.MV1SetAttachAnimBlendRate(modelHandle, attachIndex, (float)b);

            switch(state)
            {
                case States.FadeIn:
                    blend += 0.1f;
                    if (blend >= 1.0f)
                    {
                        state = States.Running;
                        blend = 1.0f;
                    }
                    break;
                case States.FadeOut:
                    blend -= 0.1f;
                    if(blend <= 0.0f)
                    {
                        Detach();
                    }
                    break;
            }

            if (state != States.Stop)
            {
                currentFrame++;
                if (currentFrame >= totalFrame)
                    currentFrame = 0;
                DX.MV1SetAttachAnimTime(modelHandle, attachIndex, currentFrame);
            }
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
        }
    }
}
