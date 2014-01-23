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

        public MV1Animation(int modelHandle, int animationIndex)
        {
            this.modelHandle = modelHandle;
            this.animationIndex = animationIndex;
        }

        public void Attach()
        {
            attachIndex = DX.MV1AttachAnim(modelHandle, animationIndex);
            totalFrame = (int)DX.MV1GetAnimTotalTime(modelHandle, attachIndex);
            currentFrame = 0;
        }

        public void Detach()
        {
            DX.MV1DetachAnim(modelHandle, attachIndex);
            attachIndex = -1;
        }

        public void NextFrame()
        {
            currentFrame++;
            if (currentFrame >= totalFrame)
                currentFrame = 0;
            DX.MV1SetAttachAnimTime(modelHandle, attachIndex, currentFrame);
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
        }
    }
}
