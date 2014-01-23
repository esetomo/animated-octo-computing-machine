using DxLibDLL;
using System;

namespace Lemonade
{
    public class MV1Model
    {
        private int handle = -1;
        private MV1Animation[] animations;
        private int serikoSurface;

        public MV1Model(string fileName)
        {
            handle = DX.MV1LoadModel(fileName);
            int animationCount = DX.MV1GetAnimNum(handle);

            animations = new MV1Animation[animationCount];
            for (int i = 0; i < animationCount; i++)
                animations[i] = new MV1Animation(handle, i);

            SerikoSurface = 0;
        }

        public void SetPosition(float x, float y, float z)
        {
            DX.MV1SetPosition(handle, DX.VGet(0.0f, 0.0f, 0.0f));
        }

        public void NextFrame()
        {
            foreach(MV1Animation anim in animations)
                anim.NextFrame();
        }

        public void Draw()
        {
            DX.MV1DrawModel(handle);
        }

        public int CurrentFrame
        {
            get
            {
                return animations[serikoSurface].CurrentFrame;
            }
        }

        public int SerikoSurface
        {
            get
            {
                return serikoSurface;
            }
            set
            {
                animations[serikoSurface].FadeOut();
                serikoSurface = value;
                animations[serikoSurface].FadeIn();
            }
        }
    }
}
